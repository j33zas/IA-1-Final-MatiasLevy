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

    public Soldier owner;
    LayerMask enemyLayer;

    private void Start()
    {
        if (owner.gameObject.layer == 11)
            enemyLayer.value = 12;
        else if (owner.gameObject.layer == 12)
            enemyLayer.value = 11;
    }

    private void Update()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerStay(Collider coll)
    {
        var enemy = coll.GetComponent<BaseUnit>();
        if(enemy)
            if(((1 << coll.gameObject.layer) & enemyLayer) == 0)
                if(!enemiesHit.Contains(enemy))
                {
                    enemy.TakeDMG(Random.Range(minDMG,maxDMG), Random.Range(minStun, maxStun));
                    enemiesHit.Add(enemy);
                }
    }
}
