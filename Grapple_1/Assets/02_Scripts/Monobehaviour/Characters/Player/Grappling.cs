using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    public LineRenderer line;                  // 플레이어와 갈고리간의 라인 이펙트
    public Transform hook;                     // 훅의 위치
    [ReadOnly] public Vector3 PlayerPos;       // 플레이어 위치, 첫 위치 조정을 위한 변수
    SpriteRenderer sprite;                     // 훅 발사할 때 마우스 방향으로 캐릭터가 바라보도록 할 변수
    Vector2 MousePos;                          // 마우스 위치
    float HookAngle;
    Animator animator;
    MovementController movectr;

    public float HookSpeed;                    // 훅 날아가는 속도
    public float HookRange;                    // 훅 사거리
    public float HookHoldSpeed;                // 훅 당기는 속도
    public float cameraShakeduration;          // 훅이 붙었을 때 카메라 흔들림 빈도(시간)
    public float cameraShakeMagnitude;         // 훅이 붙었을 때 카메라 흔들림 강도(세기)

    [ReadOnly] public bool isHookActive;       // 훅이 날아갔을 때 true
    [ReadOnly] public bool isMaxRangeLimit;    // 훅이 한계점까지 도달했을 때 true
    [ReadOnly] public bool isAttatch;          // 훅이 붙었을 경우 true

    void Start()
    {
        animator = GetComponent<Animator>();        // 애니메이션 변수
        sprite = GetComponent<SpriteRenderer>();    // 훅을 발사하는 방향을 바라보도록 하기 위한 변수
        movectr = GetComponent<MovementController>();
        PlayerPos = transform.position;             // 훅이 나오는 위치 조정
        isHookActive = false;
        isMaxRangeLimit = false;
        isAttatch = false;
        hook.gameObject.SetActive(false);

// 초기값 설정(외부 설정 안할 경우)
//        HookSpeed = 50f;
//        HookRange = 10f;
        HookHoldSpeed = 3f;

        line.positionCount = 2;                     // 라인을 그리는 위치의 개수
        line.endWidth = line.startWidth = 0.02f;    // 라인의 두께
        line.SetPosition(0, PlayerPos);             // 라인을 그리는 시작점의 위치, 플레이어의 위치
        line.SetPosition(1, hook.position);         // 라인을 그리는 마지막점의 위치, 훅의 위치
        line.useWorldSpace = true;
    }

    void Update()
    {
        PlayerPos = transform.position;    
        line.SetPosition(0, PlayerPos);             // 라인을 그리는 시작점의 위치, 플레이어의 위치
        line.SetPosition(1, hook.position);         // 라인을 그리는 마지막점의 위치, 훅의 위치

        // 마우스 왼쪽클릭 && 훅이 꺼져있음 && 안붙어있음 && 땅이 아님
        if (Input.GetMouseButtonDown(0) && !isHookActive && !isAttatch && !movectr.conditions.isGround)
        {
            hook.position = PlayerPos;                                                      // 훅 시작위치 갱신
            MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - PlayerPos;     // 마우스방향 설정

            HookAngle = Mathf.Atan2(MousePos.y, MousePos.x) * Mathf.Rad2Deg;                // 훅의 날아갈 각도 계산
            hook.transform.rotation = Quaternion.AngleAxis(HookAngle-90,Vector3.forward);   // 훅의 각도 변경

            if (MousePos.x > 0)                                                             // 마우스가 캐릭터 왼쪽에 있을 경우 왼쪽 바라보기
                sprite.flipX = true;                                            // 훅 발사시
            if (MousePos.x < 0)                                                             // 마우스가 캐릭터 오른쪽에 있을 경우 오른쪽 바라보기
                sprite.flipX = false;

            animator.SetBool("_WireShoting", true);                                         // 와이어 발사 애니메이션 on

            isHookActive = true;                                                            // 훅 on
            isMaxRangeLimit = false;
            hook.gameObject.SetActive(true);                                                // 훅 오브젝트 on
        }

        if(Input.GetMouseButtonUp(0))
        {
            animator.SetBool("_WireShoting", false);                                        // 와이어 발사 애니메이션 off
        }

        // 훅은 켜져있음 && 최대사거리 도달 안함 && 안붙어있음
        if (isHookActive && !isMaxRangeLimit && !isAttatch)
        {
            hook.Translate(Vector2.up * Time.deltaTime * HookSpeed);                   // 훅 날아감

            if (Vector2.Distance(PlayerPos, hook.position) > HookRange)                // 훅 사거리만큼 날아가면    
                isMaxRangeLimit = true;
        }
        
        // 훅이 켜져있음 && 최대사거리 도달함 && 안붙어있음
        if (isHookActive && isMaxRangeLimit && !isAttatch)
        {
            isHookActive = false;
            isMaxRangeLimit = false;
            hook.gameObject.SetActive(false);
        }

        // 훅이 붙었을 때
        if (isAttatch)
        {
            /// 블렌드 트리를 이용한 애니메이션 처리를 위한 작업
            float Hookdistance = (line.GetPosition(1) - line.GetPosition(0)).magnitude; // 훅 길이
            float distanceX = line.GetPosition(1).x - line.GetPosition(0).x;            // 훅과 플레이어의 X축 간격

            if (!sprite.flipX)
                distanceX = -distanceX;
            animator.SetBool("_Hanging", true);
            animator.SetFloat("HangingAngle", 0.5f + (distanceX / Hookdistance));       // 1 -> 0 방향으로 애니메이션 작동
            

            if (Input.GetKeyDown(KeyCode.Space))                    // 스페이스바 누를 시
            {
                Hookoff();                                          // 훅 끄는 함수
            }

            if (Input.GetMouseButton(1))                            // 오른쪽 클릭을 하면
            {                                                                                 
                hook.GetComponent<Hook>().playerToHookJoint.distance -= HookHoldSpeed * 0.003f;// 플레이어 방향 쪽으로 훅 길이 줄이기 
            }
        }
    }

    public void Hookoff()
    {
        isAttatch = false;                                  // 안붙은 생태
        isHookActive = false;                               // 훅 사용가능 상태
        isMaxRangeLimit = false;                            // 훅 최대사거리 도달 안된 상태
        hook.GetComponent<Hook>().playerToHookJoint.enabled = false;    // 훅 떼기
        hook.gameObject.SetActive(false);                   // 오브젝트 off
        animator.SetBool("_Hanging", false);                // 매달리기 애니메이션 off
    }
}
