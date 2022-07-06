using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    Item[] inventory = new Item[9];
    /// <summary>
    /// 플레이어가 들고있는 아이템의 인덱스
    /// </summary>
    int handIndex = 0;

    private void Awake()
    {
        //값 청소
        for (int repeatCount = 0; repeatCount < inventory.Length; repeatCount++)
        {
            inventory[repeatCount] = null;
        }
        //json등에서 저장된 데이터로 가져오는 행위 작성
    }

    /// <summary>
    /// 플레이어의 인벤토리에 아이템을 넣고자 할 때 사용
    /// </summary>
    /// <param name="_index">몇 번째 Index에 넣을지</param>
    /// <param name="_item">넣을 아이템의 Item객체</param>
    /// <param name="_overWrite">해당 Index에 이미 아이템이 존재해도 강제적으로 _item으로 덮어씌울지(비권장)</param>
    /// <returns>아이템을 넣었음에 성공 여부를 반환</returns>
    public bool InsertItem(int _index, Item _item, bool _overWrite = false)
    {
        //해당 칸이 비어있지 않다면
        if (inventory[_index] != null)
        {
            if (_overWrite)
            {
                Debug.Log($"{_index}번째 인벤토리의 '{inventory[_index]}'을(를) '{_item}'으로 덮어씌웠습니다.");
                return true;
            }
            Debug.Log($"{_index}번째 인벤토리가 비어있지 않아 '{_item}'을 삽입하는 행위에 실패하였습니다.");
            return false;
        }
        Debug.Log($"{_index}번째 인벤토리에 '{_item}'을 삽입하였습니다.");
        return true;
    }

    /// <summary>
    /// 인벤토리의 아이템 목록을 리턴하는 역할
    /// </summary>
    /// <returns>inventory[]</returns>1
    public Item[] GetItems()
    {
        return inventory;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    bool TryUseItem()
    {
        return true;
    }
}
