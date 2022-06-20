using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    Player player;
    Rigidbody2D rb2d;
    SpriteRenderer sprite;
    Grappling grappling;        // �׷��� �پ����� ��� velocity��� Addforce�� �����ϱ� ���� ����
    Animator animator;          // �ִϸ��̼ǿ�

    [System.Serializable]
    public class Conditions
    {
        /// <summary>
        /// �뽬 ���� ����
        /// <br />�뽬 ���� �� ��Ÿ�� ���ÿ��� ���
        /// </summary>
        [ReadOnly] public bool isDash = false;
        [ReadOnly] public float CurrDashTimer;
        public float DefaultDashCooldown;
        [ReadOnly] public Vector2 DashDir;
        public float DashSpeed = 20;
        public float DashCooldown = 2;
        /// <summary>
        /// ���� �� / ��� �����̴��� ����
        /// <br />��������Ʈ ���� ���ε ���
        /// </summary>
        [ReadOnly] public bool isMoving = false;
        /// <summary>
        /// ���� ���� ����ִ��� ����
        /// <br />�Ϲ� ���� / ���� ���� ����
        /// </summary>
        [ReadOnly] public bool isGround = false;
        /// <summary>
        /// �÷��̾ �������� �����ִ��� ����
        /// <br />Sprite.flipX�� ray���� �� ����
        /// </summary>
        //[ReadOnly] public bool lookRight = true;
        /// <summary>
        /// 'W'Ű�� ���� ���� ����
        /// <br />���� ���� ������ ���� ���
        /// </summary>
        [ReadOnly] public bool pressedWKey = false;
    }
    [System.Serializable]
    public class JumpStats
    {
        [Header("���� �Ӽ�")]
        //[Tooltip("�߰� ���� Ƚ��(���� ����)")] public uint extraJumpCount = 1;
        //[ReadOnly, Tooltip("���� ���� �߰� ���� Ƚ��")]
        //public uint extraJumpCounter = 0;
        [Tooltip("�⺻ ���� ����(���� ���¿�����)")] public float baseJumpHeight = 6.01f;
        [Tooltip("���� ���� ����(WŰ�� ����)")] public float highestJumpHeight = 8.71f;
        [Tooltip("���� �� ���ӵ� ����(�̵� �� ����)\n���ӵ�: ������ �� ����� (��/�� �ӵ� * �ش� ����)\n���� �̱���")] public float accelerationMagnification = 0.22f;

        [Header("���� ����")]
        [Tooltip("���� �� ��ٿ�\n���� �� ������ ���ٸ� �ִϸ��̼��� �ڿ��������̳� Ű ���� �Է� �� ������ �ֱ� ������ �J�� ������ �ִ� �� ���")] public float jumpCooldown = 0.19f;
        [ReadOnly, Tooltip("���� ���� ��ٿ�")]
        public float jumpCooldownCounter = 0.00f;

        public void InitializeVariables()
        {
            //extraJumpCounter = extraJumpCount;
            jumpCooldownCounter = jumpCooldown;
        }
    }
    /// <summary>
    /// ������ ���õ� ������ ��Ƴ��� ��ü
    /// </summary>
    public JumpStats jumpStats = new JumpStats();
    [Space]
    [SerializeField] float groundCheckRayLength = 0.8f; //�� üũ Ray ����

    [Header("���� ���� ������")]
    /// <summary>
    /// ���� ���ǵ��� ��Ƴ��� ��ü
    /// </summary>
    public Conditions conditions = new Conditions();

    Vector2 movement = Vector2.zero;

    /// <summary>
    /// Ű Input���� PlayerInputManager�� �߰��ϴ� ����
    /// </summary>
    void AddKeyBindInputManager()
    {
        PlayerInputManager.instance.player_move.performed += GetInputMovement;
        PlayerInputManager.instance.player_move.canceled += GetInputMovement;
        PlayerInputManager.instance.player_move.started += GetConditionIsMoving;
        PlayerInputManager.instance.player_move.canceled += GetConditionIsMoving;
        PlayerInputManager.instance.player_jump.started += TryJump;
        PlayerInputManager.instance.player_pressW.started += GetConditionPressW;
        PlayerInputManager.instance.player_pressW.canceled += GetConditionPressW;
    }

    private void Awake()
    {
        player = GetComponent<Player>();
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        grappling = GetComponent<Grappling>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        AddKeyBindInputManager();
        jumpStats.InitializeVariables();

        conditions.DefaultDashCooldown = conditions.DashCooldown;
    }

    private void Update()
    {
        // �ȱ�
        if (grappling.isAttatch)                                        // �׷��� �پ����� ���
            rb2d.AddForce(new Vector2(movement.x * 0.15f, 0));
        else                                                            // �׷��� �پ����� ���� ���
            rb2d.velocity = new Vector2(movement.x, rb2d.velocity.y);

        // �뽬
        if (Input.GetKeyDown(KeyCode.LeftShift) && !conditions.isDash && conditions.DashCooldown <= 0)
        {
            conditions.isDash = true;
            conditions.CurrDashTimer = 0.3f;
            conditions.DashCooldown = conditions.DefaultDashCooldown;
            rb2d.velocity = Vector2.zero;

            if (sprite.flipX)
                conditions.DashDir = transform.right;
            else
                conditions.DashDir = -transform.right;

            animator.Play("Dash");
            animator.SetBool("_Dash",true);
        }
        if(conditions.isDash)
        {
                rb2d.velocity = conditions.DashDir * conditions.DashSpeed;

            conditions.CurrDashTimer -= Time.deltaTime;

            if (conditions.CurrDashTimer <= 0)
            {
                conditions.isDash = false;
                animator.SetBool("_Dash", false);
            }
        }
        if(conditions.DashCooldown >= 0)
            conditions.DashCooldown -= Time.deltaTime;

        // ����
        if (jumpStats.jumpCooldownCounter >= 0.00f)                     
        {
            jumpStats.jumpCooldownCounter -= Time.deltaTime;
        }
        else
        {
            if(conditions.isGround)                                     // ���� ������ Jump �ִϸ��̼� off
                animator.SetBool("_Jump", false);
        }

        // �ȱ� �ִϸ��̼�
        if (conditions.isGround)                                        // ���� ����� ��
        {
            if (conditions.isMoving)                                    // �̵��� �̶��
            {
                animator.SetBool("_Running", true);                     // Running �ִϸ��̼� on
            }
            else                                                        // �̵����� �ƴ϶��
            {
                animator.SetBool("_Running", false);                    // Running �ִϸ��̼� off
            }
        }
        else                                                            // ���� �ƴϿ��� Running �ִϸ��̼� off
        {
            animator.SetBool("_Running", false);
        }



        CheckGroundWithRaycast();
    }

    /// <summary>
    /// conditions.isGround�� ���� �������ִ� ����
    /// </summary>
    void CheckGroundWithRaycast()
    {
        RaycastHit2D DrayHit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckRayLength, 1 << 8); // 8 == Ground Layer
        Debug.DrawRay(transform.position, Vector2.down * groundCheckRayLength, Color.red, Time.deltaTime);
        //if (conditions.isGround == false && DrayHit) //���� ������� �ʴٰ� -> ���� ���� ���
        //{
        //    jumpStats.extraJumpCounter = jumpStats.extraJumpCount; //����Ʈ�� ���� Ƚ�� �ʱ�ȭ
        //}
        conditions.isGround = DrayHit;
        Debug.Log($"{DrayHit.collider?.gameObject.name}");
    }

    /// <summary>
    /// �÷��̾��� Ű���忡�� �Է°��� �ް� �������� �ǹ��ϴ� Vector2�� ���� �ٲ��ִ� ����
    /// </summary>
    void GetInputMovement(InputAction.CallbackContext _context)
    {
        //Ű �Է��� ���� �ʴ� ����
        if (_context.action.enabled == false)
        {
            return;
        }

        Debug.Log($"{_context.ReadValue<Vector2>()}");
        movement = _context.ReadValue<Vector2>() * player.playerStat.movementSpeed;
        //���� �߰��ؾ���

        if (!conditions.isDash)
        {
            if (movement.x < 0)
                sprite.flipX = false;
            if (movement.x > 0)
                sprite.flipX = true;
        }
    }

    /// <summary>
    /// �÷��̾ Player.Jump�� ������ �� ������ �õ���
    /// </summary>
    /// <param name="_context"></param>
    void TryJump(InputAction.CallbackContext _context)
    {
        //���� �Ϲ� �������� ���� �������� �����ϴ� ���� ����
        //�߰��ؾ���
        if (conditions.isGround == false /*&& jumpStats.extraJumpCounter < 1*/)
        {
            //Debug.Log("���� �Ұ�: ���� ���� Ƚ�� ��� ����");
            Debug.Log("���� �Ұ�: Ground�� �ƴ�");
            return;
        }
        if (jumpStats.jumpCooldownCounter > 0f)
        {
            Debug.Log("���� �Ұ�: ��ٿ���");
            return;
        }

        
        //else if (conditions.isGround == false && grappling.isAttatch == false)  //�����ε� ���̾�׼� ���Ұ��
        //{
        //    jumpStats.extraJumpCounter -= 1;
        //}

        if (grappling.isAttatch == false) //�����ε� ���̾�׼� ���� ��� ����X
            Jump();
    }

    /// <summary>
    /// ������ �����Ű�� �޼ҵ�
    /// </summary>
    void Jump()
    {
        jumpStats.jumpCooldownCounter = jumpStats.jumpCooldown;

        float finalJumpForce = jumpStats.baseJumpHeight;

        if (conditions.pressedWKey)
        {
            finalJumpForce = jumpStats.highestJumpHeight;
        }
        /* �� �����̶� ���� ȿ��
        if (movement.x != 0) //�� �Ǵ� ��� �̵���(A or DŰ ����)
        {
            finalJumpForce = jumpStats.baseJumpHeight;
        }
        else if (conditions.pressedWKey) //WŰ�� ���� ���¶��
        {
            finalJumpForce = jumpStats.highestJumpHeight;
        }
        else //!(�̵��� && WŰ ����) == Idle
        {
            finalJumpForce = jumpStats.baseJumpHeight;
        }
        */
        //�������� ���¿��� ������ ���� ��(velocity.y < 0) AddForce�� �ǵ��Ѵ�� �۵���Ű�� ���� velocity.y = 0���� �ʱ�ȭ
        rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
        rb2d.AddForce(new Vector2(0, finalJumpForce), ForceMode2D.Impulse);

        conditions.isGround = false;
        animator.SetBool("_Running", false);
        animator.SetBool("_Dash", false);
        animator.SetBool("_Jump", true);
        animator.Play("Jump");

        if (rb2d.velocity.x != 0) //������� ��/�� �̵����̾��ٸ�
        {
            HorizontalAcceleration(rb2d, jumpStats.accelerationMagnification);
        }
    }

    /// <summary>
    /// ���� �������� ���ӵ��� �ִ� ����.
    /// <br />���� �� ������ �ִ� �� ���
    /// </summary>
    /// <param name="_playerRb2D">�÷��̾��� Rigidbody2D Component</param>
    /// <param name="_accelerationMagnification">���ӷ� ����</param>
    void HorizontalAcceleration(Rigidbody2D _playerRb2D, float _accelerationMagnification)
    {
        float accelerationAmount = _playerRb2D.velocity.x;
        accelerationAmount *= _accelerationMagnification;

        _playerRb2D.AddForce(new Vector2(accelerationAmount, 0));
    }

    void GetConditionPressW(InputAction.CallbackContext _context)
    {
        if (_context.started)
        {
            conditions.pressedWKey = true;
        }
        else if (_context.canceled)
        {
            conditions.pressedWKey = false;
        }
        else
        {
            Debug.LogError("����: �ڵ� Ȯ�ιٶ�");
        }
    }

    void GetConditionIsMoving(InputAction.CallbackContext _context)
    {
        if (_context.started)
        {
            conditions.isMoving = true;
        }
        else if (_context.canceled)
        {
            conditions.isMoving = false;
        }
        else
        {
            Debug.LogError("����: �ڵ� Ȯ�ιٶ�");
        }
    }

    ///���� ���� ���
    ///�ʿ� ����: 
    ///���� �� ��ٿ�(2�� �����̱� ������)
    ///���� ����������(2�� ���� ����� ���� ���)
    ///���� ���� ��� �ִ���(�⺻ ����, �߰� ���� ����)
    ///�ʿ� �ൿ: 
    ///�÷��̾� ���� ������Ʈ�� ���� Player.cs��ü(������Ʈ)�� ChangeState()�� ����Ͽ�
    ///���� ���¸� ��ȯ�ϴ� ������ �ʿ���
    ///���� �� �Ӽ�: 
    ///������ư ������ ���� Ű(����Ű)�� �Ǵ��ϴ� ���� �ʿ�
    ///
    /// bool isGround;
    /// int extraJumpCount
    /// 
    ///velocity.y�� ������� �����������̴� �׶� falling Animation�� ����?
}
