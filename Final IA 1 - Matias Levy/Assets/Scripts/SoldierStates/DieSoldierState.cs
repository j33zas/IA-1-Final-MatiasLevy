using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieSoldierState : SoldierState
{
    public DieSoldierState(StateMachine sm, Soldier S) : base(sm, S){}

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
