using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGeneral : BaseUnit
{
    public GameObject gun;
    public ParticleSystem healedPart;
    public ParticleSystem healingPart;

    GoToState walkTo;
    IdleState idleing;
    NewGeneralCombatState combatState;
    NewGeneralHealState healState;
    NewGeneralFleeState fleeState;
    DieState deadState;

    public bool isSafe = false;
    public float HealDelay;
    bool canHeal;
    float currentHealDelay;

    private void Start()
    {
        walkTo = new GoToState(SM, this);
        idleing = new IdleState(SM, this);
        combatState = new NewGeneralCombatState(SM, this);
        healState = new NewGeneralHealState(SM, this);
        fleeState = new NewGeneralFleeState(SM, this);
        deadState = new DieState(SM, this);

        SM.AddState(walkTo);
        SM.AddState(idleing);
        SM.AddState(combatState);
        SM.AddState(healState);
        SM.AddState(fleeState);
        SM.AddState(deadState);
        SM.SetState<IdleState>();
        objective = new GameObject();

        var temp = Physics.OverlapSphere(eyeSightPosition.position, visionRange);

        foreach (var coll in temp)
        {
            NewSoldier ns = coll.gameObject.GetComponentInParent<NewSoldier>();
            if (ns != null && ns.gameObject.layer==gameObject.layer)
                ns.General = this;
        }

    }

    private void Update()
    {
        SM.Update();

        #region soleccion de objetivo (RTS movement)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(path != null)
            {
                foreach (var node in path)
                {
                    node.Reset();
                    node.isPath = false;
                }
                path = null;
            }
            SM.SetState<GoToState>();
        }
        #endregion

        #region centre point calculation

        float posX = 0;
        float posY = 0;
        float posZ = 0;
        float centreX = 0;
        float centreY = 0;
        float centreZ = 0;

        if(gameObject.layer == LayerMask.NameToLayer("BlueTeam"))
        {
            foreach (var item in GameManager.gameManager.redTeam)
            {
                posX += item.transform.position.x;
                posY += item.transform.position.y;
                posZ += item.transform.position.z;
            }
            centreX = posX / GameManager.gameManager.redTeam.Count;
            centreY = posY / GameManager.gameManager.redTeam.Count;
            centreZ = posZ / GameManager.gameManager.redTeam.Count;
        }
        if(gameObject.layer == LayerMask.NameToLayer("RedTeam"))
        {
            foreach (var item in GameManager.gameManager.blueTeam)
            {
                posX += item.transform.position.x;
                posY += item.transform.position.y;
                posZ += item.transform.position.z;
            }
            centreX = posX / GameManager.gameManager.blueTeam.Count;
            centreY = posY / GameManager.gameManager.blueTeam.Count;
            centreZ = posZ / GameManager.gameManager.blueTeam.Count;
        }
        if (GameManager.gameManager.blueTeam.Count < 1 || GameManager.gameManager.redTeam.Count < 1)
            objective.transform.position = new Vector3(centreX, centreY, centreZ);
        else
            objective.transform.position = Vector3.zero;
        #endregion

        #region Enter Combat
        if (seesEnemy)
        {
            if (SM.currentstate != combatState && !lowHP)
            {
                soldierTarget = enemiesSeen[0];
                foreach (BaseUnit enemy in enemiesSeen)
                    if (Vector3.Distance(enemy.transform.position, transform.position) > Vector3.Distance(soldierTarget.transform.position, transform.position))
                        soldierTarget = enemy;
                SM.SetState<NewGeneralCombatState>();
            }
        }
        else
            soldierTarget = null;
        #endregion

        #region fleeing
        if(lowHP)
        {
            if(isSafe)
            {
                if (SM.currentstate != healState)
                    SM.SetState<NewGeneralHealState>();
            }
            else
            {
                if(SM.currentstate != fleeState)
                    SM.SetState<NewGeneralFleeState>();
            }
        }
        #endregion
    }
    private void FixedUpdate()
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
        {
            seesEnemy = true;
            isSafe = false;
        }
        else
            seesEnemy = false;
        #endregion

        #region Check HP
        if (currentHealth < maxHealth / 2)
            lowHP = true;
        else
            lowHP = false;
        #endregion

        #region cooldowns
        currentAttackDelay += Time.deltaTime;
        if (currentAttackDelay >= attackDelay)
        {
            currentAttackDelay = attackDelay;
            canAttack = true;
        }
        else
            canAttack = false;
        currentHealDelay += Time.deltaTime;
        if (currentHealDelay >= HealDelay)
        {
            currentHealDelay = HealDelay;
            canHeal = true;
        }
        else
            canAttack = false;
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
        if(obs)
        {
            var dis = Vector3.Distance(transform.position, obs.transform.position);
            if(dis>1)
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

    public void Shoot()
    {
        if (canAttack)
        {
            var b = attack;
            b.owner = this;
            Instantiate(b, attackPosition.transform.position, attackPosition.transform.rotation);
            currentAttackDelay = 0;
        }
    }

    public void Heal(int value)
    {
        if(canHeal)
        {
            healedPart.Play();
            currentHealth += value;
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
            currentHealDelay = 0;
        }
    }

    public override void Die()
    {
        base.Die();
        if (SM.currentstate != deadState)
        {
            SM.SetState<DieState>();
        }
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        #region Pos to go - node to go
        Gizmos.color = Color.yellow;
        if (objective != null)
            Gizmos.DrawCube(objective.transform.position, new Vector3(.3f, .3f, .3f));
        #endregion

        var temp = Physics.OverlapSphere(eyeSightPosition.position, visionRange, enemyLayer);
        foreach (var coll in temp)
        {
            RaycastHit hit;
            if (Physics.Raycast(eyeSightPosition.position, (coll.transform.position - transform.position), out hit, visionRange))
            {
                var e = hit.collider.GetComponent<BaseUnit>();
                if (e)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(eyeSightPosition.position, coll.transform.position);
                }
                else
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(eyeSightPosition.position, hit.point);
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(hit.point, coll.transform.position);
                }
            }
        }
    }
}
