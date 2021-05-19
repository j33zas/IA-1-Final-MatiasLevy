﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieState : BaseUnitState
{
    public DieState(StateMachine sm, BaseUnit unit) : base(sm, unit){}

    public override void Awake()
    {
        base.Awake();
        _me.COLL.isTrigger = true;
        _me.RB.isKinematic = true;
        GameObject.Destroy(_me, 10);
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