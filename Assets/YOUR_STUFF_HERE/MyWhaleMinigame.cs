using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using UnityEngine;

public enum GameStateEnum
{
    NOT_PLAYING,
    IN_LOBBY,
    PLAYING,
};

public class MyWhaleMinigame : MinigameBase
{
    public GameStateEnum _GameState;

    [SerializeField]
    PlayerManager PlayerManagerRef;

    [SerializeField]
    GameManager GameManagerRef;

    public List<PlayerRig> QueuedPlayers;

    [SerializeField] private PlayerRig[] players;
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
                gsd.PlayerScores[i] = 1;                        //Each player scored one point
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
        if (direction.magnitude != 0)
        {
            //ActivatePlayer(playerIndex);
            players[playerIndex].HandleInput(PlayerRig.inputTypes.Directional, direction);
        }
    }
    /// <summary>
    /// What should happen when the player presses the left hand button?
    /// </summary>
    /// <param name="playerIndex">Which player (0-3) pressed the button</param>
    public override void OnPrimaryFire(int playerIndex)
    {
        ActivatePlayer(playerIndex);
        players[playerIndex].HandleInput(PlayerRig.inputTypes.primaryFire);
    }

    /// <summary>
    /// What should happen when the player presses the right hand button?
    /// </summary>
    /// <param name="playerIndex">Which player (0-3) pressed the button</param>
    public override void OnSecondaryFire(int playerIndex)
    {
        //ActivatePlayer(playerIndex);
        players[playerIndex].HandleInput(PlayerRig.inputTypes.secondaryFire);
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

        _GameState = GameStateEnum.NOT_PLAYING;
        QueuedPlayers.Clear();

        for (int i = 0; i < players.Length; i++)
        {
            players[i].ResetPlayer();
        }
    }

    public override void LoadMiniGame()
    {
        base.LoadMiniGame();
        //LOAD OUR START GUI HERE.
        GameManagerRef.AlwaysPassInputToGame = true;
        QueuedPlayers = new List<PlayerRig>();
        OnResetGame();

        Player[] Players = PlayerManagerRef.players;

        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i].playerState == Player.PlayerState.ACTIVE)
            {
                ActivatePlayer(i);
            }
        }
    }

    public override void UnloadMinigame()
    {
        GameManagerRef.AlwaysPassInputToGame = false;
    }

    private void ActivatePlayer(int playerIndex)
    {
        Debug.Log("MINE: Game State Was: " + _GameState);
        Debug.Log("MINE: Player " + playerIndex + " Was in state: " + players[playerIndex]._MyPlayerState);

        if (_GameState == GameStateEnum.NOT_PLAYING)
        {
            _GameState = GameStateEnum.IN_LOBBY;
        }
        else if (players[playerIndex]._MyPlayerState == PlayerStateEnum.IN_LOBBY)
        {
            _GameState = GameStateEnum.PLAYING;

            for (int i = 0; i < players.Length;  i++)
            {
                if (players[i]._MyPlayerState == PlayerStateEnum.IN_LOBBY)
                {
                    players[i].FinalActivatePlayer(ZonesEnum.Britian);
                }
            }
        }

        Debug.Log("MINE: Game State Is Now: " + _GameState);

        if (_GameState == GameStateEnum.IN_LOBBY)
        {
            players[playerIndex]._MyPlayerState = PlayerStateEnum.IN_LOBBY;
            Debug.Log("MINE: Player: " + playerIndex + " is now waiting in the lobby!!");
        }
        else if (players[playerIndex]._MyPlayerState == PlayerStateEnum.NOT_PLAYING)
        {
            players[playerIndex]._MyPlayerState = PlayerStateEnum.QUEUED;
            QueuedPlayers.Add(players[playerIndex]);

            Debug.Log("MINE: Player: " + playerIndex + " is now queued to join at the next zone!!");
        }
    }


    private void ReachedNewZone(PlayerRig PlayerWhoReached)
    {
        PlayerWhoReached.Zone = (ZonesEnum)(((int)PlayerWhoReached.Zone) + 1);

        int HighestZone = -1;
        int HighestPlayer = -1;

        for (int i = 0; i < players.Length; i++)
        {
            int PlayerZoneNum = (int)(players[i].Zone);

            if (PlayerZoneNum > HighestZone)
            {
                HighestZone = PlayerZoneNum;
                HighestPlayer = i;
            }
            else if (PlayerZoneNum == HighestZone && i == PlayerWhoReached.PlayerIndex)
            {
                HighestZone = PlayerZoneNum;
                HighestPlayer = i;
            }
        }

        if (PlayerWhoReached.Zone == ZonesEnum.Finished)
        {
            HighestZone = (int)ZonesEnum.Antartic;
            PlayerWhoReached.DeActivatePlayer();

            //ADD switching to the "Player is Finished" GUI here!!!
        }

        bool AllPlayersFinished = true;
        
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].Zone != ZonesEnum.Finished)
            {
                AllPlayersFinished = false;
                break;
            }
        }

        if (AllPlayersFinished)
        {
            _GameState = GameStateEnum.NOT_PLAYING;

            return;
        }

        if (HighestPlayer == PlayerWhoReached.PlayerIndex)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i]._MyPlayerState == PlayerStateEnum.QUEUED)
                {
                    PlayerWhoReached.FinalActivatePlayer((ZonesEnum)HighestZone);
                }
            }
        }
    }
}
