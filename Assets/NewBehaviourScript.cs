using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class NewBehaviourScript : Agent {

    public Transform TargetTransform;
    public bool isJumping = false;
    Vector3 spawn = new Vector3(-17f, -8, 19);
    public GameObject Sphere;
    public GameObject Goal2;

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(TargetTransform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions) {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];
        float moveZ = actions.ContinuousActions[2];
       

        var rb = GetComponent<Rigidbody>();
        if (moveY > 0.75 && !isJumping)
        {
            rb.AddForce(new Vector3(0, 4, 0), ForceMode.Impulse);
            isJumping = true;
        }

        float moveSpeed = 5f;
        rb.MovePosition(transform.position + new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed);
        //transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    private void OnCollisionStay(Collision collision)
    {
        isJumping = false;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        
        if (other.tag == "GoalTag")
        {
            SetReward(1);
            other.tag = "Untagged";
            spawn = new Vector3(30, -8, 1.5f);
            Goal2.tag = "Goal2Tag";
            EndEpisode();
            
            
        }
        else if (other.tag == "OutOfBoundsTag")
        {
            var distance = Vector3.Distance(transform.position, TargetTransform.position);
            AddReward(-distance / 5f);
            EndEpisode();
        }
        else if (other.tag == "CheckpointTag")
        {
            var distance = Vector3.Distance(transform.position, TargetTransform.position);
            SetReward(1 / distance);
        }
        else if (other.tag == "StartTag")
        {
            AddReward(-0.1f);
        }
        else if (other.tag == "Goal2Tag")
        {
            SetReward(3);
            spawn = new Vector3(50, -3, 1.5f);
            EndEpisode();
        }
    }

    public override void OnEpisodeBegin()
    {
        //TargetTransform.localPosition = new Vector3((Random.value - .5f) * 3 - 2.5f, 2.5f, (Random.value - .5f) * 8);
        //transform.localPosition = new Vector3((Random.value - .5f) * 8, 0.5f, (Random.value - .5f) * 8);
        transform.localPosition = spawn;
        transform.rotation = new Quaternion();
    }
}
