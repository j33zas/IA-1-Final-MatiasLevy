using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralRangedAttackState : BaseUnitState
{
    public GeneralRangedAttackState(StateMachine SM, General G) : base(SM,G){ }

    public override void Awake()
    {
        base.Awake();
        _me.isattacking = true;
        _me.AN.SetTrigger("Ranged");
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
