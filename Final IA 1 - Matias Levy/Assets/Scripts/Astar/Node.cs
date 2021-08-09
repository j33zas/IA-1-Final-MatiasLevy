using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool gizmos;
    public bool wireSphere;
    public float g;
    public float h;
    public float f;
    public Node previous;
    public List<Node> neighbors = new List<Node>();
    public float radious;
    public LayerMask nodesLayer;
    public LayerMask obstacleLayer;
    public bool isBlocked;
    public bool isPath;

    public void Awake()
    {
        var temp = Physics.OverlapSphere(transform.position, radious, nodesLayer);
        foreach (var item in temp)
        {
            var node = item.GetComponent<Node>();
            if (!Physics.Raycast(transform.position, (node.transform.position - transform.position).normalized, radious, obstacleLayer))
            {
                if (node && !node.isBlocked && node != this)
                    neighbors.Add(node);
            }
        }
    }
    private void Start()
    {
        List<Node> temp = new List<Node>();
        foreach (var node in neighbors)
            if (!node.neighbors.Contains(this))
                temp.Add(node);

        foreach (var item in temp)
            neighbors.Remove(item);

        if (neighbors.Count == 0)
            isBlocked = true;
    }
    public void Reset()
    {
        g = Mathf.Infinity;
        isPath = false;
        previous = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if(gizmos)
        {
            if (isBlocked)
                Gizmos.color = Color.red;
            else
            {
                foreach (var item in neighbors)
                    Gizmos.DrawLine(transform.position, item.transform.position);
            }
            if(wireSphere)
                Gizmos.DrawWireSphere(transform.position, radious);
            if (isPath)
                Gizmos.color = Color.green;
        }
        Gizmos.DrawSphere(transform.position, 0.5f);      
    }
}
