using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDriver : MonoBehaviour
{
    private Rigidbody carRigidbody;
    [SerializeField] private float maxSpeed = 250.0f; //units per second
    [SerializeField] private float maxTurnSpeed = 270f; //degrees per second
    private float speed = 0.0f;
    private float angularSpeed = 0.0f;
    private float forwardAmount;
    private float turnAmount;

    private void Awake()
    {
        Physics.IgnoreLayerCollision(6, 6);
        TryGetComponent<Rigidbody>(out carRigidbody);
        carRigidbody.maxAngularVelocity = maxTurnSpeed;
    }

    private void FixedUpdate()
    {
        //Dont keep this
        //SetInputs(Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal"));
        //end of no keep
        speed += forwardAmount * Time.fixedDeltaTime;
        speed = Mathf.Clamp(speed, 0.0f, maxSpeed);
        carRigidbody.velocity = transform.forward * speed;
        carRigidbody.velocity = new Vector3(carRigidbody.velocity.x, 0f, carRigidbody.velocity.z);
        transform.Rotate(new Vector3(0.0f, turnAmount, 0.0f) * speed * maxTurnSpeed * Time.fixedDeltaTime, Space.World);
    }
    public void StopCompletley()
    {
        carRigidbody.velocity = Vector3.zero;
    }

    public void SetInputs(float forwardAmount, float turnAmount)
    {
        this.forwardAmount = forwardAmount * 50f;
        this.turnAmount = turnAmount;
    }
}
