using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : BaseUnit
{
    Soldier[] Batallion;

    General _commander;
    public General commander
    {
        get
        {
            return _commander;
        }
        set
        {
            _commander = value;
        }
    }

    public HitBox lightAttack;
    public HitBox heavyAttack;

    //states
    IdleState idleState;
    GoToState walkToState;
    GoToRunState runToState;
    FleeState fleeState;
    DieState dieState;
    HitState hitState;
    LightAttackSoldierState attackLightState;
    HeavyAttackSoldierState attackHeavyState;
    CombatSoldierState combatState;

    private void Start()
    {
        idleState = new IdleState(SM, this);
        walkToState = new GoToState(SM, this);
        runToState = new GoToRunState(SM, this, runSpeed, "Walking");
        fleeState = new FleeState(SM, this);
        attackLightState = new LightAttackSoldierState(SM, this);
        attackHeavyState = new HeavyAttackSoldierState(SM, this);
        dieState = new DieState(SM, this);
        combatState = new CombatSoldierState(SM, this);
        hitState = new HitState(SM, this);

        SM.AddState(idleState);
        SM.AddState(walkToState);
        SM.AddState(runToState);
        SM.AddState(fleeState);
        SM.AddState(attackLightState);
        SM.AddState(attackHeavyState);
        SM.AddState(dieState);
        SM.AddState(combatState);
        SM.AddState(hitState);
        SM.SetState<IdleState>();

    }

    private void Update()
    {
        if (stateDebug != null)
            stateDebug.text = SM.currentstate + " HP: " + currentHealth + " Obj: " + soldierTarget;

        if (SM.currentstate == dieState)
            return;
        AN.SetBool("Stunned", stunned);

        SM.Update();

        if (stunned)
        {
            if (SM.currentstate != hitState)
                SM.SetState<HitState>();

            canAttack = false;
            stunTime -= Time.deltaTime;
            if (stunTime <= 0)
            {
                if(SM.currentstate!= idleState)
                    SM.SetState<IdleState>();
                stunTime = 0;
            }
            return;
        }

        if(GoToTroop)
        {
            //poner un objetivo al azar o hace un estado wander donde camina al azar
        }


        if (currentHealth <= 40)
        {
            BaseUnit closest = null;
            if(enemiesSeen.Count >= 1)
            {
                foreach (var item in enemiesSeen)
                {
                    if (closest == null)
                        closest = item;

                    if (Vector3.Distance(item.transform.position, transform.position) < Vector3.Distance(closest.transform.position, transform.position))
                        closest = item;
                }
            }
            if(closest)
            {
                if(Vector3.Distance(closest.transform.position,transform.position) <= 3)
                {
                    if(!lowHP)
                    {
                        fleeState.attacker = closest;
                        SM.SetState<FleeState>();
                    }
                }
                else
                {
                    objective = _commander.gameObject;
                    if(SM.currentstate != walkToState)
                        SM.SetState<GoToRunState>();
                }
            }
            return;
        }

        if(soldierTarget != null)
        {
            if (soldierTarget.dead)
            {
                enemiesSeen.Remove(soldierTarget);
                soldierTarget = null;
            }
        }

        #region Busqueda de enemigos
        var temp = Physics.OverlapSphere(eyeSightPosition.position, visionRange, enemyLayer);
        if(temp.Length > 0)
        {
            foreach (var item in temp)
            {
                var soldier = item.GetComponent<BaseUnit>();
                    if(soldier && !enemiesSeen.Contains(soldier) && !soldier.dead)
                        enemiesSeen.Add(soldier);
            }
        }
        #endregion

        if (enemiesSeen.Count > 0 && SM.currentstate != combatState)
        {
            soldierTarget = enemiesSeen[0];
            if(soldierTarget)
                foreach (BaseUnit enemy in enemiesSeen)
                    if (Vector3.Distance(enemy.transform.position, transform.position) > Vector3.Distance(soldierTarget.transform.position, transform.position))
                        soldierTarget = enemy;
            if(!canAttack)
            {
                combatState.target = soldierTarget;
                if(SM.currentstate != combatState)
                    SM.SetState<CombatSoldierState>();
            }
        }
    }

    void InstanceAttackHeavy()
    {
        HitBox atk = Instantiate(heavyAttack, attackPosition.position, attackPosition.rotation);
        atk.enemyTag = enemyTag;
        atk.owner = this;
    }

    void InstanceAttackLight()
    {
        HitBox atk = Instantiate(lightAttack, attackPosition.position, attackPosition.rotation);
        atk.enemyTag = enemyTag;
        atk.owner = this;
    }

    void FinishAttack()
    {
        canAttack = false;
    }

    public override void AttackRouletteWheel()
    {
        float R = Random.Range(0, _totalAttackWeight);
        R = Mathf.RoundToInt(R);
        foreach (var Attack in _attacks)
        {
            R -= Attack.Value;
            if (R <= 0)
            {
                if (Attack.Key == "Light")
                    SM.SetState<LightAttackSoldierState>();
                else if (Attack.Key == "Heavy")
                    SM.SetState<HeavyAttackSoldierState>();
            }
        }
    }

    public override void TakeDMG(int DMG, float stun)
    {
        base.TakeDMG(DMG,stun);
        hitParticle.Play();
        stunTime += stun;
        stunned = true;
        AN.SetFloat("Health", currentHealth);
        if (currentHealth <= 0)
            SM.SetState<DieState>();
    }

    //void Die()
    //{
    //    AN.speed = 0;
    //}
}
