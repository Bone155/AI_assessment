using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thief : MonoBehaviour
{
    Agent agent;
    public List<Transform> guards;
    List<Transform> nearbyGuard;
    Decision root;
    public int score;
    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        agent = GetComponent<Agent>();
        root = new Detected(agent,
                new Caught(agent,
                    new Evade(agent),
                    new Busted(agent)),
                new ThiefMovement(agent,
                    new ThiefNewDestination(agent),
                    new ThiefMove(agent)));
    }

    // Update is called once per frame
    void Update()
    {
        getNearbyGuard(guards);
        if (Input.GetMouseButtonDown(0))
        {
            agent.target.position = Input.mousePosition;
        }
        Decision currentDecision = root;
        while (currentDecision != null)
        {
            currentDecision = currentDecision.makeDecision();
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Diamond")
        {
            Destroy(other);
            score++;
        }
    }

    void getNearbyGuard(List<Transform> security)
    {
        for (int i = 0; i < security.Count; i++)
        {
            if ((security[i].position.x < transform.position.x + 5 || security[i].position.x > transform.position.x - 5) && (security[i].position.z < transform.position.z + 5 || security[i].position.z > transform.position.z - 5))
                nearbyGuard.Add(security[i]);
            if ((security[i].position.x > transform.position.x + 5 || security[i].position.x < transform.position.x - 5) && (security[i].position.z > transform.position.z + 5 || security[i].position.z < transform.position.z - 5))
                nearbyGuard.Remove(security[i]);
        }

    }
}
public class Detected : Decision // question node // have been detected?
{
    Agent agent;
    Decision detected;
    Decision notDetected;

    public Detected() { }

    public Detected(Agent agent, Decision detectedDecision, Decision notDetectedDecision)
    {
        this.agent = agent;
        detected = detectedDecision;
        notDetected = notDetectedDecision;
    }

    public Decision makeDecision()
    {
        if (inView(agent))
        {
            return detected;
        }
        else
        {
            return notDetected;
        }
    }

    bool inView(Agent agent)
    {
        Ray ray = new Ray(agent.transform.position, agent.transform.forward);
        if (agent.transform.position.x > agent.target.transform.forward.x - 2 && agent.transform.position.x < agent.target.transform.forward.x + 2 && Physics.Raycast(ray, out RaycastHit hit, 4))
            return true;
        else
            return false;
    }
}

public class Caught : Decision // question node // yes
{
    Agent agent;
    Decision busted;
    Decision avoided;

    public Caught() { }

    public Caught(Agent agent, Decision bustedDecision, Decision avoidedDecision)
    {
        this.agent = agent;
        busted = bustedDecision;
        avoided = avoidedDecision;
    }

    public Decision makeDecision()
    {
        if (agent.dj.atTarget(agent.target, agent.transform) == true)
        {
            return busted;
        }
        else
        {
            return avoided;
        }

    }
}

public class Evade : Decision // answer node // no // avoided capture
{
    Agent agent;

    public Evade() { }

    public Evade(Agent agent)
    {
        this.agent = agent;
    }

    public Decision makeDecision()
    {
        agent.transform.position += agent.path[agent.idx].position;
        agent.idx++;
        return null;
    }

}

public class Busted : Decision // answer node // yes
{
    Agent agent;

    public Busted() { }

    public Busted(Agent agent)
    {
        this.agent = agent;
    }

    public Decision makeDecision()
    {
        //Game over
        return null;
    }
}

public class ThiefMovement : Decision // question node // no // reached destination?
{
    Agent agent;
    Decision closeEnough;
    Decision tooFar;

    public ThiefMovement() { }

    public ThiefMovement(Agent agent, Decision closeEnoughDecision, Decision tooFarDecision)
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

public class ThiefNewDestination : Decision // answer node // yes // set new destination (left mouse button click position)
{
    Agent agent;

    public ThiefNewDestination() { }

    public ThiefNewDestination(Agent agent)
    {
        this.agent = agent;
    }

    public Decision makeDecision()
    {
        if (Input.GetMouseButtonDown(0))
        {
            agent.target.position = Input.mousePosition;
        }
        agent.path = agent.dj.calculatePath(agent.transform, agent.target);
        return null;
    }
}

public class ThiefMove : Decision // answer node // no // move towards destination (a.k.a the mouse position)
{
    Agent agent;
    public ThiefMove() { }

    public ThiefMove(Agent agent)
    {
        this.agent = agent;
    }

    public Decision makeDecision()
    {
        agent.transform.position += agent.path[agent.idx].position * Time.deltaTime;
        agent.idx++;
        return null;
    }
}

