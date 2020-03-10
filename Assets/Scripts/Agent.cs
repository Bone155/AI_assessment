using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public Djikstra dj;
    public Vector3 velocity;
    public float mass;
    public float maxSpeed;
    float maxForce = 0.5f;
    Vector3 steeringForces;
    public Transform target;
    public List<Transform> path;
    public int idx = 0;
    // Update is called once per frame
    void Update()
    {
        Vector3 steering = Vector3.ClampMagnitude(steeringForces, maxForce);
        steeringForces = Vector3.zero;
        steering /= mass;
        if (velocity != Vector3.zero)
        {
            transform.forward = velocity.normalized;
        }
        steeringForces += steering;

    }
}
