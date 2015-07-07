using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class CameraOrbitScript : MonoBehaviour {

    public Transform target;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;
    public float wheelSpeed = 10;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;

    private bool isDragging;

    float x = 0.0f;
    float y = 0.0f;

    private bool IsDragging
    {
        get
        {
            if (this.isDragging)
            {
                this.isDragging = !Input.GetMouseButtonUp(0);
            }
            else
            {
                this.isDragging = Input.GetMouseButtonDown(0);
            }

            return this.isDragging;
        }
    }

    void Start()
    {
        this.isDragging = false;
        Vector3 angles = this.transform.eulerAngles;
        this.x = angles.y;
        this.y = angles.x;
    }

    void LateUpdate()
    {
        if (this.target)
        {
            if (this.IsDragging)
            {
                this.x += Input.GetAxis("Mouse X") * this.xSpeed * this.distance * 0.02f;
                this.y -= Input.GetAxis("Mouse Y") * this.ySpeed * 0.02f;

                this.y = ClampAngle(this.y, this.yMinLimit, this.yMaxLimit);
            }

            Quaternion rotation = Quaternion.Euler(this.y, this.x, 0);

            this.distance = Mathf.Clamp(this.distance - Input.GetAxis("Mouse ScrollWheel") * this.wheelSpeed, this.distanceMin, this.distanceMax);

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + this.target.position;

            this.transform.rotation = rotation;
            this.transform.position = position;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
