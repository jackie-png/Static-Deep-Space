using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;
using UnityEngine.Events;

public class Jumpscare : MonoBehaviour
{

    public Transform rightHand;
    public Transform leftHand;

    // ending position 0,0,1
    public Transform forwardHand;

    public Transform eyeCamera;

    public enum JumpScareDirection { None, FromFront, FromBack };
    public JumpScareDirection jumpDirection = JumpScareDirection.None;

    private float progress = 0f;

    public bool hasBeenJumped = false;
    public bool jumpComplete = false;

    public float jumpSpeed = 1f;
    public float jumpProgress = 0f;

    private Quaternion leftAngle; 
    private Quaternion rightAngle;

    public float leftEndAngle;
    public float rightEndAngle;

    private float leftTargetAngle;
    private float rightTargetAngle;

    public float startLeftAngle;
    public float startRightAngle;


    private Vector3 forwardStartingPos;

    [Header("Forward hand's ending positions")]
    [Tooltip("In Local Space")]
    public Vector3 forwardEndingPos;

    public Volume postProcessingVolume;
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;

    public float fadeSpeed;

    public UnityEvent gameManagerTP;


    // left hand 0 -> 166
    // right hand 0 -> 157

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        forwardHand.gameObject.SetActive(false);
    }
    void Start()
    {

        postProcessingVolume.profile.TryGet(out vignette);
        postProcessingVolume.profile.TryGet(out colorAdjustments);
        //Debug.Log($"vignette: {vignette} | color adustment: {colorAdjustments}");

        leftAngle = leftHand.localRotation;
        rightAngle = rightHand.localRotation;

        forwardStartingPos = forwardHand.localPosition;

    }

    // Update is called once per frame
    void Update()
    {
        if (hasBeenJumped) {

            if (jumpDirection == JumpScareDirection.FromBack && !jumpComplete)
            {


                jumpProgress += Time.deltaTime * jumpSpeed;
                jumpProgress = Mathf.Clamp01(jumpProgress);

                float currLeftAngle = Mathf.LerpAngle(leftAngle.eulerAngles.z, leftTargetAngle, jumpProgress);
                float currRightAngle = Mathf.LerpAngle(rightAngle.eulerAngles.z, rightTargetAngle, jumpProgress);

                //Debug.Log($"Current Left right angles: {currLeftAngle} , {currRightAngle} | requirements: {leftEndAngle - leftAngle.eulerAngles.z} , {rightEndAngle - rightAngle.eulerAngles.z}");


                if (currLeftAngle >= leftTargetAngle && currRightAngle >= rightTargetAngle)
                {

                    jumpComplete = true;
                    currLeftAngle = leftTargetAngle;
                    currRightAngle = rightTargetAngle;

                }

                leftHand.localRotation = leftAngle * Quaternion.AngleAxis(currLeftAngle, Vector3.forward);
                rightHand.localRotation = rightAngle * Quaternion.AngleAxis(currRightAngle, Vector3.forward);



            }
            else if (jumpDirection == JumpScareDirection.FromFront && !jumpComplete) {

                jumpProgress += Time.deltaTime * jumpSpeed;
                jumpProgress = Mathf.Clamp01(jumpProgress);

                Vector3 currentPos = Vector3.Lerp(forwardStartingPos, forwardEndingPos, jumpProgress);

                if (Vector3.Distance(currentPos, forwardEndingPos) <= 0.5) {
                    jumpComplete = true;
                    currentPos = forwardEndingPos;
                
                }


                forwardHand.localPosition = currentPos;

            }
        
        
        }

        if (jumpComplete) {

            StartCoroutine(FadeIn());
        
        }
    }

    void ActivateJumpScare() {

        hasBeenJumped = true;
        jumpProgress = 0f;


        vignette.intensity.value = 0.5f;
        vignette.smoothness.value = 1f;

        colorAdjustments.colorFilter.value = new Color(0.49f, 0.19f, 0.19f);


        if (jumpDirection == JumpScareDirection.FromBack)
        {
            leftTargetAngle = leftEndAngle - startLeftAngle;
            rightTargetAngle = rightEndAngle - startRightAngle;

        }
        else if (jumpDirection == JumpScareDirection.FromFront) {

            forwardHand.gameObject.SetActive(true);

        
        }

        


        
    
    }

    IEnumerator FadeIn() {

        //Debug.Log("Fade In");



        yield return new WaitForSeconds(0.1f);
       

        colorAdjustments.colorFilter.value = Color.black;
        vignette.intensity.value = 1f;       


        yield return new WaitForSeconds(1f);
        gameManagerTP.Invoke();
        StartCoroutine(FadeOut());
        Reset();


    
    }

    IEnumerator FadeOut()
    {
        //Debug.Log("Fade Out");

        

        Color startColourFilter = colorAdjustments.colorFilter.value;
        float startIntensity = vignette.intensity.value;
        float startSmoothness = vignette.smoothness.value;
        float fadeProgress = 0f;

        while (fadeProgress < 1f)
        {

            fadeProgress += Time.deltaTime * fadeSpeed;
            fadeProgress = Mathf.Clamp01(fadeProgress);

            colorAdjustments.colorFilter.value = Color.Lerp(startColourFilter, Color.white, fadeProgress);
            vignette.intensity.value = Mathf.Lerp(startIntensity, 0f, fadeProgress);
            vignette.smoothness.value = Mathf.Lerp(startSmoothness, 0f, fadeProgress);
            yield return null;
        }


        

        yield return null;
    }


    public void Reset()
    {
        jumpComplete = false;
        hasBeenJumped= false;    
        jumpDirection = JumpScareDirection.None;
        forwardHand.gameObject.SetActive(false);

        leftHand.localRotation = leftAngle;
        rightHand.localRotation = rightAngle;
    }

    public void WhichScare(JumpScareDirection direction) { 
    
        switch (direction)
        {
            case JumpScareDirection.FromFront:
                jumpDirection = JumpScareDirection.FromFront;
                break;
            case JumpScareDirection.FromBack:
                jumpDirection = JumpScareDirection.FromBack;
                break;
        }

        ActivateJumpScare();
    
    }




}
