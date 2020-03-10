using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Decision
{
    Decision makeDecision();
}

public class AIGuard : MonoBehaviour
{
    Agent agent;
    public Decision root;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<Agent>();
        root = new Discovered(agent, new thiefCaught(agent, new Patrol(agent), new Chase(agent)),
                new Waypoint(agent, new newWayPoint(agent), new seekWayPoint(agent)));
    }

    // Update is called once per frame
    void Update()
    {
        Decision currentDecision = root;
        while (currentDecision != null)
        {
            currentDecision = currentDecision.makeDecision();
        }

    }

}

public class Discovered : Decision // question node // discovered thief
{
    Agent agent;
    Decision discovered;
    Decision notDiscovered;

    public Discovered() { }

    public Discovered(Agent agent, Decision discoveredDecision, Decision notdiscoveredDecision)
    {
        this.agent = agent;
        discovered = discoveredDecision;
        notDiscovered = notdiscoveredDecision;
    }

    public Decision makeDecision()
    {
        if (thiefInView(agent))
        {
            return discovered;
        }
        else
        {
            return notDiscovered;
        }
    }

    bool thiefInView(Agent agent)
    {
        Ray ray = new Ray(agent.transform.position, agent.transform.forward);
        if (agent.target.position.x > agent.transform.forward.x - 2 && agent.target.position.x < agent.transform.forward.x + 2 && Physics.Raycast(ray, out RaycastHit hit, 4))
            return true;
        else
            return false;
    }
}

public class thiefCaught : Decision // question node // yes // found him?
{
    Agent agent;
    Decision patrol;
    Decision chase;
    float sec = 0;
    public thiefCaught() { }

    public thiefCaught(Agent agent, Decision patrolDecision, Decision chaseDecision)
    {
        this.agent = agent;
        patrol = patrolDecision;
        chase = chaseDecision;
    }

    public Decision makeDecision()
    {
        if (!thiefInView(agent))
            sec += Time.deltaTime;
        else
            sec = 0;
        if (sec > 5)
        {
            return patrol;
        }
        else
        {
            return chase;
        }

    }

    bool thiefInView(Agent agent)
    {
        Ray ray = Camera.main.ScreenPointToRay(agent.target.position);
        if (agent.target.position.x > agent.transform.forward.x - 2 && agent.target.position.x < agent.transform.forward.x + 2 && !Physics.Raycast(ray, out RaycastHit hit, 4))
            return true;
        else
            return false;
    }
}

public class Patrol : Decision // answer node // yes // patrol
{
    Agent agent;

    public Patrol() { }

    public Patrol(Agent agent)
    {
        this.agent = agent;
    }

    public Decision makeDecision()
    {
        agent.transform.position += agent.path[agent.idx].position * Time.deltaTime;
        return null;
    }
}

public class Chase : Decision // answer node // no
{
    Agent agent;

    public Chase() { }

    public Chase(Agent agent)
    {
        this.agent = agent;
    }

    public Decision makeDecision()
    {
        agent.transform.position += agent.path[agent.idx].position * Time.deltaTime;
        agent.idx++;
        if (agent.idx >= agent.path.Count)
            agent.idx = 0;
        return null;
    }
}

public class Waypoint : Decision //question node // no // reached waypoint?
{
    Agent agent;
    Decision closeEnough;
    Decision tooFar;

    public Waypoint() { }

    public Waypoint(Agent agent, Decision closeEnoughDecision, Decision tooFarDecision)
    {
        this.agent = agent;
        closeEnough = closeEnoughDecision;
        tooFar = tooFarDecision;
    }

    public Decision makeDecision()
    {
        if (agent.dj.atTarget(agent.transform, agent.target) == true)
        {
            return closeEnough;
        }
        else
        {
            return tooFar;
        }

    }

}

public class seekWayPoint : Decision //answer node // No // move towards waypoint
{
    Agent agent;
    public seekWayPoint() { }

    public seekWayPoint(Agent agent)
    {
        this.agent = agent;
    }

    public Decision makeDecision()
    {
        agent.transform.position += agent.path[agent.idx].position * Time.deltaTime;
        return null;
    }
}

public class newWayPoint : Decision //answer node // yes // get new waypoint
{
    Agent agent;

    public newWayPoint() { }

    public newWayPoint(Agent agent)
    {
        this.agent = agent;
    }

    public Decision makeDecision()
    {
        agent.path = agent.dj.calculatePath(agent.transform, agent.target);
        agent.idx++;
        return null;
    }
}
