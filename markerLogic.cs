using UnityEngine;
using UnityEngine.Events;

public class markerLogic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public UnityEvent<GameObject> onGrab;

    OVRInput.Controller curController;

    void Start()
    {
        
    }

    public void OnGripBegin(OVRInput.Controller controller) {
    
        curController = controller;

        onGrab.Invoke(gameObject);
    
    }

    public void OnGripEnd(OVRInput.Controller controller) {

        if (curController == controller) {

            curController = OVRInput.Controller.None;
            onGrab.Invoke(gameObject);
        
        }
    
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
