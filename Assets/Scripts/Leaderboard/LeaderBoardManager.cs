using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class LeaderBoardManager : MonoBehaviour
{
    public List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>();

    [SerializeField]
    LeaderboardDisplay[] leaderboards;

    private void Awake()
    {
        for (int i = 0; i < leaderboards.Length; i++) 
        {
            leaderboards[i].leaderboardManager = this;
        }
    }

    private void Start()
    {
        //leaderboardEntries.Add(new LeaderboardEntry("PDF", 123));
        //for (int i = 0; i < 50; i++)
        //{
        //    string randomName = "TXT";
        //    int randomScore = Random.Range(50, 1000);

        //       leaderboardEntries.Add(new LeaderboardEntry(randomName, randomScore));
        //}
        //SaveHighscores();

        //leaderboardEntries.Clear();
        //leaderboardEntries.Add(new LeaderboardEntry("PDF", 123));
        //leaderboardEntries.Add(new LeaderboardEntry("PNG", 567));
        //leaderboardEntries.Add(new LeaderboardEntry("JPG", 118));
        //leaderboardEntries.Add(new LeaderboardEntry("CPP", 512));
        //SaveHighscores();

        LoadHighScores();

        ShowScores("PDF");
    }

    public void AddScore(string teamName, int score) 
    {
        LeaderboardEntry newEntry = new LeaderboardEntry(teamName, score);
        leaderboardEntries.Add(newEntry);
        OrderByScore();

        SaveHighscores();
    }

    public void ShowScores(string teamName) 
    {
        for (int i = 0; i < leaderboards.Length; i++) 
        {
            leaderboards[i].UpdateLeaderboardDisplay(teamName);
        }
    }

    public void OrderByScore() 
    {
        leaderboardEntries = new List<LeaderboardEntry>(leaderboardEntries.OrderByDescending(leaderboardEntries => leaderboardEntries.score));
        for (int i = 0; i < leaderboardEntries.Count; i++) 
        {
            Debug.Log("Team: " + leaderboardEntries[i].teamName + ", score: " + leaderboardEntries[i].score);
        }
    }

    public int GetIndexOfTeam(string teamName) 
    {
        for (int i = 0; i < leaderboardEntries.Count; i++) 
        {
            if (leaderboardEntries[i].teamName == teamName) 
            {
                return i;
            }
        }
        return -1;
    }

    public void SaveHighscores() 
    {
        LeaderboardEntry[] leaderBoardToSave = leaderboardEntries.ToArray();
        LeaderboardSaveWrapper saveWrapper = new LeaderboardSaveWrapper();
        saveWrapper.leaderboardEntries = leaderBoardToSave;
        string saveJSON = JsonUtility.ToJson(saveWrapper);

        File.WriteAllText("highscores.json", saveJSON);

        Debug.Log("Highscores saved");
    }

    public void LoadHighScores() 
    {
        LeaderboardSaveWrapper saveWrapper = JsonUtility.FromJson<LeaderboardSaveWrapper>(File.ReadAllText("highscores.json"));
        leaderboardEntries = new List<LeaderboardEntry>(saveWrapper.leaderboardEntries);

        OrderByScore();

        Debug.Log("Imported leaderboard count: " + leaderboardEntries.Count);
    }
}

[System.Serializable]
public struct LeaderboardEntry 
{
    public string teamName;
    public int score;

    public LeaderboardEntry(string teamName, int score) 
    {
        this.teamName = teamName;
        this.score = score;
    }
}

[System.Serializable]
public struct LeaderboardSaveWrapper
{
    public LeaderboardEntry[] leaderboardEntries;
}