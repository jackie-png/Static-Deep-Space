using UnityEngine;

public class WinAreaManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public DoorManager door;
    public GameObject winAreaObjects;
    public GameManager gameManager;
    public AudioSource winSound;
    public bool hasWon = false;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Player" && !hasWon)
        {
            hasWon = true;
            door.Reset();
            winAreaObjects.SetActive(true);
            winSound.Play();
            gameManager.WinReset();
        }
    }

}
