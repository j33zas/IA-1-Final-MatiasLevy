using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : BaseUnit
{
    Soldier[] Batallion;

    General commander;

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
        walkToState = new GoToState(SM, this, walkSpeed, "Running");
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

        var temp = Physics.OverlapSphere(eyeSightPosition.position, eyeSightLength);
        foreach (var item in temp)
        {
            var G = item.gameObject.GetComponent<General>();
            if(G!= null)
                commander = G;
        }
    }

    private void Update()
    {
        debugText.text = SM.currentstate.ToString();
        if (SM.currentstate == dieState)
            return;
        AN.SetBool("Stunned", stunned);

        foreach (var item in enemiesSeen)
            if (item == null)
                enemiesSeen.Remove(item);

        SM.Update();
        while (stunned)
        {
            if (SM.currentstate != hitState)
                SM.SetState<HitState>();

            isattacking = false;
            stunTime -= Time.deltaTime;
            if (stunTime <= 0)
            {
                if(SM.currentstate!= idleState)
                    SM.SetState<IdleState>();
                stunTime = 0;
            }
            return;
        }

        if (_currentHealth <= 20)
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
                    fleeState.attacker = closest;
                    if(!fleeing)
                        SM.SetState<FleeState>();
                }
                else
                {
                objective = commander.gameObject;
                if(SM.currentstate != walkToState)
                    SM.SetState<GoToRunState>();
                }
            }
        }
        #region Busqueda de enemigos
        var temp = Physics.OverlapSphere(eyeSightPosition.position, eyeSightLength, enemyLayer);
        if(temp.Length > 0)
        {
            foreach (var item in temp)
            {
                var soldier = item.GetComponent<Soldier>();
                    if(soldier && !enemiesSeen.Contains(soldier))
                        enemiesSeen.Add(soldier);
            }
        }
        #endregion
        if(enemiesSeen.Count > 0)
        {
            soldierTarget = enemiesSeen[0];
            if(soldierTarget)
                foreach (Soldier enemy in enemiesSeen)
                    if (Vector3.Distance(enemy.transform.position, transform.position) > Vector3.Distance(soldierTarget.transform.position, transform.position))
                        soldierTarget = enemy;
            if(enemiesSeen.Count >= 3)
            {
                fleeState.attacker = soldierTarget;
                if(!fleeing)
                    SM.SetState<FleeState>();
            }
            else
            {
                if(!isattacking)
                {
                    combatState.target = soldierTarget;
                    if(SM.currentstate != combatState)
                        SM.SetState<CombatSoldierState>();
                }
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
        isattacking = false;
    }

    void Die()
    {
        Destroy(gameObject);
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
        AN.SetFloat("Health", _currentHealth);
        if (_currentHealth <= 0)
            SM.SetState<DieState>();
    }
}
