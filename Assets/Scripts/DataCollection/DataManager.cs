using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

public static class DataManager
{
    public static List<DataEvent> eventLog = new List<DataEvent>();
    private static System.Random random = new System.Random();

    public static List<ErrorEvent> errorLog = new List<ErrorEvent>();

    public static void AddEvent(DataEvent newEvent)
    {
        eventLog.Add(newEvent);
    }

    public static void AddError(ErrorEvent errorEvent) 
    {
        errorLog.Add(errorEvent);
    }

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static void OutputEventsToFile() 
    {
        Debug.Log("Outputting event log with length: " + eventLog.Count);

        if (!Directory.Exists(Application.dataPath + "/CSV")) 
        {
            Directory.CreateDirectory(Application.dataPath + "/CSV");
        }

        string dateTimeString = DateTime.Now.ToString();
        dateTimeString = dateTimeString.Replace("/", "-");
        dateTimeString = dateTimeString.Replace(":", "-");
        string filePath = Application.dataPath + "/CSV/" + "EventLog" + dateTimeString + ".csv";
        StreamWriter writer = new StreamWriter(filePath);

        for (int i = 0; i < eventLog.Count; i++) 
        {
            PlayerJoinedEvent playerJoinedEvent = eventLog[i] as PlayerJoinedEvent;
            if (playerJoinedEvent != null)
            {
                writer.WriteLine(playerJoinedEvent.GetFormattedOutput());
                continue;
            }

            PlayerLeftEvent playerLeftEvent = eventLog[i] as PlayerLeftEvent;
            if (playerLeftEvent != null)
            {
                writer.WriteLine(playerLeftEvent.GetFormattedOutput());
                continue;
            }

            MinigameStarted minigameStartedEvent = eventLog[i] as MinigameStarted;
            if (minigameStartedEvent != null)
            {
                writer.WriteLine(minigameStartedEvent.GetFormattedOutput());
                continue;
            }

            MinigameCompleted minigameCompletedEvent = eventLog[i] as MinigameCompleted;
            if (minigameCompletedEvent != null)
            {
                writer.WriteLine(minigameCompletedEvent.GetFormattedOutput());
                continue;
            }
        }

        writer.Flush();
        writer.Close();
    }

    public static void OutputErrorLog()
    {
        Debug.Log("Outputting error log with length: " + eventLog.Count);

        if (!Directory.Exists(Application.dataPath + "/Errors"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Errors");
        }

        if (errorLog.Count > 0)
        {
            string dateTimeString = DateTime.Now.ToString();
            dateTimeString = dateTimeString.Replace("/", "-");
            dateTimeString = dateTimeString.Replace(":", "-");
            string filePath = Application.dataPath + "/Errors/" + "ErrorLog" + dateTimeString + ".csv";
            StreamWriter writer = new StreamWriter(filePath);

            for (int i = 0; i < errorLog.Count; i++)
            {
                writer.WriteLine(errorLog[i].GetFormattedOutput());
            }

            writer.Flush();
            writer.Close();
        }
    }
}

public class DataEvent 
{
    public DateTime timestamp;
}

public class ErrorEvent : DataEvent 
{
    string errorType;
    string errorString;

    public ErrorEvent(string errorType, string errorString) 
    {
        this.errorType = errorType;
        this.errorString = errorString;
    }

    public string GetFormattedOutput()
    {
        return "ERROR" + ", " + timestamp.ToString() + ", Error Type: " + errorType + ", " + "Body: " + errorString;
    }
}

public class PlayerJoinedEvent : DataEvent
{
    string playerID;
    string activeMinigame;

    public PlayerJoinedEvent(string playerID, string activeMinigame)
    {
        this.timestamp = DateTime.Now;
        this.playerID = playerID;
        this.activeMinigame = activeMinigame;
    }

    public string GetFormattedOutput() 
    {
        return "PLAYER_JOINED" + "," + timestamp.ToString() + "," + playerID + "," + activeMinigame;
    }
}

public class PlayerLeftEvent : DataEvent
{
    string playerID;
    string activeMinigame;
    int minigameTimeLeft;

    public PlayerLeftEvent(string playerID, string activeMinigame, int minigameTimeLeft)
    {
        this.timestamp = DateTime.Now;
        this.playerID = playerID;
        this.activeMinigame = activeMinigame;
        this.minigameTimeLeft = minigameTimeLeft;
    }

    public string GetFormattedOutput()
    {
        return "PLAYER_LEFT" + "," + timestamp.ToString() + "," + playerID + "," + activeMinigame + "," + minigameTimeLeft.ToString();
    }
}

public class MinigameStarted : DataEvent 
{
    string minigame;
    string minigameID;
    int iteration;
    int teamSize;

    public MinigameStarted(string minigame, string minigameID, int iteration, int teamSize)
    {
        this.timestamp = DateTime.Now;
        this.minigame = minigame;
        this.minigameID = minigameID;
        this.iteration = iteration;
        this.teamSize = teamSize;
    }

    public string GetFormattedOutput()
    {
        return "MINIGAME_START" + "," + timestamp.ToString() + "," + minigame + "," + minigameID + "," + iteration.ToString() + "," + teamSize.ToString();
    }
}

public class MinigameCompleted : DataEvent 
{
    string minigame;
    string minigameID;
    int iteration;
    int teamSize;
    bool success;

    public MinigameCompleted(string minigame, string minigameID, int iteration, int teamSize, bool success)
    {
        this.timestamp = DateTime.Now;
        this.minigame = minigame;
        this.minigameID = minigameID;
        this.iteration = iteration;
        this.teamSize = teamSize;
        this.success = success;
    }

    public string GetFormattedOutput()
    {
        return "MINIGAME_COMPLETE" + "," + timestamp.ToString() + "," + minigame + "," + minigameID + "," + iteration.ToString() + "," + teamSize.ToString() + "," + success.ToString();
    }
}

public class PlayerCompletedMinigame : DataEvent
{
    string minigame;
    string minigameID;
    int timeEarnedSingle;
    bool success;

    public PlayerCompletedMinigame(string minigame, string minigameID, int timeEarnedSingle, bool success)
    {
        this.timestamp = DateTime.Now;
        this.minigame = minigame;
        this.minigameID = minigameID;
        this.timeEarnedSingle = timeEarnedSingle;
        this.success = success;
    }

    public string GetFormattedOutput()
    {
        return "PLAYER_COMPLETE" + "," + timestamp.ToString() + "," + minigame + "," + minigameID + "," + timeEarnedSingle.ToString() + "," + success.ToString();
    }
}
