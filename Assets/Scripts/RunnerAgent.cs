using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class RunnerAgent : Agent
{
    public Transform goal;
    public SlidingObstacle[] obstacles;
    public float moveSpeed = 3f;
    public float turnSpeed = 180f;

    Rigidbody rb;
    float prevDist;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.localPosition = new Vector3(Random.Range(-4.5f, 4.5f), 0.5f, -4.5f);
        transform.localRotation = Quaternion.Euler(0f, Random.Range(-60f, 60f), 0f);
        goal.localPosition = new Vector3(Random.Range(-4.5f, 4.5f), 0.5f, Random.Range(3.5f, 5f));
        foreach (var o in obstacles) o.ResetObstacle();
        prevDist = Vector3.Distance(transform.localPosition, goal.localPosition);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 localVel = transform.InverseTransformDirection(rb.velocity) / moveSpeed;
        Vector3 toGoal = transform.InverseTransformDirection(goal.position - transform.position);
        sensor.AddObservation(localVel.x);
        sensor.AddObservation(localVel.z);
        sensor.AddObservation(toGoal.normalized.x);
        sensor.AddObservation(toGoal.normalized.z);
        sensor.AddObservation(toGoal.magnitude / 17f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var a = actions.ContinuousActions;
        float forward = Mathf.Clamp(a[0], -1f, 1f);
        float strafe  = Mathf.Clamp(a[1], -1f, 1f);
        float turn    = Mathf.Clamp(a[2], -1f, 1f);

        Vector3 move = (transform.forward * forward + transform.right * strafe) * moveSpeed;
        rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
        transform.Rotate(0f, turn * turnSpeed * Time.fixedDeltaTime, 0f);

        float dist = Vector3.Distance(transform.localPosition, goal.localPosition);
        AddReward(0.1f * (prevDist - dist));
        prevDist = dist;
        AddReward(-1f / MaxStep);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var a = actionsOut.ContinuousActions;
        a[0] = Input.GetAxis("Vertical");
        a[1] = Input.GetAxis("Horizontal");
        a[2] = (Input.GetKey(KeyCode.E) ? 1f : 0f) - (Input.GetKey(KeyCode.Q) ? 1f : 0f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal")) { AddReward(1f); EndEpisode(); }
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.collider.CompareTag("Obstacle")) { AddReward(-0.5f); EndEpisode(); }
        else if (c.collider.CompareTag("Wall")) { AddReward(-0.1f); }
    }
}
