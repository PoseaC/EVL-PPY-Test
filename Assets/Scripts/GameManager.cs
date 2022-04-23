using UnityEngine;
using Cinemachine;
using TMPro;
using SimpleInputNamespace;

public class GameManager : MonoBehaviour
{
    public CarBehaviour[] cars; //an array with the car prefabs
    CarBehaviour currentCar; //the currently responsive car
    public CinemachineVirtualCamera mainCamera; 
    public TextMeshProUGUI carInfo; //where to display details about the active car
    public TextMeshProUGUI speedDisplay; //where to display the speed of the active car
    public SteeringWheel steeringWheel; //steering wheel input

    CarBehaviour[] activeCars; //an array with the already spawned cars
    int currentCarIndex = 0; //index to keep track of the active car in the activeCars array
    private void Awake()
    {
        Vector3 spawnPosition = transform.position; //starting spawn position
        activeCars = new CarBehaviour[cars.Length]; //initialize the spawned cars array
        int carSpawnIndex = 0;
        foreach (CarBehaviour c in cars)
        {
            CarBehaviour car = Instantiate(c.gameObject, spawnPosition, Quaternion.identity).GetComponent<CarBehaviour>(); //spawn the current car in the array
            car.steeringWheel = steeringWheel; //assign the steering wheel for input
            car.enabled = false; //disable the script until needed
            spawnPosition += Vector3.right * 5; //spawn the next car 5 units to the right
            activeCars[carSpawnIndex] = car; //add the car to the spawned array
            carSpawnIndex++; 
        }
        currentCar = activeCars[0];
    }
    void Start()
    {
        ChangeCar();
    }

    public void ChangeCar()
    {
        //move the index to the next car in the array
        currentCarIndex += 1; 
        if (currentCarIndex >= activeCars.Length)
            currentCarIndex = 0;

        //disable the current active car, then activate the next one and change the camera and the car info display
        currentCar.enabled = false; 
        currentCar = activeCars[currentCarIndex];
        currentCar.enabled = true;
        mainCamera.Follow = mainCamera.LookAt = currentCar.playerPOV;
        carInfo.text = currentCar.carInfo;

    }

    //all 4 functions are used for the Pointer Down and Pointer Up events on the Acceleration and Brake pedal buttons for touch and hold functionality
    public void StartAcceleration()
    {
        currentCar.isAccelerating = true;
    }
    public void StopAcceleration()
    {
        currentCar.isAccelerating = false;
    }
    public void StartBraking()
    {
        currentCar.isBraking = true;

        //light up the brake lights if the car can brake
        if (currentCar.canBrake)
        {
            foreach (Light l in currentCar.brakeLights)
            {
                l.intensity = 1;
            }
        }
    }
    public void StopBraking()
    {
        currentCar.isBraking = false;
        foreach (Light l in currentCar.brakeLights)
        {
            l.intensity = 0;
        }
    }
}
