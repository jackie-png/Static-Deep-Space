using UnityEngine;
using UnityEngine.Events;
public class BraceletScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject settingsMenu;
    public Transform settingsTransform;

    private bool menuOn = false;

    public Transform menuSpawnPoint;
    private OVRInput.Controller controller = OVRInput.Controller.None;
    public UnityEvent<OVRInput.Controller, float, float, float> hapticEvent;

    public float touchFrequency = 1f;
    public float touchAmplitude = 0.1f;
    public float touchDuration = 0.1f;

    public float offset;

    public float coolDown = 1f;
    private float lastPressed = 0f;

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

            Debug.Log(other.gameObject.name);

            if (other.gameObject.CompareTag("Left Controller"))
            {
                controller = OVRInput.Controller.LTouch;
            }
            else if (other.gameObject.CompareTag("Right Controller"))
            {

                controller = OVRInput.Controller.RTouch;

            }

            float time = Time.time;
            if (Mathf.Abs(time - lastPressed) > coolDown) { 
            
                menuOn = !menuOn;
                settingsMenu.SetActive(menuOn);            
                lastPressed = time;

                hapticEvent.Invoke(controller, touchFrequency, touchAmplitude, touchDuration);

            }


            if (menuOn) {

                settingsTransform.position = menuSpawnPoint.position + menuSpawnPoint.right * offset;
                settingsTransform.rotation = menuSpawnPoint.rotation;
            }


        }
    }
}
