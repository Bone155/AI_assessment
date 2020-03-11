using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Djikstra : MonoBehaviour
{
    public NodeGraph graph;
    List<Node> open = new List<Node>();
    List<Node> close = new List<Node>();
    Node cur;
    Node node_target;
    Node startNode;

    public bool atTarget(Transform pos, Transform target)
    {
        if (pos.position.x <= getNode(graph.nodes, target).node.position.x + 0.5 && pos.position.z <= getNode(graph.nodes, target).node.position.z + 0.5 && pos.position.x >= getNode(graph.nodes, target).node.position.x - 0.5 && pos.position.z >= getNode(graph.nodes, target).node.position.z - 0.5)
            return true;
        else
            return false;
    }

    public Node getNode(List<Node> nodes, Transform pos)
    {
        Node node = new Node();
        for (int i = 0; i < nodes.Count; i++)
        {
            if (pos.position.x <= nodes[i].node.position.x + 0.5 && pos.position.z <= nodes[i].node.position.z + 0.5 && pos.position.x >= nodes[i].node.position.x - 0.5 && pos.position.z >= nodes[i].node.position.z - 0.5)
            {
                node = nodes[i];
            }
        }
        return node;
    }

    public Node getNode(List<Node> nodes, Vector3 pos)
    {
        Node node = new Node();
        for (int i = 0; i < nodes.Count; i++)
        {
            if (pos.x <= nodes[i].node.position.x + 0.5 && pos.z <= nodes[i].node.position.z + 0.5 && pos.x >= nodes[i].node.position.x - 0.5 && pos.z >= nodes[i].node.position.z - 0.5)
            {
                node = nodes[i];
            }
        }
        return node;
    }

    public List<Transform> improvisedPath(Transform start, Transform target, Vector3 guard)
    {
        Node guardNode = getNode(graph.nodes, guard);
        startNode = getNode(graph.nodes, start);
        startNode.gScore = 0;
        node_target = getNode(graph.nodes, target);
        open.Add(startNode);
        while (open.Count > 0)
        {
            cur = open[0];
            open.RemoveAt(0);
            close.Add(cur);

            // TODO: iterate through connections by using cur.Connections

            for (int i = 0; i < cur.Connections.Count; i++)
            {
                if (cur == node_target)
                {
                    if (cur.gScore < node_target.gScore)
                    {
                        node_target.gScore = cur.gScore;
                        node_target.previous = cur;
                    }
                }
                if(cur == guardNode)
                {
                    cur.Connections[i].gScore += 10000;
                    cur.Connections[i].previous = cur;
                    open.Add(cur.Connections[i]);
                }
                else if (cur.gScore + 1 < cur.Connections[i].gScore)
                {
                    cur.Connections[i].gScore = cur.gScore + 1;
                    cur.Connections[i].previous = cur;
                    open.Add(cur.Connections[i]);
                }
            }

            gScoreSort(open);
        }
        List<Transform> targetPath = new List<Transform>();
        while (cur != null && cur != startNode)
        {
            Node node = cur.previous;
            targetPath.Add(cur.node);
            cur = node;
        }
        targetPath.Reverse();
        close.Clear();
        return targetPath;
    }

    public List<Transform> calculatePath(Transform start, Transform target)
    {
        startNode = getNode(graph.nodes, start);
        startNode.gScore = 0;
        node_target = getNode(graph.nodes, target);
        open.Add(startNode);
        while (open.Count > 0)
        {
            cur = open[0];
            open.RemoveAt(0);
            close.Add(cur);

            // TODO: iterate through connections by using cur.Connections

            for (int i = 0; i < cur.Connections.Count; i++)
            {
                if (cur == node_target)
                {
                    if (cur.gScore < node_target.gScore)
                    {
                        node_target.gScore = cur.gScore;
                        node_target.previous = cur;
                    }
                }
                if (cur.gScore + 1 < cur.Connections[i].gScore)
                {
                    cur.Connections[i].gScore = cur.gScore + 1;
                    cur.Connections[i].previous = cur;
                    open.Add(cur.Connections[i]);
                }
            }

            gScoreSort(open);
        }

        List<Transform> targetPath = new List<Transform>();
        while (cur != null && cur != startNode)
        {
            Node node = cur.previous;
            targetPath.Add(cur.node);
            cur = node;
        }
        targetPath.Reverse();
        close.Clear();
        return targetPath;
    }

    void gScoreSort(List<Node> list)
    {
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i].gScore < list[i - 1].gScore)
            {
                Node temp = list[i];
                list[i] = list[i - 1];
                list[i - 1] = temp;
            }
        }
    }

}

