using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToRunSoldierState : SoldierState
{
    public GameObject target;

    GameObject obstacle;

    float distToTarget;

    Node[] _path;

    int currentNode;

    float rotSpeedMultiplier = 1;

    float maxTimeLost = 10;

    float currentTimeLost = 0;

    public GoToRunSoldierState(StateMachine sm, Soldier S) : base(sm, S){}

    public override void Awake()
    {
        base.Awake();

        currentNode = 0;

        _me.AN.SetBool("Has destination", true);

        _path = _me.GetAstarPath(_me.FindClosestNode(_me.eyeSightPosition, _me.eyeSightLength), _me.FindClosestNode(target.transform, _me.eyeSightLength)).ToArray();

        foreach (var item in _path)// solo para debugear
            item.isPath = true;
    }

    public override void Execute()
    {
        base.Execute();
        distToTarget = Vector3.Distance(target.transform.position, _me.transform.position);

        _me.AN.SetFloat("Dist. to destination", distToTarget);

        if (currentNode < _path.Length)
        {
            obstacle = _me.GetObstacle(_me.transform, _me.obsAvoidanceRadious, _me.obstacleMask);

            if(obstacle)//Si hay un obstaculo lo esquivo
                _me.transform.forward = Vector3.Lerp(_me.transform.forward, (_path[currentNode].transform.position - _me.transform.position), Time.deltaTime * _me.rotSpeed);
            else//sino camino hacia donde debo
                _me.transform.forward = Vector3.Lerp(_me.transform.forward, _path[currentNode].transform.position - _me.transform.position, _me.rotSpeed * Time.deltaTime * rotSpeedMultiplier);

            _me.transform.position += _me.transform.forward * _me.runSpeed * Time.deltaTime;
            if (Vector3.Distance(_me.transform.position, _path[currentNode].transform.position) <= 0.3f)
            {
                if (currentNode > _path.Length)
                {
                    _me.objective = null;
                    target = null;
                }
                else
                    currentNode++;

                currentTimeLost = 0;
                rotSpeedMultiplier = 1;
            }
            else
                currentTimeLost += Time.deltaTime;

            if (currentTimeLost >= maxTimeLost)
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
    }
}
