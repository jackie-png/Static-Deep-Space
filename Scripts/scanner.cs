using UnityEngine;
using System.Collections;
using UnityEngine.Events;
public class scanner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    private GameObject controller;
    private OVRInput.Controller controllerInput = OVRInput.Controller.None;
    private float threshold = 0.8f;

    public GameObject marker;

    private Transform anchorPoint;

    private bool isScannerRectangleOn = false;

    public Transform scannerScreen;
    private MeshRenderer screenRenderer;
    private float screenWidth;
    private float screenHeight;

    // positions from laser origin to a screen in front of it
    private Vector3[,] screenPoints = null;

    // line renderers for each ray fired
    private LineRenderer[,] laserLines = null;

    // a raycastHit for each ray to see what it hit if anything
    private RaycastHit[,] rayCastsHits = null;

    public Camera eyeCamera;
    public GameObject markerSpritePreFab;
    //public ScannerMarkerDot markerScript;

    //private GameObject[,] markers = null;

    public ParticleSystem markerParticleSystem;

    [Header("Number of Rays Fired")]
    [Tooltip("Make sure the max particles in the laser particle system has enough to handle verticalRays * Horizontal rays")]
    public int verticalRays = 10;
    public int horizontalRays = 10;
    public Transform rayOrigin;

    private Material markerMaterial;
    private bool isPickedUp = false;

    [Header("Dot Colours")]
    public Color closeColour;
    public Color farColour;
    public Color enemyOutlineColour;
    public Color doorHitColour;

    [Header("Laser Draw Time")]
    public float laserDelay = 0.01f;
    public float laserDrawTime = 0.1f;

    [Header("Marker Characteristics")]
    public float markerSize = 0.01f;
    public float markerLifeTime = 5f;

    [Header("Scanner Cooldown")]
    public float coolDown = 5f;
    private float lastFired = 0f;

    [Tooltip("Material for the laser. Unlit additive looks nice.")]
    public Material laserMaterial;

    [Tooltip("Laser width (meters).")]
    public float laserWidth = 0.01f;

    [Tooltip("Max laser distance when nothing is hit.")]
    public float laserMaxDistance = 12f;

    [Tooltip("Layers the laser can hit.")]
    private LayerMask laserMask;

    [Header("Laser (LineRenderer)")]
    [Tooltip("Optional: assign an existing LineRenderer. If null, one will be created in Awake().")]
    public GameObject laserPrefab;


    public DotColour dotColour;


    public Transform reloadBar;
    public Color reloadStart;
    public Color reloadEnd;
    public Material reloadMat;

    public float grabFrequency = 1f;
    public float grabAmplitude = 1f;
    public float grabDuration = 0.1f;

    public float fireFrequency = 1f;
    public float fireAmplitude = 1f;
    public float fireDuration = 2f;

    private int particleCount = 0;


    public UnityEvent hitEnemyEvent;

    public AudioSource scanSound;
    public AudioSource scanPickUp;
    public AudioSource scanReload;

    private void Awake()
    {
        // Laser can only hit objects with "Target" Layer Mask
        laserMask = LayerMask.GetMask("Laserable", "Floor", "Enemy");
        laserLines = new LineRenderer[verticalRays, horizontalRays];
        rayCastsHits = new RaycastHit[verticalRays, horizontalRays];

        markerMaterial = markerParticleSystem.GetComponent<ParticleSystemRenderer>().material;
        //markerMaterial.SetVector("_Close_Colour", closeColour);
        //markerMaterial.SetVector("_Far_Colour", farColour);
        //markerMaterial.SetFloat("_MaxLength", laserMaxDistance);

        Debug.Log("Running Awake");
    }

    void Start()
    {
        Debug.Log("Running Start");
        screenRenderer= scannerScreen.GetComponent<MeshRenderer>();
        screenWidth = scannerScreen.localScale.x * 10f;
        screenHeight = scannerScreen.localScale.z * 10f;
        screenPoints = new Vector3[verticalRays, horizontalRays];

        // find the points the rays will hit on the plane
        float localLength = 10f;
        float verticalGap = localLength / verticalRays;
        float horizontalGap = localLength / horizontalRays;
        float localx = -5;
        float localz = 5;

        Debug.Log($"Vertical Rays: {verticalRays} | Horizontal Rays: {horizontalRays}");

        for (int i = 0; i < verticalRays; i++)
        {
            Debug.Log($"Rays at row: {i}");

            for (int j = 0; j < horizontalRays; j++)
            {

                Debug.Log($"Ray [{i}, {j}]");

                Vector3 rayPosition = new Vector3(localx, 0, localz);

                //Debug.Log(rayPosition);
                
                screenPoints[i,j] = rayPosition;
                GameObject laserObj = Instantiate(laserPrefab);
                laserLines[i, j] = laserObj.GetComponent<LineRenderer>();
                laserLines[i, j].enabled = false;

                rayCastsHits[i, j] = new RaycastHit();

                Debug.Log(rayPosition);

                //GameObject m = Instantiate(marker, scannerScreen);
                //m.transform.localPosition = rayPosition;
                //m.transform.localScale= new Vector3(0.1f, 0.1f, 0.1f);

                localx += horizontalGap;

            }

            Debug.Log("all local positions found");

            localz -= verticalGap;
            localx = -5;
        }

        closeColour = dotColour.closeDotsPanel.color;
        farColour = dotColour.farDotsPanel.color;

    }

    // Update is called once per frame
    void Update()
    {
        if (controller != null) {


            float sideTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch);
            float primaryTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch);
            bool button2 = OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.LTouch);

            if (button2) {
                isScannerRectangleOn = !isScannerRectangleOn;
                screenRenderer.enabled = !isScannerRectangleOn;
            }


            //Debug.Log(sideTrigger);

            if (sideTrigger > threshold && !isPickedUp)
            {
                isPickedUp = true;
                scanPickUp.Play();
                anchorPoint = controller.transform.Find("ScannerAnchor");
                transform.SetParent(anchorPoint, false);
                
                //Debug.Log(controller.transform.rotation.eulerAngles);
                
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                StartCoroutine(SendHaptic(grabFrequency, grabAmplitude, grabDuration, controllerInput));

                turnOffControllerModel();


                //Debug.Log(transform.rotation.eulerAngles);
            }

            // if the index finger trigger is pressed and the time since fired is greater than the cooldown, fire rays
            if (primaryTrigger > threshold)
            {
                // Fire rays at a fixed cooldown interval
                if (Time.time - lastFired > coolDown)
                {
                    lastFired= Time.time;
                    StartCoroutine(CastRays());
                }

            }
        
        }
    }

    private void turnOffControllerModel() {


        foreach (Transform child in controller.transform)
        {

            if (LayerMask.LayerToName(child.gameObject.layer) == "Controllers")
            {

                //Debug.Log(child.name + " is a controller");
                child.gameObject.SetActive(false);

            }

        }

    }

    IEnumerator CastRays() {

        scanSound.Play();
        if (laserPrefab == null) {
            yield break;
        }

        // Calculate laser origin, direction, and end position - use controller position
        // and controller coordinate system


        markerParticleSystem.Clear();

            Transform laserOrigin = rayOrigin;

        //markerMaterial.SetVector("_User_Position", rayOrigin.position);

            Vector3 laserDirection = rayOrigin.forward;

        StartCoroutine(SendHaptic(fireFrequency, fireAmplitude, fireDuration, controllerInput));
        yield return StartCoroutine(FireLaserLine(laserOrigin));

        StartCoroutine(AnimateReloadBar());
        
    }

    IEnumerator FireLaserLine(Transform laserOrigin) {

        particleCount = 0;

        for (int row = 0; row < screenPoints.GetLength(0); row++) {

            for (int col = 0; col < screenPoints.GetLength(1); col++)
            {

                
                Vector3 laserEndOnScreen = scannerScreen.TransformPoint(screenPoints[row, col]);

                Vector3 direction = (laserEndOnScreen - laserOrigin.position).normalized;

                Vector3 laserEnd;

                Vector3 markerPos;

                bool hitLaserable = Physics.Raycast(laserOrigin.position, direction, out rayCastsHits[row, col], 20f, laserMask);
                bool hitEnemy = false;
                bool hitDoor = false;

                if (hitLaserable)
                {


                    //Debug.Log($"hit something at {rayCastsHits[row, col].point}");

                    if (LayerMask.LayerToName(rayCastsHits[row, col].collider.gameObject.layer) == "Enemy")
                    {

                       hitEnemy = true;
                       hitEnemyEvent.Invoke();                        
                        
                        


                    }
                    else if (rayCastsHits[row, col].collider.gameObject.CompareTag("Door")){

                        hitDoor = true;
                    }

                    laserEnd = rayCastsHits[row, col].point;

                    markerPos = laserEnd + rayCastsHits[row, col].normal * 0.01f;

                }
                else {

                    ///Debug.Log($"Hit Nothing, setting laserEnd to be: {laserOrigin.position + (direction * 20f)}");
                    laserEnd = laserOrigin.position + (direction * 20f);
                    markerPos = laserEnd;

                }


                laserLines[row, col].enabled = true;

                ///Debug.Log($"Laser Origin: {laserOrigin} | Laser End: {laserEnd}");

                

                StartCoroutine(AnimateLaserLine(laserLines[row, col], laserOrigin, laserEnd, markerPos, hitEnemy, hitDoor));


            }

            yield return new WaitForSeconds(laserDelay);

        }
        

    }



    void SpawnLaserHitParticle(Vector3 spawnPos, bool hitEnemy, bool hitDoor) {

        particleCount++;
        Debug.Log($"Particle # {particleCount}");
        ParticleSystem.EmitParams marker = new ParticleSystem.EmitParams();

        marker.position = spawnPos;
        marker.startSize = markerSize;
        marker.startLifetime = markerLifeTime;
        marker.velocity = new Vector3(0f,0f,0f);

        Debug.Log($"Spawning particle at {spawnPos}"); // weird bug where only half the dots are fired despite them hitting something?



        Transform laserOrigin = rayOrigin;

        float orgToHit = (spawnPos - laserOrigin.position).magnitude;

        float tVal = orgToHit / laserMaxDistance;

        Color lerpedColour = Color.Lerp(closeColour, farColour, tVal);

        markerParticleSystem.transform.LookAt(eyeCamera.transform.position);

        if (hitEnemy)
        {
            marker.startColor = enemyOutlineColour;
        }
        else if (hitDoor)
        {
            marker.startColor = doorHitColour;
        }
        else { 
            marker.startColor = lerpedColour;
        
        }

        markerParticleSystem.Emit(marker, 1);
    
    
    }
    IEnumerator AnimateReloadBar() {

        float elaspedTime = 0f;

        float startScale = 0f;
        float endScale = 1f;

        while (elaspedTime < coolDown) { 
        
            elaspedTime+= Time.deltaTime;

            float scalePercent = Mathf.Clamp01(elaspedTime/ coolDown);

            float currentScale = Mathf.Lerp(startScale, endScale, scalePercent);

            Color currentColour = Color.Lerp(reloadStart, reloadEnd, scalePercent);

            reloadBar.localScale = new Vector3(1f, 1f, currentScale);
            reloadMat.color = currentColour;


            yield return null;
        
        }

        reloadBar.localScale = new Vector3(1f, 1f, 1f);
        reloadMat.color = reloadEnd;
        //scanReload.Play();

    }

    IEnumerator AnimateLaserLine(LineRenderer laserLine, Transform startPoint, Vector3 endPoint, Vector3 markerSpawnPos, bool hitEnemy, bool hitDoor) {

        float time = 0f;
    
        laserLine.SetPosition(0, startPoint.position);
        laserLine.SetPosition(1, startPoint.position);


        while (time < laserDrawTime) {

            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / laserDrawTime);

            Vector3 lerpEnd = Vector3.Lerp(startPoint.position, endPoint, t);

            laserLine.SetPosition(1, lerpEnd);

            yield return null;
        
        }

        laserLine.SetPosition(1, endPoint);
        laserLine.enabled = false;

        SpawnLaserHitParticle(markerSpawnPos, hitEnemy, hitDoor);

        //markerSprite.position = endPoint;
    
    }

    // ------------ Laser Setup ------------
    private void ConfigureLaser(LineRenderer lr, bool keepMaterialIfAssigned = false)
    {
        // Set all the line renderer (lr) paramters
        UnityEngine.Debug.Log("Laser Created");

        lr.positionCount = 2;

        lr.startColor = Color.red;
        lr.endColor = Color.red;

        lr.startWidth = laserWidth; lr.endWidth = laserWidth;

        lr.useWorldSpace = true;

        lr.material = laserMaterial;


        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;


        lr.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Left Controller")) {

            Debug.Log(other.gameObject);

            controller = other.gameObject;
            controllerInput = OVRInput.Controller.LTouch;
            
        }
    }

    IEnumerator SendHaptic(float frequency, float amplitude, float duration, OVRInput.Controller controller) {

        OVRInput.SetControllerVibration(frequency, amplitude, controller);
        yield return new WaitForSeconds(duration);

        OVRInput.SetControllerVibration(0f, 0f, controller);
        
    
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Left Controller"))
        {

            controller = null;
            controllerInput = OVRInput.Controller.None;

        }
    }

    public void setColours(Color close, Color far) { 
    
        closeColour= close;
        farColour= far;
    
    }
}
