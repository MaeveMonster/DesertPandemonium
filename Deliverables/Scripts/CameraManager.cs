using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    // Camera array that holds a reference to every camera in the scene
    public Camera[] cameras;

    // Current camera 
    private int currentCameraIndex;

    // Use this for initialization
    void Start()
    {
        cameras[1].GetComponent<FollowCamera>().target = GameObject.Find("Manager").GetComponent<Manager>().flockTracker.transform;
        cameras[2].GetComponent<FollowCamera>().target = GameObject.Find("Manager").GetComponent<Manager>().flowFieldFollower.transform;
        cameras[3].GetComponent<FollowCamera>().target = GameObject.Find("Manager").GetComponent<Manager>().pathFollower.transform;

        currentCameraIndex = 0;
        // Turn all cameras off, except the first default one
        for (int i = 1; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(false);
        }
        // If any cameras were added to the controller, enable the first one
        if (cameras.Length > 0)
        {
            cameras[0].gameObject.SetActive(true);
        }
    }
    // Update is called once per frame
    void Update()
    {
        cameras[1].GetComponent<FollowCamera>().target = GameObject.Find("Manager").GetComponent<Manager>().flockTracker.transform;
        cameras[2].GetComponent<FollowCamera>().target = GameObject.Find("Manager").GetComponent<Manager>().flowFieldFollower.transform;
        cameras[3].GetComponent<FollowCamera>().target = GameObject.Find("Manager").GetComponent<Manager>().pathFollower.transform;

        // Press the 'C' key to cycle through cameras in the array
        if (Input.GetKeyDown(KeyCode.C))
        {
            // Cycle to the next camera
            currentCameraIndex++;
            // If cameraIndexis in bounds, set this camera active and last one inactive
            if (currentCameraIndex < cameras.Length)
            {
                cameras[currentCameraIndex - 1].gameObject.SetActive(false);
                cameras[currentCameraIndex].gameObject.SetActive(true);
            }
            // If last camera, cycle back to first camera
            else
            {
                cameras[currentCameraIndex - 1].gameObject.SetActive(false);
                currentCameraIndex = 0;
                cameras[currentCameraIndex].gameObject.SetActive(true);
            }
        }
    }
}
