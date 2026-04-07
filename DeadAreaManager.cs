using UnityEngine;

public class DeadAreaManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public DoorManager[] doors;
    
    void Start()
    {
        Debug.Log(doors.Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset()
    {
        Debug.Log($"Reset called on: {gameObject.name}, door count: {doors.Length}", this);
        foreach (DoorManager door in doors) {

            Debug.Log(door.name + "resetting");
            door.Reset();
        
        }
    }
}
