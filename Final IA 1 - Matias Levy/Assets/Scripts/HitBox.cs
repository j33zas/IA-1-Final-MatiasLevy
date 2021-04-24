using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public int minDMG;
    public int maxDMG;
    public float lifeTime;

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

    private void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.layer == _enemyLayer)
        {
            var enemy = coll.GetComponent<BaseUnit>();
            enemy.TakeDMG(Random.Range(minDMG,maxDMG));
        }
    }
}
