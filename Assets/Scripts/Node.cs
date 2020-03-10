using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Transform node;
    public List<Node> Connections;
    public Node previous;
    public int gScore;

    public Node()
    {
        Connections = new List<Node>();
        previous = null;
        gScore = int.MaxValue;
    }
}
