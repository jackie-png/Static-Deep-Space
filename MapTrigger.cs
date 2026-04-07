using UnityEngine;
using UnityEngine.Events;

public class MapTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private bool hasEntered = false;
    public UnityEvent<string> gameManagerMapEvent;

    private void OnTriggerEnter(Collider Other)
    {

        if (LayerMask.LayerToName(Other.gameObject.layer) == "Player" && !hasEntered){
            Debug.Log($"Player has entered {gameObject.name}");

            hasEntered = true;
            gameManagerMapEvent.Invoke(gameObject.name);


        }


    }

}
