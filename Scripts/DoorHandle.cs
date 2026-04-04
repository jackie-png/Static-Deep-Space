using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(Rigidbody))]
public class DoorHandle : MonoBehaviour
{
    [Header("Range")]
    [Tooltip("Half-range in degrees. Example: 45 means dial can go from -45..+45. 0 = unlimited.")]
    public float degreeRange = 45f;

    [Header("Feel")]
    [Tooltip("How fast the dial follows your wrist while gripping. 0 = instant.")]
    [Range(0f, 40f)] public float followSpeed = 25f;

    [Tooltip("Release grip if controller drifts too far from dial (meters).")]
    public float breakDistance = 0.40f;

    [Header("Output")]
    [Tooltip("Signed dial angle in degrees (about local Y axis).")]
    public float angleDeg;

    [Header("Haptics")]
    public bool haptics = true;
    [Range(0f, 1f)] public float dragHaptics = 0.25f;
    [Range(0f, 1f)] public float touchHaptics = 0.25f;

    // -------- Internal state --------
    private GameObject controller;
    private OVRInput.Controller controllerInput;
    Vector3 dialRightInCntlrLocal;            // dial reference dir stored in controller local space

    private bool isInHandle = false;
    private bool isGripping = false;

    Quaternion dialAngleAtGrab;

    //
    Quaternion controllerRotationAtGrab;

    float angleAtGrab;                        // dial angle when grip begins
    float controllerAngleAtGrab;              // controller-implied angle when grip begins (prevents snapping)

    float lastValue01 = -999f;

    public float gripThreshold = 1f;

    private float sideTrigger = 0f;
    private float primaryTrigger = 0f;

    public UnityEvent doorOpener;
    private bool doorOpened = false;

    public Renderer handleRenderer;
    private Material handleMaterial;

    public Color grabColour;

    //0.1108491, 0.4433962, 0.4002889

    void Awake()
    {
        // Kinematic RB is recommended for trigger-based interaction.
        var rb = GetComponent<Rigidbody>();

        handleMaterial = handleRenderer.material;
        handleMaterial.EnableKeyword("_EMISSION");

        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void Start()
    {
        // Initialize angle from transform (convert 0..360 to -180..180)
        angleDeg = Normalize180(transform.localEulerAngles.y);

        // Clamp if we have a bounded dial
        if (degreeRange > 0f) angleDeg = Mathf.Clamp(angleDeg, -degreeRange, degreeRange);

        // Set the dial
        ApplyAngle(angleDeg);
        // Output the value to the control panel via an event
    }

    public void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Controllers")
        {

            //Debug.Log("Controller Enters");
            isInHandle = true;
            controller = other.gameObject;

            //Debug.Log(controller.name);

            if (controller.CompareTag("Left Controller"))
            {

               // Debug.Log("Left Entered");
                controllerInput = OVRInput.Controller.LTouch;

            }
            else if (controller.CompareTag("Right Controller"))
            {

             //   Debug.Log("Right Controller");
                controllerInput = OVRInput.Controller.RTouch;

            }


        };
    }

   
     
    public void OnTriggerExit(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Controllers")
        {

            //Debug.Log("Controller Exits");
            isInHandle = false;
            handleMaterial.SetColor("_EmissionColor", Color.black);
            controller = null;
            controllerInput = OVRInput.Controller.None;

        }
    }     
    



    public void OnGripBegin(GameObject c)
    {
        controller = c;

       // Debug.Log("Gripping Handle");

        angleAtGrab = angleDeg;
        dialAngleAtGrab = transform.localRotation;

        handleMaterial.SetColor("_EmissionColor", grabColour);


        controllerRotationAtGrab = controller.transform.rotation;

    }

    public void OnGripEnd(GameObject c)
    {
       // Debug.Log("Let go of Handle");
        isGripping = false;
        handleMaterial.SetColor("_EmissionColor", Color.black);

    }

    void Update()
    {
        if (controller == null) return;

        // Break if controller moves too far away
        if (Vector3.Distance(controller.transform.position, transform.position) > breakDistance)
        {
            controller = null;
            return;
        }

        if (isInHandle && controller != null)
        {

            sideTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, controllerInput);
            primaryTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controllerInput);


            //Debug.Log($"sideTrigger: {sideTrigger} | primarytrigger: {primaryTrigger} : Controller: {controllerInput}");

            if (sideTrigger >= gripThreshold && primaryTrigger >= gripThreshold)
            {



                if (!isGripping)
                {
                    isGripping = true;
                    OnGripBegin(controller);
                }



            }

                if (sideTrigger < gripThreshold && primaryTrigger < gripThreshold && isGripping)
                {

                    isGripping = false;
                    OnGripEnd(controller);

                }


        }

        if (isGripping && controller != null) {
            //Debug.Log("Turning Handle");

            // Compute current controller-implied angle (signed, -180..180)
            float controllerAngleNow = ComputeControllerAngle();


            Quaternion delta =
                Quaternion.Inverse(controllerRotationAtGrab) *
                controller.transform.rotation;

            float deltaZ = delta.eulerAngles.z;
            deltaZ = -Normalize180(deltaZ);

            float deadZone = 0.5f; // try 0.3f–1.0f depending on sensitivity

            if (Mathf.Abs(deltaZ) < deadZone)
                deltaZ = 0f;

            float targetAngle = angleAtGrab + deltaZ;

            // TODO - Clamp it if bounded using degree range

            targetAngle = Mathf.Clamp(targetAngle, 0f, degreeRange);

            //Debug.Log("Target Clamped: " + targetAngle);

            // TODO - Smooth follow (optional) - update angleDeg with target angle computed above
            if (followSpeed <= 0f)
                angleDeg = targetAngle; // replace 0 with target angle
            else
            {
                // TODO - replace with Mathf.LerpAngle to target angle
                // (don't forget to multiply follow speed by Time.deltaTime)
                angleDeg = Mathf.LerpAngle(angleDeg, targetAngle, followSpeed * Time.deltaTime);
            }

            // Apply to transform + emit value
            ApplyAngle(angleDeg);

        }

        /*
            
    
         
         */


    }

    // ---------------- Core math ----------------

    float ComputeControllerAngle()
    {
        // TODO - Take the saved dialRightInCntlrLocal and transform it into WORLD space

        Vector3 dialRightWorld = controller.transform.TransformDirection(dialRightInCntlrLocal);


        // TODO - Then bring this world direction vector into the *dial's* LOCAL space

        Vector3 dialLocalDir = transform.InverseTransformDirection(dialRightWorld);

        // TODO - We only care about rotation around dial local Y, so we then project onto the XZ plane

        Vector3 projection = Vector3.ProjectOnPlane(dialLocalDir, transform.up);

        // Use Atan2 to give the angle of this projected vector in the dial XZ plane (signed)
        // Convert to degrees and Normalize180

        float angle = Mathf.Atan2(projection.z, projection.x) * Mathf.Rad2Deg;

        return Normalize180(angle);
    }

    void ApplyAngle(float a)
    {
        // TODO - Update dial local rotation
        // Use a Quaternion.AngleAxis to avoid 0/360 Euler snapping (i.e. convert to quat, use Vector3.up as axis)

        transform.localRotation = Quaternion.AngleAxis(a, Vector3.up);

        Debug.Log(transform.localRotation.eulerAngles);

        if (transform.localRotation.eulerAngles.y >= 88f && !doorOpened) {
            transform.localRotation = Quaternion.AngleAxis(90f, Vector3.up);
            doorOpened = true;
            OnGripEnd(controller);
            isInHandle = false;
            controller = null;
            controllerInput = OVRInput.Controller.None;
            //Debug.Log("Door Opened");
            doorOpener.Invoke();
        }


    }

    // ---------------- Output mapping ----------------

    float AngleTo01(float a)
    {
        if (degreeRange <= 1e-6f) return Mathf.Repeat(a / 360f, 1f);
        return Mathf.InverseLerp(-degreeRange, degreeRange, a);
    }


    // Normalize to [-180, +180]
    static float Normalize180(float a)
    {
        a = (a + 180f) % 360f;
        if (a < 0f) a += 360f;
        return a - 180f;
    }

    public void Reset()
    {
        doorOpened = false;
        
        ApplyAngle(0.0f);
        isInHandle = false;
        controller = null;
        controllerInput = OVRInput.Controller.None;
        isGripping= false;
        angleDeg = 0.0f;
    }
}