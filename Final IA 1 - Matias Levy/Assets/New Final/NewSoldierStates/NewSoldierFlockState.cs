using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSoldierFlockState : BaseUnitState
{
    public NewSoldierFlockState(StateMachine sm, BaseUnit unit) : base(sm, unit) { }

    List<Transform> friends = new List<Transform>();

    Vector3 dir = new Vector3(0,0,0);

    NewSoldier _meS;

    GameObject obstacle;

    public override void Awake()
    {
        base.Awake();
        _meS = _me.GetComponent<NewSoldier>();
    }

    public override void Execute()
    {
        base.Execute();

        obstacle = _me.GetObstacle(_me.transform, _me.obsAvoidanceRadious, _me.obstacleMask);

        var inRange = Physics.OverlapSphere(_me.transform.position, _me.visionRange, _me.allyLayer);

        foreach (var friend in inRange)
        {
            var f = friend.GetComponentInParent<NewSoldier>();
            if(f)
                friends.Add(friend.transform);
        }

        if(obstacle)
            dir += (_me.transform.position - obstacle.transform.position).normalized * _me.obsAvoidanceWeight;
        else
            dir  =  (Cohesion() * _meS.cohesionWeight) + 
                    (Alineation() * _meS.alineationWeight) + 
                    (Separation() * _meS.separationWeight) +
                    (_meS.General.transform.position - _me.transform.position) * _meS.leaderWeight;


        dir = Vector3.Scale(dir, new Vector3(1, 0, 1));

        _me.transform.forward = Vector3.Slerp(_me.transform.forward, dir, Time.deltaTime * _me.rotSpeed);


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

    Vector3 Cohesion()
    {
        Vector3 cohesion = Vector3.zero;
        foreach (var friend in friends)
            cohesion += friend.position - _me.transform.position;

        cohesion /= friends.Count;
        return cohesion;
    }

    Vector3 Separation()
    {
        Vector3 separation = Vector3.zero;

        foreach (var friend in friends)
        {
            Vector3 friendDir = _me.transform.position - friend.position;
            float distance = _me.visionRange - friendDir.magnitude;
            friendDir.Normalize();
            friendDir *= distance;
            separation += friendDir;
        }
        separation /= friends.Count;
        return separation;
    }

    Vector3 Alineation()
    {
        Vector3 alineation = Vector3.zero;
        foreach (var friend in friends)
            alineation += friend.forward;

        alineation /= friends.Count;
        return alineation;
    }
}
