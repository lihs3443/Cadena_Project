using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tool : Item
{
    /// <summary>
    /// Ű �Է� ������ �ش� �������� ����Ϸ��� �õ��� �� ��Ÿ�� �� ������ üũ
    /// </summary>
    /// <returns>��� ���� ����</returns>
    public abstract bool TryUse();

    /// <summary>
    /// �ش� ���� ��� ���� ����
    /// </summary>
    protected abstract void UseEffect();
}
