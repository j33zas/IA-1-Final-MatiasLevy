﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    StateMachine _SM;
    public StateMachine SM
    {
        get
        {
            return _SM;
        }
    }
    Animator _AN;
    public Animator AN
    {
        get
        {
            return _AN;
        }
    }
    AudioSource _AU;
    public AudioSource AU
    {
        get
        {
            return _AU;
        }
    }
    Rigidbody _RB;
    public Rigidbody RB
    {
        get
        {
            return _RB;
        }
    }

    public LayerMask nodesLayer;
    public LayerMask EnemyLayer;

    protected int currentHealth;
    public int maxHealth;

    public int minDamage;
    public int maxDamage;
    public float AttackDistance;

    public float rotSpeed;
    public float walkSpeed;
    public float runSpeed;

    public float obsAvoidanceRadious;
    public float obsAvoidanceWeight;


    protected bool inCombat;
    protected Node destination;
    protected bool hasDestination;
    protected float distanceToDestination;

    public List<BaseUnit> enemiesClose;

    public Transform eyeSightPosition;
    public float eyeSightLength;
    public float combatLength;

    public LayerMask obstacleMask;

    List<Node> _openNodes = new List<Node>();

    List<Node> _closedNodes = new List<Node>();

    private void Awake()
    {
        _AU = GetComponentInChildren<AudioSource>();
        _AN = GetComponentInChildren<Animator>();
        _SM = new StateMachine();
        _RB = GetComponent<Rigidbody>();
        enemiesClose = new List<BaseUnit>();
    }

    public  Node FindClosestNode(Transform target)
    {
        var nodes = Physics.OverlapSphere(target.position, 15, nodesLayer);
        Node Closest = null;
        foreach (var item in nodes)
        {
            var itemNode = item.GetComponent<Node>();
            if (Closest == null || Vector3.Distance(Closest.transform.position, target.position) > Vector3.Distance(item.transform.position, target.position))
                if (!itemNode.isBlocked)
                    Closest = item.GetComponent<Node>();
        }
        return Closest;
    }

    //public void GetPath()
    //{
    //    foreach (var item in _openNodes)
    //        item.Reset();
    //    foreach (var item in _closedNodes)
    //        item.Reset();
    //    foreach (var item in pathNodes)
    //        item.Reset();
    //    _openNodes = new List<Node>();
    //    _closedNodes = new List<Node>();
    //    pathNodes = new List<Node>();

    //    while (_openNodes.Count > 0)
    //    {
    //        Node current = SearchNextNode();
    //        _closedNodes.Add(current);
    //        foreach (var item in current.neighbors)
    //        {
    //            if (_closedNodes.Contains(item))
    //                continue;

    //            if (!_openNodes.Contains(item))
    //            {
    //                _openNodes.Add(item);

    //                item.h = Mathf.Abs(aStarEnd.transform.position.x - item.transform.position.x)
    //                           + Mathf.Abs(aStarEnd.transform.position.y - item.transform.position.y)
    //                           + Mathf.Abs(aStarEnd.transform.position.z - item.transform.position.z);
    //            }
    //            float distanceToNeighbor = Vector3.Distance(current.transform.position, item.transform.position);
    //            float newG = current.g + distanceToNeighbor;
    //            if (newG < item.g)
    //            {
    //                item.g = newG;
    //                item.f = item.g + item.h;
    //                item.previous = current;
    //            }
    //        }
    //        if (_openNodes.Contains(aStarEnd))
    //        {
    //            pathNodes.Add(aStarEnd);
    //            Node node = aStarEnd.previous;
    //            while (node)
    //            {
    //                pathNodes.Insert(0, node);
    //                node = node.previous;
    //            }
    //        }
    //    }
    //}

    public List<Node> GetAstarPath(Node startNode, Node EndNode)
    {
        foreach (var item in _openNodes)
            item.Reset();

        foreach (var item in _closedNodes)
            item.Reset();

        List<Node> result = new List<Node>();
        _openNodes = new List<Node>();
        _closedNodes = new List<Node>();

        startNode.g = 0;
        _openNodes.Add(startNode);

        while (_openNodes.Count > 0)
        {
            Node current = SearchNextNode();
            _closedNodes.Add(current);
            foreach (var currentNeighbor in current.neighbors)
            {
                if (_closedNodes.Contains(currentNeighbor))
                    continue;

                if(!_openNodes.Contains(currentNeighbor))
                {
                    _openNodes.Add(currentNeighbor);
                    currentNeighbor.h = Mathf.Abs(EndNode.transform.position.x - EndNode.transform.position.x) + Mathf.Abs(EndNode.transform.position.y - EndNode.transform.position.y) + Mathf.Abs(EndNode.transform.position.z - EndNode.transform.position.z);
                }
                float distToNeighbor = Vector3.Distance(current.transform.position, currentNeighbor.transform.position);

                float newG = current.g + distToNeighbor;

                if (newG < currentNeighbor.g)
                {
                    currentNeighbor.g = newG;
                    currentNeighbor.f = currentNeighbor.g + currentNeighbor.h;
                    currentNeighbor.previous = current;
                }
            }
            if (_openNodes.Contains(EndNode))
            {
                result.Add(EndNode);
                Node N = EndNode.previous;
                while (N)
                {
                    result.Insert(0, N);
                    N = N.previous;
                }
            }
        }
        for (int i = 0; i < result.Count-1 ; i++)//esto esta aca ya que me estaba retornando nodos duplicados? muy extraño
            for (int k = 0; k < result.Count-1; k++)
                if (result[i] == result[k] && k != i)
                    result.RemoveAt(k);
        return result;
    }

    public Node SearchNextNode()
    {
        Node n = _openNodes[0];
        for (int i = 1; i < _openNodes.Count; i++)
        {
            if (_openNodes[i].f < n.f)
            {
                n = _openNodes[i];
            }
        }
        _openNodes.Remove(n);
        return n;
    }

    public GameObject GetObstacle(Transform transform, float radious, LayerMask obstaclesLayer)
    {
        GameObject _obs = null;
        var obstacles = Physics.OverlapSphere(transform.position, radious, obstaclesLayer);
        if (obstacles.Length > 0)
        {
            foreach (var item in obstacles)
            {
                if (!_obs)
                    _obs = item.gameObject;
                else if (Vector3.Distance(item.transform.position, transform.position) < Vector3.Distance(_obs.transform.position, transform.position))
                    _obs = item.gameObject;
            }
        }
        return _obs;
    }

    public virtual void LightAttack()
    {

    }

    public virtual void HeavyAttack()
    {

    }

    public virtual void TakeDMG(int DMG)
    {
        currentHealth -= DMG;
    }
}
