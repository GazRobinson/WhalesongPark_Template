using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BikeGame : MinigameBase
{

    [SerializeField] private PlayerBike[] players;
    [SerializeField] Transform finishLine;
    [Tooltip("How much score each player gets for 1st place, 2nd place and so on.")]
    [SerializeField] int[] playerRacePositionScore = new int[4];


    private void Awake()
    {
        MinigameLoaded.AddListener(InitialiseGame);
    }

    /// <summary>
    /// This function is called at the end of the game so that it knows what to display on the score screen.
    /// You give it information about what each players score was, how much time they earned individually, and also how much time they've earned together
    /// </summary>
    /// <returns>A class that contains all the necessary information to display the score page</returns>
    public override GameScoreData GetScoreData()
    {
        //Here's an example of how you might generate scores
        int teamTime = 0;
        GameScoreData gsd = new GameScoreData();
        for (int i = 0; i < 4; i++)
        {
            if (PlayerUtilities.GetPlayerState(i) == Player.PlayerState.ACTIVE)
            {
                var playerPositions = DeterminePlayerRacePositions();
                //each player is scored on how far they were from the finish
                gsd.PlayerScores[playerPositions[i]] = playerRacePositionScore[i];
                
                gsd.PlayerTimes[i] = gsd.PlayerScores[i] * 2;   //Each player gets two seconds per point scored
                teamTime += gsd.PlayerTimes[i];                 //Keep a running total of the total time scored by all players
            }
        }
        gsd.ScoreSuffix = " points";    //This lets you write something after the player's score.
        gsd.TeamTime = teamTime;
        return gsd;
    }

    /// <summary>
    /// How do you want to handle input from the four directional buttons?
    /// </summary>
    /// <param name="playerIndex">Which player (0-3) pressed the button</param>
    /// <param name="direction">Which direction(s) are they pressing</param>
    public override void OnDirectionalInput(int playerIndex, Vector2 direction)
    {

    }
    /// <summary>
    /// What should happen when the player presses the left hand button?
    /// </summary>
    /// <param name="playerIndex">Which player (0-3) pressed the button</param>
    public override void OnPrimaryFire(int playerIndex)
    {
        players[playerIndex].HandleButtonInput(0);
    }

    /// <summary>
    /// What should happen when the player presses the right hand button?
    /// </summary>
    /// <param name="playerIndex">Which player (0-3) pressed the button</param>
    public override void OnSecondaryFire(int playerIndex)
    {
        players[playerIndex].HandleButtonInput(1);
    }

    public override void TimeUp()
    {
        //Do you want to do something when the minigame timer runs out?
        //This is where you do that!
    }

    protected override void OnResetGame()
    {
        //Is there any cleanup you have to do when the game gets totally reset?
        //This might just be empty!

    }

    public void OnFinishReached()
    {
        OnGameComplete(true);
    }

    public void InitialiseGame()
    {
        //TODO: Reset players to start point
    }
    int[] DeterminePlayerRacePositions()
    {
        Dictionary<int, float> playerDistances = new Dictionary<int, float>();
        int[] playerPositions = new int[4];

        //get each player's distance from the finish line
        for (int i = 0; i < 4; i++) {
            playerDistances.Add(i,(finishLine.position.y - players[i].transform.position.y));
        }
        //create a list from the dictionary (TODO: could just make the dictionary a keyvalue list tbh)
        var distanceAndPlayerID = new List<KeyValuePair<int, float>>(playerDistances);
        //Sort the list based on distnace (shortest distance goes first, then second shortest etc.)
        distanceAndPlayerID.Sort((a, b) => a.Value.CompareTo(b.Value));
        
        //get each playerID from the list now in order of how far they were from the finish
        for (int i = 0; i < 4; i++){
            playerPositions[i] = distanceAndPlayerID[i].Key;
            print("Player " + (playerPositions[i] + 1) + " came in " + (i + 1) + "th place");
        }
        return playerPositions;
    }

}
