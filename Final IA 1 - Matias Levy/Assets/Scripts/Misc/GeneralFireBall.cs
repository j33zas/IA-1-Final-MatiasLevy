using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralFireBall : MonoBehaviour
{
    public float speed;
    public LayerMask obstacle;
    void Start()
    {
        
    }
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.layer == obstacle)
            Destroy(gameObject, 0.2f);
    }
}
