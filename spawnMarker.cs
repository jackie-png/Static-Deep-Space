using UnityEngine;
using Oculus.Interaction;
using UnityEngine.Events;

public class spawnMarker : MonoBehaviour
{

    public OVRCameraRig CameraRig;
    private OVRInput.Controller controllerInput = OVRInput.Controller.RTouch;

    private Vector3 forwardVec;
    public Transform controllerTip;
    public GameObject marker;
    private GameObject currentMarker;

    public Material baseMaterial;

    public GameObject lastTouchedMarker;

    private bool wasBehind;
    private Component grabbedThing;
    public float threshold = 0.8f;

    public GrabInteractor grabber;
    private bool isGrabbing = false;
    public int maxMarkers = 5;
    private int markerCount = 0;

    public UnityEvent<GameObject> markerColourManager;

    public UnityEvent<OVRInput.Controller, float, float, float> hapticEvent;
    public float touchFrequency = 1f;
    public float touchAmplitude = 0.1f;
    public float touchDuration = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void Awake()
    {
        Debug.Log(grabber);
    }


    private void OnEnable()
        {
            grabber.WhenInteractableSelected.Action += HandleGrab;
            grabber.WhenInteractableUnselected.Action += HandleRelease;
        }

        private void OnDisable()
        {
            grabber.WhenInteractableSelected.Action -= HandleGrab;
            grabber.WhenInteractableUnselected.Action -= HandleRelease;
        }

        private void HandleGrab(IInteractable interactable)
        {
            isGrabbing = true;
            grabbedThing = interactable as Component;

            lastTouchedMarker = grabbedThing.transform.parent.gameObject;
            hapticEvent.Invoke(controllerInput, touchFrequency, touchAmplitude, touchDuration);
            markerColourManager.Invoke(lastTouchedMarker);
            //Debug.Log("Grabbed: " + grabbedThing.transform.parent.name);
        }

        private void HandleRelease(IInteractable interactable)
        {
            isGrabbing = false;
            grabbedThing = null;
            //Debug.Log("Released: " + interactable);
        }     
   





    // Update is called once per frame
    void Update()
    {
     
        forwardVec = CameraRig.centerEyeAnchor.forward;
        Vector3 controllerToEye = transform.position - CameraRig.centerEyeAnchor.position;
        float dot = Vector3.Dot(forwardVec, controllerToEye);

        float sideTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch);

        //Debug.Log(isGrabbing);

        // Check if controller is behind camera
        if (dot < 0)
        {
            wasBehind = true; // mark that it went behind
        }

        // Check if controller moved forward while holding trigger and not holding anything already, only spawn in markers when there are less than the max amount
        if (dot <= 0 && sideTrigger >= 1.0f && wasBehind && !isGrabbing && markerCount <= maxMarkers)
        {
            isGrabbing = true;
            if (currentMarker == null)
            {
                currentMarker = Instantiate(marker, controllerTip.position, controllerTip.rotation, controllerTip);

                Material markerMaterial = new Material(baseMaterial);

                currentMarker.GetComponent<Renderer>().material = markerMaterial;

                lastTouchedMarker= currentMarker;
                markerColourManager.Invoke(lastTouchedMarker);
                markerCount++;

                currentMarker.GetComponent<Rigidbody>().isKinematic = true;
                hapticEvent.Invoke(controllerInput, touchFrequency, touchAmplitude, touchDuration);
                Debug.Log("marker created");
            }

            wasBehind = false; // reset after spawning
        }

        // unparent marker when trigger released
        if (sideTrigger < threshold && currentMarker != null) // tthere is a bug where grabbing the marker with the interactor and bringing it behind my head causes the marker to stay in place some how
        {
            currentMarker.transform.parent = null;

            Debug.Log("Marker About to be released" + currentMarker);
            Debug.Log($"sidetrigger: {sideTrigger} | currentMarker: {currentMarker} | isGrabbing: {isGrabbing} | Dot Product: {dot} | if condition {sideTrigger < 1.0f && currentMarker != null}");


            isGrabbing = false;

            currentMarker = null;
        }

        if (dot < 0 && isGrabbing) { // if the user's controller is behind them while they are holding something
            Debug.Log("controller behind");
            if (sideTrigger < threshold) { // if they let go of the marker behind them, destroy that marker and decrease the number of markers made
                Debug.Log(grabbedThing);
                Destroy(grabbedThing.transform.root.gameObject);
                isGrabbing = false;
                grabbedThing = null;
                lastTouchedMarker= null;
                markerCount--;
            
            }
        
        }

    }
}
