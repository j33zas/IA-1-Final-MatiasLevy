using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleSoldierState : SoldierState
{
    public IdleSoldierState(StateMachine sm, Soldier S):base(sm,S)
    {
    }
    public override void Awake()
    {
        base.Awake();
        _me.AN.SetBool("Has destination", false);
        _me.AN.SetBool("Enemy close", false);
        _me.AN.SetFloat("Dist. to destination", 0);
        //Vector3.Distance(_me.eyeSightPosition.position, _me.objective.transform.position)
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
