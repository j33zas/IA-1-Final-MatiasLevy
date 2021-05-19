using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToSoldierState : SoldierState
{
    GameObject obstacle;

    float distToTarget;

    Vector3 dir;

    Node[] _path;

    int currentNode;

    float rotSpeedMultiplier = 1;

    float maxTimeLost = 10;

    float currentTimeLost = 0;

    float moveSpeed;

    string animation;

    public GoToSoldierState(StateMachine sm, Soldier S, float speed, string animationType) : base(sm, S)
    {
        moveSpeed = speed;
        animation = animationType;
    }

    public override void Awake()
    {
        base.Awake();

        currentNode = 0;

        _me.AN.SetBool("Has destination", true);
        _me.AN.SetBool(animation, true);

        _path = _me.GetAstarPath(_me.FindClosestNode(_me.eyeSightPosition, _me.eyeSightLength), _me.FindClosestNode(_me.objective.transform, _me.eyeSightLength)).ToArray();

        foreach (var item in _path)// solo para debugear
            item.isPath = true;
    }

    public override void Execute()
    {
        base.Execute();
        distToTarget = Vector3.Distance(_me.objective.transform.position, _me.transform.position);

        if (currentNode < _path.Length)
        {
            obstacle = _me.GetObstacle(_me.transform, _me.obsAvoidanceRadious, _me.obstacleMask);

            if (obstacle)//Si hay un obstaculo lo esquivo
                dir = Vector3.Lerp(_me.transform.forward, (_path[currentNode].transform.position - _me.transform.position), Time.deltaTime * _me.rotSpeed);
            else//sino camino hacia donde debo
                dir = Vector3.Lerp(_me.transform.forward, _path[currentNode].transform.position - _me.transform.position, _me.rotSpeed * Time.deltaTime * rotSpeedMultiplier);

            dir = Vector3.Scale(dir, new Vector3(1, 0, 1));// para que no roten hacia arria y abajo, solo para los costados

            _me.transform.forward = Vector3.Lerp(_me.transform.forward, dir.normalized, Time.deltaTime * _me.rotSpeed);

            _me.transform.position += _me.transform.forward * _me.walkSpeed * Time.deltaTime;

            if(Vector3.Distance(_me.transform.position, _path[currentNode].transform.position) <= 0.3f)
            {
                if (currentNode >= _path.Length)
                    _me.objective = null;
                else
                    currentNode++;

                currentTimeLost = 0;
                rotSpeedMultiplier = 1;
            }
            else
                currentTimeLost += Time.deltaTime;
            
            if(currentTimeLost >= maxTimeLost)
                rotSpeedMultiplier += Time.deltaTime;
        }
    }

    public override void LateExecute()
    {
        base.LateExecute();
    }

    public override void Sleep()
    {
        base.Sleep();
        _me.AN.SetBool("Has destination", false);
        _me.AN.SetBool(animation, false);
    }
}
