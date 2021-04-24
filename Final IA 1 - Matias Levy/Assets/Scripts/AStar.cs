using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    static AStar _instance;
    public static AStar Instance
    {
        get
        {
            return _instance;
        }
    }

    List<Node> _openNodes = new List<Node>();

    List<Node> _closedNodes = new List<Node>();

    List<Node> _pathNode = new List<Node>();

    public List<Node> pathNodes
    {
        get
        {
            return _pathNode;
        }
    }

    Node _start;
    public Node start
    {
        get
        {
            return _start;
        }
        set
        {
            _start = value;
        }
    }
    Node _end;
    public Node end
    {
        get
        {
            return _end;
        }
        set
        {
            _end = value;
        }
    }

    private void Awake()
    {
        _instance = this;
    }
    private void Start()
    {
        GetPath();
    }
    public void GetPath()
    {
        foreach (var item in _openNodes)
            item.Reset();
        foreach (var item in _closedNodes)
            item.Reset();
        foreach (var item in _pathNode)
            item.Reset();
        _openNodes = new List<Node>();
        _closedNodes = new List<Node>();
        _pathNode = new List<Node>();

        while (_openNodes.Count > 0)
        {
            //Elegimos el nodo a evaluar y lo guardamos en una variable
            Node current = SearchNextNode();
            //Añadimos el nodo a la lista de cerrados porque ya no necesitara volver a ser evaluado
            _closedNodes.Add(current);

            //Por cada uno de los vecinos del nodo que estamos evaluando
            foreach (var item in current.neighbors)
            {
                //Si el nodo esta en la lista de cerrados, es decir que ya fue evaluado, continuamos
                //con el proximo elemento de la lista, no volvemos a evaluar nodos ya cerrados
                if (_closedNodes.Contains(item))
                    continue;

                //En caso de que el vecino no este contenido en la lista de nodos abiertos
                if (!_openNodes.Contains(item))
                {
                    //Lo agregamos a la lista
                    _openNodes.Add(item);
                    //Le damos valor a su h teniendo en cuenta uno de los caminos mas largos que podria tomar
                    //que es la suma de las distancias a recorrer en cada uno de los ejes
                    item.h = Mathf.Abs(end.transform.position.x - item.transform.position.x)
                               + Mathf.Abs(end.transform.position.y - item.transform.position.y)
                               + Mathf.Abs(end.transform.position.z - item.transform.position.z);
                }
                //Calculamos la distancia entre el nodo actual y su vecino
                float distanceToNeighbor = Vector3.Distance(current.transform.position, item.transform.position);
                //Calculamos el G que tendria este vecino si se llegara a el a traves de nuestro current
                float newG = current.g + distanceToNeighbor;
                //Si el nuevo G es menor al que ya tenia nuestro vecino
                if(newG < item.g)
                {
                    //Actualizamos sus valores de G y de F
                    item.g = newG;
                    item.f = item.g + item.h;
                    //Le setteamos como su nuevo padre a nuestro nodo actual para indicar que el camino
                    //por el que se llego a este nodo es a traves de nuestro current y no de otro nodo
                    item.previous = current;
                }
            }

            //Si nuestro nodo final esta contenido en la lista de abiertos significa que ya encontramos
            //un camino hacia el y por lo tanto podemos reconstruir ese camino
            if (_openNodes.Contains(end))
            {
                //Agregamos el nodo final a nuestra lista que contiene el camino
                _pathNode.Add(end);
                //Nos guardamos el anterior de ese nodo en una variable
                Node node = end.previous;
                //Mientras el valor de esta variable no sea nulo
                while (node)
                {
                    //Insertamos ese nodo al principio de nuestra lista asi despues no necesitamos invertirla
                    _pathNode.Insert(0, node);
                    //Cambiamos el valor de la variable por su anterior para que continue con el proceso
                    node = node.previous;
                }
            }
        }
    }

    public Node SearchNextNode()
    {
        //Nos guardamos el primer indice de la lista para tener un valor inicial que no sea nulo
        Node n = _openNodes[0];
        //Recorremos el resto de la lista
        for (int i = 1; i < _openNodes.Count; i++)
        {
            //Buscamos entre todos los elementos cual es el que tiene el F menor y nos lo guardamos en
            //la variable n
            if (_openNodes[i].f < n.f)
            {
                n = _openNodes[i];
            }
        }
        //Como ya va a pasar a ser evaluado lo removemos de la lista de abiertos
        _openNodes.Remove(n);
        //Retornamos el nodo que ya sabemos tiene el menor valor de F, es decir que estima el camino mas corto
        return n;
    }

}
