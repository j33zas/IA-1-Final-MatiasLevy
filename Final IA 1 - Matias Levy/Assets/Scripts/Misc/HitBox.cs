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
    public string enemyTag;


    private void Awake()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider coll)
    {
        var enemy = coll.GetComponentInParent<BaseUnit>();
        if(enemy && enemy.gameObject != owner.gameObject)
            enemy.TakeDMG(Random.Range(minDMG, maxDMG), Random.Range(minStun, maxStun));
    }
}
