using UnityEngine;
using System.Collections;
using UnityEngine.Events;


public class DoorManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public enum DoorDirection { X, Y, Z };
    public enum DoorMovement { Left, Right};

    public enum DoorMovementAmount { Length, Width };

    public DoorDirection openDirection;
    public DoorMovement openMovement;
    public DoorMovementAmount amount;


    public bool isOpened = false;
    private BoxCollider collider;

    private float openProgress = 0;
    public float openSpeed = 1f;
    private float lengthOfDoor;

    public Transform leftDoor;
    public Transform rightDoor;
    private Vector3 leftStart;
    private Vector3 leftEnd;

    private Vector3 rightStart;
    private Vector3 rightEnd;




    public AudioSource doorOpenSound;

    public UnityEvent doorHandleReset;
    void Start()
    {
        collider= leftDoor.gameObject.GetComponent<BoxCollider>();
        leftStart = leftDoor.localPosition;
        rightStart = rightDoor.localPosition;

    }

    // Update is called once per frame
    void Update()
    {

        if (isOpened)
        {
            openProgress += Time.deltaTime * openSpeed; 
            openProgress = Mathf.Clamp01(openProgress);


            float t = Mathf.SmoothStep(0f, 1f, openProgress);
            //Debug.Log(t);      
            
            leftDoor.localPosition = Vector3.Lerp(leftStart, leftEnd, t);
            rightDoor.localPosition = Vector3.Lerp(rightStart, rightEnd, t);

            //Debug.Log($"Left Door: {leftDoor.localPosition}, Right Door: {rightDoor.localPosition}");

        }


    }

    public void OpenDoor() {

        //Debug.Log("openning Door");
        StartCoroutine(SetOpenDoor());



    }

    IEnumerator SetOpenDoor() {


        doorOpenSound.Play();
        yield return new WaitForSeconds(0.5f);



        Vector3 direction = GetDirection();

        lengthOfDoor = GetMovementAmount();
        
        Vector3 newPosDirection = direction * lengthOfDoor;


        switch (openMovement) {

            case DoorMovement.Left:
                leftEnd = leftDoor.localPosition + newPosDirection;
                rightEnd = rightDoor.localPosition - newPosDirection;
                break;
            case DoorMovement.Right:
                leftEnd = leftDoor.localPosition - newPosDirection;
                rightEnd = rightDoor.localPosition + newPosDirection;
                break;
        } 



        //Debug.Log($"length of door: {lengthOfDoor} , Direction: {newPosDirection}, LeftEnd: {leftEnd} , Rightend: {rightEnd} ");

        openProgress = 0f;

        isOpened = true;

    }

    Vector3 GetDirection()
    {
        Vector3 localDirection;
        switch (openDirection)
        {
            case DoorDirection.X:
                localDirection = Vector3.right;
                break;
                
            case DoorDirection.Y:
                localDirection =  Vector3.up;
                break;
                
            default:
                localDirection = Vector3.forward;
                break;
                
        }

        return localDirection;
    }

    float GetMovementAmount() { 
    
        switch(amount)
        {
            case DoorMovementAmount.Length: 
                return collider.size.y;
            case DoorMovementAmount.Width: 
                return collider.size.x;
            default:
                return collider.size.x;
        }
    
    }

    public void Reset()
    {
        //Debug.Log("Door Reset in Manager");

        isOpened = false;
        openProgress = 0f;
        leftDoor.localPosition = leftStart;
        rightDoor.localPosition = rightStart;
        doorHandleReset.Invoke();
    }
}
