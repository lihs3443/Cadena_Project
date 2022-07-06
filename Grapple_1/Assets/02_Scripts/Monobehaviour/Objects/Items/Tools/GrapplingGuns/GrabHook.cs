using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*후크를 발사하였을 때, 후크의 이동성이 그래플건의 위치와 상관이 있는가?
있을 경우: 후크의 이동방식을 단순히 후크의 어느 한 축으로 이동하게 설정
없을 경우: 발사를 한 시점에 ray등을 쏴 최종 충돌 지점을 정해놓고 해당 방향으로 나아가도록 설정*/

//후크의 collider.isTrigger = true

public class GrabHook : MonoBehaviour
{
    delegate void WhenUseEnded();
    //public event WhenUseEnded whenUseEnded;

    GrapplingGun parentHook = null; //자신(GO)가 붙어있는 훅 게임오브젝트의 GrapplingHook컴포넌트

    /// <summary>
    /// 자신(그랩 훅)이 특정 오브젝트와 충돌하는 것을 감지하는 역할
    /// </summary>
    /// <param name="_collision">충돌한 콜라이더</param>

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
