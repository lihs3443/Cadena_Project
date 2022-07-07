using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;
    Vector3 initPos;
    public CinemachineVirtualCamera CMvcam;

    void Start()
    {
        instance = this;
        //CMvcam = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>();
        initPos = CMvcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset;
    }

    // timer : Èçµé¸²ÀÇ ºóµµ ¼ö(½Ã°£), magnitude : Èçµé¸²ÀÇ Á¤µµ
    public IEnumerator Shake(float duration, float magnitude)
    {
        float timer = 0;

        while(timer <= duration)
        {
            CMvcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = Random.insideUnitSphere * magnitude;

            timer += Time.deltaTime;
            yield return null;
        }
        
        CMvcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = initPos;
    }
}
