using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class General : BaseUnit
{
    public HitBox RangedATK;
    public HitBox lightATK;
    public HitBox heavyATK;

    List<Soldier> myTroops = new List<Soldier>();
    List<Soldier> soldiersClose = new List<Soldier>();

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
        SM.Update();

        var temp = Physics.OverlapSphere(eyeSightPosition.position, eyeSightLength, gameObject.layer);
        foreach (var item in temp)
        {
            var soldier= item.GetComponent<Soldier>();
            if(soldier)
            {
                if(!soldiersClose.Contains(soldier))
                {
                    soldiersClose.Add(soldier);
                }
            }
        }

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
        }

        foreach (var ally in soldiersClose)
        {
            if(ally.currentHealth <=20)
            {
                var obj = objective.GetComponent<BaseUnit>();
                if (obj && ally.currentHealth < obj.currentHealth)
                    objective = ally.gameObject;
                if(SM.currentstate != healState)
                    SM.SetState<GeneralHealingState>();
            }
        }


    }
}
