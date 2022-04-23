using UnityEngine;
using TMPro;
using SimpleInputNamespace;
public class CarBehaviour : MonoBehaviour
{
    //the wheel colliders of the car
    [Header("Wheels Colliders")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider backLeftWheel;
    public WheelCollider backRightWheel;
   
    //the visual representation of the wheels
    [Header("Wheels Visual Transforms")]
    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform backLeftWheelTransform;
    public Transform backRightWheelTransform;

    [Header("Car Attributes")]
    public string carInfo; //description for the car
    public Transform playerPOV; //what point the camera should follow
    public float maxSpeed = 20; 
    public float motorTorque = 1000; //the force of the motor, determines the acceleration speed
    public float brakeTorque = 3000; //how hard the car can brake
    public bool canSteerRight = true;
    public bool canBrake = true;
    public Light[] brakeLights;

    //public variables used for player input that need to be accesed from the GameManager
    [HideInInspector] public bool isAccelerating = false;
    [HideInInspector] public bool isBraking = false;
    [HideInInspector] public SteeringWheel steeringWheel;

    Rigidbody carBody;
    float steeringAngle; //the steering wheel input value
    TextMeshProUGUI speedDisplay;
    private void Start()
    {
        carBody = GetComponent<Rigidbody>();
        carBody.centerOfMass = new Vector3(0, 0.125f, 0); //artificially lower the center of mass of the car to prevent the car from tipping over when turning
        speedDisplay = FindObjectOfType<GameManager>().speedDisplay;
    }
    //get input each frame, but act on it only on physics updates
    private void Update()
    {
        GetInput();
        speedDisplay.text = carBody.velocity.magnitude.ToString("0.0") + " km/h";
    }

    private void FixedUpdate()
    {
        HandleInput();
    }

    private void GetInput()
    {
        //limit the car speed; using the squared magnitude to check the car velocity saves a square root call compared to acessing directly the magnitude
        if (carBody.velocity.sqrMagnitude >= maxSpeed * maxSpeed)
            carBody.velocity = carBody.velocity.normalized * maxSpeed;

        if (canSteerRight)
            steeringAngle = steeringWheel.Angle;
        else
            steeringAngle = Mathf.Clamp(steeringWheel.Angle, -steeringWheel.maximumSteeringAngle, 0); //if the car can't steer right we ignore all positive input values of the steering wheel
    }

    private void HandleInput()
    {
        if (isAccelerating)
            frontLeftWheel.motorTorque = frontRightWheel.motorTorque = motorTorque;
        else
            frontLeftWheel.motorTorque = frontRightWheel.motorTorque = 0;

        if (isBraking && canBrake) 
            frontLeftWheel.brakeTorque = frontRightWheel.brakeTorque = backLeftWheel.brakeTorque = backRightWheel.brakeTorque = brakeTorque;
        else 
            frontLeftWheel.brakeTorque = frontRightWheel.brakeTorque = backLeftWheel.brakeTorque = backRightWheel.brakeTorque = 0;

        frontLeftWheel.steerAngle = frontRightWheel.steerAngle = steeringAngle; //tilt the front wheels according ot the steering wheel input

        //update each wheels visual, the collider itself doesn't move
        UpdateWheel(frontLeftWheel, frontLeftWheelTransform);
        UpdateWheel(frontRightWheel, frontRightWheelTransform);
        UpdateWheel(backLeftWheel, backLeftWheelTransform);
        UpdateWheel(backRightWheel, backRightWheelTransform);
    }

    private void UpdateWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        wheelCollider.GetWorldPose(out Vector3 position, out Quaternion rotation);
        wheelTransform.transform.SetPositionAndRotation(position, rotation);
    }
}
