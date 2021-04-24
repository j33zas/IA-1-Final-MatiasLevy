using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeSoldierState : SoldierState
{
    public FleeSoldierState(StateMachine sm, Soldier S) : base(sm, S)
    {

    }

    public override void Awake()
    {
        base.Awake();
    }

    public override void Execute()
    {
        base.Execute();
    }

    public override void LateExecute()
    {
        base.LateExecute();
    }

    public override void Sleep()
    {
        base.Sleep();
    }
}
