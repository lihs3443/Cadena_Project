using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [System.Serializable]
    public struct ItemInfo
    {
        [Tooltip("������ �̸�")] public string name;
        [Tooltip("������ ����")] public string itemDescription;
    }

    [SerializeField] protected ItemInfo itemInfo;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        GetInfoAtCSV();
    }

    /// <summary>
    /// ��ü�� ������ ��, ������ ������ �������� ����(���� ��)
    /// </summary>
    void GetInfoAtCSV()
    {

    }
}
