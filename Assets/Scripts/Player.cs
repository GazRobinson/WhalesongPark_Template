using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public enum PlayerState { IDLE, HOLDING, ACTIVE };

    public PlayerState playerState;

    public int index = 0;
    bool active = false;
    public int score = 0;

    public float idleTimer = 0.0f;
    public bool showingCountdown = false;

    public bool idling;

    public GameManager gameManager;
    public PlayerManager playerManager;

    string uniqueID;

    public Player() { }

    // Initial initialisation of player upon appliation boot
    public void Initialise(int index)
    {
        this.index = index;
    }

    // On player connection and activation
    public void Connect()
    {
        score = 0;

        /*if (playerState == PlayerState.IDLE && !gameManager.creditsPlaying)
        {
            uniqueID = DataManager.RandomString(8);
            DataManager.AddEvent(new PlayerJoinedEvent(index.ToString(), gameManager.GetCurrentGameName()));
        }*/

        if (gameManager.CanPlayerJoinGame())
        {
            playerState = PlayerState.ACTIVE;
            playerManager.SetIdleScreenVisibility(index, false);
            playerManager.SetCountdownScreenVisibility(index, false);
            gameManager.TryExitIdleState();
        }
        else if(gameManager.CanPlayerHoldGame())
        {
            playerState = PlayerState.HOLDING;
        }
    }

    // On player disconnect and disactivation
    public void Disconnect()
    {
        score = 0;

        playerState = PlayerState.IDLE;
        playerManager.SetIdleScreenVisibility(index, true);

        //DataManager.AddEvent(new PlayerLeftEvent(index.ToString(), gameManager.GetCurrentGameName(), (int)gameManager.timeLeft));
    }

    // On player primary fire input pressed
    public void OnPrimaryFire()
    {
        idleTimer = 0.0f;

        if (playerState == PlayerState.IDLE)
        {
            GameAPI.Transitions.PlayBubbleTransition(index, Connect);
        }
        else
        {
            playerManager.SetCountdownScreenVisibility(index, false);
        }
    }

    // On player secondary fire input pressed
    public void OnSecondaryFire()
    {
        idleTimer = 0.0f;

        if (playerState == PlayerState.IDLE)
        {
            GameAPI.Transitions.PlayBubbleTransition(index, Connect);
        }
        else
        {
            playerManager.SetCountdownScreenVisibility(index, false);
        }
    }
}
