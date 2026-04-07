using UnityEngine;
using System.Collections;

public class StartArea : MonoBehaviour
{

    public DoorManager door;
    public GameManager gameManager;
    public AudioSource startSound;
    public GameObject light;

    public Material emissionMat;
    public Material screenMat;
    public Color screenStartColour;

    private bool gameStarted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        emissionMat.EnableKeyword("_EMISSION");
        screenMat.EnableKeyword("_EMISSION");
        screenMat.SetColor("_EmissionColor", screenStartColour);
    }

    // Update is called once per frame
    void Update()
    {

        if (door.isOpened && !gameStarted) {

            gameStarted = true;
            light.SetActive(false);
            Debug.Log("Start Door Opened");
            emissionMat.SetColor("_EmissionColor", Color.black);
            screenMat.SetColor("_EmissionColor", Color.black);
            startSound.Play();
            StartCoroutine(PlayStartSounds());
        
        }

    }

    IEnumerator PlayStartSounds() {

        
        yield return new WaitForSeconds(8.5f);
        gameManager.PlayEnvironmentSound();

    }


}
