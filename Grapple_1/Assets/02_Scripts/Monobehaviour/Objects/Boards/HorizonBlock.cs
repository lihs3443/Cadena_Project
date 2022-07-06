using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizonBlock : MonoBehaviour
{
    [SerializeField]
    float speed;    //블럭 이동 속도
    [SerializeField]
    float minXpos;  //최소 x 좌표   +이런건 오브젝트 중심 기준으로 값을 주고 싶은데
    [SerializeField]
    float maxXpos;  //최대 x 좌표
    [SerializeField]
    float delay;    //종점에 얼마나 오래 위치할 것이냐
    float timer;    //시간 측정용
    bool dirSwitch; //방향 스위치. 0일 경우 오른쪽 이동부터

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
