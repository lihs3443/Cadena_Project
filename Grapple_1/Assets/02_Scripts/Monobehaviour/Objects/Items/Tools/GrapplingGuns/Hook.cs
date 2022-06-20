using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Hook(갈고리) 관련 코드
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
            joint.distance = Vector2.Distance(grappling.transform.position, transform.position); // 플레이어 위치를 기준으로 훅의 길이 조절
            joint.anchor = new Vector2(0, 0);           // 훅의 길이설정 초기화
            joint.enabled = true;                       // 훅과 플레이어를 연결
            grappling.isAttatch = true;                 // 훅이 붙은 상태
        }
    }
}
