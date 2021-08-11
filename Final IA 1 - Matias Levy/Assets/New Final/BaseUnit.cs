using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public bool gizmos = false;
    public bool ShowNodes = false;

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

    Collider _COLL;
    public Collider COLL
    {
        get
        {
            return _COLL;
        }
    }

    GameObject _objective;
    public GameObject objective
    {
        get
        {
            return _objective;
        }
        set
        {
            _objective = value;
        }
    }

    Node[] _path;
    public Node[] path
    {
        get
        {
            return _path;
        }
        set
        {
            _path = value;
        }
    }

    [SerializeField]
    int _currentHealth;
    public int currentHealth
    {
        get
        {
            return _currentHealth;
        }
        set
        {
            _currentHealth = value;
        }
    }

    public LayerMask nodesLayer;
    public LayerMask enemyLayer;
    public LayerMask allyLayer;
    public string enemyTag;

    public StateText stateDebug;

    public string[] attackString;
    public int[] attackValues;

    public int maxHealth;

    public float AttackDistance;
    public bool stunned;
    public bool lowHP;
    public bool seesEnemy;
    public bool dead = false;
    public bool GoToTroop;
    public float stunTime = 0;
    public float attackDelay;
    protected float currentAttackDelay = 0;

    public HitBox attack;

    public float rotSpeed;
    public float walkSpeed;
    protected float walkSpeedDefault;
    public float runSpeed;
    protected float runSpeedDefault;

    public float obsAvoidanceRadious;
    public float obsAvoidanceWeight;

    BaseUnit _soldierTarget;
    public BaseUnit soldierTarget
    {
        get
        {
            return _soldierTarget;
        }
        set
        {
            _soldierTarget = value;
        }
    }
    
    public bool canAttack = true;
    protected Node destination;
    protected bool hasDestination;
    protected float distanceToDestination;

    public List<BaseUnit> enemiesSeen;

    public Transform eyeSightPosition;
    public Transform attackPosition;
    public float visionRange;

    public ParticleSystem hitParticle;
    public ParticleSystem stunnedParticle;

    protected Dictionary<string, int> _attacks = new Dictionary<string, int>();
    protected float _totalAttackWeight = 10;

    public LayerMask obstacleMask;

    List<Node> _openNodes = new List<Node>();

    List<Node> _closedNodes = new List<Node>();

    private void Awake()
    {
        _AU = GetComponentInChildren<AudioSource>();
        _AN = GetComponentInChildren<Animator>();
        _RB = GetComponent<Rigidbody>();
        _COLL = GetComponent<Collider>();
        _SM = new StateMachine();
        enemiesSeen = new List<BaseUnit>();
        _currentHealth = maxHealth;

        for (int i = 0; i < attackString.Length; i++)
            _attacks.Add(attackString[i], attackValues[i]); //agrego los ataques al diccionario

        foreach (var item in _attacks)
            _totalAttackWeight += item.Value;//defino el peso total de los ataques

        walkSpeedDefault = walkSpeed;
        runSpeedDefault = runSpeed;
    }

    public Node FindClosestNode(Transform target, float viewRange)
    {
        var nodes = Physics.OverlapSphere(target.position, viewRange, nodesLayer);
        if(nodes.Length<=0)
        {
            for (int i = 0; i < 10; i++)
            {
                if (nodes.Length>0)
                    continue;
                nodes = Physics.OverlapSphere(target.position, viewRange * i, nodesLayer);
            }
        }
        Node Closest = null;
        foreach (var currrent in nodes)
        {
            var itemNode = currrent.GetComponent<Node>();
            if (Closest == null || Vector3.Distance(Closest.transform.position, target.position) > Vector3.Distance(currrent.transform.position, target.position))
                if (!itemNode.isBlocked)
                    Closest = currrent.GetComponent<Node>();
        }
        return Closest;
    }

    public Node FindFurthestNode(Transform target, float viewRange)
    {
        var nodes = Physics.OverlapSphere(target.position, viewRange, nodesLayer);
        if (nodes.Length <= 0)
        {
            for (int i = 0; i < 10; i++)
            {
                if (nodes.Length > 0)
                    continue;
                nodes = Physics.OverlapSphere(target.position, viewRange * i, nodesLayer);
            }
        }
        Node Furthest = null;
        foreach (var current in nodes)
        {
            var itemNode = current.GetComponent<Node>();
            if (Furthest == null || Vector3.Distance(Furthest.transform.position, target.position) < Vector3.Distance(current.transform.position, target.position))
                if (!itemNode.isBlocked)
                    Furthest = current.GetComponent<Node>();
        }
        return Furthest;
    }

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

    public virtual void AttackRouletteWheel()
    {

    }

    public virtual void TakeDMG(int DMG, float stun)
    {
        _currentHealth -= DMG;
        if(_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }
        hitParticle.Play();
    }
    public virtual void Die()
    {
        dead = true;
    }

    public virtual void OnDrawGizmos()
    {
        if(gizmos)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, obsAvoidanceRadious);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, AttackDistance);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(eyeSightPosition.position, visionRange);

            Gizmos.DrawRay(eyeSightPosition.position, transform.forward * visionRange);
            var temp = Physics.OverlapSphere(eyeSightPosition.position, visionRange, enemyLayer);
            foreach (var item in temp)
                Gizmos.DrawRay(eyeSightPosition.position, (item.transform.position - eyeSightPosition.position));

        }
        if (objective)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, objective.transform.position);
        }
        if(soldierTarget)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, soldierTarget.transform.position);
        }
        if(ShowNodes)
        {
            Gizmos.color = Color.green;
            if(path != null)
                for (int i = 0; i < path.Length; i++)
                {
                    if(path[i].previous)
                        Gizmos.DrawLine(path[i].transform.position, path[i].previous.transform.position);

                }
        }
    }
}
