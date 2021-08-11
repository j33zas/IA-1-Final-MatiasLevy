using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGeneralFleeState : BaseUnitState
{
    public NewGeneralFleeState(StateMachine SM, BaseUnit unit) : base(SM, unit) { }

    GameObject obstacle;

    public BaseUnit attacker;

    NewGeneral _meG;

    Vector3 dir;

    float _currentTime = 0;

    float _timeTillScape = 2;

    public override void Awake()
    {
        base.Awake();
        _meG = _me.GetComponent<NewGeneral>();
    }

    public override void Execute()
    {
        base.Execute();

        var GO = _me.GetObstacle(_me.transform, _me.visionRange, _me.enemyLayer);

        if (GO)
            attacker = GO.GetComponent<BaseUnit>();

        if (attacker)
        {
            dir = (_me.transform.position - attacker.transform.position).normalized;
            if (Vector3.Distance(attacker.transform.position, _me.transform.position) > 2)
            {
                _currentTime += Time.deltaTime;
                if (_currentTime >= _timeTillScape)
                {
                    dir = Vector3.zero;
                    _meG.isSafe = true;
                }
            }
        }
        else
        {
            _currentTime += Time.deltaTime;
            if (_currentTime >= _timeTillScape)
            {
                dir = Vector3.zero;
                _meG.isSafe = true;
            }
        }

        obstacle = _me.GetObstacle(_me.transform, _me.obsAvoidanceRadious, _me.obstacleMask);

        if (obstacle)
            dir += (_me.transform.position - obstacle.transform.position).normalized * _me.obsAvoidanceWeight;

        dir = Vector3.Scale(dir, new Vector3(1, 0, 1));// para que no roten hacia arria y abajo, solo para los costados

        _me.transform.forward = Vector3.Lerp(_me.transform.forward, dir, _me.rotSpeed * Time.deltaTime);

        _me.transform.position += _me.transform.forward * _me.runSpeed * Time.deltaTime;
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
