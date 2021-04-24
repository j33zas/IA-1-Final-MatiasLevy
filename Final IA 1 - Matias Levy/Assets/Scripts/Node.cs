using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
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
        if (temp.Length == 0)
            isBlocked = true;

        foreach (var item in temp)
        {
            var node = item.GetComponent<Node>();

            if (!Physics.Raycast(transform.position, (node.transform.position - transform.position).normalized, radious, obstacleLayer))
            {
                if (node && !node.isBlocked && node != this)
                    neighbors.Add(node);
            }
            else if (node.neighbors.Contains(this))
                node.neighbors.Remove(this);
        }
    }

    public void Reset()
    {
        g = Mathf.Infinity;
        previous = null;
    }

    private void OnDrawGizmos()
    {
        if (isBlocked)
            Gizmos.color = Color.red;
        else if (isPath)
        {
            Gizmos.color = Color.green;
            if(previous)
                Gizmos.DrawLine(transform.position, previous.transform.position);
        }
        else
        {
            Gizmos.color = Color.blue;
            foreach (var item in neighbors)
                Gizmos.DrawLine(transform.position, item.transform.position);
        }

        //Gizmos.DrawWireSphere(transform.position, radious);
        Gizmos.DrawSphere(transform.position, 0.3f);        
    }
}
