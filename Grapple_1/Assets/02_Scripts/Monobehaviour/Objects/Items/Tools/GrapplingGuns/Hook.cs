using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Hook(����) ���� �ڵ�
public class Hook : MonoBehaviour
{
    Grappling grappling;
    public DistanceJoint2D playerToHookJoint;   //�÷��̾�� ���� �����ϴ� ����Ʈ
    public HingeJoint2D hookToBoardJoint;    //�Ű� ���带 �����ϴ� ����Ʈ

    public MoveBlock block;
    public HangingBlock hBlock;     //hangingBlock�� ������ ���� ����

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
        if (block.isMove == true)//���� �����̸� �ŵ� ���� �������� �ϴµ� .. ���� �����̴� ���� ���ͼ� �ŵ� �׸�ŭ �����̰� �Ѵ�
        {
            transform.position += Vector3.right * block.speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Ground"))
        {
            //�÷��̾�� �� ����(��մ��)
            playerToHookJoint.distance = Vector2.Distance(grappling.transform.position, transform.position); // �÷��̾� ��ġ�� �������� ���� ���� ����
            playerToHookJoint.anchor = new Vector2(0, 0);           // ���� ���̼��� �ʱ�ȭ
            playerToHookJoint.enabled = true;                       // �Ű� �÷��̾ ����

            /* �����κ�
            //�Ű� ���� ����
            attatchedBoard = collision;
            hookToBoardJoint.connectedBody = attatchedBoard.attachedRigidbody;
            hookToBoardJoint.enabled = true;
            */

            grappling.isAttatch = true;                 // ���� ���� ����

            if (collision.GetComponent<MoveBlock>() != null) //�ش� �÷����� ��ũ��Ʈ�� �ִ��� üũ�Ѵ�.
            {
                block = collision.GetComponent<MoveBlock>();

                block.isMove = true;
            }

            if (collision.GetComponent<HangingBlock>() != null) //�ش� �÷����� ��ũ��Ʈ�� �ִ��� üũ�Ѵ�.
            {
                hBlock = collision.GetComponent<HangingBlock>();

                hBlock.isHanging = true;
            }

            // �� ������ ī�޶� ���� 
            StartCoroutine(CameraShake.instance.Shake(grappling.cameraShakeduration, grappling.cameraShakeMagnitude));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        block.isMove = false;
    }
}
