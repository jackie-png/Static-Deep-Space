using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.XR;

public class Lever : MonoBehaviour
{

    public enum Axis { X, Y, Z }

    [Header("Pivot (optional but recommended)")]
    [Tooltip("Point about which the lever rotates. If null, uses this transform.")]
    public Transform hingePivot;

    [Header("Hinge Axis")]
    public Axis hingeAxis = Axis.Z;

    [Tooltip("Minimum lever angle (e.g. -75).")]
    public float minAngle = -65f;

    [Tooltip("Maximum lever angle (e.g. 0).")]
    public float maxAngle = 0f;

    [Header("Motion")]
    [Tooltip("Max angular speed (deg/sec). 360–1200 feels good in VR.")]
    public float maxDegreesPerSecond = 900f;

    [Tooltip("Ignore tiny controller-induced angle changes (deg).")]
    public float deadZoneDeg = 0.15f;

    // Lever rest angle when lever off
    public float restAngle = 0f;

    [Header("Events")]
    [Tooltip("Fires continuously with value in range [0..1].")]
    public UnityEvent<float> onValueChanged;

    [Header("Haptics")]
    public bool haptics = false;
    [Range(0f, 1f)] public float dragHaptics = 0.2f;
    public float hapticInterval = 0.05f;

    public GameObject levelHandleObj;

    // State
    Quaternion startLocalRot;
    float currentAngle;

    // Grip state
    private GameObject controller;
    private OVRInput.Controller controllerInput;
    bool isGripping;
    bool isInHandle;

    public float gripThreshold = 1f;



    // Stable plane basis
    Vector3 axisWorld;
    Vector3 uWorld;
    Vector3 vWorld;

    // --- NO-SNAP GRAB OFFSET STATE ---
    float angleAtGrab;          // lever angle at grip start
    float ctrlAngleAtGrab;      // controller “angle” at grip start (in our u/v basis)

    float lastHapticTime;

    private bool isPulled = false;

    private bool isSwitched = false;
    public UnityEvent triggerCheckpoint;

    public UnityEvent<OVRInput.Controller, float, float, float> hapticEvent;
    public float touchFrequency = 1f;
    public float touchAmplitude = 0.1f;
    public float touchDuration = 0.1f;

    public float moveFrequency = 1f;
    public float moveAmplitude = 0.1f;
    public float moveDuration = 0.1f;

    public float endFrequency = 1f;
    public float endAmplitude = 0.1f;
    public float endDuration = 0.1f;

    public AudioSource leverSound;
    public AudioSource grabSound;

    public Renderer leverHandle;

    public Material leverStickMaterial;
    public Material leverHandleMaterial;
    public Color grabColour;

    public Renderer[] leverIndicators;

    private Material[] indicatorMaterial;

    float fullOnAngle; // used for mapping 0..1 (we treat minAngle as full-on with this prefab!)

    void Awake()
    {
        startLocalRot = transform.localRotation;
        indicatorMaterial = new Material[leverIndicators.Length];

        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        if (hingePivot == null) hingePivot = transform;

        // For this hinge lever, up position is a more negative angle,  rest angle = 0,
        // full-on= minAngle (negative)
        fullOnAngle = minAngle;

        currentAngle = Mathf.Clamp(restAngle, minAngle, maxAngle);
        ApplyRotation(currentAngle);

        leverStickMaterial = leverHandle.materials[0];
        leverHandleMaterial = leverHandle.materials[1];

        leverHandleMaterial.EnableKeyword("_EMISSION");
        leverStickMaterial.EnableKeyword("_EMISSION");


        for (int i = 0; i < leverIndicators.Length; i++) {

            indicatorMaterial[i] = leverIndicators[i].material;

        }

        //EmitValue(currentAngle);
    }
 public void OnGripBegin(GameObject ctrl)
    {
        controller = ctrl;
        isGripping = true;

        // TODO - Build a stable reference frame around the hinge axis (world)
        // Hint: see ToggleSwitch.cs 

        axisWorld = GetAxisWorld();
        uWorld = Vector3.up;
        vWorld = Vector3.forward;

        Debug.Log(axisWorld);


        // Capture grab angles to prevent snapping
        angleAtGrab = currentAngle;
        ctrlAngleAtGrab = ComputeControllerAngle(ctrl.transform.position);

        //Debug.Log(ctrlAngleAtGrab);

        //if (haptics) ctrl.HapticClick(0.2f, 0.02f);
        lastHapticTime = Time.time;
        grabSound.Play();
        hapticEvent.Invoke(controllerInput, touchFrequency, touchAmplitude, touchDuration);

        leverHandleMaterial.SetColor("_EmissionColor", grabColour);
        leverStickMaterial.SetColor("_EmissionColor", grabColour);

    }


    public void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Controllers") {

            Debug.Log("Controller Enters");
            isInHandle = true;
            controller = other.gameObject;

            //Debug.Log(controller.name);

            if (controller.tag.Equals("Left Controller"))
            {

                Debug.Log("Left Entered");
                controllerInput = OVRInput.Controller.LTouch;

            }
            else if(controller.tag.Equals("Right Controller")) {

                Debug.Log("Right Controller");
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

            controller = null;
            controllerInput = OVRInput.Controller.None;

            isGripping = false;

            leverHandleMaterial.SetColor("_EmissionColor", Color.black);
            leverStickMaterial.SetColor("_EmissionColor", Color.black);

        }


    }




    // Update is called once per frame
    void Update()
    {




        if (isInHandle && controller != null) { 
        
            float sideTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, controllerInput);
            float primaryTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controllerInput);
            bool button2 = OVRInput.Get(OVRInput.Button.Two, controllerInput);


            //Debug.Log($"sideTrigger: {sideTrigger} | primarytrigger: {primaryTrigger} : Controller: {controllerInput}");

            if (sideTrigger >= gripThreshold && primaryTrigger >= gripThreshold) {

                

                if (!isGripping) { 
                    isGripping = true;
                    OnGripBegin(controller);                
                }
                


            }

        
        }


        if (isGripping && controller != null)
        {
            // TODO
            // Hint: follow dial.cs and toggleswitch.ca
            // Lever has elements of both
            float angleNow = ComputeControllerAngle(controller.transform.position);

            float deltaAngle = Mathf.DeltaAngle(ctrlAngleAtGrab, angleNow);

            if (Mathf.Abs(deltaAngle) < deadZoneDeg)
            {

                deltaAngle = 0f;

            }
            else {

                hapticEvent.Invoke(controllerInput, moveFrequency, moveAmplitude, moveDuration);

            }

            float targetAngle = angleAtGrab + deltaAngle;

            targetAngle = Mathf.Clamp(targetAngle, minAngle, maxAngle);


            // Haptics
            if (haptics && Time.time - lastHapticTime >= hapticInterval)
            {
                lastHapticTime = Time.time;
                //controller.HapticTick(Mathf.Clamp01(dragHaptics), 0.015f);
                
            }
            currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, maxDegreesPerSecond * Time.deltaTime);

            if (currentAngle <= minAngle + 1 && isPulled == false) {

                //Debug.Log("Lever Pulled");
                isPulled = true;
                hapticEvent.Invoke(controllerInput, endFrequency, endAmplitude, endDuration);
                SwitchLever();

            
            }

            if (currentAngle == 0 && isPulled == true) {

                //Debug.Log("Lever Pulled off");
                isPulled = false;

            }

            /*
            if (currentAngle == -65f)
            {

                controller.HapticTick(0.5f, 0.015f);

            }             
             */




           //Debug.Log($"Angle At Grab: {ctrlAngleAtGrab} | Angle Now: {angleNow} | DeltaAngle: {deltaAngle} | targetAngle: {targetAngle} | current angle: {currentAngle}");

            ApplyRotation(currentAngle);

        }
    }


    protected virtual void SwitchLever() {

        if (!isSwitched)
        {
            onValueChanged.Invoke(1f);
            isSwitched = true;
            leverSound.Play();

            foreach (Material indicator in indicatorMaterial) {

                indicator.color = Color.green;
            
            }
        }

    }

    public bool IsSwitched { get { return isSwitched; } }

    float ComputeControllerAngle(Vector3 ctrlWorldPos)
    {
        // TODO - see dial.cs and toggleswitch.cs

        Transform p = transform.parent;


        Vector3 leverPos = p.InverseTransformPoint(levelHandleObj.transform.position);

        Vector3 controllerPos = p.InverseTransformPoint(ctrlWorldPos);


        Vector3 normalizedHingeAxis = GetAxisLocal().normalized;

        Vector3 handleToController = controllerPos - leverPos;


        Vector3 perpendicularHingeAxis = GetPerpendicularAxisLocal().normalized;

        Vector3 projection = Vector3.ProjectOnPlane(handleToController, normalizedHingeAxis);

        if (projection.magnitude < 1e-8f)
        {


            return 0f;
        }

        projection = projection.normalized;

        Vector2 basis = ConstructPlaneCS(normalizedHingeAxis, projection);


        float angle = Mathf.Atan2(basis.y, basis.x) * Mathf.Rad2Deg;


        return angle;
    }

    // Given plane normal and a vector projected onto this plane
    // contruct an orthonormal CS then define the projected vector in this CS
    Vector2 ConstructPlaneCS(Vector3 n, Vector3 projected)
    {
        Vector3 refDir = Vector3.up;
        if (Mathf.Abs(Vector3.Dot(refDir, n)) > 0.85f) refDir = Vector3.right;

        Vector3 u = Vector3.ProjectOnPlane(refDir, n).normalized;
        Vector3 v = Vector3.Cross(n, u).normalized;

        float x = Vector3.Dot(projected, u);
        float y = Vector3.Dot(projected, v);

        return new Vector2(x, y);
    }

    void ApplyRotation(float angle)
    {
        // Rotate relative to the rest angle
        transform.localRotation = startLocalRot * Quaternion.AngleAxis(angle, GetAxisLocal());
    }


    Vector3 GetAxisLocal()
    {
        return hingeAxis == Axis.X ? Vector3.right :
               hingeAxis == Axis.Y ? Vector3.up :
                                     Vector3.forward;
    }

    Vector3 GetPerpendicularAxisLocal()
    {

        return hingeAxis == Axis.X ? Vector3.up :
               hingeAxis == Axis.Y ? Vector3.forward :
                                     Vector3.right;

    }

    Vector3 GetAxisWorld()
    {
        // Axis in world based on the lever’s parent space 
        return transform.parent
            ? transform.parent.TransformDirection(GetAxisLocal()).normalized
            : transform.TransformDirection(GetAxisLocal()).normalized;
    }

    public void Reset()
    {
        ApplyRotation(0.0f);
        isPulled= false;
        isSwitched= false;
        isGripping= false;
        isInHandle= false;
        currentAngle = 0.0f;
    }


}
