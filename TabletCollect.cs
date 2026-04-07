using UnityEngine;
using UnityEngine.Events;
using System.Collections;
public class TabletCollect : MonoBehaviour
{

    public UnityEvent gameManagerCollectEvent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider Other)
    {
        Debug.Log(LayerMask.LayerToName(Other.gameObject.layer));
        if (LayerMask.LayerToName(Other.gameObject.layer) == "Controllers")
        {

            gameManagerCollectEvent.Invoke();
            gameObject.SetActive(false);


        }


    }
}
