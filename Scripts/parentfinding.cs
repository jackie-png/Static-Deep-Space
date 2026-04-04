using UnityEngine;

public class parentfinding : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent != null) {
            Debug.Log(transform.parent);        
        }

    }
}
