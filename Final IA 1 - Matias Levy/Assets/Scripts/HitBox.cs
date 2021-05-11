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
    
    private void Update()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider coll)
    {
        Debug.Log(coll.gameObject.tag + " my owner: " + owner.name + "- my enemy: " + enemyTag);
        if(coll.gameObject.tag == enemyTag)
        {
            var enemy = coll.GetComponent<BaseUnit>();
            enemy.TakeDMG(Random.Range(minDMG, maxDMG), Random.Range(minStun, maxStun));
        }
    }
}
