using UnityEngine;
using UnityEngine.Events;

public class TpArea : MonoBehaviour
{

    public UnityEvent<GameManager.TpAreas> tpArea;

    public GameManager.TpAreas area;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnTriggerEnter(Collider other)
    {

        if (LayerMask.LayerToName(other.gameObject.layer) == "Controllers" || LayerMask.LayerToName(other.gameObject.layer) == "Player")
        {
            Debug.Log("player entered");
            tpArea.Invoke(area);

        }
    }
}
