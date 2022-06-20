using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Hook(����) ���� �ڵ�
public class Hook : MonoBehaviour
{
    Grappling grappling;
    public DistanceJoint2D joint;

    void Start()
    {
        grappling = GameObject.Find("Player").GetComponent<Grappling>();
        joint = GetComponent<DistanceJoint2D>();
        joint.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Ground"))
        {
            joint.distance = Vector2.Distance(grappling.transform.position, transform.position); // �÷��̾� ��ġ�� �������� ���� ���� ����
            joint.anchor = new Vector2(0, 0);           // ���� ���̼��� �ʱ�ȭ
            joint.enabled = true;                       // �Ű� �÷��̾ ����
            grappling.isAttatch = true;                 // ���� ���� ����
        }
    }
}
