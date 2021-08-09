using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralFireBall : MonoBehaviour
{
    public float speed;
    public LayerMask obstacles;
    public LayerMask redTeam;
    public LayerMask blueTeam;

    void Start()
    {
        
    }
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider coll)
    {
        Debug.Log(coll.gameObject.name);
        Destroy(gameObject, .01f);
    }
}
