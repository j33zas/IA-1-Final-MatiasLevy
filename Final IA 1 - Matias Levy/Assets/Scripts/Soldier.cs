using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : BaseUnit
{
    public GameObject objective;

    Soldier[] Batallion;

    public HitBox lightAttack;
    public HitBox heavyAttack;

    //states
    IdleSoldierState idleState;
    GoToSoldierState walkToState;
    GoToRunSoldierState runToState;
    HealSoldierState healState;
    FleeSoldierState fleeState;
    LightAttackSoldierState attackLightState;
    HeavyAttackSoldierState attackHeavyState;
    DieSoldierState dieState;
    CombatSoldierState combatState;
    HitSoldierState hitState;

    private void Start()
    {
        idleState = new IdleSoldierState(SM, this);
        walkToState = new GoToSoldierState(SM, this);
        runToState = new GoToRunSoldierState(SM, this);
        healState = new HealSoldierState(SM, this);
        fleeState = new FleeSoldierState(SM, this);
        attackLightState = new LightAttackSoldierState(SM, this);
        attackHeavyState = new HeavyAttackSoldierState(SM, this);
        dieState = new DieSoldierState(SM, this);
        combatState = new CombatSoldierState(SM, this);
        hitState = new HitSoldierState(SM, this);

        SM.AddState(idleState);
        SM.AddState(walkToState);
        SM.AddState(runToState);
        SM.AddState(healState);
        SM.AddState(fleeState);
        SM.AddState(attackLightState);
        SM.AddState(attackHeavyState);
        SM.AddState(dieState);
        SM.AddState(combatState);
        SM.AddState(hitState);
        SM.SetState<IdleSoldierState>();

        heavyAttack.owner = this;
        heavyAttack.enemyLayer = EnemyLayer;
        lightAttack.owner = this;
        lightAttack.enemyLayer = EnemyLayer;
    }

    private void Update()
    {
        if(objective==null)
            SM.SetState<IdleSoldierState>();

        if(Input.GetKey(KeyCode.Space))
        {
            walkToState.target = objective;
            runToState.target = objective;
            if (Vector3.Distance(transform.position, objective.transform.position) > 10)
                SM.SetState<GoToRunSoldierState>();
            else
                SM.SetState<GoToSoldierState>();
        }

        #region Busqueda de enemigos
        var temp = Physics.OverlapSphere(eyeSightPosition.position, eyeSightLength, EnemyLayer);
        if(temp.Length > 0)
        {
            foreach (var item in temp)
            {
                var soldier = item.GetComponent<Soldier>();
                //if(Physics.Raycast(eyeSightPosition.position, (item.transform.position - eyeSightPosition.position), eyeSightLength ,obstacleMask))
                    if(soldier && !enemiesClose.Contains(soldier))
                        enemiesClose.Add(soldier);
            }
        }
        if(enemiesClose.Count > 0)
        {
            if(enemiesClose.Count >= 3)
            {
                fleeState.attacker = enemiesClose[0].transform;
                SM.SetState<FleeSoldierState>();
            }
            else
            {
                var soldierTarget = enemiesClose[0].GetComponent<Soldier>();
                foreach (Soldier enemy in enemiesClose)
                    if (Vector3.Distance(enemy.transform.position, transform.position) > Vector3.Distance(soldierTarget.transform.position, transform.position))
                        soldierTarget = enemy;
                if(!SM.IsActualState<CombatSoldierState>() || !isattacking)
                {
                    combatState.target = soldierTarget;
                    SM.SetState<CombatSoldierState>();
                }
            }
        }
        #endregion

        //Debug.Log(SM.currentstate + " " + this.name);
        //Debug.Log(this.name + " " + isattacking + " Is attacking");

        SM.Update();
    }

    void InstanceAttackHeavy()
    {
        Instantiate(heavyAttack, attackPosition.position, attackPosition.rotation);
    }

    void InstanceAttackLight()
    {
        Instantiate(lightAttack, attackPosition.position, attackPosition.rotation);
    }

    public override void HeavyAttack()
    {
        base.HeavyAttack();
        SM.SetState<HeavyAttackSoldierState>();
    }

    public override void LightAttack()
    {
        base.LightAttack();
        SM.SetState<LightAttackSoldierState>();
    }

    public override void TakeDMG(int DMG)
    {
        base.TakeDMG(DMG);
        SM.SetState<HitSoldierState>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, obsAvoidanceRadious);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(eyeSightPosition.position, eyeSightLength);

        Gizmos.DrawRay(eyeSightPosition.position, transform.forward * eyeSightLength);
        var temp = Physics.OverlapSphere(eyeSightPosition.position, eyeSightLength, EnemyLayer);
        foreach (var item in temp)
            Gizmos.DrawRay(eyeSightPosition.position, (item.transform.position - eyeSightPosition.position));
    }
}
