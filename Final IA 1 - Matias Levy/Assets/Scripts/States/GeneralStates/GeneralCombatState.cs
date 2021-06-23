using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralCombatState : BaseUnitState
{
    GameObject obstacle;

    Vector3 precictedPos = Vector3.zero;

    public GeneralCombatState(StateMachine SM, General G) : base(SM, G) { }

    public override void Awake()
    {
        base.Awake();
        _me.AN.SetBool("Has Destination", true);
        _me.AN.SetBool("Walking", true);
    }

    public override void Execute()
    {
        base.Execute();

        if (_me.objective.GetComponent<BaseUnit>().dead)
            return;

        obstacle = _me.GetObstacle(_me.transform, _me.obsAvoidanceRadious, _me.obstacleMask);

        Vector3 dir = (_me.objective.transform.position - _me.transform.position).normalized;

        if(Vector3.Distance(_me.transform.position, _me.objective.transform.position) >= _me.AttackDistance)
        {
            if (obstacle)
                dir += (_me.transform.position - obstacle.transform.position).normalized * _me.obsAvoidanceWeight;
            else
                dir = (_me.objective.transform.position - _me.transform.position);

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
