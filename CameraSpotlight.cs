using UnityEngine;

public class CameraSpotlight : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Camera eyeCam;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation= eyeCam.transform.rotation;
        transform.LookAt(eyeCam.transform.forward);
        transform.position= eyeCam.transform.position;
    }
}
