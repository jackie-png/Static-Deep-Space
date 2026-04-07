using UnityEngine;
using UnityEngine.Events;
using System.Collections;
public class ColourSlider : MonoBehaviour
{

    public enum LocalAxis { X, Y, Z }
    public enum RGB { Red, Green, Blue}
    public RGB sliderColour; 

    [Header("Parts")]
    [Tooltip("The moving part (handle). If null, will search for a child named 'Handle'.")]
    public Transform handle;

    [Header("Linear Motion")]
    public LocalAxis axis = LocalAxis.Z;


    private OVRInput.Controller controllerInput;
    private GameObject controller;
    public Rigidbody handleRb;
    public float gripThreshold = 1f;

    private bool isInHandle;
    private bool isGripping;

    private Vector3 whereControllerWas;
    private Vector3 whereSliderHandleWas;

    private Vector3 sliderDirection;
    private float handleStartPos = -4.0f;

    public float moveSpeed = 10.0f;

    public MonoBehaviour managerTarget;
    private IColourManager colourManager;

    private Vector3 handleInitialPos;
    private float initialHue;

    public UnityEvent<OVRInput.Controller, float, float, float> hapticEvent;

    public float touchFrequency = 1f;
    public float touchAmplitude = 0.1f;
    public float touchDuration = 0.1f;

    public float endsFrequency = 1f;
    public float endsAmplitude = 0.1f;
    public float endsDuration = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sliderDirection = Vector3.forward;

        colourManager = managerTarget as IColourManager;

        if (sliderColour == RGB.Red) {

            initialHue = colourManager.Red;
        
        } else if (sliderColour == RGB.Green)
        {
            initialHue = colourManager.Green;
        } else if(sliderColour == RGB.Blue)
        {
            initialHue = colourManager.Blue;
        }

        SetSliderPos(initialHue);



    }


    public void OnGripBegin(GameObject controller) {

        //convert the controller's starting position
        whereControllerWas = controller.transform.position;
        whereSliderHandleWas = transform.InverseTransformPoint(handle.position);
        hapticEvent.Invoke(controllerInput, touchFrequency, touchAmplitude, touchDuration);
    }

    public void OnGripEnd()
    {

        controller = null;
        controllerInput = OVRInput.Controller.None;

    }

    // Update is called once per frame
    void Update()
    {

        if (isInHandle && controller != null)
        {

            float sideTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, controllerInput);
            float primaryTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controllerInput);
            bool button2 = OVRInput.Get(OVRInput.Button.Two, controllerInput);


            //Debug.Log($"sideTrigger: {sideTrigger} | primarytrigger: {primaryTrigger} : Controller: {controllerInput}");

            if (primaryTrigger >= gripThreshold)
            {



                if (!isGripping)
                {
                    isGripping = true;
                    OnGripBegin(controller);
                }








            } else if (primaryTrigger <= gripThreshold && isGripping) {

                isGripping = false;
                OnGripEnd();
            
            }

            if (isGripping) {


                // moving the slider handle
                // delta vector is in the local space of the slider
                Vector3 worldDeltaVector = controller.transform.position - whereControllerWas;
                Vector3 localDeltaVector = transform.InverseTransformDirection(worldDeltaVector);

                float deltaMove = Vector3.Dot(localDeltaVector, sliderDirection);



                Vector3 sliderMovementVector = sliderDirection * deltaMove;

                Vector3 newPos = whereSliderHandleWas + sliderMovementVector;


                deltaMove = Mathf.Clamp(deltaMove, -4.0f, 4.0f);

                Vector3 potentialPos = whereSliderHandleWas + sliderDirection * deltaMove * moveSpeed;

                if (potentialPos.z > 4.0f) {

                    potentialPos = new Vector3(potentialPos.x, potentialPos.y, 4.0f);
                    hapticEvent.Invoke(controllerInput, endsFrequency, endsAmplitude, endsDuration);
                } else if (potentialPos.z < -4.0f) {

                    potentialPos = new Vector3(potentialPos.x, potentialPos.y, -4.0f);
                    hapticEvent.Invoke(controllerInput, endsFrequency, endsAmplitude, endsDuration);
                }

                handle.localPosition = potentialPos;

                // translating the slider's position to a coresponding colour

                Vector3 handlePos = transform.InverseTransformPoint(handle.position);

                float distanceTravelled = handlePos.z - handleStartPos;

                float totalDistance = 4.0f - (-4.0f);

                float percentTravelled = distanceTravelled / totalDistance;

                float colour = percentTravelled;

                if (sliderColour == RGB.Red)
                {

                    colourManager.Red = colour;

                }
                else if (sliderColour == RGB.Green)
                {
                    colourManager.Green = colour;


                }
                else if (sliderColour == RGB.Blue) { 
                
                    colourManager.Blue = colour;
                
                }


                Debug.Log($"NewPos: {newPos} | SliderMovementVector: {sliderMovementVector} | deltaMove: {deltaMove} | SliderDirection: {sliderDirection} |final pos = {handle.localPosition} | red: {colour}");

            
            }


        }


    }

    public void handleTriggerEnter(Collider other) {

        Debug.Log(LayerMask.LayerToName(other.gameObject.layer));
        if (LayerMask.LayerToName(other.gameObject.layer) == "Controllers")
        {

            Debug.Log("Controller Enters");
            isInHandle = true;
            controller = other.gameObject;

            Debug.Log(controller.name);
            Debug.Log(controller.tag);

            if (controller.tag.Equals("Left Controller"))
            {

                Debug.Log("Left Entered");
                controllerInput = OVRInput.Controller.LTouch;

            }
            else if (controller.tag.Equals("Right Controller"))
            {

                Debug.Log("Right Controller");
                controllerInput = OVRInput.Controller.RTouch;

            }


        };

    }

    public void handleTriggerExit(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Controllers")
        {

            Debug.Log("Controller Exits");
            isInHandle = false;

            controller = null;
            controllerInput = OVRInput.Controller.None;

            isGripping = false;

        }

    }

    public void SetSliderPos(float hue) {

        
        Vector3 pos = handle.localPosition;
        float t = Mathf.Lerp(-4f, 4f, hue);
        pos.z = t;
        handle.localPosition = pos;
        

    }


    Vector3 AxisLocal()
    => axis == LocalAxis.X ? handle.right :
       axis == LocalAxis.Y ? handle.up :
                             handle.forward;

}
