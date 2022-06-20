using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    /// <summary>
    /// ��� ĳ���Ͱ� �������� ���� ���� & �޼ҵ��
    /// </summary>
    protected class CharacterStat
    {
        public float maxHitPoint;
        public float hitPoint;
    }

    protected CharacterStat characterStat = new CharacterStat();
    
    /// <summary>
    /// ĳ������ ���� ����(FSM) <br />
    /// ���� �޼ҵ�: GetCurrentState(), ChangeState();
    /// </summary>
    protected IState currentState;

    protected virtual void Update()
    {
        currentState.OnStateUpdate();
    }

    /// <summary>
    /// ���� ĳ������ ���� ������ �ʿ��Ҷ� ���
    /// </summary>
    /// <returns>���� ���¸� IState�� ��ȯ</returns>
    public IState GetCurrentState()
    {
        return currentState;
    }

    /// <summary>
    /// �� / �ܺο��� IState�������̽��� ������ ��ü�� �Է¹޾� ���� ���¸� �ٲ� �� ���
    /// </summary>
    /// <param name="_state"></param>
    public void ChangeState(IState _state)
    {
        currentState.OnStateExit();
        currentState = _state;
        currentState.OnStateEnter();
    }

    /// <summary>
    /// �ش� Character��ü�� CharacterStat�� hitPoint�� ���ҽ�ų�� ���
    /// </summary>
    /// <param name="_damage">���� �����</param>
    /// <returns>���������� ���� ����� ��ȯ</returns>
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
    /// ��� �� ���� �ൿ��
    /// </summary>
    protected abstract void DeathAction();
}
