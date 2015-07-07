using UnityEngine;
using System.Collections;
using Assets._Scripts;

public class WhiteBallScript : MonoBehaviour {

    public float ballSpeed;
    private Rigidbody body;
    private Vector3 initialPosition;

    void Start()
    {
        this.body = this.GetComponent<Rigidbody>();
        this.initialPosition = this.body.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ObjectTags.TriggerPlane))
        {
            this.body.MovePosition(this.initialPosition);
        }
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
             
        this.body.AddForce(new Vector3(moveHorizontal, 0, moveVertical) * this.ballSpeed);
    }
}
