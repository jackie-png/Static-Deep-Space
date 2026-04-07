using UnityEngine;

public class ScannerMarkerDot : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Camera eyeCam;


    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(GetComponent<Camera>().transform.position, Vector3.up);
    }
}
