using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGraph : MonoBehaviour
{
    public List<Node> nodes;
    public List<Transform> nodeObjects;
    // Update is called once per frame
    void Awake()
    {
        nodes = new List<Node>();
        for (int i = 0; i < nodeObjects.Count; i++)
        {
            Node temp = new Node
            {
                node = nodeObjects[i]
            };
            nodes.Add(temp);

        }
        connectingNodes();
    }

    void connectingNodes()
    {
        float width = Mathf.Sqrt(nodeObjects.Count);
        for (int i = 0; i < nodeObjects.Count; i++)
        {
            if (!(i < (int)width) && (nodes[i].node.gameObject.tag == "Grid" || nodes[i].node.gameObject.tag == "Exit"))//north
            {
                nodes[i].Connections.Add(nodes[i - (int)width]);
            }
            if (i + (int)width < nodeObjects.Count && (nodes[i].node.gameObject.tag == "Grid" || nodes[i].node.gameObject.tag == "Exit"))//south
            {
                nodes[i].Connections.Add(nodes[i + (int)width]);
            }
            if (i + 1 < nodeObjects.Count && (nodes[i].node.gameObject.tag == "Grid" || nodes[i].node.gameObject.tag == "Exit"))//east
            {
                nodes[i].Connections.Add(nodes[i + 1]);
            }
            if (!(i % (int)width == 0) && (nodes[i].node.gameObject.tag == "Grid" || nodes[i].node.gameObject.tag == "Exit"))//west
            {
                nodes[i].Connections.Add(nodes[i - 1]);
            }
            
        }

    }
}
