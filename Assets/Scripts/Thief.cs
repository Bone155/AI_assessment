using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thief : MonoBehaviour
{
    public Agent agent;
    public List<Transform> guards;
    Decision root;
    public int score;
    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        root = new Detected(agent, guards,
                new Caught(agent, //yes
                    new imporvisedPath(agent, guards), //no
                    new Busted(agent)), //yes
                new guardNearby(agent, guards, //no
                    new imporvisedPath(agent, guards), //yes
                    new ThiefMovement(agent, //no
                        new ThiefExitPath(agent), //yes
                        new ThiefMove(agent)))); //no
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

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Diamond")
        {
            Destroy(other);
            score += 100;
        }
        if (other.gameObject.tag == "Exit")
        {
            agent.speed = 0;
        }
    }

}
public class Detected : Decision // question node // have been detected?
{
    Agent agent;
    List<Transform> guards;
    Decision detected;
    Decision notDetected;

    public Detected() { }

    public Detected(Agent agent, List<Transform> guards, Decision detectedDecision, Decision notDetectedDecision)
    {
        this.agent = agent;
        this.guards = guards;
        detected = detectedDecision;
        notDetected = notDetectedDecision;
    }

    public Decision makeDecision()
    {
        if (inView(agent, guards))
        {
            return detected;
        }
        else
        {
            return notDetected;
        }
    }

    bool inView(Agent agent, List<Transform> guards)
    {
        bool spotted = false;
        Ray ray;
        for(int i = 0; i < guards.Count; i++)
        {
            ray = new Ray(guards[i].position, guards[i].forward);
            if ((agent.transform.position.x > guards[i].forward.x - 2 || agent.transform.position.x < guards[i].forward.x + 2) && Physics.Raycast(ray, out RaycastHit hit, 3))
                spotted = true;
        }
        return spotted;
    }
}

public class Caught : Decision // question node // have been caught?
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

public class Busted : Decision // answer node // got busted!
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
        agent.speed = 0;
        return null;
    }
}

public class guardNearby : Decision // question node // guard nearby?
{
    Agent agent;
    List<Transform> guards;
    Decision guardnear;
    Decision path;

    public guardNearby() { }

    public guardNearby(Agent agent, List<Transform> guards, Decision guardnearDecision, Decision pathDecision)
    {
        this.agent = agent;
        this.guards = guards;
        guardnear = guardnearDecision;
        path = pathDecision;
    }

    public Decision makeDecision()
    {
        Vector3 guard = new Vector3();
        for (int i = 0; i < guards.Count; i++)
        {
            if (nearbyGuard(guards[i].position))
            {
                guard = guards[i].position;
            }
        }
        if (nearbyGuard(guard))
        {
            return guardnear;
        }
        else
        {
            return path;
        }
    }

    bool nearbyGuard(Vector3 guard)
    {
        if ((guard.x < agent.transform.position.x + 5 || guard.x > agent.transform.position.x - 5) && (guard.z < agent.transform.position.z + 5 || guard.z > agent.transform.position.z - 5))
            return true;
        else
            return false;
    }
}

public class imporvisedPath : Decision // answer node // calculate new path to target
{
    Agent agent;
    List<Transform> guards;
    public imporvisedPath() { }

    public imporvisedPath(Agent agent, List<Transform> guards)
    {
        this.agent = agent;
        this.guards = guards;
    }

    public Decision makeDecision()
    {
        Vector3 guard = nearestGuard(guards);
        agent.path = agent.dj.improvisedPath(agent.transform, agent.target, guard);
        return null;
    }

    Vector3 nearestGuard(List<Transform> guards)
    {
        Vector3 guard = new Vector3();
        for(int i = 0; i < guards.Count; i++)
        {
            if ((guards[i].position.x < agent.transform.position.x + 5 || guards[i].position.x > agent.transform.position.x - 5) && (guards[i].position.z < agent.transform.position.z + 5 || guards[i].position.z > agent.transform.position.z - 5))
            {
                guard = guards[i].position;
            }
        }
        return guard;
    }
}

public class ThiefMovement : Decision // question node // reached end of path?
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

public class ThiefExitPath : Decision // answer node // set exit path
{
    Agent agent;

    public ThiefExitPath() { }

    public ThiefExitPath(Agent agent)
    {
        this.agent = agent;
    }

    public Decision makeDecision()
    {
        GameObject exit = GameObject.FindGameObjectWithTag("Exit");
        agent.target = exit.transform;
        agent.path = agent.dj.calculatePath(agent.transform, agent.target);
        return null;
    }
}

public class ThiefMove : Decision // answer node // follow path
{
    Agent agent;
    public ThiefMove() { }

    public ThiefMove(Agent agent)
    {
        this.agent = agent;
    }

    public Decision makeDecision()
    {
        if (agent.idx >= agent.path.Count)
            agent.idx = 0;
        Vector3 force = desiredVelocity - agent.velocity;
        agent.velocity += force * Time.deltaTime;
        agent.transform.position += agent.velocity * Time.deltaTime;
        agent.transform.rotation = Quaternion.LookRotation(desiredVelocity);
        agent.idx++;
        return null;
    }

    Vector3 desiredVelocity
    {
        get
        {
            return (agent.path[agent.idx].position - agent.transform.position).normalized * agent.speed;
        }
    }
}
