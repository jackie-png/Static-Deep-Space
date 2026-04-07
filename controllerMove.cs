using UnityEngine;
using System.Collections;
using Unity.Android.Gradle.Manifest;

public class controllerMove : MonoBehaviour
{

    public OVRCameraRig cameraRig;
    public CharacterController characterController;

    //public Oculus.Interaction.Locomotion.CharacterController characterController;
    private float moveThreshold = 0.5f;
    public float moveSpeed = 1.0f;
    private bool isMoving = false;

    private Vector3 cameraForward;
    private Vector3 cameraRight;

    public AudioSource movingSound;

    private Vector3 velocity = Vector3.zero;
    public float gravity = -9.81f;

    public float slopeLimit = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Debug.Log(characterController);

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 joystickValue = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);

        //Debug.Log(joystickValue.magnitude);


        if (Mathf.Abs(joystickValue.y) > moveThreshold)
        {
            cameraForward = cameraRig.centerEyeAnchor.forward;
            cameraRight = cameraRig.centerEyeAnchor.right;

            if (!isMoving)
            {
                isMoving = true;
                StopAllCoroutines();
                movingSound.volume = 1f;
                movingSound.Play();
            }
        }
        else if (joystickValue.magnitude == 0f && isMoving)
        {
            isMoving = false;
            StartCoroutine(fadeOutSound());
        }

        MoveCamera(joystickValue);

    }

    void MoveCamera(Vector2 input)
    {
        cameraForward = cameraRig.centerEyeAnchor.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();
        cameraRight = cameraRig.centerEyeAnchor.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        Vector3 moveDir = cameraForward * input.y * moveSpeed * Time.deltaTime;

        if (!characterController.isGrounded) {

            Debug.Log("character not grounded");


        }

        float cameraHeight = cameraRig.centerEyeAnchor.localPosition.y;
        Vector3 cameraLocalPos = cameraRig.centerEyeAnchor.localPosition;
        characterController.height = cameraHeight;
        characterController.center = new Vector3(
            0f,
            cameraHeight / 2f,
            0f
        );

        velocity.y += Physics.gravity.y * Time.deltaTime;

        moveDir = moveDir + Vector3.up * velocity.y;

        characterController.Move(moveDir);
    }


    IEnumerator fadeOutSound()
    {



        while (movingSound.volume > 0.01f)
        {

            movingSound.volume = Mathf.Lerp(movingSound.volume, 0f, Time.deltaTime * 3f);
            yield return null;
        }

        movingSound.Stop();



    }
}

    