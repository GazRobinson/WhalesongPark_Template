using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUtilities
{
    private static PlayerManager playerManager;
    public static void Initialise(PlayerManager playerManager)
    {
        PlayerUtilities.playerManager = playerManager;
    }

    public static Player.PlayerState GetPlayerState(int playerIndex)
    {
        if (playerIndex < playerManager.players.Length)
        {
            return playerManager.players[playerIndex].playerState;
        }
        else
        {
            return Player.PlayerState.IDLE;
        }
    }

    //TODO: this is a hack for now and should be removed
    public static void SetPlayerState(int playerIndex, Player.PlayerState newState)
    {
        if (playerIndex < playerManager.players.Length)
        {
            playerManager.players[playerIndex].playerState = newState;
        }
    }

    public static int GetActivePlayerCount() 
    {
        int count = 0;
        for (int i = 0; i < playerManager.players.Length; i++) 
        {
            if (playerManager.players[i].playerState == Player.PlayerState.ACTIVE) 
            {
                count++;
            }
        }
        return count;
    }

    public static int GetTotalPlayerCount(){
        return playerManager.players.Length;
    }

    public static Color[] GetPlayerColors()
    {
        return playerManager.playerColours;
    }
    public static Color GetPlayerColor(int playerIndex)
    {
        return playerManager.playerColours[playerIndex];
    }
}
