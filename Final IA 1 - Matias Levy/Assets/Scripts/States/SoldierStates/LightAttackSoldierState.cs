using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAttackSoldierState : BaseUnitState
{
    public LightAttackSoldierState(StateMachine sm, Soldier S) : base(sm, S){}

    public override void Awake()
    {
        base.Awake();
        _me.canAttack = true;
        _me.AN.SetTrigger("Light");
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
