using UnityEngine;
using UnityEngine.Events; 

public class FloorTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameManager.Floor floor;
    public UnityEvent<GameManager.Floor> FloorEvent;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Controllers" || LayerMask.LayerToName(other.gameObject.layer) == "Player") {
            Debug.Log($"Changing floor to {floor}");
            FloorEvent.Invoke(floor);        
        }

    }
}
