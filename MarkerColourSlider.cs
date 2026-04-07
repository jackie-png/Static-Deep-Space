using UnityEngine;

public class MarkerColourSlider : MonoBehaviour
{

    public enum LocalAxis { X, Y, Z }
    public enum RGB { Red, Green, Blue }
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

    public MarkerColourManager markerColour;

    private Vector3 handleInitialPos;
    private float initialHue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sliderDirection = Vector3.forward;

        if (sliderColour == RGB.Red)
        {

            initialHue = markerColour.red;

        }
        else if (sliderColour == RGB.Green)
        {
            initialHue = markerColour.green;
        }
        else if (sliderColour == RGB.Blue)
        {
            initialHue = markerColour.blue;
        }

        SetSliderPos(initialHue);



    }


    public void OnGripBegin(GameObject controller)
    {

        //convert the controller's starting position
        whereControllerWas = controller.transform.position;
        whereSliderHandleWas = transform.InverseTransformPoint(handle.position);

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








            }
            else if (primaryTrigger <= gripThreshold && isGripping)
            {

                isGripping = false;
                OnGripEnd();

            }

            if (isGripping)
            {


                // moving the slider handle
                // delta vector is in the local space of the slider
                Vector3 worldDeltaVector = controller.transform.position - whereControllerWas;
                Vector3 localDeltaVector = transform.InverseTransformDirection(worldDeltaVector);

                float deltaMove = Vector3.Dot(localDeltaVector, sliderDirection);



                Vector3 sliderMovementVector = sliderDirection * deltaMove;

                Vector3 newPos = whereSliderHandleWas + sliderMovementVector;


                deltaMove = Mathf.Clamp(deltaMove, -4.0f, 4.0f);

                Vector3 potentialPos = whereSliderHandleWas + sliderDirection * deltaMove * moveSpeed;

                if (potentialPos.z > 4.0f)
                {

                    potentialPos = new Vector3(potentialPos.x, potentialPos.y, 4.0f);
                }
                else if (potentialPos.z < -4.0f)
                {

                    potentialPos = new Vector3(potentialPos.x, potentialPos.y, -4.0f);

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

                    markerColour.ChangeRed(colour);

                }
                else if (sliderColour == RGB.Green)
                {
                    markerColour.ChangeGreen(colour);


                }
                else if (sliderColour == RGB.Blue)
                {

                    markerColour.ChangeBlue(colour);

                }


                Debug.Log($"NewPos: {newPos} | SliderMovementVector: {sliderMovementVector} | deltaMove: {deltaMove} | SliderDirection: {sliderDirection} |final pos = {handle.localPosition} | red: {colour}");


            }


        }


    }

    public void handleTriggerEnter(Collider other)
    {

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

    public void SetSliderPos(float hue)
    {


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
