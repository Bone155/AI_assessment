using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGraph : MonoBehaviour
{
    public List<Node> nodes;
    public Transform nodeObject;
    public List<Transform> nodeObjects;
    public int row;
    public int col;
    Vector3 pos;
    // Update is called once per frame
    void Start()
    {
        nodes = new List<Node>();
        pos = new Vector3(0, 1, 0);
        for (int i = 0; i < row * col; i++)
        {
            Node temp = new Node();
            temp.node = nodeObjects[i];
            nodes.Add(temp);

        }
        connectingNodes();
    }

    void connectingNodes()
    {
        int width = col;
        for (int i = 0; i < row * col; i++)
        {
            if (!(i < width) && nodes[i].node.gameObject.tag == "Grid")//north
            {
                nodes[i].Connections.Add(nodes[i - width]);
            }
            if (i + width < col * row && nodes[i].node.gameObject.tag == "Grid")//south
            {
                nodes[i].Connections.Add(nodes[i + width]);
            }
            if (i + 1 < col * row && nodes[i].node.gameObject.tag == "Grid")//east
            {
                nodes[i].Connections.Add(nodes[i + 1]);
            }
            if (!(i % width == 0) && nodes[i].node.gameObject.tag == "Grid")//west
            {
                nodes[i].Connections.Add(nodes[i - 1]);
            }
            
        }

    }
}
