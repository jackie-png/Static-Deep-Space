using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject player;
    public enum Floor { floor1, floor2, floor3, floor4};
    public Floor currentFloor = Floor.floor1;
    public bool isLastLevel = false;

    public GameObject agent;
    private EnemyScript agentScript;
    public Transform[] agentStartingSpawnpoints;
    public int currentStartingSpawn = 1;

    public Transform endArea;

    public Transform[] checkpoints;

    public Transform thirdFloor;

    public Transform mainMenu;

    public int currentCheckpoint = 0;

    public DoorManager[] L1Doors;
    public DoorManager[] L2Doors;
    public DoorManager[] L3Doors;

    public DeadAreaManager deadAreaManager;

    public AudioSource envSound;

    public Lever[] L1PowerLevers;
    public Lever[] L2PowerLevers;
    public Lever[] L3PowerLevers;

    public TwoWayTeleporter floor3Teleporter;

    public bool hasReachedFloor2 = false;

    public Material emittingLights;

    public AudioSource powerOn;
    public AudioSource pickUp;


    public enum TpAreas { StartMenu, LastCheckPoint, ThirdFloor}

    public GameObject[] mapTriggers;
    public GameObject[] mapLabels;

    private Dictionary<string, GameObject> mapTriggerLabels;

    private int _score = 0;
    public int score
    {
        get => _score;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        mapTriggerLabels = new Dictionary<string, GameObject>();
        agent.SetActive(false);
        emittingLights.EnableKeyword("_EMISSION");
        emittingLights.SetColor("_EmissionColor", Color.black);

        for (int i = 0; i < mapTriggers.Length; i++)
        {

            mapTriggerLabels.TryAdd(mapTriggers[i].gameObject.name, mapLabels[i]);

        }

        Debug.Log("Map labels dictiopnary");
        Debug.Log(mapTriggerLabels.Count);
        Debug.Log(mapTriggerLabels.Values);
        Debug.Log(mapTriggerLabels.Keys);

    }

    void Start()
    {

        agentScript = agent.GetComponent<EnemyScript>();
        agentScript.startingPos = agentStartingSpawnpoints[currentStartingSpawn];
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TurnOnLabel(String name)
    {

        mapTriggerLabels.TryGetValue(name, out GameObject label);
        label.SetActive(true);


    }

    public void IncreaseScore() {

        pickUp.Play();
        _score++;
    
    }


    public void TpPlayerToDeadArea() { 
    
        player.transform.position = endArea.position;

    }

    public void TpPlayerToArea(TpAreas area)
    {

        CharacterController cc = player.GetComponent<CharacterController>();

        Debug.Log("tping player");

        switch ((int)currentFloor)
        {

            case 0:
                ResetAreaL1();
                break;

            case 1:
                ResetAreaL2();
                break;

            case 2:
                ResetAreaL3();
                break;

            case 3:
                ResetAreaL4();
                break;

        }
        cc.enabled = false;
        switch (area)
        {
            case TpAreas.StartMenu:
                Debug.Log("to start menu");
                player.transform.position = mainMenu.position;
                break;
            case TpAreas.LastCheckPoint:
                player.transform.position = checkpoints[currentCheckpoint].position;
                break;
            case TpAreas.ThirdFloor:
                Debug.Log("to third floor");
                player.transform.position = thirdFloor.position;
                break;

        }

        cc.enabled = true;
        ResetDeadArea();
        agentScript.Reset();
        


    }

    public void IncreaseCheckpoint() {


        currentCheckpoint++;

        if (currentCheckpoint == 2) {

            floor3Teleporter.TPon();
        
        }

        if (currentCheckpoint == 3) { 
        
            SetLastLevel();
        
        }
       

    }

    public void ResetDeadArea() {

        Debug.Log("Reseting dead area");
        deadAreaManager.Reset();
    
    }

    public void PlayEnvironmentSound() { 
    
        envSound.Play();
    }

    public void WinReset() { 
    
        envSound.Stop();
        agent.SetActive(false);
    
    }

    //only fire increasefloor when player enters a trigger to change the current floor
    public void IncreaseFloor(Floor floorNum) {

        if (floorNum == Floor.floor1)
        {
            if (isLastLevel)
            {
                currentFloor = floorNum;
                agentScript.FloorChange((int)Floor.floor1, agentStartingSpawnpoints[(int)currentFloor]);
            }
            return;
        }

        if (floorNum != currentFloor)
        {
            currentFloor = floorNum;

            if (currentFloor == Floor.floor2 && !hasReachedFloor2) {

                hasReachedFloor2= true;
                agent.SetActive(true);
            
            }

            agentScript.FloorChange((int)currentFloor, agentStartingSpawnpoints[(int)currentFloor]);
        }

    }


    public void ResetAreaL1() {

        foreach (Lever lever in L1PowerLevers) { 
        
            lever.Reset();
        
        }

        foreach (DoorManager door in L1Doors)
        {
            door.Reset();
        }


    }

    public void ResetAreaL2() {

        foreach (Lever lever in L2PowerLevers) { 
        
            lever.Reset();
        
        }

        foreach (DoorManager door in L2Doors)
        {
            door.Reset();
        }


    }

    public void ResetAreaL3() {

        foreach (Lever lever in L3PowerLevers)
        {

            lever.Reset();

        }

        foreach (DoorManager door in L3Doors)
        {
            door.Reset();
        }

    }

    public void SetLastLevel() { 
    
        isLastLevel= true;
        currentFloor = Floor.floor4;
        emittingLights.SetColor("_EmissionColor", Color.white);
        agentScript.isLastLevel = true;
        powerOn.Play();
    
    }

    public void ResetAreaL4() {

        L3Doors[L3Doors.Length-1].Reset();
    
    }
}
