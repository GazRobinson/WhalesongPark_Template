using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NetMinigame : MinigameBase
{
    public GameObject[] warningSymbols;
    public int shipZone = 3;
    public int[] fishLeft;

    [SerializeField]
    FishCounterAnimation winStateAnimation;

    public int[] scoresPerFish = { 7, 5, 3, 2, 1};
    public int teamTimeWon;
    public int[] playerTimeWon = new int[4] { 0, 0, 0, 0 };

    [SerializeField] FollowTarget[] fishControllers;

    // boat movement speed is declared here rather than in the boat movement script
    // this is so that each boat has the same movement speed
    [SerializeField] [Range(0.1f,1f)]public float boatMovementSpeed = 0.1f;

    protected override void OnUpdate()
    {
        
        if (m_IsMinigameActive) 
        {
            for (int i = 0; i < 4; i++)
            {
                if (shipZone == i - 1 || (i == 0 && shipZone == 3))
                    warningSymbols[i].SetActive(true);
                else
                    warningSymbols[i].SetActive(false);
            }
        }
    }

    protected override void OnResetGame()
    {

    }

    public override void LoadMiniGame()
    {
        base.LoadMiniGame();

        fishLeft = new int[4] { 32, 32, 32, 32 };
        shipZone = 3;

        teamTimeWon = 0;
        playerTimeWon = new int[4] { 0,0,0,0};

        //m_WinScreen.ResetCount();
    }


    public override void TimeUp()
    {
        OnGameComplete(true);
    }

    public override void OnDirectionalInput(int playerIndex, Vector2 direction)
    {
        fishControllers[playerIndex].ProvideInput(direction);
    }

    public override void OnPrimaryFire(int playerIndex)
    {
    }

    public override void OnSecondaryFire(int playerIndex)
    {
    }

    public override GameScoreData GetScoreData()
    {
        int teamTime = 0;
        GameScoreData gsd = new GameScoreData();
        for (int i = 0; i < 4; i++)
        {
            if (PlayerUtilities.GetPlayerState(i) == Player.PlayerState.ACTIVE)
            {
                gsd.PlayerScores[i] = Mathf.CeilToInt((float)fishLeft[i] / 32.0f * 100.0f);
                gsd.PlayerTimes[i] = Mathf.CeilToInt(((float)gsd.PlayerScores[i] / 100.0f) * 7.0f);
                teamTime += gsd.PlayerTimes[i];
            }
        }
        gsd.ScoreSuffix = "%";
        gsd.TeamTime = teamTime;
        return gsd;
    }
}
