using UnityEngine;
using TMPro;
public class compass : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Camera eyeCam;
    public TMPro.TextMeshPro directionText;

    public Vector3 north = new Vector3(-1f, 0f, 0f);
    private Vector3 cameraForward;

    void Start()
    {
        directionText.text = "hi terhe";
    }

    // Update is called once per frame
    void Update()
    {
        cameraForward = eyeCam.transform.forward;
        Vector3 projXZ = Vector3.ProjectOnPlane(cameraForward, new Vector3(0f, 1f, 0f)).normalized;

        float angle = Vector3.SignedAngle(north, projXZ, new Vector3(0f,1f,0f));

        if (-22.5f <= angle  && angle <= 22.5) {

            directionText.text = "N";

        } else if (22.5f < angle && angle <= 67.5)
        {

            directionText.text = "NE";

        }
        else if (67.5f < angle && angle <= 112.5)
        {

            directionText.text = "E";

        }
        else if (112.5f < angle && angle <= 157.5)
        {

            directionText.text = "SE";

        }
        else if (157.5f < angle && angle <= 180f || -180f >= angle && angle < -157.5f)
        {

            directionText.text = "S";

        }
        else if (-157.5f <= angle && angle < -112.5f) 
        {
            directionText.text = "SW";
        }
        else if (-112.5f <= angle && angle < -67.5f)
        {
            directionText.text = "W";
        }
        else if (-67.5 <= angle && angle < -22.5)
        {

            directionText.text = "NW";

        }


    }
}
