using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGeneral : BaseUnit
{
    [Range(0,1)]
    public int mouseSelect;

    public GameObject gun;
    GoToState walkTo;
    IdleState idleing;
    NewGeneralCombatState combatState;
    NewGeneralHealState healState;
    NewGeneralFleeState fleeState;

    List<BaseUnit> allEnemyUnits = new List<BaseUnit>();

    private void Start()
    {
        walkTo = new GoToState(SM, this);
        idleing = new IdleState(SM, this);
        combatState = new NewGeneralCombatState(SM, this);
        healState = new NewGeneralHealState(SM, this);
        fleeState = new NewGeneralFleeState(SM, this);
        SM.AddState(walkTo);
        SM.AddState(idleing);
        SM.AddState(combatState);
        SM.AddState(healState);
        SM.AddState(fleeState);
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
        if (Input.GetMouseButtonDown(mouseSelect))
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
            RaycastHit hit;
            Ray raymond = Camera.main.ScreenPointToRay(Input.mousePosition);
            Collider[] colliders = new Collider[1];

            if(Physics.Raycast(raymond, out hit))
                colliders = Physics.OverlapSphere(hit.point, 3);

            objective.transform.position = hit.point;
            SM.SetState<GoToState>();
        }
        #endregion

        #region centre point calculation

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
            if(seesEnemy)
            {
                if (soldierTarget)
                    fleeState.enemy = soldierTarget.gameObject;

                if(SM.currentstate != fleeState)
                {
                    SM.SetState<NewGeneralFleeState>();
                }
            }
            else
            {
                if (SM.currentstate != healState)
                {
                    SM.SetState<NewGeneralHealState>();
                }
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
                    if (!U.dead && !enemiesSeen.Contains(U))
                        enemiesSeen.Add(U);
            }
        }
        if (enemiesSeen.Count > 0)
            seesEnemy = true;
        else
            seesEnemy = false;
        #endregion

        #region Check HP
        if (currentHealth < currentHealth / 4)
        {
            if(!lowHP)
                lowHP = true;
        }
        else
            if (lowHP)
                lowHP = false;
        #endregion

        #region shooting delay
        currentAttackDelay += Time.deltaTime;
        if (currentAttackDelay >= attackDelay)
        {
            currentAttackDelay = attackDelay;
            canAttack = true;
        }
        else
            canAttack = false;
        #endregion
    }

    public void Shoot()
    {
        if (canAttack)
        {
            var b = Instantiate(attack, attackPosition.transform.position, attackPosition.transform.rotation);
            b.owner = this;
            currentAttackDelay = 0;
        }
    }

    public override void TakeDMG(int DMG, float stun)
    {
        base.TakeDMG(DMG, stun);
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
