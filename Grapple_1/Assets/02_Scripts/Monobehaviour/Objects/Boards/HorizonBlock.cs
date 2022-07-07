using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizonBlock : MonoBehaviour
{
    [SerializeField]
    float speed;    //�� �̵� �ӵ�
    [SerializeField]
    float minXpos;  //�ּ� x ��ǥ   +�̷��� ������Ʈ �߽� �������� ���� �ְ� ������
    [SerializeField]
    float maxXpos;  //�ִ� x ��ǥ
    [SerializeField]
    float delay;    //������ �󸶳� ���� ��ġ�� ���̳�
    float timer;    //�ð� ������
    bool dirSwitch; //���� ����ġ. 0�� ��� ������ �̵�����

    private void Awake()
    {
        speed = 3.0f;
        timer = 0.0f;
        delay = 1.0f;
        dirSwitch = false;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > delay)
        {
            if(!dirSwitch)
            {
                if (transform.position.x > maxXpos)
                    transform.position += Vector3.right * speed * Time.deltaTime;
                else
                    timer = 0;
            }

        }
    }
}
