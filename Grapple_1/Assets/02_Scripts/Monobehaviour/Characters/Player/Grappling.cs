using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    public LineRenderer line;                  // �÷��̾�� �������� ���� ����Ʈ
    public Transform hook;                     // ���� ��ġ
    [ReadOnly] public Vector3 PlayerPos;       // �÷��̾� ��ġ, ù ��ġ ������ ���� ����
    SpriteRenderer sprite;                     // �� �߻��� �� ���콺 �������� ĳ���Ͱ� �ٶ󺸵��� �� ����
    Vector2 MousePos;                          // ���콺 ��ġ
    float HookAngle;
    Animator animator;
    MovementController movectr;

    public float HookSpeed;                    // �� ���ư��� �ӵ�
    public float HookRange;                    // �� ��Ÿ�
    public float HookHoldSpeed;                // �� ���� �ӵ�
    public float cameraShakeduration;          // ���� �پ��� �� ī�޶� ��鸲 ��(�ð�)
    public float cameraShakeMagnitude;         // ���� �پ��� �� ī�޶� ��鸲 ����(����)

    [ReadOnly] public bool isHookActive;       // ���� ���ư��� �� true
    [ReadOnly] public bool isMaxRangeLimit;    // ���� �Ѱ������� �������� �� true
    [ReadOnly] public bool isAttatch;          // ���� �پ��� ��� true

    void Start()
    {
        animator = GetComponent<Animator>();        // �ִϸ��̼� ����
        sprite = GetComponent<SpriteRenderer>();    // ���� �߻��ϴ� ������ �ٶ󺸵��� �ϱ� ���� ����
        movectr = GetComponent<MovementController>();
        PlayerPos = transform.position;             // ���� ������ ��ġ ����
        isHookActive = false;
        isMaxRangeLimit = false;
        isAttatch = false;
        hook.gameObject.SetActive(false);

// �ʱⰪ ����(�ܺ� ���� ���� ���)
//        HookSpeed = 50f;
//        HookRange = 10f;
        HookHoldSpeed = 3f;

        line.positionCount = 2;                     // ������ �׸��� ��ġ�� ����
        line.endWidth = line.startWidth = 0.02f;    // ������ �β�
        line.SetPosition(0, PlayerPos);             // ������ �׸��� �������� ��ġ, �÷��̾��� ��ġ
        line.SetPosition(1, hook.position);         // ������ �׸��� ���������� ��ġ, ���� ��ġ
        line.useWorldSpace = true;
    }

    void Update()
    {
        PlayerPos = transform.position;    
        line.SetPosition(0, PlayerPos);             // ������ �׸��� �������� ��ġ, �÷��̾��� ��ġ
        line.SetPosition(1, hook.position);         // ������ �׸��� ���������� ��ġ, ���� ��ġ

        // ���콺 ����Ŭ�� && ���� �������� && �Ⱥپ����� && ���� �ƴ�
        if (Input.GetMouseButtonDown(0) && !isHookActive && !isAttatch && !movectr.conditions.isGround)
        {
            hook.position = PlayerPos;                                                      // �� ������ġ ����
            MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - PlayerPos;     // ���콺���� ����

            HookAngle = Mathf.Atan2(MousePos.y, MousePos.x) * Mathf.Rad2Deg;                // ���� ���ư� ���� ���
            hook.transform.rotation = Quaternion.AngleAxis(HookAngle-90,Vector3.forward);   // ���� ���� ����

            if (MousePos.x > 0)                                                             // ���콺�� ĳ���� ���ʿ� ���� ��� ���� �ٶ󺸱�
                sprite.flipX = true;                                            // �� �߻��
            if (MousePos.x < 0)                                                             // ���콺�� ĳ���� �����ʿ� ���� ��� ������ �ٶ󺸱�
                sprite.flipX = false;

            animator.SetBool("_WireShoting", true);                                         // ���̾� �߻� �ִϸ��̼� on

            isHookActive = true;                                                            // �� on
            isMaxRangeLimit = false;
            hook.gameObject.SetActive(true);                                                // �� ������Ʈ on
        }

        if(Input.GetMouseButtonUp(0))
        {
            animator.SetBool("_WireShoting", false);                                        // ���̾� �߻� �ִϸ��̼� off
        }

        // ���� �������� && �ִ��Ÿ� ���� ���� && �Ⱥپ�����
        if (isHookActive && !isMaxRangeLimit && !isAttatch)
        {
            hook.Translate(Vector2.up * Time.deltaTime * HookSpeed);                   // �� ���ư�

            if (Vector2.Distance(PlayerPos, hook.position) > HookRange)                // �� ��Ÿ���ŭ ���ư���    
                isMaxRangeLimit = true;
        }
        
        // ���� �������� && �ִ��Ÿ� ������ && �Ⱥپ�����
        if (isHookActive && isMaxRangeLimit && !isAttatch)
        {
            isHookActive = false;
            isMaxRangeLimit = false;
            hook.gameObject.SetActive(false);
        }

        // ���� �پ��� ��
        if (isAttatch)
        {
            /// ���� Ʈ���� �̿��� �ִϸ��̼� ó���� ���� �۾�
            float Hookdistance = (line.GetPosition(1) - line.GetPosition(0)).magnitude; // �� ����
            float distanceX = line.GetPosition(1).x - line.GetPosition(0).x;            // �Ű� �÷��̾��� X�� ����

            if (!sprite.flipX)
                distanceX = -distanceX;
            animator.SetBool("_Hanging", true);
            animator.SetFloat("HangingAngle", 0.5f + (distanceX / Hookdistance));       // 1 -> 0 �������� �ִϸ��̼� �۵�
            

            if (Input.GetKeyDown(KeyCode.Space))                    // �����̽��� ���� ��
            {
                Hookoff();                                          // �� ���� �Լ�
            }

            if (Input.GetMouseButton(1))                            // ������ Ŭ���� �ϸ�
            {                                                                                 
                hook.GetComponent<Hook>().playerToHookJoint.distance -= HookHoldSpeed * 0.003f;// �÷��̾� ���� ������ �� ���� ���̱� 
            }
        }
    }

    public void Hookoff()
    {
        isAttatch = false;                                  // �Ⱥ��� ����
        isHookActive = false;                               // �� ��밡�� ����
        isMaxRangeLimit = false;                            // �� �ִ��Ÿ� ���� �ȵ� ����
        hook.GetComponent<Hook>().playerToHookJoint.enabled = false;    // �� ����
        hook.gameObject.SetActive(false);                   // ������Ʈ off
        animator.SetBool("_Hanging", false);                // �Ŵ޸��� �ִϸ��̼� off
    }
}
