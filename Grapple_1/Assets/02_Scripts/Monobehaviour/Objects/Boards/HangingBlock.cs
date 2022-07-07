using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�÷��̾ ������ �Ŵްų� �� ���� �ö󰡴� ��
//���� ���Ը� ���ϴ� ������ �� �� �������� ���Դϴ�.
public class HangingBlock : MonoBehaviour
{
    public bool isHanging = false;  //�Ŵ޷� �ִ���.. + �� hook�̳� grappling���� �޾ƿ��°� ������
    public float speed = 3.0f;     //�ϰ� �ӵ�
    public float landSpeed; //��� �ӵ�
    public float resetTime; //�Է� ���� �� �� �� ����ġ �ϴ���
    public float topPos;         //�ֻ� ����
    public float bottomPos;      //���� ����

    
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //�÷��̾ ���� �Ŵ޷� ���� ��
        if(isHanging == true)
        {
            if(transform.position.y >= bottomPos)//���� ���� ������ ������ �� ����
            {
                transform.position += Vector3.down * speed * Time.deltaTime;
            }
            
        }

        //�Ŵ޷� ���� ������ �߰�
        if(transform.position.y<= topPos)//���� �ֻ� ���� ���� �Ʒ��� ������
        {
            transform.position += Vector3.up * landSpeed * Time.deltaTime;
        }
    }
}
