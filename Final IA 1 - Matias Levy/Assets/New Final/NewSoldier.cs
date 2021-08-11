using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSoldier : BaseUnit
{
    NewSoldierCombatState combatState;
    FleeState fleeState;
    NewSoldierFlockState flockState;
    HitState stunnedState;
    DieState deadState;

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
        deadState = new DieState(SM, this);

        SM.AddState(combatState);
        SM.AddState(fleeState);
        SM.AddState(flockState);
        SM.AddState(stunnedState);
        SM.AddState(deadState);
        SM.SetState<NewSoldierFlockState>();

    }

    private void Update()
    {
        SM.Update();
        if (stateDebug)
            stateDebug.text = SM.currentstate.ToString();
        if (dead)
            return;

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
                    if (!U.dead && !enemiesSeen.Contains(U) && U.gameObject.layer != gameObject.layer)
                        enemiesSeen.Add(U);
            }
        }
        if (enemiesSeen.Count > 0)
            seesEnemy = true;
        else
            seesEnemy = false;
        BaseUnit def = null;
        foreach (var enemy in enemiesSeen)
        {
            if (def == null)
                def = enemy;
            if (Vector3.Distance(transform.position, enemy.transform.position) < Vector3.Distance(transform.position, def.transform.position))
                def = enemy;
        }
        soldierTarget = def;
        #endregion

        #region attack delay
        currentAttackDelay += Time.deltaTime;
        if (currentAttackDelay >= attackDelay)
            canAttack = true;
        else
            canAttack = false;
        #endregion

        #region Check HP
        if (currentHealth < maxHealth / 4)
        {
            if (!lowHP)
                lowHP = true;
        }
        else
    if (lowHP)
            lowHP = false;
        #endregion

        #region remuevo enemigos muertos de forma segura
        List<BaseUnit> BUU = new List<BaseUnit>();
        foreach (var enemy in enemiesSeen)
            if (enemy.dead)
                BUU.Add(enemy);
        foreach (var enemy in BUU)
            enemiesSeen.Remove(enemy);
        #endregion

        #region modificador de velocidad
        var obs = GetObstacle(transform, obsAvoidanceRadious, obstacleMask);
        if (obs)
        {
            var dis = Vector3.Distance(transform.position, obs.transform.position);
            if (dis > 1)
                dis = 1;

            walkSpeed *= dis;
            runSpeed *= dis;
        }
        else
        {
            walkSpeed = walkSpeedDefault;
            runSpeed = runSpeedDefault;
        }
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

    public override void Die()
    {
        base.Die();
        if(SM.currentstate != deadState)
        {
            SM.SetState<DieState>();
        }
    }
}
