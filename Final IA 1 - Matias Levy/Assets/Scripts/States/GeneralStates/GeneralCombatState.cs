﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralCombatState : BaseUnitState
{
    public GeneralCombatState(StateMachine SM, General G) : base(SM, G) { }

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
