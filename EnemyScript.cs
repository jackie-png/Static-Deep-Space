using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Events;
public class EnemyScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Transform target;
    public Transform targetCamera;

    public NavMeshAgent agent;

    private List<List<Transform>> spawnPoints;
    public Transform floor1SpawnLocs;
    public Transform floor2SpawnLocs;
    public Transform floor3SpawnLocs;
    private List<Transform> currentSpawnLocs;


    public Transform floor1PatrolPoints;
    public Transform floor2PatrolPoints;
    public Transform floor3PatrolPoints;

    private List<Transform> currentPatrolPoints;


    public int currentFloor = 1;

    public bool isPatrolling = true;
    public bool isChasing = false;
    public bool hasCaughtPlayer = false;

    public bool isAiOff = true;

    private List<List<Transform>> patrolPoints;

    public Transform startingPos;

    public float moveSpeed = 1f;

    private int currentPatrolPoint = 0;

    public float playerSenseThreshold = 10f;

    public bool hasBeenHit;
    private Vector3 lastPosHit = Vector3.zero;

    [Header("Audio Cues")]
    public AudioSource patrolSound;
    public AudioSource chasingSound;
    public AudioSource caughtSound;

    public float patrolRadius = 20f;
    public float patrolSpeed = 3f;


    public float fadeSpeed = 2f;

    public float chaseAudioPulse = 10f;
    public float chaseFadeSpeed = 2f;

    public bool canPounce = true;
    private bool isPouncing = false;
    public Transform sight;
    public float pounceInterval = 10f;
    private float lastPounce = 0f;
    public AudioSource pounceSound;



    private float lastPulse = 0f;

    private int lastSpawnPoint = 0;

    private List<Transform> validSpawnPoints = new List<Transform>();

    public float respawnRadius = 20f;

    public UnityEvent<Jumpscare.JumpScareDirection> jumpscareEvent;

    public float fov;

    private LayerMask layermask;

    public float respawnTime = 1f;

    public bool isLastLevel = false;

    private void Awake()
    {

        patrolPoints = new List<List<Transform>>{

            new List<Transform>(), new List<Transform>(), new List<Transform>()
        
        };

        int floor1PatrolCount = floor1PatrolPoints.childCount;

        for (int i = 0; i < floor1PatrolCount; i++) {

            patrolPoints[0].Add(floor1PatrolPoints.GetChild(i));

        }

        int floor2PatrolCount = floor2PatrolPoints.childCount;

        for (int i = 0; i < floor2PatrolCount; i++)
        {

            patrolPoints[1].Add(floor2PatrolPoints.GetChild(i));

        }

        int floor3PatrolCount = floor3PatrolPoints.childCount;

        for (int i = 0; i < floor3PatrolCount; i++)
        {

            patrolPoints[2].Add(floor3PatrolPoints.GetChild(i));

        }

        spawnPoints = new List<List<Transform>>{

            new List<Transform>(), new List<Transform>(), new List<Transform>()

        };

        int floor1SpawnCount = floor1SpawnLocs.childCount;

        for (int i = 0; i < floor1SpawnCount; i++)
        {

            spawnPoints[0].Add(floor1SpawnLocs.GetChild(i));

        }

        int floor2SpawnCount = floor2SpawnLocs.childCount;

        for (int i = 0; i < floor2SpawnCount; i++)
        {

            spawnPoints[1].Add(floor2SpawnLocs.GetChild(i));

        }

        int floor3SpawnCount = floor3SpawnLocs.childCount;

        for (int i = 0; i < floor3SpawnCount; i++)
        {

            spawnPoints[2].Add(floor3SpawnLocs.GetChild(i));

        }

        Debug.Log("PatrolPoints 0");
        foreach (Transform t in patrolPoints[0])
        {
            Debug.Log(t.position);
        }


        Debug.Log("PatrolPoints 1");
        foreach (Transform t in patrolPoints[1])
        {
            Debug.Log(t.position);
        }

        Debug.Log("PatrolPoints 2");
        foreach (Transform t in patrolPoints[2])
        {
            Debug.Log(t.position);
        }

        currentPatrolPoints = patrolPoints[currentFloor];
        currentSpawnLocs = spawnPoints[currentFloor];

    }

    void Start()
    {
        agent.speed = patrolSpeed;

        layermask = LayerMask.GetMask("Controllers", "Player", "Laserable");

        agent.SetDestination(currentPatrolPoints[0].position);

    }

    // Update is called once per frame
    void Update()
    {
        //agent.SetDestination(target.position);

        float distanceFromPlayer = Vector3.Distance(transform.position, target.position);
        //Debug.Log($"Distance to Player: {distanceFromPlayer}");



        if (distanceFromPlayer <= playerSenseThreshold && !isAiOff) {

            isPatrolling = false;
            isChasing = true;
            //Debug.Log("Player Locked On");

        }

        if (distanceFromPlayer > patrolRadius && isPatrolling) // stop patrol audio when the player is far away
        {

            patrolSound.volume = Mathf.Lerp(patrolSound.volume, 0f, Time.deltaTime * fadeSpeed);

            if (patrolSound.volume < 0.01f && patrolSound.isPlaying)
            {
                patrolSound.Stop();

            }

        }

        else if (distanceFromPlayer <= patrolRadius && isPatrolling) // play patrol audio when player is within patrol audio's range
        {

            if (!patrolSound.isPlaying)
            {

                patrolSound.Play();

            }

            patrolSound.volume = Mathf.Lerp(patrolSound.volume, 1f, Time.deltaTime * fadeSpeed);

        }

        if ( (isChasing && !isAiOff ) || isLastLevel) { // if the agent has spotted the player it will start chasing

            patrolSound.volume = Mathf.Lerp(patrolSound.volume, 0f, Time.deltaTime * fadeSpeed);

            if (patrolSound.volume < 0.01f)
            {
                patrolSound.Stop();

            }


            //Debug.Log($"Distance to Player: {distanceFromPlayer}");

            if (!canPounce && Mathf.Abs(Time.time - lastPounce) >= pounceInterval) // check if it can pounce
            {
                canPounce = true;
            }

            if (canPounce && distanceFromPlayer <= playerSenseThreshold) // if it can pounce and the player is within its range
            {
                if (lineOfSightCheck() && !isPouncing) // check if it has a direct line of sight
                {
                    Debug.Log("is pouncing");
                    isPouncing = true;
                    StartCoroutine(StartPounce());
                }
            }

            if (!isPouncing) { // rubber banding, make the agent move faster when it is far away from the player while chasing, allow it to catch up to the player, only rubberband when not pouncing to prevent speed overwrite
                agent.SetDestination(target.position);
                if (distanceFromPlayer > playerSenseThreshold * 2 && !hasBeenHit)
                {

                    agent.speed = moveSpeed * 5;

                }
                else if (distanceFromPlayer > playerSenseThreshold / 2 && !hasBeenHit)
                {

                    agent.speed = moveSpeed * 3;

                }
                else if (distanceFromPlayer <= playerSenseThreshold / 2 && !hasBeenHit) {

                    agent.speed = moveSpeed * 1;

                }            
            }


            if (Mathf.Abs(Time.time - lastPulse) >= chaseAudioPulse) { // play chasing sound periodically


                chasingSound.Play();

                chasingSound.volume = Mathf.Lerp(chasingSound.volume, 0f, Time.deltaTime * chaseFadeSpeed);

                lastPulse = Time.time;

            }

          

        }



        if (isPatrolling && !isAiOff && !isLastLevel) { // go to next patrol point

            float distance = Vector3.Distance(transform.position, currentPatrolPoints[currentPatrolPoint].position);

            //Debug.Log(distance <=0.2f);
            //Debug.Log(currentPatrolPoint);

            //Debug.Log($"Patrol Point: {currentPatrolPoint} | Distance to Patrol Point: {distance}");

            if (distance <= 0.2f)
            {
                //Debug.Log($"Patrol Point {currentPatrolPoint} reached");

                if (currentPatrolPoint + 1 >= currentPatrolPoints.Count)
                {

                    currentPatrolPoint = 0;

                }
                else
                {

                    currentPatrolPoint++;

                }

                agent.SetDestination(currentPatrolPoints[currentPatrolPoint].position);
            }



        }

    }

    IEnumerator StartPounce() {



        agent.speed = moveSpeed * 20f;
        pounceSound.Play();
        yield return new WaitForSeconds(1f);
        agent.speed = moveSpeed;
        lastPounce= Time.time;
        isPouncing = false;
        canPounce = false;
        Debug.Log("Stopped pouncing");
    }


    void goToNextPoint()
    {
        agent.SetDestination(currentPatrolPoints[currentPatrolPoint].position);
    }
    private void FireJumpscare() {

        Vector3 enemyFacing = transform.forward;
        Vector3 targetFacing = targetCamera.forward;

        float dot = Vector3.Dot(targetFacing, enemyFacing);
        if (dot > 0f)
        { //agent caught player from the back
            //Debug.Log("Got player from the back");
            jumpscareEvent.Invoke(Jumpscare.JumpScareDirection.FromBack);

        }
        else { // agent caught player from the front

           // Debug.Log("Got player from the front");
            jumpscareEvent.Invoke(Jumpscare.JumpScareDirection.FromFront);

        }

    
    }

    

    private bool lineOfSightCheck() {


        Vector3 directionToPlayer = target.position - transform.position;

        float angleBetweenPlayer = Vector3.Angle(sight.forward, directionToPlayer);

        //Debug.Log(angleBetweenPlayer);

        if (angleBetweenPlayer > fov / 2) {

            return false;
        
        }


        bool inDirectLineOfsight = Physics.Raycast(sight.position, sight.forward, out RaycastHit rayCastInfo, playerSenseThreshold, layermask);
        //Debug.DrawRay(sight.position, directionToPlayer.normalized * playerSenseThreshold, Color.lightYellow, 100000f);
        Debug.DrawRay(sight.position, sight.forward * playerSenseThreshold, Color.lightGreen, 100000f);


        if (inDirectLineOfsight && (LayerMask.LayerToName(rayCastInfo.collider.gameObject.layer) == "Player" || LayerMask.LayerToName(rayCastInfo.collider.gameObject.layer) == "Controllers"))
        {
            Debug.Log("Can See Player directly");
            return true;

        }
        else {
            Debug.Log("cannot see player directly");
            return false;

        }            


    }

     


    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Player" || LayerMask.LayerToName(other.gameObject.layer) == "Controllers")
        {

            //Debug.Log($"Player caught: {LayerMask.LayerToName(other.gameObject.layer)}");
            if (!hasCaughtPlayer && !hasBeenHit) { 
            
                caughtSound.Play();    
                hasCaughtPlayer= true;

                FireJumpscare();
                Reset();
            }


        }

    }




    public void TriggerHit() {

        if (!hasBeenHit) {
            hasBeenHit = true;
            isChasing= false;
            isPouncing = false;
            agent.speed = 0f;
            agent.isStopped = true;
            agent.ResetPath();
            lastPosHit = target.position;
            StartCoroutine(TriggerRespawn());        
        
        }


    
    }

    IEnumerator TriggerRespawn()
    {

        yield return new WaitForSeconds(respawnTime);
        transform.position = new Vector3(0f,-10f,0f);
        respawn();
    }

    public void Reset()
    {
        Debug.Log($"Reseting enemy, startingPos is {startingPos.position}");
        hasBeenHit = false;
        hasCaughtPlayer = false;
        isPatrolling = true;
        isChasing = false;
        agent.speed = patrolSpeed;
        currentPatrolPoint = 0;
        transform.position = startingPos.position;
        agent.Warp(startingPos.position);
        agent.SetDestination(currentPatrolPoints[currentPatrolPoint].position);

    }

    void respawn() {



        //Debug.Log("Respawning");
        validSpawnPoints.Clear();
        float Maxdistance = 0f;
        string name = "";
        Vector3 farthestSpawnPoint = currentSpawnLocs[0].position;

        foreach (Transform spawnPoint in currentSpawnLocs)
        {

            float playerToSpoint = Vector3.Distance(lastPosHit, spawnPoint.position);

            Debug.Log($"Distance from player and {spawnPoint.name}: {playerToSpoint}");

            if (playerToSpoint > Maxdistance) {

                farthestSpawnPoint = spawnPoint.position;
                Maxdistance = playerToSpoint;
                name = spawnPoint.name;

            }


        }

        Debug.Log($"Farthest spawnpoint found at {farthestSpawnPoint} by {name}");



        agent.enabled = false;
        transform.position = farthestSpawnPoint;
        agent.enabled = true;
        agent.Warp(farthestSpawnPoint);
        StartCoroutine(ResumeAfterWarp());
    }

    public void FloorChange(int floor, Transform newStartingPos) { 
    
        currentFloor = floor;

        Debug.Log($"enemy changed to floor {currentFloor}");

        foreach (Transform t in patrolPoints[currentFloor])
        {
            Debug.Log(t.position);
        }

        currentPatrolPoints = patrolPoints[currentFloor];
        currentSpawnLocs = spawnPoints[currentFloor];
        startingPos = newStartingPos;

        Reset();
    
    }
    IEnumerator ResumeAfterWarp()
    {
        yield return null;
        agent.SetDestination(currentPatrolPoints[currentPatrolPoint].position);
        isPatrolling = true;
        isChasing = false;
        agent.speed = patrolSpeed;
        hasBeenHit = false;
    }
}
