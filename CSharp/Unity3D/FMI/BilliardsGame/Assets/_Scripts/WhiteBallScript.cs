using UnityEngine;
using System.Collections;
using Assets._Scripts;
using UnityEngine.UI;
using System;

public class WhiteBallScript : MonoBehaviour {

    public float maxHitTimeInterval;
    public float maxBallSpeed;
    public Text hitTextUI;
    private Rigidbody body;
    private Vector3 initialPosition;
    private bool isHitting;
    private DateTime? hitStartTime;

    private bool IsHitting
    {
        get
        {
            if (this.isHitting)
            {
                this.isHitting = !Input.GetMouseButtonUp(0);
            }
            else
            {
                this.isHitting = Input.GetMouseButtonDown(0);
            }

            return this.isHitting;
        }
    }

    void Start()
    {
        this.isHitting = false;
        this.body = this.GetComponent<Rigidbody>();
        this.initialPosition = this.body.position;
        this.HideHitAmount();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ObjectTags.TriggerPlane))
        {
            this.body.velocity = new Vector3();
            this.body.angularVelocity = new Vector3();
            this.body.MovePosition(this.initialPosition);
        }
    }

    void Update()
    {
        this.body.velocity = new Vector3(this.body.velocity.x, this.body.velocity.y > 0 ? 0 : this.body.velocity.y, this.body.velocity.z);

        if (this.hitStartTime.HasValue && this.IsHitting)
        {
            this.ShowHitAmount(DateTime.Now);
        }
    }

    void OnMouseExit()
    {
        this.HideHitAmount();
    }

    void OnMouseDown()
    {
        this.hitStartTime = DateTime.Now;
        this.ShowHitAmount(this.hitStartTime.Value);
    }

    void OnMouseUp()
    {
        this.TryHitTheBall();
    }

    private void ShowHitAmount(DateTime time)
    {
        float amount;

        if (this.TryGetHitAmount(time, out amount))
        {
            this.hitTextUI.text = string.Format(@"Hit!
{0}%", Math.Round(amount * 100));
        }
    }

    private bool TryGetHitAmount(DateTime time, out float amount)
    {
        if (this.hitStartTime.HasValue)
        {
            TimeSpan deltaTime = time - this.hitStartTime.Value;
            amount = this.maxHitTimeInterval > 0 ? (Math.Min(1, (float)deltaTime.TotalSeconds / this.maxHitTimeInterval)) : 1;

            return true;
        }

        amount = 0;
        return false;
    }

    private void HideHitAmount()
    {
        this.hitTextUI.text = string.Empty;
        this.hitStartTime = null;
    }

    private void TryHitTheBall()
    {
        float amount;
        if (this.TryGetHitAmount(DateTime.Now, out amount))
        {
            this.HideHitAmount();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float speed = amount * maxBallSpeed;
            Vector3 direction = new Vector3(ray.direction.x, 0, ray.direction.z);

            if (direction.magnitude > 0)
            {
                direction.Normalize();
                //this.body.AddForceAtPosition(ray.direction * speed, this.body.centerOfMass, ForceMode.Impulse);
                this.body.AddForce(ray.direction * speed, ForceMode.Impulse);                
            }
        }
    }
}
