using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Diagnostics.Contracts;

[System.Serializable]
public class WinScreenConfig
{
    public float ScoreCounterTime = 3.0f;
}

public class GameScoreData
{
    public Sprite   ScoreIcon       = null;
    public int[]    PlayerScores    = new int[4];
    public int[]    PlayerTimes     = new int[4];
    public int      TeamTime        = 0;

    /// <summary>
    /// Use this if you want some text after your score. 
    /// e.g. "%" or " fishes!"
    /// </summary>
    public string   ScoreSuffix     = "";
}

public class WinScreenData
{
    public int PlayerScore;
    public int PlayerTime;
    public int TeamTime;
}

public class WinScreen : MonoBehaviour
{
    [SerializeField] private WinScreenConfig config;
    [SerializeField] private WinScreen_Player[] m_PlayerScreens;

    public float scoreProgress = 0.0f;
    private MinigameBase minigameRef = null;
    private int teamTime = 0;
    
    public void ShowWinScreen(MinigameBase minigame, GameScoreData scoreData, System.Action completionCallback) {
        minigameRef = minigame;
        for (int i = 0; i < m_PlayerScreens.Length; i++)
        {
            m_PlayerScreens[i].ShowWinScreen(scoreData.ScoreIcon, scoreData, i);
        }
        teamTime = scoreData.TeamTime;
        gameObject.SetActive(true);
        StartCoroutine(TempWaitAWhile(3.0f, completionCallback));
    }

    private IEnumerator TempWaitAWhile(float t, System.Action callback)
    {
        yield return new WaitForSeconds(t);
        yield return StartIncrementTimer();
        gameObject.SetActive(false);
        callback();
    }

    IEnumerator StartIncrementTimer()
    {
        minigameRef.OnTimerChange(true);
        for (int i = 0; i < teamTime; i++)
        {
            yield return new WaitForSeconds(0.05f);
            minigameRef.OnIncreaseTimer.Invoke(1.0f);

            if (i % 2 == 0)
            {
                //PlayerAudioManager.PlayGlobalOneShot(countAudioTick, 0.75f);
            }
        }
        minigameRef.OnTimerChange(false);

        minigameRef = null;
        teamTime = 0;
    }
}
