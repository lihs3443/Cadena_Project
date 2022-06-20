using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    Player player;
    Rigidbody2D rb2d;
    SpriteRenderer sprite;
    Grappling grappling;        // 그랩이 붙어있을 경우 velocity대신 Addforce로 변경하기 위한 변수
    Animator animator;          // 애니메이션용

    [System.Serializable]
    public class Conditions
    {
        /// <summary>
        /// 대쉬 상태 여부
        /// <br />대쉬 시전 및 쿨타임 관련에서 사용
        /// </summary>
        [ReadOnly] public bool isDash = false;
        [ReadOnly] public float CurrDashTimer;
        public float DefaultDashCooldown;
        [ReadOnly] public Vector2 DashDir;
        public float DashSpeed = 20;
        public float DashCooldown = 2;
        /// <summary>
        /// 현재 좌 / 우로 움직이는지 여부
        /// <br />스프라이트 반전 여부등에 사용
        /// </summary>
        [ReadOnly] public bool isMoving = false;
        /// <summary>
        /// 현재 땅을 밟고있는지 여부
        /// <br />일반 점프 / 공중 점프 구분
        /// </summary>
        [ReadOnly] public bool isGround = false;
        /// <summary>
        /// 플레이어가 오른쪽을 보고있는지 여부
        /// <br />Sprite.flipX나 ray방향 등 선정
        /// </summary>
        //[ReadOnly] public bool lookRight = true;
        /// <summary>
        /// 'W'키가 눌린 상태 여부
        /// <br />점프 높이 조절을 위해 사용
        /// </summary>
        [ReadOnly] public bool pressedWKey = false;
    }
    [System.Serializable]
    public class JumpStats
    {
        [Header("점프 속성")]
        //[Tooltip("추가 점프 횟수(공중 점프)")] public uint extraJumpCount = 1;
        //[ReadOnly, Tooltip("현재 남은 추가 점프 횟수")]
        //public uint extraJumpCounter = 0;
        [Tooltip("기본 점프 높이(정지 상태에서의)")] public float baseJumpHeight = 6.01f;
        [Tooltip("높은 점프 높이(W키를 누름)")] public float highestJumpHeight = 8.71f;
        [Tooltip("점프 중 가속도 배율(이동 중 점프)\n가속도: 점프를 할 당시의 (좌/우 속도 * 해당 배율)\n현재 미구현")] public float accelerationMagnification = 0.22f;

        [Header("점프 스탯")]
        [Tooltip("점프 간 쿨다운\n점프 간 간격이 없다면 애니메이션의 자연스러움이나 키 다중 입력 등 문제가 있기 때문에 짫은 간격을 주는 데 사용")] public float jumpCooldown = 0.19f;
        [ReadOnly, Tooltip("현재 점프 쿨다운")]
        public float jumpCooldownCounter = 0.00f;

        public void InitializeVariables()
        {
            //extraJumpCounter = extraJumpCount;
            jumpCooldownCounter = jumpCooldown;
        }
    }
    /// <summary>
    /// 점프에 관련된 정보를 모아놓은 객체
    /// </summary>
    public JumpStats jumpStats = new JumpStats();
    [Space]
    [SerializeField] float groundCheckRayLength = 0.8f; //땅 체크 Ray 길이

    [Header("각종 조건 디버깅용")]
    /// <summary>
    /// 각종 조건들을 모아놓은 객체
    /// </summary>
    public Conditions conditions = new Conditions();

    Vector2 movement = Vector2.zero;

    /// <summary>
    /// 키 Input들을 PlayerInputManager에 추가하는 역할
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
        // 걷기
        if (grappling.isAttatch)                                        // 그랩이 붙어있을 경우
            rb2d.AddForce(new Vector2(movement.x * 0.15f, 0));
        else                                                            // 그랩이 붙어있지 않을 경우
            rb2d.velocity = new Vector2(movement.x, rb2d.velocity.y);

        // 대쉬
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

        // 점프
        if (jumpStats.jumpCooldownCounter >= 0.00f)                     
        {
            jumpStats.jumpCooldownCounter -= Time.deltaTime;
        }
        else
        {
            if(conditions.isGround)                                     // 땅에 닿으면 Jump 애니메이션 off
                animator.SetBool("_Jump", false);
        }

        // 걷기 애니메이션
        if (conditions.isGround)                                        // 땅에 닿았을 때
        {
            if (conditions.isMoving)                                    // 이동중 이라면
            {
                animator.SetBool("_Running", true);                     // Running 애니메이션 on
            }
            else                                                        // 이동중이 아니라면
            {
                animator.SetBool("_Running", false);                    // Running 애니메이션 off
            }
        }
        else                                                            // 땅이 아니여도 Running 애니메이션 off
        {
            animator.SetBool("_Running", false);
        }



        CheckGroundWithRaycast();
    }

    /// <summary>
    /// conditions.isGround의 값을 변경해주는 역할
    /// </summary>
    void CheckGroundWithRaycast()
    {
        RaycastHit2D DrayHit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckRayLength, 1 << 8); // 8 == Ground Layer
        Debug.DrawRay(transform.position, Vector2.down * groundCheckRayLength, Color.red, Time.deltaTime);
        //if (conditions.isGround == false && DrayHit) //땅을 밟고있지 않다가 -> 땅을 밟은 경우
        //{
        //    jumpStats.extraJumpCounter = jumpStats.extraJumpCount; //엑스트라 점프 횟수 초기화
        //}
        conditions.isGround = DrayHit;
        Debug.Log($"{DrayHit.collider?.gameObject.name}");
    }

    /// <summary>
    /// 플레이어의 키보드에서 입력값을 받고 움직임을 의미하는 Vector2의 값을 바꿔주는 역할
    /// </summary>
    void GetInputMovement(InputAction.CallbackContext _context)
    {
        //키 입력을 받지 않는 상태
        if (_context.action.enabled == false)
        {
            return;
        }

        Debug.Log($"{_context.ReadValue<Vector2>()}");
        movement = _context.ReadValue<Vector2>() * player.playerStat.movementSpeed;
        //조건 추가해야함

        if (!conditions.isDash)
        {
            if (movement.x < 0)
                sprite.flipX = false;
            if (movement.x > 0)
                sprite.flipX = true;
        }
    }

    /// <summary>
    /// 플레이어가 Player.Jump를 눌렀을 때 점프를 시도함
    /// </summary>
    /// <param name="_context"></param>
    void TryJump(InputAction.CallbackContext _context)
    {
        //현재 일반 점프인지 공중 점프인지 구분하는 조건 없음
        //추가해야함
        if (conditions.isGround == false /*&& jumpStats.extraJumpCounter < 1*/)
        {
            //Debug.Log("점프 불가: 공중 점프 횟수 모두 소진");
            Debug.Log("점프 불가: Ground가 아님");
            return;
        }
        if (jumpStats.jumpCooldownCounter > 0f)
        {
            Debug.Log("점프 불가: 쿨다운중");
            return;
        }

        
        //else if (conditions.isGround == false && grappling.isAttatch == false)  //공중인데 와이어액션 안할경우
        //{
        //    jumpStats.extraJumpCounter -= 1;
        //}

        if (grappling.isAttatch == false) //공중인데 와이어액션 중일 경우 점프X
            Jump();
    }

    /// <summary>
    /// 점프를 실행시키는 메소드
    /// </summary>
    void Jump()
    {
        jumpStats.jumpCooldownCounter = jumpStats.jumpCooldown;

        float finalJumpForce = jumpStats.baseJumpHeight;

        if (conditions.pressedWKey)
        {
            finalJumpForce = jumpStats.highestJumpHeight;
        }
        /* 위 내용이랑 같은 효과
        if (movement.x != 0) //좌 또는 우로 이동중(A or D키 누름)
        {
            finalJumpForce = jumpStats.baseJumpHeight;
        }
        else if (conditions.pressedWKey) //W키가 눌린 상태라면
        {
            finalJumpForce = jumpStats.highestJumpHeight;
        }
        else //!(이동중 && W키 눌림) == Idle
        {
            finalJumpForce = jumpStats.baseJumpHeight;
        }
        */
        //떨어지는 상태에서 점프를 했을 때(velocity.y < 0) AddForce를 의도한대로 작동시키기 위해 velocity.y = 0으로 초기화
        rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
        rb2d.AddForce(new Vector2(0, finalJumpForce), ForceMode2D.Impulse);

        conditions.isGround = false;
        animator.SetBool("_Running", false);
        animator.SetBool("_Dash", false);
        animator.SetBool("_Jump", true);
        animator.Play("Jump");

        if (rb2d.velocity.x != 0) //점프당시 좌/우 이동중이었다면
        {
            HorizontalAcceleration(rb2d, jumpStats.accelerationMagnification);
        }
    }

    /// <summary>
    /// 수평 방향으로 가속도를 주는 역할.
    /// <br />점프 시 가속을 주는 데 사용
    /// </summary>
    /// <param name="_playerRb2D">플레이어의 Rigidbody2D Component</param>
    /// <param name="_accelerationMagnification">가속량 배율</param>
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
            Debug.LogError("에러: 코드 확인바람");
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
            Debug.LogError("에러: 코드 확인바람");
        }
    }

    ///점프 구현 방식
    ///필요 변수: 
    ///점프 간 쿨다운(2단 점프이기 때문에)
    ///현재 점프중인지(2단 점프 모션이 있을 경우)
    ///현재 땅을 밟고 있는지(기본 점프, 추가 점프 구분)
    ///필요 행동: 
    ///플레이어 게임 오브젝트가 가진 Player.cs객체(컴포넌트)의 ChangeState()를 사용하여
    ///현재 상태를 전환하는 행위가 필요함
    ///조건 및 속성: 
    ///점프버튼 이전에 누른 키(조합키)를 판단하는 행위 필요
    ///
    /// bool isGround;
    /// int extraJumpCount
    /// 
    ///velocity.y가 음수라면 떨어지는중이니 그때 falling Animation을 실행?
}
