using Oculus.Interaction.Input;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class ColourIndicator: MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public UnityEvent<DotColour.Dots> onSwitchDot;
    public UnityEvent<bool> onToggleDot;
    public UnityEvent<OVRInput.Controller, float, float, float> hapticEvent;

    public DotColour.Dots value;

    public GameObject outline;
    public bool isSelected;

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
            if (other.gameObject.tag.Equals("Left Controller"))
            {
                controller = OVRInput.Controller.LTouch;
            }
            else if (other.gameObject.tag.Equals("Right Controller"))
            {

                controller = OVRInput.Controller.RTouch;

            }
            onSwitchDot.Invoke(value);
            onToggleDot.Invoke(false);
            isSelected = true;
            outline.SetActive(true);
            hapticEvent.Invoke(controller, touchFrequency, touchAmplitude, touchDuration);
        }
    
    }

    public void toggleOn(bool isToggled) { 
    
        isSelected= isToggled;
        outline.SetActive(isToggled);
    
    }



}
