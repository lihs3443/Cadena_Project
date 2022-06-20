using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    public LineRenderer line;       // �÷��̾�� �Ű��� ���� ����Ʈ
    public Transform hook;          // ���� ��ġ
    public Vector3 PlayerPos;       // �÷��̾� ��ġ, ù ��ġ ������ ���� ����
    SpriteRenderer sprite;          // �� �߻��� �� ���콺 �������� ĳ���Ͱ� �ٶ󺸵��� �� ����
    Vector2 MousePos;               // ���콺 ��ġ
    float HookAngle;
    Animator animator;

    public float HookSpeed;         // �� ���ư��� �ӵ�
    public float HookRange;         // �� ��Ÿ�
    public float HookReturnSpeed;   // �� ���ƿ��� �ӵ�
    public float HookHoldSpeed;     // �� ���� �ӵ�

    public bool isHookActive;       // ���� ���ư��� �� true
    public bool isMaxRangeLimit;    // ���� �Ѱ������� �������� �� true
    public bool isAttatch;          // ���� �پ��� ��� true

    void Start()
    {
        animator = GetComponent<Animator>();        // �ִϸ��̼� ����
        sprite = GetComponent<SpriteRenderer>();    // ���� �߻��ϴ� ������ �ٶ󺸵��� �ϱ� ���� ����
        PlayerPos = transform.position;             // ���� ������ ��ġ ����
        isHookActive = false;
        isMaxRangeLimit = false;
        isAttatch = false;
        hook.gameObject.SetActive(false);

// �ʱⰪ ����(�ܺ� ���� ���� ���)
//        HookSpeed = 50f;
//        HookRange = 10f;
        HookReturnSpeed = 30f;
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


        // ���콺 ����Ŭ�� && ���� �������� && �Ⱥپ�����
        if (Input.GetMouseButtonDown(0) && !isHookActive && !isAttatch)
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

            // ���� �ִ� ��Ÿ��� �������� �� ���� ���ƿ��� ����
            //hook.position = Vector2.MoveTowards(hook.position, PlayerPos, Time.deltaTime * HookReturnSpeed);    // �� ���ƿ�
            //if (Vector2.Distance(PlayerPos, hook.position) < 0.1f)  // ���� ���ƿ��� ��
            //{
            //    isHookActive = false;                               // �� ��밡�� ����
            //    isMaxRangeLimit = false;                            // �� �ִ��Ÿ� ���� �ȵ� ����
            //    hook.gameObject.SetActive(false);                   // ������Ʈ off
            //}
        }

        // ���� �پ��� ��
        if (isAttatch)
        {
            // �浹�� ���̾� ȸ���� ���� Ray ���� ��..
            //RaycastHit2D LrayHit = Physics2D.Raycast(PlayerPos, Vector2.left, 0.35f, 1 << 8);   // �÷��̾ ���� ���� �浹�� ���� ȸ���� �ϱ� ���� Ray
            //RaycastHit2D RrayHit = Physics2D.Raycast(PlayerPos, Vector2.right, 0.35f, 1 << 8);  // �÷��̾ ������ ���� �浹�� ���� ȸ���� �ϱ� ���� Ray
            //RaycastHit2D UrayHit = Physics2D.Raycast(PlayerPos, Vector2.up, 0.50f, 1 << 8);     // �÷��̾ ���� ���� �浹�� ���� ȸ���� �ϱ� ���� Ray
            //RaycastHit2D DrayHit = Physics2D.Raycast(PlayerPos, Vector2.down, 1.33f, 1 << 8);   // �÷��̾ �Ʒ��� ���� �浹�� ���� ȸ���� �ϱ� ���� Ray

            if (Input.GetKeyDown(KeyCode.Space))                    // ����Ŭ�� �ѹ� �� ���� ��
            {
                Hookoff();                                          // �� ���� �Լ�
            }

            if (Input.GetMouseButton(1))                            // ������ Ŭ���� �ϸ�
            {                                                                                 
                hook.GetComponent<Hook>().joint.distance -= HookHoldSpeed * 0.003f;// �÷��̾� ���� ������ �� ���� ���̱� 
            }
        }
    }

    public void Hookoff()
    {
        isAttatch = false;                                  // �Ⱥ��� ����
        isHookActive = false;                               // �� ��밡�� ����
        isMaxRangeLimit = false;                            // �� �ִ��Ÿ� ���� �ȵ� ����
        hook.GetComponent<Hook>().joint.enabled = false;    // �� ����
        hook.gameObject.SetActive(false);                   // ������Ʈ off
    }
}
