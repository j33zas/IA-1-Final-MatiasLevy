using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeSoldierState : SoldierState
{
    public BaseUnit attacker;

    Vector3 _dir = new Vector3();

    public FleeSoldierState(StateMachine sm, Soldier S) : base(sm, S){}

    public override void Awake()
    {
        base.Awake();
        _dir = -(_me.objective.transform.position - _me.transform.position);

        _me.AN.SetBool("Has destination", true);
        _me.AN.SetBool("Running", true);
    }

    public override void Execute()
    {
        base.Execute();

        _dir = -(attacker.transform.position - _me.transform.position).normalized;

        _me.transform.forward = Vector3.Lerp(_me.transform.forward, _dir, _me.rotSpeed * Time.deltaTime);

        _me.transform.position += _me.transform.forward * _me.runSpeed * Time.deltaTime;
    }

    public override void LateExecute()
    {
        base.LateExecute();
    }

    public override void Sleep()
    {
        base.Sleep();
        _me.AN.SetBool("Has destination", false);
        _me.AN.SetBool("Running", false);

    }
}
