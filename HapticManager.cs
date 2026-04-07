using UnityEngine;
using System.Collections;
public class HapticManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public const float defaultFrequency = 1f;
    public const float defaultAmplitude = 0.1f;
    public const float defaultDuration = 0.1f;
    
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void TriggerHaptic(OVRInput.Controller controller, float frequency = defaultFrequency, float amplitude = defaultAmplitude, float duration = defaultDuration) {

        StartCoroutine(SendHaptic(frequency, amplitude, duration, controller));
    
    }

    IEnumerator SendHaptic(float frequency, float amplitude, float duration, OVRInput.Controller controller)
    {

        OVRInput.SetControllerVibration(frequency, amplitude, controller);
        yield return new WaitForSeconds(duration);

        OVRInput.SetControllerVibration(0f, 0f, controller);


    }
}
