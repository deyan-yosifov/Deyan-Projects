using UnityEngine;
using System.Collections;

public class WhiteBallScript : MonoBehaviour {

    public float ballSpeed;
    private Rigidbody rigidbody;

    void Start()
    {
        this.rigidbody = this.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        this.rigidbody.AddForce(new Vector3(moveHorizontal, 0, moveVertical) * this.ballSpeed);
    }
}
