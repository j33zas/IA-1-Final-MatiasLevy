using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnitState : State
{
    protected BaseUnit _me;

    public BaseUnitState(StateMachine sm, BaseUnit S) : base (sm)
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
