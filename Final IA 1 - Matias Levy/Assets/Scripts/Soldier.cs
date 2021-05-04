using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : BaseUnit
{
    public ParticleSystem hitParticle;

    public GameObject objective;

    Soldier[] Batallion;

    General commander;

    public HitBox lightAttack;
    public HitBox heavyAttack;

    //states
    IdleSoldierState idleState;
    GoToSoldierState walkToState;
    GoToRunSoldierState runToState;
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
        fleeState = new FleeSoldierState(SM, this);
        attackLightState = new LightAttackSoldierState(SM, this);
        attackHeavyState = new HeavyAttackSoldierState(SM, this);
        dieState = new DieSoldierState(SM, this);
        combatState = new CombatSoldierState(SM, this);
        hitState = new HitSoldierState(SM, this);

        SM.AddState(idleState);
        SM.AddState(walkToState);
        SM.AddState(runToState);
        SM.AddState(fleeState);
        SM.AddState(attackLightState);
        SM.AddState(attackHeavyState);
        SM.AddState(dieState);
        SM.AddState(combatState);
        SM.AddState(hitState);
        SM.SetState<IdleSoldierState>();

        var temp = Physics.OverlapSphere(eyeSightPosition.position, eyeSightLength, gameObject.layer);
        if(temp.Length>0)
        {
            foreach (var item in temp)
            {
                var G = item.GetComponent<General>();
                if(G)
                    commander = G;
            }
        }
    }

    private void Update()
    {
        if (stunned)
        {
            if(SM.currentstate != hitState)
                SM.SetState<HitSoldierState>();
            stunTime -= Time.deltaTime;
            if (stunTime <= 0)
            {
                SM.SetState<IdleSoldierState>();
                stunTime = 0;
            }
            return;
        }

        if (currentHealth <= 20)
        {
            if(enemiesClose.Count == 0)
            {
                objective = commander.gameObject;
                SM.SetState<GoToRunSoldierState>();
            }
            else
            {
                objective = commander.gameObject;
                SM.SetState<FleeSoldierState>();
            }
        }

        if(Input.GetKey(KeyCode.Space))
        {
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
                    if(soldier && !enemiesClose.Contains(soldier))
                        enemiesClose.Add(soldier);
            }
        }
        if(enemiesClose.Count > 0)
        {
            soldierTarget = enemiesClose[0].GetComponent<Soldier>();
            foreach (Soldier enemy in enemiesClose)
                if (Vector3.Distance(enemy.transform.position, transform.position) > Vector3.Distance(soldierTarget.transform.position, transform.position))
                    soldierTarget = enemy;
        }
        #endregion
        if(enemiesClose.Count >= 3)
        {
            fleeState.attacker = soldierTarget;
            SM.SetState<FleeSoldierState>();
        }
        else
        {
            if(!isattacking)
            {
                combatState.target = soldierTarget;
                SM.SetState<CombatSoldierState>();
            }
        }

        SM.Update();
    }

    void InstanceAttackHeavy()
    {
        HitBox atk = Instantiate(heavyAttack, attackPosition.position, attackPosition.rotation);
        atk.owner = this;
    }

    void InstanceAttackLight()
    {
        HitBox atk = Instantiate(lightAttack, attackPosition.position, attackPosition.rotation);
        atk.owner = this;
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

    public override void TakeDMG(int DMG, float stun)
    {
        base.TakeDMG(DMG,stun);
        stunTime = stun;
        stunned = true;
    }
}
