using UnityEngine;

public class ExitDoorManager : MonoBehaviour
{

    public DoorManager Door;
    public int leversNeeded = 4;
    private int leversPulled = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnLeverPulled(float isUp) {

        if (isUp == 1f)
        {
            leversPulled++;
        }
        else { 
        
            leversPulled--;

        }

        if (leversPulled == leversNeeded) {




                Door.OpenDoor();


        }

    
    }
}
