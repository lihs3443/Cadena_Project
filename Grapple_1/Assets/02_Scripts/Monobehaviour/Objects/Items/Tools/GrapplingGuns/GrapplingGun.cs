using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : Tool
{
    [Space, Header("Hook Propertys:")]
    [SerializeField] protected GrabHook grabHook;
    LineRenderer grappleLineRenderer; //�׷��� ��� �� �ѱ��� ���� ������ �̾��ֱ� ���� ���(�ð��� ȿ��)
    [SerializeField] protected float useCooldown = 1.5f;
    /// <summary>
    /// ����������� ���� ����
    /// �ش� bool������ ������ �׸��� ���� � ���
    /// </summary>
    [SerializeField] protected bool inUse = false;
    [SerializeField] protected float cooldownCounter = 0f;
    [SerializeField, Tooltip("���̳� �ٴڿ� �׷� ����")] bool canGrabWall = true;
    [SerializeField, Tooltip("���̳� Ư�� ������Ʈ �׷� ����")] bool canGrabEntity = false;

    [Space, Header("Hook Stats")]
    [SerializeField, Tooltip("�߻� �ӵ�")] protected float launchVelocity = 0.2f;
    [SerializeField, Tooltip("���� ������ ���� �ӵ�")] protected float pullVelocity = 0.25f;
    [SerializeField, Tooltip("�߻簡 �����Ǵ� �ִ� �ð�")] protected float launchDuration = 0.67f;


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
            Debug.Log($"���� {gameObject.name}�� ��� ��ٿ��� {cooldownCounter:N2}�� ���ҽ��ϴ�!");
            return false;
        }
        cooldownCounter = useCooldown;
        UseEffect();

        return true;
    }

    /// <summary>
    /// ���� ���� �ɷ����� �� ����
    /// ��: GrabHook.cs���� ȣ���ϴ� �뵵
    /// </summary>
    /// <param name="_grabbedPos">�׷����� �ε��� ��ġ</param>
    public virtual void HookOnTheWall(Vector2 _grabbedPos)
    {
        StopCoroutine(Coroutine_fireGrab);
        Coroutine_fireGrab = null;
    }

    /// <summary>
    /// ���� ��ƼƼ(NPC��)���� �ɷ����� �� ����
    /// ��: GrabHook.cs���� ȣ����
    /// </summary>
    /// <param name="_grabbedPos">�׷����� �浹�� ��ġ</param>
    public virtual void HookOnEntity(Vector2 _grabbedPos)
    {
        StopCoroutine(Coroutine_fireGrab);
        Coroutine_fireGrab = null;
    }

    /// <summary>
    /// ����� ����� �� ���� �ൿ
    /// �׷��� �÷��̾�� �ٽ� �ǵ��ƿ� ��Ȳ � ���
    /// </summary>
    protected virtual void EndOfUse()
    {
        inUse = false;
        cooldownCounter = useCooldown; //��Ÿ�� �ʱ�ȭ �� ����
        grappleLineRenderer.gameObject.SetActive(false);
    }

    /// <summary>
    /// GrabHook�� ������ �����̰� �ϴ�(�߻��ϴ�)������ �ڷ�ƾ
    /// ���� GrabHook�� ���𰡿� �����Ͽ��ٸ� �ڷ�ƾ�� ������Ѿ���
    /// 
    /// *��: �ش� ������ GrabHook���� �Ѱ��ٰ�
    /// 
    /// </summary>
    /// <param name="_destination">���� ���������� ������ �˱� ����</param>
    /// <returns></returns>
    protected IEnumerator FireGrab(Vector2 _destination)
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        float durationCounter = launchDuration;

        grappleLineRenderer.gameObject.SetActive(true);

        while (true)
        {
            launchDuration -= Time.deltaTime;
            //grabHook�� ��ġ�� ��ȭ��Ű�� ���� �ʿ�

            //���ӽð��� ������ ���
            if (launchDuration <= 0f)
            {
                inUse = false; //����� ����

                //���ư� �׷��� �ٽ� ���ڸ��� �������� �����
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
    /// ���ӽð��� ���� �׷��� �ڿ����� �������� ���� ���� �뵵
    /// </summary>
    /// <returns></returns>
    protected IEnumerator ReturnGrab()
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        yield return waitForEndOfFrame;
    }

    /// <summary>
    /// ��� �� ��Ÿ���� �����ϴ�(�ٽ� �ٿ��ִ�) ����
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
    /// �ڷ�ƾ�� Toggle�ϱ� ���� �޼ҵ�.
    /// Delegate�� Ȱ���Ͽ� �ܺο��� Toggle�� �����ϵ��� ������
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
    /// �Ű� �ѱ� ���̸� ������ �̾��ִ� �ڷ�ƾ. ���� ������ ������ ����� �����ٸ�
    /// �ܺο��� StopCoroutine�� Ȱ���Ͽ� ���������־����.
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
