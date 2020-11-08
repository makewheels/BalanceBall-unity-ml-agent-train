using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class PlatformAgent : Agent
{

    public Transform ball;
    private Rigidbody ballRigidbody;
    private float rotateSpeed=8f;

    // Start is called before the first frame update
    void Start()
    {
        ballRigidbody = ball.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnEpisodeBegin()
    {
        transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        transform.Rotate(new Vector3(1, 0, 0), Random.Range(-10f, 10f));
        transform.Rotate(new Vector3(0, 0, 1), Random.Range(-10f, 10f));
        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
        ball.transform.position = transform.position
            + new Vector3(Random.Range(-2.5f, 2.5f), 2.5f, Random.Range(-2.5f, 2.5f));

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //球的位置
        sensor.AddObservation(transform.localPosition - ball.localPosition);
        //板子的旋转角度
        sensor.AddObservation(transform.rotation.x);
        sensor.AddObservation(transform.rotation.z);
        //球的速度
        sensor.AddObservation(ballRigidbody.velocity);
        sensor.AddObservation(ballRigidbody.angularVelocity);

    }

    public override void OnActionReceived(float[] vectorAction)
    {
        transform.Rotate(Vector3.forward, vectorAction[0] * rotateSpeed);
        transform.Rotate(Vector3.right, vectorAction[1] * rotateSpeed);

        //如果掉下去了，惩罚，结束本次训练
        if (ball.localPosition.y < -1)
        {
            SetReward(-20f);
            EndEpisode();
        }
        else
        {
            SetReward(1f);
        }
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }
}
