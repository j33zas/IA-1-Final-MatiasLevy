﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeState : BaseUnitState
{
    public BaseUnit attacker;

    GameObject obstacle;

    float timeTillScape = 2;
    float CurrentTime = 0;

    Vector3 dir;

    public FleeState(StateMachine sm, BaseUnit unit) : base(sm, unit){}

    public override void Awake()
    {
        base.Awake();

        _me.transform.forward = -(attacker.transform.position - _me.transform.position).normalized;
        _me.lowHP = true;
        _me.AN.SetBool("Has Destination", true);
        _me.AN.SetBool("Running", true);
    }

    public override void Execute()
    {
        base.Execute();

        dir = -(attacker.transform.position - _me.transform.position).normalized;

        obstacle = _me.GetObstacle(_me.transform, _me.obsAvoidanceRadious, _me.obstacleMask);

        if (obstacle)
            dir += (_me.transform.position - obstacle.transform.position).normalized * _me.obsAvoidanceWeight;

        dir = Vector3.Scale(dir, new Vector3(1, 0, 1));// para que no roten hacia arria y abajo, solo para los costados

        _me.transform.forward = Vector3.Lerp(_me.transform.forward, dir, _me.rotSpeed * Time.deltaTime);

        _me.transform.position += _me.transform.forward * _me.runSpeed * Time.deltaTime;

        if(Vector3.Distance(attacker.transform.position, _me.transform.position) > 2)
        {
            CurrentTime += Time.deltaTime;
            if (CurrentTime >= timeTillScape)
            {
                dir = Vector3.zero;
                _me.lowHP = false;
            }
        }
    }

    public override void LateExecute()
    {
        base.LateExecute();
    }

    public override void Sleep()
    {
        base.Sleep();
        _me.AN.SetBool("Has Destination", false);
        _me.AN.SetBool("Running", false);

    }
}
