using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//플레이어가 로프를 매달거나 블럭 위에 올라가는 등
//블럭에 무게를 가하는 행위를 할 때 내려가는 블럭입니다.
public class HangingBlock : MonoBehaviour
{
    public bool isHanging = false;  //매달려 있는지.. + 걍 hook이나 grappling에서 받아오는게 나을듯
    public float speed = 3.0f;     //하강 속도
    public float landSpeed; //상승 속도
    public float resetTime; //입력 없을 시 얼마 후 재위치 하는지
    public float topPos;         //최상 지점
    public float bottomPos;      //최하 지점

    
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //플레이어가 블럭에 매달려 있을 때
        if(isHanging == true)
        {
            if(transform.position.y >= bottomPos)//블럭이 최하 지점에 도달할 때 까지
            {
                transform.position += Vector3.down * speed * Time.deltaTime;
            }
            
        }

        //매달려 있지 않으면 추가
        if(transform.position.y<= topPos)//블럭이 최상 지점 보다 아래에 있으면
        {
            transform.position += Vector3.up * landSpeed * Time.deltaTime;
        }
    }
}
