using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tool : Item
{
    /// <summary>
    /// 키 입력 등으로 해당 아이템을 사용하려고 시도할 때 쿨타임 등 조건을 체크
    /// </summary>
    /// <returns>사용 성공 여부</returns>
    public abstract bool TryUse();

    /// <summary>
    /// 해당 도구 사용 시의 행위
    /// </summary>
    protected abstract void UseEffect();
}
