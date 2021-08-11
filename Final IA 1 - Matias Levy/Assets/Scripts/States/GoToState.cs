using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToState : BaseUnitState
{
    GameObject obstacle;

    Vector3 dir;

    int currentNode;

    float rotSpeedMultiplier = 1;

    float maxTimeLost = 10;

    float currentTimeLost = 0;

    float moveSpeed;

    string animation;

    public GoToState(StateMachine sm, BaseUnit unit) : base(sm, unit){}

    public override void Awake()
    {
        base.Awake();
        currentNode = 0;

        _me.path = _me.GetAstarPath(_me.FindClosestNode(_me.eyeSightPosition, _me.visionRange), _me.FindClosestNode(_me.objective.transform, _me.visionRange)).ToArray();
        
        //_me.transform.forward = Vector3.Scale(_me.path[0].transform.position - _me.transform.position, new Vector3(1, 0, 1));

        foreach (var item in _me.path)// solo para debugear
            item.isPath = true;
    }

    public override void Execute()
    {
        base.Execute();

        obstacle = _me.GetObstacle(_me.transform, _me.obsAvoidanceRadious, _me.obstacleMask);

        if (currentNode < _me.path.Length)
        {
            dir = (_me.path[currentNode].transform.position - _me.transform.position).normalized;

            if (obstacle)//Si hay un obstaculo lo esquivo
                dir += (_me.transform.position - obstacle.transform.position).normalized * _me.obsAvoidanceWeight;

            dir = Vector3.Scale(dir, new Vector3(1, 0, 1));// para que no roten hacia arria y abajo, solo para los costados

            _me.transform.forward = Vector3.Lerp(_me.transform.forward, dir.normalized, Time.deltaTime * _me.rotSpeed);

            _me.transform.position += _me.transform.forward * _me.walkSpeed * Time.deltaTime;

            if(Vector3.Distance(_me.transform.position, _me.path[currentNode].transform.position) <= .6f)
            {
                currentNode++;
                if (currentNode > _me.path.Length)
                    _me.objective = null;

                currentTimeLost = 0;
                rotSpeedMultiplier = 1;
            }
            else
                currentTimeLost += Time.deltaTime;
            
            if(currentTimeLost >= maxTimeLost)
                rotSpeedMultiplier += Time.deltaTime;
        }
        else
        {
            if (Vector3.Distance(_me.transform.position, _me.objective.transform.position) <= .6f)
            {
                foreach (var Node in _me.path)
                    Node.Reset();
                _me.path = new Node [0];
                return;
            }

            dir = (_me.objective.transform.position - _me.transform.position).normalized;

            if (obstacle)//Si hay un obstaculo lo esquivo
                dir += (_me.transform.position - obstacle.transform.position).normalized * _me.obsAvoidanceWeight;

            dir = Vector3.Scale(dir, new Vector3(1, 0, 1));// para que no roten hacia arria y abajo, solo para los costados

            _me.transform.forward = Vector3.Lerp(_me.transform.forward, dir.normalized, Time.deltaTime * _me.rotSpeed);

            _me.transform.position += _me.transform.forward * _me.walkSpeed * Time.deltaTime;

        }
    }

    public override void LateExecute()
    {
        base.LateExecute();
    }

    public override void Sleep()
    {
        base.Sleep();
        if(_me.path != null)
            foreach (var node in _me.path)
            {
            node.Reset();
            node.isPath = false;
            }
    }
}
