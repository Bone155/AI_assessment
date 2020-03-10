using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Djikstra : MonoBehaviour
{
    public NodeGraph graph;
    List<Node> open;
    List<Node> close;
    Node cur;
    Node tile_target;
    Node startTile;

    // Start is called before the first frame update
    void Start()
    {
        open = new List<Node>();
        close = new List<Node>();
    }

    public bool atTarget(Transform pos, Transform target)
    {
        if (pos.position.x <= getTargetTile(graph.nodes, target).node.position.x + 0.5 && pos.position.z <= getTargetTile(graph.nodes, target).node.position.z + 0.5 && pos.position.x >= getTargetTile(graph.nodes, target).node.position.x - 0.5 && pos.position.z >= getTargetTile(graph.nodes, target).node.position.z - 0.5)
            return true;
        else
            return false;
    }

    public Node getStartTile(List<Node> tiles, Transform startPos)
    {
        Node startTile = new Node();
        for (int i = 0; i < tiles.Count; i++)
        {
            if (startPos.position.x <= tiles[i].node.position.x + 0.5 && startPos.position.z <= tiles[i].node.position.z + 0.5 && startPos.position.x >= tiles[i].node.position.x - 0.5 && startPos.position.z >= tiles[i].node.position.z - 0.5)
            {
                startTile = tiles[i];
            }
        }
        return startTile;
    }

    public Node getTargetTile(List<Node> tiles, Transform targetPos)
    {
        Node targetTile = new Node();
        for (int i = 0; i < tiles.Count; i++)
        {
            if (targetPos.position.x <= tiles[i].node.position.x + 0.5 && targetPos.position.z <= tiles[i].node.position.z + 0.5 && targetPos.position.x >= tiles[i].node.position.x - 0.5 && targetPos.position.z >= tiles[i].node.position.z - 0.5)
            {
                targetTile = tiles[i];
            }
        }
        return targetTile;
    }

    public List<Transform> calculatePath(Transform start, Transform target)
    {
        startTile = getStartTile(graph.nodes, start);
        startTile.gScore = 0;
        tile_target = getTargetTile(graph.nodes, target);
        open.Add(startTile);
        while (open.Count > 0)
        {
            cur = open[0];
            open.RemoveAt(0);
            close.Add(cur);

            // TODO: iterate through connections by using cur.Connections

            for (int i = 0; i < cur.Connections.Count; i++)
            {
                if (cur == tile_target)
                {
                    if (cur.gScore < tile_target.gScore)
                    {
                        tile_target.gScore = cur.gScore;
                        tile_target.previous = cur;
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
        while (cur != null && cur != startTile)
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
        for (int j = 0; j < list.Count; j++)
        {
            if (j - 1 >= 0)
            {
                if (list[j].gScore > list[j - 1].gScore)
                {
                    Node temp = list[j];
                    list[j] = list[j - 1];
                    list[j - 1] = temp;
                }
            }
        }
    }

    void displayPath(List<Node> path)
    {
        float height = 0.0f;
        for (int i = 0; i < path.Count; i++)
        {
            height = path[i].node.position.y;
            height += 1;
            Debug.DrawLine(new Vector3(path[i].node.position.x, height, path[i].node.position.z), new Vector3(path[i].previous.node.position.x, height, path[i].previous.node.position.z), Color.red);
        }
    }

}

