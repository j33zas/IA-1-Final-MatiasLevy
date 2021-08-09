using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    List<Transform> allies = new List<Transform>();
    public LayerMask alliesLayer;
    public float radious;
    public Transform andreaLeader;
    public float alineationWeight;
    public float separationWeight;
    public float cohesionWeight;
    public float leaderWeight;
    public float speed;
    public float rotSpeed;

    void Update()
    {
        var temp = Physics.OverlapSphere(transform.position, radious, alliesLayer);
        foreach (var item in temp)
            allies.Add(item.transform);

        Vector3 dir = GetCohesion() * cohesionWeight +
                      GetAlineation() * alineationWeight +
                      GetSeparation() * separationWeight +
                      GetLeader() * leaderWeight;

        transform.forward = Vector3.Slerp(transform.forward, dir, Time.deltaTime * rotSpeed);
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    Vector3 GetCohesion()
    {
        Vector3 cohesion = Vector3.zero;
        foreach (var ally in allies)
        {
            cohesion += ally.position - transform.position;
        }

        cohesion /= allies.Count;
        return cohesion;
    }

    Vector3 GetSeparation()
    {
        Vector3 separation = Vector3.zero;
        foreach (var ally in allies)
        {
            Vector3 dirToAlly = transform.position - ally.position;
            float distToRadious = radious - dirToAlly.magnitude;
            dirToAlly.Normalize();
            dirToAlly *= distToRadious;
            separation += dirToAlly;
        }
        separation /= allies.Count;
        return separation;
    }

    Vector3 GetAlineation()
    {
        Vector3 alineation = Vector3.zero;
        foreach (var ally in allies)
        {
            alineation += ally.forward;
        }
        alineation /= allies.Count;
        return alineation;
    }

    Vector3 GetLeader()
    {
        return andreaLeader.position - transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radious);
    }
}
