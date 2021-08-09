using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGeneralCombatState : BaseUnitState
{
    NewGeneral _meG;

    float speedMult = 0;

    Vector3 dir;

    public NewGeneralCombatState(StateMachine sm, BaseUnit unit) : base(sm, unit) { }

    public override void Awake()
    {
        base.Awake();
        _meG = _me.GetComponent<NewGeneral>();
        dir = _me.soldierTarget.transform.position - _me.transform.position;
    }

    public override void Execute()
    {
        base.Execute();
        _meG.gun.transform.LookAt(_me.soldierTarget.transform);
        _meG.gun.transform.Rotate(new Vector3(-15, 0, 0));

        if(Vector3.Distance(_me.transform.position, _me.soldierTarget.transform.position) <= _me.AttackDistance)
        {
            speedMult = 0;
            _meG.Shoot();
        }
        else
        {
            dir = _me.soldierTarget.transform.position - _me.transform.position;
            speedMult = 1;
        }

        dir = Vector3.Scale(dir, new Vector3(1, 0, 1));

        _me.transform.forward = Vector3.Lerp(_me.transform.forward, dir.normalized, Time.deltaTime * _me.rotSpeed);

        _me.transform.position += _me.transform.forward * _me.walkSpeed * speedMult * Time.deltaTime;
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
