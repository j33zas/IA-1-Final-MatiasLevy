using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseUnitState
{
    float timeIdle;
    public IdleState(StateMachine sm, BaseUnit unit):base(sm,unit)
    {
    }
    public override void Awake()
    {
        base.Awake();
        //_me.AN.SetBool("Has Destination", false);
        //_me.AN.SetBool("Enemy close", false);
        //Vector3.Distance(_me.eyeSightPosition.position, _me.objective.transform.position)
    }

    public override void Execute()
    {
        base.Execute();
        timeIdle += Time.deltaTime;
        if(timeIdle >= 10)
        {
            _me.GoToTroop = true;
        }
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
