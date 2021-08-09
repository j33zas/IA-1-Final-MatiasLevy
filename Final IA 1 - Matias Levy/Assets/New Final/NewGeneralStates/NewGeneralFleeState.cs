using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGeneralFleeState : BaseUnitState
{
    public NewGeneralFleeState(StateMachine SM, BaseUnit unit) : base(SM, unit) { }

    GameObject obstacle;
    public GameObject enemy;
    Vector3 dir;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Execute()
    {
        base.Execute();
        obstacle = _me.GetObstacle(_me.transform, _me.obsAvoidanceRadious, _me.obstacleMask);

        dir = -(enemy.transform.position - _me.transform.position).normalized;

        if (obstacle)//Si hay un obstaculo lo esquivo
            dir += (_me.transform.position - obstacle.transform.position).normalized * _me.obsAvoidanceWeight;

        dir = Vector3.Scale(dir, new Vector3(1, 0, 1));// para que no roten hacia arria y abajo, solo para los costados

        _me.transform.forward = Vector3.Lerp(_me.transform.forward, dir.normalized, Time.deltaTime * _me.rotSpeed);

        _me.transform.position += _me.transform.forward * _me.walkSpeed * Time.deltaTime;
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
