using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public int minDMG;
    public int maxDMG;

    public float minStun;
    public float maxStun;

    public float lifeTime;

    List<BaseUnit> enemiesHit = new List<BaseUnit>();

    Soldier _owner;
    public Soldier owner
    {
        get
        {
            return _owner;
        }
        set
        {
            _owner = value;
        }
    }
    LayerMask _enemyLayer;
    public LayerMask enemyLayer
    {
        get
        {
            return _enemyLayer;
        }
        set
        {
            _enemyLayer = value;
        }
    }

    private void Update()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerStay(Collider coll)
    {
        if(enemyLayer == (coll.gameObject.layer | (1 << coll.gameObject.layer)))
        {
            Debug.Log("golpee a " + coll.name);
            var enemy = coll.GetComponent<BaseUnit>();
            if(!enemiesHit.Contains(enemy))
            {
                enemy.TakeDMG(Random.Range(minDMG,maxDMG), minStun,maxStun);
                enemiesHit.Add(enemy);
            }
        }
    }
}
