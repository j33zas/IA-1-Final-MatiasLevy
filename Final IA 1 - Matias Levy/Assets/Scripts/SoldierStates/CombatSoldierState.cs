using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSoldierState : SoldierState
{
    public Soldier target;

    GameObject obstacle;

    Vector3 _predictedPos = Vector3.zero;

    bool Attacked;

    Dictionary<string, int> _attacks = new Dictionary<string, int>();
    float _totalAttackWeight = 10;

    public CombatSoldierState(StateMachine sm, Soldier S) : base(sm, S){}

    public override void Awake()
    {
        base.Awake();
        _me.AN.SetBool("Has destination", true);

        if(!_attacks.ContainsKey("Heavy"))
            _attacks.Add("Heavy", 3);
        if(!_attacks.ContainsKey("Light"))
        _attacks.Add("Light", 7);

        Attacked = false;
    }

    public override void Execute()
    {
        base.Execute();

        obstacle = _me.GetObstacle(_me.transform, _me.obsAvoidanceRadious, _me.obstacleMask);

        Vector3 dir = (target.transform.position - _me.transform.position).normalized;

        _me.AN.SetFloat("Dist. to destination", Vector3.Distance(target.transform.position, _me.transform.position));

        if(Vector3.Distance(_me.transform.position, target.transform.position) > _me.AttackDistance)
        {
            if (obstacle)
                dir += (_me.transform.position - obstacle.transform.position).normalized * _me.obsAvoidanceWeight;
            else
                dir = target.transform.position - _me.transform.position;

            _me.transform.forward = Vector3.Lerp(_me.transform.forward, dir, Time.deltaTime * _me.rotSpeed);

            _me.transform.position += _me.transform.forward * _me.walkSpeed * Time.deltaTime;
        }
        else
        {
            if(!Attacked)
            {
                Attacked = true;
                float R = Random.Range(0, _totalAttackWeight);
                foreach (var Attack in _attacks)
                {
                    R -= Attack.Value;
                    if(R<=0)
                    {
                        Debug.Log("Roulette Wheel Selecting");
                        if (Attack.Key == "Heavy")
                            _me.HeavyAttack();
                        else if (Attack.Key == "Light")
                            _me.LightAttack();
                    }
                }
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
        _me.AN.SetBool("Has destination", false);
    }
}