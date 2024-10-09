using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerStateEnum
{
    NOT_PLAYING,
    IN_LOBBY,
    QUEUED,
    PLAYING,
    FINISHED,
}

public enum ZonesEnum
{
    Britian,
    North_Africa,
    South_Africa,
    Oceania,
    Antartic,
    Finished,
}

public enum TutorialsEnum
{
    None = 0,
    RaiseSails = 1,
    LowerSails = 2,
    Left_Right = 4,
    Dolphin = 8,
}


public class PlayerRig : MonoBehaviour
{
    public PlayerStateEnum _MyPlayerState;

    public ZonesEnum Zone;

    public int Score;

    [SerializeField]
    public int PlayerIndex;

    TutorialsEnum TutorialsDoneFlag;
    TutorialsEnum CurrentTutorial;

    bool EventTimerActive;

    float EventTimer;

    int TimedEventIndex = 0;

    float[] EventTimes = new float[]
    {
        3.5f,
        3.5f,
        3.5f,
        3.5f,
    };

    Action[] TimedEvents;

    void RaiseSailsTut() { StartTutorial(TutorialsEnum.RaiseSails); }
    void LowerSailsTut() { StartTutorial(TutorialsEnum.LowerSails); }
    void LeftRightTut() { StartTutorial(TutorialsEnum.Left_Right); }
    void StartObstacleSpawn() { StartTutorial(TutorialsEnum.Left_Right); }
    void StartDolphinSpawn() {  }

    [SerializeField]
    Image[] TutorialImages;

    static Dictionary<TutorialsEnum, int> FlagToIndex = new Dictionary<TutorialsEnum, int>()
    {
        { TutorialsEnum.RaiseSails, 1},
        { TutorialsEnum.LowerSails, 2},
        { TutorialsEnum.Left_Right, 3},
        { TutorialsEnum.Dolphin, 4},
    };

    // PLAYER
    public enum inputTypes
    {
        primaryFire,
        secondaryFire,
        Directional
    }
    public float playerSpeed;
    [SerializeField] private float playerAcceleration;
    [SerializeField] private float playerSlideSpeed = 5;

    // OBSTACLES
    //The distance obstacles will spawn from the player, in units, through local Z-axis.
    [SerializeField] private int ObstacleSpawnDistance = 30;
    [SerializeField] private int landDistanceOffset = 25;
    GameObject obstacles;
    [SerializeField] private GameObject prefab1x1;
    [SerializeField] private GameObject prefabLand;
    float timeSinceLastSpawn = 0f;
    float timeSinceLastLand = 0f;
    [SerializeField] private float spawnRate;
    public LayerMask mask;

    void Start()
    {
        obstacles = transform.Find("Obstacles").gameObject;

        TimedEvents = new Action[]
        {
            RaiseSailsTut,
            LowerSailsTut,
            LeftRightTut,
            StartObstacleSpawn,
            StartDolphinSpawn,
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (EventTimerActive)
        {
            EventTimer -= Time.deltaTime;

            if (EventTimer < 0.0f)
            {
                TimedEventTimeOut();
            }
        }

        playerSpeed += playerAcceleration * Time.deltaTime;
        playerSpeed = Mathf.Clamp(playerSpeed, 5, Mathf.Infinity);
        ProcessObjectSpawning();
    }

    public void HandleInput(inputTypes type, Vector2 direction = new Vector2())
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        switch (type)
        {
            case inputTypes.primaryFire:
                ProcessMovement(new Vector3(1, 0, 0));
                break;
            case inputTypes.secondaryFire:
                ProcessMovement(new Vector3(-1, 0, 0));
                break;
            case inputTypes.Directional:
                ProcessMovement(new Vector3(-direction.x, 0, 0));
                break;
        }

    }

    private void ProcessMovement(Vector3 direction)
    {
        obstacles.transform.localPosition = obstacles.transform.localPosition + direction * Time.deltaTime * playerSlideSpeed * playerSpeed;
    }

    private void ProcessObjectSpawning()
    {
        if (timeSinceLastSpawn >= 1/spawnRate / playerSpeed)
        {
            //generate random offset
            int offset = UnityEngine.Random.Range(-15, 15);
            offset *= 2;
            //get base position
            Vector3 spawnPos = transform.TransformPoint(new Vector3(offset, 0, ObstacleSpawnDistance));

            if (timeSinceLastLand >= 20/spawnRate / playerSpeed)
            {
                SpawnLand(spawnPos);
            }
            else SpawnSingleObject(spawnPos);

            timeSinceLastSpawn = 0;
        }
        timeSinceLastSpawn += Time.deltaTime;
        timeSinceLastLand += Time.deltaTime;
    }
    private void SpawnSingleObject(Vector3 spawnPos)
    {
        Collider[] overlaps = Physics.OverlapSphere(spawnPos, 0.1f, mask, QueryTriggerInteraction.UseGlobal);
        foreach (Collider collision in overlaps)
        {
            if (collision.gameObject != null) return;
        }
        Instantiate(prefab1x1, spawnPos, transform.rotation, obstacles.transform);
    }
    private void SpawnLand(Vector3 spawnPos)
    {
        timeSinceLastLand = 0;
        //Convert to local space and apply landmass offset
        Vector3 leftPos = obstacles.transform.InverseTransformPoint(spawnPos) - new Vector3(-27.5f, 0, -landDistanceOffset);
        Vector3 rightPos = obstacles.transform.InverseTransformPoint(spawnPos) - new Vector3(27.5f, 0, -landDistanceOffset);
        //Convert back to world space
        leftPos = obstacles.transform.TransformPoint(leftPos);
        rightPos = obstacles.transform.TransformPoint(rightPos);

        Instantiate(prefabLand, leftPos, transform.rotation, obstacles.transform);
        Instantiate(prefabLand, rightPos, transform.rotation, obstacles.transform);
    }

    public void ResetPlayer()
    {
        EventTimerActive = false;
        gameObject.SetActive(false);
        _MyPlayerState = PlayerStateEnum.NOT_PLAYING;
    }

    public void FinishedTutorial(TutorialsEnum TutorialFinished)
    {
        if (!TutorialsDoneFlag.HasFlag(TutorialFinished))
        {
            TutorialImages[FlagToIndex[TutorialFinished]].gameObject.SetActive(true);
        }

        TutorialsDoneFlag = TutorialsDoneFlag | TutorialFinished;
    }

    public void StartTutorial(TutorialsEnum NewTutorial)
    {
        FinishedTutorial(CurrentTutorial);

        TutorialImages[FlagToIndex[NewTutorial]].gameObject.SetActive(true);
        CurrentTutorial = NewTutorial;
    }

    public void FinalActivatePlayer(ZonesEnum StartZone)
    {
        Debug.Log("MINE: Activating player " + PlayerIndex);
        gameObject.SetActive(true);
        Zone = StartZone;
        _MyPlayerState = PlayerStateEnum.PLAYING;
        CurrentTutorial = TutorialsEnum.None;
        TutorialsDoneFlag = TutorialsEnum.None;
        Score = 0;
        EventTimerActive = true;
        TimedEventIndex = -1;

        TimedEventTimeOut();
    }

    public void DeActivatePlayer()
    {
        gameObject.SetActive(false);
        _MyPlayerState = PlayerStateEnum.FINISHED;
    }

    public void TimedEventTimeOut()
    {
        TimedEventIndex++;

        if (TimedEventIndex < TimedEvents.Length)
        {
            TimedEvents[TimedEventIndex]();
            EventTimer = EventTimes[TimedEventIndex];
        }
        else
        {
            EventTimerActive = false;
        }
    }    

}
