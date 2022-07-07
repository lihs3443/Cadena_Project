using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    /// <summary>
    /// 모든 캐릭터가 가져야할 공통 변수 & 메소드들
    /// </summary>
    protected class CharacterStat
    {
        public float maxHitPoint;
        public float hitPoint;
    }

    protected CharacterStat characterStat = new CharacterStat();
    
    /// <summary>
    /// 캐릭터의 현재 상태(FSM) <br />
    /// 관련 메소드: GetCurrentState(), ChangeState();
    /// </summary>
    protected IState currentState;

    protected virtual void Update()
    {
        currentState.OnStateUpdate();
    }

    /// <summary>
    /// 현재 캐릭터의 상태 정보가 필요할때 사용
    /// </summary>
    /// <returns>현재 상태를 IState로 반환</returns>
    public IState GetCurrentState()
    {
        return currentState;
    }

    /// <summary>
    /// 내 / 외부에서 IState인터페이스를 구현한 객체를 입력받아 현재 상태를 바꿀 때 사용
    /// </summary>
    /// <param name="_state"></param>
    public void ChangeState(IState _state)
    {
        currentState.OnStateExit();
        currentState = _state;
        currentState.OnStateEnter();
    }

    /// <summary>
    /// 해당 Character객체의 CharacterStat의 hitPoint를 감소시킬때 사용
    /// </summary>
    /// <param name="_damage">가할 대미지</param>
    /// <returns>최종적으로 입힌 대미지 반환</returns>
    public float InflictDamage(float _damage)
    {
        float finalDamage = _damage;
        characterStat.hitPoint -= _damage;

        if (characterStat.hitPoint <= 0f)
        {
            DeathAction();
        }
        return finalDamage;
    }

    /// <summary>
    /// 사망 시 취할 행동들
    /// </summary>
    protected abstract void DeathAction();
}
