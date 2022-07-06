using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Idle : IState
{
    public void OnStateEnter()
    {
         
    }

    public void OnStateExit()
    {

    }

    public void OnStateUpdate()
    {
        Debug.Log($"현재 상태: {this.GetType().Name}");
    }
}

public class State_Moving : IState
{
    public void OnStateEnter()
    {

    }

    public void OnStateExit()
    {

    }

    public void OnStateUpdate()
    {
        Debug.Log($"현재 상태: {this.GetType().Name}");
    }
}

public class State_Attacking : IState
{
    public void OnStateEnter()
    {

    }

    public void OnStateExit()
    {

    }

    public void OnStateUpdate()
    {
        Debug.Log($"현재 상태: {this.GetType().Name}");
    }
}
