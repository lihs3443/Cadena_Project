using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Hook(갈고리) 관련 코드
public class Hook : MonoBehaviour
{
    Grappling grappling;
    public DistanceJoint2D playerToHookJoint;   //플레이어와 훅을 연결하는 조인트
    public HingeJoint2D hookToBoardJoint;    //훅과 보드를 연결하는 조인트

    public MoveBlock block;
    public HangingBlock hBlock;     //hangingBlock을 얻어오기 위한 변수

    Collider2D attatchedBoard;

    void Start()
    {
        grappling = GameObject.Find("Player").GetComponent<Grappling>();
        playerToHookJoint = GetComponent<DistanceJoint2D>();
        playerToHookJoint.enabled = false;
        hookToBoardJoint = GetComponent<HingeJoint2D>();
        hookToBoardJoint.enabled = false;
    }

    private void Update()
    {
        if (block.isMove == true)//블럭이 움직이면 훅도 같이 움직여야 하는데 .. 블럭이 움직이는 값을 얻어와서 훅도 그만큼 움직이게 한다
        {
            transform.position += Vector3.right * block.speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Ground"))
        {
            //플레이어와 훅 연결(대롱대롱)
            playerToHookJoint.distance = Vector2.Distance(grappling.transform.position, transform.position); // 플레이어 위치를 기준으로 훅의 길이 조절
            playerToHookJoint.anchor = new Vector2(0, 0);           // 훅의 길이설정 초기화
            playerToHookJoint.enabled = true;                       // 훅과 플레이어를 연결

            /* 오류부분
            //훅과 보드 연결
            attatchedBoard = collision;
            hookToBoardJoint.connectedBody = attatchedBoard.attachedRigidbody;
            hookToBoardJoint.enabled = true;
            */

            grappling.isAttatch = true;                 // 훅이 붙은 상태

            if (collision.GetComponent<MoveBlock>() != null) //해당 플랫폼에 스크립트가 있는지 체크한다.
            {
                block = collision.GetComponent<MoveBlock>();

                block.isMove = true;
            }

            if (collision.GetComponent<HangingBlock>() != null) //해당 플랫폼에 스크립트가 있는지 체크한다.
            {
                hBlock = collision.GetComponent<HangingBlock>();

                hBlock.isHanging = true;
            }

            // 훅 부착시 카메라 흔들기 
            StartCoroutine(CameraShake.instance.Shake(grappling.cameraShakeduration, grappling.cameraShakeMagnitude));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        block.isMove = false;
    }
}
