using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class General : BaseUnit
{
    public HitBox RangedATK;
    public HitBox lightATK;
    public HitBox heavyATK;

    public Transform rangedPos;
    public float healSpeed;
    public bool healing = false;

    List<Soldier> myTroops = new List<Soldier>();
    List<Soldier> soldiersClose = new List<Soldier>();
    public ParticleSystem HealParticles;

    IdleState idleState;
    GoToState WalkToState;
    GoToRunState runToState;
    FleeState fleestate;
    DieState dieState;
    HitState hitState;
    GeneralChargeState chargeState;
    GeneralCombatState combatState;
    GeneralHealingState healState;
    GeneralHeavyAttackState heavyATKState;
    GeneralLightAttackState lightATKState;
    GeneralRangedAttackState rangedATKState;

    private void Start()
    {
        idleState = new IdleState(SM, this);
        WalkToState = new GoToState(SM, this, walkSpeed, "Walking");
        runToState = new GoToRunState(SM, this, runSpeed, "Running");
        fleestate = new FleeState(SM, this);
        dieState = new DieState(SM, this);
        hitState = new HitState(SM, this);
        chargeState = new GeneralChargeState(SM, this);
        combatState = new GeneralCombatState(SM, this);
        healState = new GeneralHealingState(SM, this);
        heavyATKState = new GeneralHeavyAttackState(SM, this);
        lightATKState = new GeneralLightAttackState(SM, this);
        rangedATKState = new GeneralRangedAttackState(SM, this);

        SM.AddState(idleState);
        SM.AddState(WalkToState);
        SM.AddState(runToState);
        SM.AddState(fleestate);
        SM.AddState(dieState);
        SM.AddState(hitState);
        SM.AddState(chargeState);
        SM.AddState(combatState);
        SM.AddState(healState);
        SM.AddState(heavyATKState);
        SM.AddState(lightATKState);
        SM.AddState(rangedATKState);

        SM.SetState<IdleState>();
        var temp = FindObjectsOfType<Soldier>();
        foreach (var item in temp)
            if (item.gameObject.tag == gameObject.tag)
                myTroops.Add(item);

    }

    private void Update()
    {
        debugText.text = SM.currentstate.ToString();

        SM.Update();

        if (stunned)
        {
            if (SM.currentstate != hitState)
                SM.SetState<HitState>();
            stunTime -= Time.deltaTime;
            if(stunTime<=0)
            {
                SM.SetState<IdleState>();
                stunTime = 0;
                stunned = false;
            }
            return;
        }

        if (healing)
        {
            if (SM.currentstate != healState)
                SM.SetState<GeneralHealingState>();

            var S = objective.GetComponent<BaseUnit>();
            if (S)
                if(S.currentHealth > 80)
                {
                    healing = false;
                    SM.SetState<IdleState>();
                }
            return;
        }

        var allyTemp = Physics.OverlapSphere(eyeSightPosition.position, eyeSightLength);
        foreach (var item in allyTemp)
        {
            var soldier = item.GetComponent<Soldier>();
            if(soldier && soldier.gameObject.tag == gameObject.tag)
                if(!soldiersClose.Contains(soldier))
                    soldiersClose.Add(soldier);
        }
        var enemyTemp = Physics.OverlapSphere(eyeSightPosition.position, eyeSightLength, enemyLayer);
        if(enemyTemp.Length>0)
        {
            foreach (var item in enemyTemp)
            {
                var E = item.GetComponent<BaseUnit>();
                if (E)
                    if (!enemiesSeen.Contains(E))
                        enemiesSeen.Add(E);
            }
        }
        if(enemiesSeen.Count > 0)
        {
            soldierTarget = enemiesSeen[0];
            if(soldierTarget)
                foreach (var enemy in enemiesSeen)
                    if (Vector3.Distance(enemy.transform.position, transform.position) > Vector3.Distance(soldierTarget.transform.position, transform.position))
                        soldierTarget = enemy;
        }

        if(soldierTarget)
            if(Vector3.Distance(transform.position, soldierTarget.transform.position) <= AttackDistance)
                if (SM.currentstate != combatState && !isattacking)
                    SM.SetState<GeneralCombatState>();

        foreach (var ally in soldiersClose)
        {
            if(ally.currentHealth <= 20)
            {
                if(!healing && SM.currentstate != healState)
                {
                    objective = ally.gameObject;
                    SM.SetState<GeneralHealingState>();
                }
            }
        }

    }
    void InstanceLight()
    {
        HitBox atk = Instantiate(lightATK, attackPosition.position, attackPosition.rotation);
        atk.owner = this;
        atk.enemyTag = enemyTag;
    }
    void InstanceHeavy()
    {
        HitBox atk = Instantiate(heavyATK, soldierTarget.transform.position, transform.rotation);
        atk.owner = this;
        atk.enemyTag = enemyTag;
    }
    void InstanceRanged()
    {
        HitBox atk = Instantiate(RangedATK, rangedPos.position, rangedPos.rotation);
        atk.owner = this;
        atk.enemyTag = enemyTag;
    }
    public override void AttackRouletteWheel()
    {
        base.AttackRouletteWheel();
        var R = Random.Range(0, _totalAttackWeight);
        foreach (var Attack in _attacks)
        {
            R -= Attack.Value;
            if (Attack.Key == "Heavy")
                SM.SetState<GeneralHeavyAttackState>();
            else if (Attack.Key == "Light")
                SM.SetState<GeneralLightAttackState>();
            else if (Attack.Key == "Ranged")
                SM.SetState<GeneralRangedAttackState>();
        }
    }

    public override void TakeDMG(int DMG, float stun)
    {
        base.TakeDMG(DMG, stun);
        stunTime = stun / 2;

    }
}
