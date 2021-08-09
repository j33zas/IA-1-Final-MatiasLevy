using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSoldier : BaseUnit
{
    NewSoldierCombatState combatState;
    FleeState fleeState;
    NewSoldierFlockState flockState;
    HitState stunnedState;

    NewGeneral _G;
    public NewGeneral General
    {
        get
        {
            return _G;
        }
        set
        {
            _G = value;
        }
    }

    public float alineationWeight;
    public float separationWeight;
    public float cohesionWeight;
    public float leaderWeight;

    private void Start()
    {
        combatState = new NewSoldierCombatState(SM, this);
        fleeState = new FleeState(SM, this);
        flockState = new NewSoldierFlockState(SM, this);
        stunnedState = new HitState(SM, this);
        SM.AddState(combatState);
        SM.AddState(fleeState);
        SM.AddState(flockState);
        SM.AddState(stunnedState);
        SM.SetState<NewSoldierFlockState>();

    }

    private void Update()
    {
        if (stateDebug)
            stateDebug.text = SM.currentstate.ToString();
        SM.Update();
        if(enemiesSeen.Count > 0)
        {
            if (soldierTarget == null)
                soldierTarget = enemiesSeen[0];

            foreach (BaseUnit enemy in enemiesSeen)
                if (Vector3.Distance(enemy.transform.position, transform.position) > Vector3.Distance(soldierTarget.transform.position, transform.position))
                    soldierTarget = enemy;
        }

        if (seesEnemy && !lowHP)
        {
            if (SM.currentstate != combatState && !lowHP)
                SM.SetState<NewSoldierCombatState>();
        }
        else if (General && !lowHP)
        {
            if (SM.currentstate != flockState)
                SM.SetState<NewSoldierFlockState>();
        }
        if(lowHP)
        {
            if (SM.currentstate != fleeState)
                SM.SetState<FleeState>();
        }
    }
    private void LateUpdate()
    {
        SM.LateUpdate();
        #region Busqueda de enemigos
        var temp = Physics.OverlapSphere(eyeSightPosition.position, visionRange, enemyLayer);
        foreach (var coll in temp)
        {
            RaycastHit hit;
            if (Physics.Raycast(eyeSightPosition.position, (coll.transform.position - transform.position), out hit, visionRange))
            {
                BaseUnit U = hit.collider.GetComponentInParent<BaseUnit>();
                if (U)
                    if (!U.dead && !enemiesSeen.Contains(U))
                        enemiesSeen.Add(U);
            }
        }
        if (enemiesSeen.Count > 0)
            seesEnemy = true;
        else
            seesEnemy = false;
        #endregion

        #region attack delay
        currentAttackDelay += Time.deltaTime;
        if (currentAttackDelay >= attackDelay)
            canAttack = true;
        else
            canAttack = false;
        #endregion
    }

    public void Strike()
    {
        if (canAttack)
        {
            var a = Instantiate(attack, attackPosition.transform.position, attackPosition.transform.rotation);
            a.owner = this;
            currentAttackDelay = 0;
        }
    }
}
