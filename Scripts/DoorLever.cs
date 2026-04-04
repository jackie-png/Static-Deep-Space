using UnityEngine;
using UnityEngine.Events;

public class DoorLever : Lever
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Lever[] requiredLevers;

    public UnityEvent checkpointEvent;

    public DoorManager nextLevelDoor;


    protected override void SwitchLever() {

        Debug.Log("Child Version of Switch Lever");

        bool requiredFlipped = true;

        foreach (Lever lever in requiredLevers) {
            if (!lever.IsSwitched) {

                requiredFlipped = false;
                break;
            
            }
        }

        if (requiredFlipped) { 
        
            base.SwitchLever();
            checkpointEvent.Invoke();
            nextLevelDoor.OpenDoor();
            

        }
    
    }

}
