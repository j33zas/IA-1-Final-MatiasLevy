using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSoldierCombatState : BaseUnitState
{
    public NewSoldierCombatState(StateMachine sm, BaseUnit unit) : base(sm, unit) { }

    float speedMult = 0;

    Vector3 dir;

    GameObject obstacle;

    NewSoldier _meS;

    public override void Awake()
    {
        base.Awake();

        dir = _me.soldierTarget.transform.position - _me.transform.position;

        _meS = _me.GetComponent<NewSoldier>();
    }

    public override void Execute()
    {
        base.Execute();

        obstacle = _me.GetObstacle(_me.transform, _me.visionRange, _me.obstacleMask);

        if (_me.soldierTarget != null)
        {
            dir = _me.soldierTarget.transform.position - _me.transform.position;
            if (Vector3.Distance(_me.transform.position, _me.soldierTarget.transform.position) <= _me.AttackDistance)
            {
                _meS.Strike();
                speedMult = 0;
            }
            else
            {
                dir = _me.soldierTarget.transform.position - _me.transform.position;
                speedMult = 1;
            }
        }

        if (obstacle)
            dir += (_me.transform.position - obstacle.transform.position).normalized * _me.obsAvoidanceWeight;

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
