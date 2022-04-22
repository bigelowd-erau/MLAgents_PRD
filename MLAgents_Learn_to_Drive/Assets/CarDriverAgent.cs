using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CarDriverAgent : Agent
{
    [SerializeField] private TrackCheckpoints trackCheckpoints;
    [SerializeField] private Transform spawnPosition;

    private CarDriver carDriver;

    private void Awake()
    {
        carDriver = GetComponent<CarDriver>();
        //spawnPosition = transform;
    }

    private void Start()
    {
        trackCheckpoints.OnCarWrongCheckpoint += TrackCheckpoints_OnCarWrongCheckpoint;
        trackCheckpoints.OnCarCorrectCheckpoint += TrackCheckpoints_OnCarCorrectCheckpoint;
    }

    private void TrackCheckpoints_OnCarWrongCheckpoint(object sender, TrackCheckpoints.CarCheckPointEventArgs e)
    {
        if (e.carTransform == transform)
            AddReward(-1f);
    }

    private void TrackCheckpoints_OnCarCorrectCheckpoint(object sender, TrackCheckpoints.CarCheckPointEventArgs e)
    {
        if (e.carTransform == transform)
            AddReward(1f);
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("New Episode for " + transform.name);
        transform.position = spawnPosition.position + new Vector3(Random.Range(-7.5f, 7.5f), spawnPosition.position.y, Random.Range(-7.5f, 0f));
        transform.forward = spawnPosition.forward;
        trackCheckpoints.ResetCheckpoint(transform);
        carDriver.StopCompletley();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        float directionDot = Vector3.Dot(transform.forward, trackCheckpoints.GetNextCheckpoint(transform).transform.forward);
        sensor.AddObservation(directionDot);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardAmount = 0f;
        float turnAmount = 0f;
        switch (actions.DiscreteActions[0])
        {
            case 0: forwardAmount = 0f; break;
            case 1: forwardAmount = 1f; break;
            case 2: forwardAmount = -1f; break;
        }
        switch (actions.DiscreteActions[1])
        {
            case 0: turnAmount = 0f; break;
            case 1: turnAmount = 1f; break;
            case 2: turnAmount = -1f; break;
        }
        carDriver.SetInputs(forwardAmount, turnAmount);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        int forwardAction = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
        if (forwardAction == -1) //-1
            forwardAction = 2;
        int turnAction = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
        if (turnAction == -1) //-1
            turnAction = 2;
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = forwardAction;
        discreteActions[1] = turnAction;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            //hit wall
            AddReward(-0.5f);
            EndEpisode();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            //hit wall
            AddReward(-0.1f);
        }
    }
}
