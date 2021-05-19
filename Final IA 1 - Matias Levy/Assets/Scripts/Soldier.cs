using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : BaseUnit
{
    public ParticleSystem hitParticle;
    public ParticleSystem stunnedParticle;

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
        walkToState = new GoToSoldierState(SM, this, walkSpeed, "Running");
        runToState = new GoToRunSoldierState(SM, this, runSpeed, "Walking");
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
        AN.SetBool("Stunned", stunned);
        debugText.text = SM.currentstate.ToString();
        SM.Update();

        while (stunned)
        {
            if (SM.currentstate != hitState)
                SM.SetState<HitSoldierState>();

            isattacking = false;
            stunTime -= Time.deltaTime;
            if (stunTime <= 0)
            {
                if(SM.currentstate!= idleState)
                    SM.SetState<IdleSoldierState>();
                stunTime = 0;
            }
            return;
        }

        if (currentHealth <= 20)
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

            if(Vector3.Distance(closest.transform.position,transform.position) <= 3)
            {
                fleeState.attacker = closest;
                if(SM.currentstate != fleeState)
                    SM.SetState<FleeSoldierState>();
            }
            else
            {
                objective = commander.gameObject;
                if(SM.currentstate != walkToState)
                    SM.SetState<GoToRunSoldierState>();
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
        if(enemiesSeen.Count > 0)
        {
            soldierTarget = enemiesSeen[0].GetComponent<Soldier>();
            foreach (Soldier enemy in enemiesSeen)
                if (Vector3.Distance(enemy.transform.position, transform.position) > Vector3.Distance(soldierTarget.transform.position, transform.position))
                    soldierTarget = enemy;
        }
        #endregion

        if(enemiesSeen.Count >= 3)
        {
            fleeState.attacker = soldierTarget;
            if(SM.currentstate != fleeState)
                SM.SetState<FleeSoldierState>();
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

    public override void AttackRouletteWheel()
    {
        float R = Random.Range(0, _totalAttackWeight);
        foreach (var Attack in _attacks)
        {
            R -= Attack.Value;
            if (R <= 0)
            {
                if (Attack.Key == "Heavy")
                    SM.SetState<HeavyAttackSoldierState>();
                else if (Attack.Key == "Light")
                    SM.SetState<LightAttackSoldierState>();
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
            SM.SetState<DieSoldierState>();
    }
}
