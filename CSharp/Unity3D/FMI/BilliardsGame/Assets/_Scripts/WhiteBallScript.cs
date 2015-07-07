using UnityEngine;
using System.Collections;

public class WhiteBallScript : MonoBehaviour {

    public float ballSpeed;
    private Rigidbody body;

    void Start()
    {
        this.body = this.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        this.body.AddForce(new Vector3(moveHorizontal, 0, moveVertical) * this.ballSpeed);
    }
}
