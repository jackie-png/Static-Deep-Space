using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class SliderHandle : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {




    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Slider Triggered");
        transform.parent.GetComponent<ColourSlider>().handleTriggerEnter(other);
    }

    
    private void OnTriggerExit(Collider other)
    {
        transform.parent.GetComponent<ColourSlider>().handleTriggerExit(other);
    }     
   


}
