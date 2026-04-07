using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class MenuScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public MenuManager.MenuOption menuOption;

    public UnityEvent<MenuManager.MenuOption> menuOptionEvent;
   


    private OVRInput.Controller controller = OVRInput.Controller.None;
    public UnityEvent<OVRInput.Controller, float, float, float> hapticEvent;
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
            if (other.gameObject.tag.Equals("Left Controller"))
            {
                controller = OVRInput.Controller.LTouch;
            }
            else if (other.gameObject.tag.Equals("Right Controller")) { 
            
                controller = OVRInput.Controller.RTouch;

            }

            Debug.Log("back button pressed");
            menuOptionEvent.Invoke(menuOption);
            hapticEvent.Invoke(controller, touchFrequency, touchAmplitude, touchDuration);
        }
    }

}
