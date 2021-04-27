using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSoldierState : SoldierState
{
    public HitSoldierState(StateMachine SM, Soldier S):base(SM,S){}

    public override void Awake()
    {
        base.Awake();
        _me.AN.SetTrigger("Hit");
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
