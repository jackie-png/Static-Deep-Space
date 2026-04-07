using UnityEngine;
using System.Collections;
public class TwoWayTeleporter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Transform teleportExit;
    public TwoWayTeleporter teleporterExitScript;
    public bool isOn = false;
    public bool canTeleport = true;
    public Transform player;
    public Material emittingMaterial;
    public Color emittingColour;
    public AudioSource tpSound;
    public ParticleSystem particles;

    void Start()
    {
        emittingMaterial.EnableKeyword("_EMISSION");
        emittingMaterial.SetColor("_EmissionColor", Color.black);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Player" && isOn && canTeleport)
        {
            Debug.Log($"Player Entered teleporter {gameObject.name}");
            player.position = teleportExit.position;
            tpSound.Play();
            StartCoroutine(TeleporterCoolDown());
        }
    }

    public void TPon() { 
    
        isOn= true;
        teleporterExitScript.isOn= true;
        particles.Play();
        teleporterExitScript.particles.Play();
        emittingMaterial.SetColor("_EmissionColor", emittingColour);

    }

    

    public IEnumerator TeleporterCoolDown() {

        yield return new WaitForSeconds(5f);
        canTeleport = false;
        teleporterExitScript.canTeleport= true;

    }


}
