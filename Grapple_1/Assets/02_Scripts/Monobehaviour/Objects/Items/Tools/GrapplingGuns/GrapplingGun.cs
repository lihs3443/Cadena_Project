using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : Tool
{
    [Space, Header("Hook Propertys:")]
    [SerializeField] protected GrabHook grabHook;
    LineRenderer grappleLineRenderer; //그래플 사용 시 총구와 훅을 선으로 이어주기 위해 사용(시각적 효과)
    [SerializeField] protected float useCooldown = 1.5f;
    /// <summary>
    /// 사용중인지에 대한 여부
    /// 해당 bool값으로 라인을 그릴지 말지 등에 사용
    /// </summary>
    [SerializeField] protected bool inUse = false;
    [SerializeField] protected float cooldownCounter = 0f;
    [SerializeField, Tooltip("벽이나 바닥에 그랩 가능")] bool canGrabWall = true;
    [SerializeField, Tooltip("적이나 특정 오브젝트 그랩 가능")] bool canGrabEntity = false;

    [Space, Header("Hook Stats")]
    [SerializeField, Tooltip("발사 속도")] protected float launchVelocity = 0.2f;
    [SerializeField, Tooltip("부착 성공시 당기는 속도")] protected float pullVelocity = 0.25f;
    [SerializeField, Tooltip("발사가 유지되는 최대 시간")] protected float launchDuration = 0.67f;


    protected Coroutine Coroutine_fireGrab = null;

    protected virtual void Awake()
    {
        grappleLineRenderer = GetComponent<LineRenderer>();
    }

    protected override void Start()
    {
        base.Start();
        grappleLineRenderer.SetPosition(1, grabHook.transform.position);
    }

    public override bool TryUse()
    {
        if (cooldownCounter > 0f)
        {
            Debug.Log($"아직 {gameObject.name}의 사용 쿨다운이 {cooldownCounter:N2}초 남았습니다!");
            return false;
        }
        cooldownCounter = useCooldown;
        UseEffect();

        return true;
    }

    /// <summary>
    /// 훅이 벽에 걸렸을때 할 행위
    /// 주: GrabHook.cs에서 호출하는 용도
    /// </summary>
    /// <param name="_grabbedPos">그랩훅이 부딪힌 위치</param>
    public virtual void HookOnTheWall(Vector2 _grabbedPos)
    {
        StopCoroutine(Coroutine_fireGrab);
        Coroutine_fireGrab = null;
    }

    /// <summary>
    /// 훅이 엔티티(NPC등)에게 걸렸을때 할 행위
    /// 주: GrabHook.cs에서 호출함
    /// </summary>
    /// <param name="_grabbedPos">그랩훅이 충돌한 위치</param>
    public virtual void HookOnEntity(Vector2 _grabbedPos)
    {
        StopCoroutine(Coroutine_fireGrab);
        Coroutine_fireGrab = null;
    }

    /// <summary>
    /// 사용이 종료된 후 취할 행동
    /// 그랩이 플레이어에게 다시 되돌아온 상황 등에 사용
    /// </summary>
    protected virtual void EndOfUse()
    {
        inUse = false;
        cooldownCounter = useCooldown; //쿨타임 초기화 및 시작
        grappleLineRenderer.gameObject.SetActive(false);
    }

    /// <summary>
    /// GrabHook을 실제로 움직이게 하는(발사하는)역할의 코루틴
    /// 만약 GrabHook이 무언가와 접촉하였다면 코루틴을 종료시켜야함
    /// 
    /// *주: 해당 역할을 GrabHook에게 넘겨줄것
    /// 
    /// </summary>
    /// <param name="_destination">최종 목적지점의 방향을 알기 위함</param>
    /// <returns></returns>
    protected IEnumerator FireGrab(Vector2 _destination)
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        float durationCounter = launchDuration;

        grappleLineRenderer.gameObject.SetActive(true);

        while (true)
        {
            launchDuration -= Time.deltaTime;
            //grabHook의 위치를 변화시키는 구문 필요

            //지속시간이 끝났을 경우
            if (launchDuration <= 0f)
            {
                inUse = false; //사용이 끝남

                //날아간 그랩을 다시 제자리로 돌려놓는 연출용
                while (true)
                {
                    yield return waitForEndOfFrame;

                    if (Vector2.Distance(transform.position, grabHook.transform.position) <= 0.15f)
                    {
                        break;
                    }
                }
                break;
            }
            

            yield return waitForEndOfFrame;

            Coroutine_fireGrab = null;
        }
    }

    /// <summary>
    /// 지속시간이 끝난 그랩을 자연스레 돌려놓기 위한 연출 용도
    /// </summary>
    /// <returns></returns>
    protected IEnumerator ReturnGrab()
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        yield return waitForEndOfFrame;
    }

    /// <summary>
    /// 사용 후 쿨타임을 관리하는(다시 줄여주는) 역할
    /// </summary>
    /// <returns></returns>
    protected IEnumerator CooldownManagement()
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        while (true)
        {
            cooldownCounter -= Time.deltaTime;

            if (cooldownCounter <= 0f)
            {
                break;
            }
            yield return waitForEndOfFrame;
        }
    }

    Coroutine Coroutine_toggleHookLine = null;

    /// <summary>
    /// 코루틴을 Toggle하기 위한 메소드.
    /// Delegate를 활용하여 외부에서 Toggle이 가능하도록 설계함
    /// </summary>
    void ToggleConnectHookLine()
    {
        //is running
        if (Coroutine_toggleHookLine != null)
        {
            StopCoroutine(Coroutine_toggleHookLine);
            Coroutine_toggleHookLine = null;

            return;
        }
        //no running
        Coroutine_toggleHookLine = StartCoroutine(ConnectHookLine());
    }

    /// <summary>
    /// 훅과 총구 사이를 선으로 이어주는 코루틴. 무한 루프기 때문에 사용이 끝났다면
    /// 외부에서 StopCoroutine을 활용하여 중지시켜주어야함.
    /// </summary>
    /// <returns></returns>
    IEnumerator ConnectHookLine()
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        while (true)
        {
            grappleLineRenderer.SetPosition(1, grabHook.transform.localPosition);

            yield return waitForEndOfFrame;
        }
    }

    protected override void UseEffect()
    {
        
    }
}
