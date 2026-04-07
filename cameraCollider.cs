using Oculus.Interaction.Locomotion;
using UnityEngine;

public class cameraCollider : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private UnityEngine.CharacterController characterController;
    public Transform centerEye;
    public Transform cameraRig;


    void Start()
    {
        characterController= GetComponent<UnityEngine.CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float headHeight = Mathf.Clamp(centerEye.localPosition.y, 1f, 2f);

        characterController.height = headHeight;
        characterController.center = new Vector3(0, headHeight / 2f, 0);

        transform.position = new Vector3(cameraRig.position.x, 0, cameraRig.position.z);

    }
}
