using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MapScrollScript: MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public MapManager.MapButton mapOption;

    public UnityEvent<MapManager.MapButton> mapOptionEvent;
    public UnityEvent<OVRInput.Controller, float, float, float> hapticEvent;
    private OVRInput.Controller controller = OVRInput.Controller.None;

    public float touchFrequency = 1f;
    public float touchAmplitude = 0.1f;
    public float touchDuration = 0.1f;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Controllers")
        {
            if (other.gameObject.CompareTag("Left Controller"))
            {
                controller = OVRInput.Controller.LTouch;
            }
            else if (other.gameObject.CompareTag("Right Controller"))
            {

                controller = OVRInput.Controller.RTouch;

            }
            mapOptionEvent.Invoke(mapOption);
            hapticEvent.Invoke(controller, touchFrequency, touchAmplitude, touchDuration); 

        }
    }

}
