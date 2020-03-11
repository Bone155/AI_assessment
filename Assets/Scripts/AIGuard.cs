﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Decision
{
    Decision makeDecision();
}

public class AIGuard : MonoBehaviour
{
    public Agent agent;
    public Decision root;
    // Start is called before the first frame update
    void Start()
    {
        root = new Discovered(agent, 
                new thiefCaught(agent, 
                    new newWayPoint(agent), 
                    new Seek(agent)),
                new Waypoint(agent, 
                    new newWayPoint(agent), 
                    new HavePath(agent, 
                        new seekWayPoint(agent),
                        new newWayPoint(agent))));
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
        if ((agent.target.position.x > agent.transform.forward.x - 2 || agent.target.position.x < agent.transform.forward.x + 2) && Physics.Raycast(ray, out RaycastHit hit, 3))
            return true;
        else
            return false;
    }
}

public class thiefCaught : Decision // question node // caught him?
{
    Agent agent;
    Decision patrol;
    Decision seek;
    float sec = 0;
    public thiefCaught() { }

    public thiefCaught(Agent agent, Decision patrolDecision, Decision seekDecision)
    {
        this.agent = agent;
        patrol = patrolDecision;
        seek = seekDecision;
    }

    public Decision makeDecision()
    {
        if (!thiefInView(agent))
            sec += Time.deltaTime;
        else
            sec = 0;
        if (sec < 5)
        {
            return seek;
        }
        else
        {
            return patrol;
        }

    }

    bool thiefInView(Agent agent)
    {
        Ray ray = Camera.main.ScreenPointToRay(agent.target.position);
        if ((agent.target.position.x > agent.transform.forward.x - 2 || agent.target.position.x < agent.transform.forward.x + 2) && Physics.Raycast(ray, out RaycastHit hit, 3))
            return true;
        else
            return false;
    }
}

public class Seek : Decision // answer node // seeking theif
{
    Agent agent;

    public Seek() { }

    public Seek(Agent agent)
    {
        this.agent = agent;
    }

    public Decision makeDecision()
    {
        agent.transform.position += agent.target.position * agent.speed * Time.deltaTime;
        return null;
    }
}

public class Waypoint : Decision //question node // reached waypoint?
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
        if (agent.dj.atTarget(agent.transform, agent.target))
        {
            return closeEnough;
        }
        else
        {
            return tooFar;
        }

    }

}

public class seekWayPoint : Decision //answer node // move towards waypoint
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

public class newWayPoint : Decision //answer node // get new waypoint
{
    Agent agent;

    public newWayPoint() { }

    public newWayPoint(Agent agent)
    {
        this.agent = agent;
    }

    public Decision makeDecision()
    {
        agent.idx++;
        if (agent.idx >= agent.path.Count)
            agent.idx = 0;
        agent.path = agent.dj.calculatePath(agent.transform, agent.pathObjects[agent.idx]);
        return null;
    }
}

public class HavePath : Decision // question node // does the agent have a path
{
    Agent agent;
    Decision yes;
    Decision no;

    public HavePath() { }

    public HavePath(Agent agent, Decision yesDecision, Decision noDecision)
    {
        this.agent = agent;
        yes = yesDecision;
        no = noDecision;
    }

    public Decision makeDecision()
    {
        if (agent.path.Count <= 0)
        {
            return no;
        }
        else
        {
            return yes;
        }

    }
}