using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [System.Serializable]
    public struct ItemInfo
    {
        [Tooltip("아이템 이름")] public string name;
        [Tooltip("아이템 설명")] public string itemDescription;
    }

    [SerializeField] protected ItemInfo itemInfo;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        GetInfoAtCSV();
    }

    /// <summary>
    /// 객체가 생성될 때, 아이템 정보를 가져오는 역할(설명 등)
    /// </summary>
    void GetInfoAtCSV()
    {

    }
}
