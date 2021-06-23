using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSoldierState : BaseUnitState
{
    public BaseUnit target;

    GameObject obstacle;

    Vector3 _predictedPos = Vector3.zero;

    public CombatSoldierState(StateMachine sm, Soldier S) : base(sm, S){}

    public override void Awake()
    {
        base.Awake();
        _me.AN.SetBool("Has destination", true);
        _me.AN.SetBool("Walking", true);

    }

    public override void Execute()
    {
        base.Execute();


        if (target)
        {
            obstacle = _me.GetObstacle(_me.transform, _me.obsAvoidanceRadious, _me.obstacleMask);

            Vector3 dir = (target.transform.position - _me.transform.position).normalized;

            if (Vector3.Distance(_me.transform.position, target.transform.position) >= _me.AttackDistance)
            {
                _me.AN.SetBool("Has destination", true);
                _me.AN.SetBool("Walking", true);

                if (obstacle)
                    dir += (_me.transform.position - obstacle.transform.position).normalized * _me.obsAvoidanceWeight;
                else
                    dir = (target.transform.position - _me.transform.position);

                dir = Vector3.Scale(dir, new Vector3(1, 0, 1));// para que no roten hacia arria y abajo, solo para los costados

                _me.transform.forward = Vector3.Lerp(_me.transform.forward, dir.normalized, Time.deltaTime * _me.rotSpeed);

                _me.transform.position += _me.transform.forward * _me.walkSpeed * Time.deltaTime;

            }
            else
            {

                _me.AN.SetBool("Has destination", false);
                _me.AN.SetBool("Walking", false);

                if (!_me.isattacking)
                    _me.AttackRouletteWheel();
            }

            if (target.dead)
            {
                _me.enemiesSeen.Remove(target);
                target = null;
            }
        }
        else
            return;
    }

    public override void LateExecute()
    {
        base.LateExecute();
    }

    public override void Sleep()
    {
        base.Sleep();
        _me.AN.SetBool("Has destination", false);
        _me.AN.SetBool("Walking", false);
    }
}