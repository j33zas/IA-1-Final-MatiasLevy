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

    public BaseUnit owner;
    public LayerMask enemyLayer;
    
    private void Update()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider coll)
    {
        if(((1 << coll.gameObject.layer) & enemyLayer) == 0)
        {
            var enemy = coll.GetComponent<BaseUnit>();
            if(enemy != owner)
            {
                enemy.TakeDMG(Random.Range(minDMG, maxDMG), Random.Range(minStun, maxStun));
            }
        }
    }
}
