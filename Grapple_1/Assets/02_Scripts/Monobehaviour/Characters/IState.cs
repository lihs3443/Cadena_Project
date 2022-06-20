using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    public void OnStateEnter();
    public void OnStateUpdate();
    public void OnStateExit();
}
