using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*��ũ�� �߻��Ͽ��� ��, ��ũ�� �̵����� �׷��ð��� ��ġ�� ����� �ִ°�?
���� ���: ��ũ�� �̵������ �ܼ��� ��ũ�� ��� �� ������ �̵��ϰ� ����
���� ���: �߻縦 �� ������ ray���� �� ���� �浹 ������ ���س��� �ش� �������� ���ư����� ����*/

//��ũ�� collider.isTrigger = true

public class GrabHook : MonoBehaviour
{
    delegate void WhenUseEnded();
    //public event WhenUseEnded whenUseEnded;

    GrapplingGun parentHook = null; //�ڽ�(GO)�� �پ��ִ� �� ���ӿ�����Ʈ�� GrapplingHook������Ʈ

    /// <summary>
    /// �ڽ�(�׷� ��)�� Ư�� ������Ʈ�� �浹�ϴ� ���� �����ϴ� ����
    /// </summary>
    /// <param name="_collision">�浹�� �ݶ��̴�</param>

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.gameObject.CompareTag("Wall") || _collision.gameObject.CompareTag("Ground"))
        {
            parentHook.HookOnTheWall(transform.position);
        }
        else if (_collision.gameObject.CompareTag("CatchableObject"))
        {
            parentHook.HookOnEntity(transform.position);
        }
    }
}
