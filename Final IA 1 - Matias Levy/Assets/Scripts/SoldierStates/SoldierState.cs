using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierState : State
{
    protected Soldier _me;

    public SoldierState(StateMachine sm, Soldier S) : base (sm)
    {
        _me = S;
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
