using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    Item[] inventory = new Item[9];
    /// <summary>
    /// �÷��̾ ����ִ� �������� �ε���
    /// </summary>
    int handIndex = 0;

    private void Awake()
    {
        //�� û��
        for (int repeatCount = 0; repeatCount < inventory.Length; repeatCount++)
        {
            inventory[repeatCount] = null;
        }
        //json��� ����� �����ͷ� �������� ���� �ۼ�
    }

    /// <summary>
    /// �÷��̾��� �κ��丮�� �������� �ְ��� �� �� ���
    /// </summary>
    /// <param name="_index">�� ��° Index�� ������</param>
    /// <param name="_item">���� �������� Item��ü</param>
    /// <param name="_overWrite">�ش� Index�� �̹� �������� �����ص� ���������� _item���� �������(�����)</param>
    /// <returns>�������� �־����� ���� ���θ� ��ȯ</returns>
    public bool InsertItem(int _index, Item _item, bool _overWrite = false)
    {
        //�ش� ĭ�� ������� �ʴٸ�
        if (inventory[_index] != null)
        {
            if (_overWrite)
            {
                Debug.Log($"{_index}��° �κ��丮�� '{inventory[_index]}'��(��) '{_item}'���� ��������ϴ�.");
                return true;
            }
            Debug.Log($"{_index}��° �κ��丮�� ������� �ʾ� '{_item}'�� �����ϴ� ������ �����Ͽ����ϴ�.");
            return false;
        }
        Debug.Log($"{_index}��° �κ��丮�� '{_item}'�� �����Ͽ����ϴ�.");
        return true;
    }

    /// <summary>
    /// �κ��丮�� ������ ����� �����ϴ� ����
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
