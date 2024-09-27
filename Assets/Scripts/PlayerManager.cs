using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    const int NUM_PLAYERS = 4;
    const float MAX_IDLE_TIME = 5;

    public Player[] players;
    public Color[] playerColours; //TODO: Move into Player Manager

    [SerializeField]
    GameObject[] playerIdleScreens;
    [SerializeField]
    GameObject[] playerCountdownScreens;
    Animation[] playerCountdownAnimations;
    TMPro.TMP_Text[] countdownText;

    [SerializeField]
    GameObject[] playerHoldingScreens;

    GameManager gameManager;

    [SerializeField] public AudioClip joinSound;

    private void Awake()
    {
        // Get references to player countdown UI and animation controllers
        playerCountdownAnimations = new Animation[4];
        countdownText = new TMPro.TMP_Text[4];
        for (int i = 0; i < playerCountdownScreens.Length; i++)
        {
            string childName = "Countdown" + i;
            playerCountdownAnimations[i] = playerCountdownScreens[i].transform.Find(childName).GetComponent<Animation>();
            countdownText[i] = playerCountdownScreens[i].transform.Find(childName).GetComponent<TMPro.TMP_Text>();
        }
        PlayerUtilities.Initialise(this);
    }

    public Player.PlayerState GetPlayerState(int playerIndex)
    {
        if (playerIndex < players.Length)
        {
            return players[playerIndex].playerState;
        }
        else
        {
            return Player.PlayerState.IDLE;
        }
    }

    public int GetActivePlayerCount() 
    {
        int count = 0;
        for (int i = 0; i < players.Length; i++) 
        {
            if (players[i].playerState == Player.PlayerState.ACTIVE) 
            {
                count++;
            }
        }
        return count;
    }

    public void Initialise(GameManager gameManager)
    {
        // Initailise player list upon application boot and assign indices
        players = new Player[NUM_PLAYERS];
        for (int i = 0; i < players.Length; i++)
        {
            this.gameManager = gameManager;

            players[i] = new Player();
            players[i].Initialise(i);
            players[i].gameManager = gameManager;
            players[i].playerManager = this;
        }
    }

    public void UpdateHoldingScreens() 
    {
        for (int i = 0; i < players.Length; i++) 
        {
            if (players[i].playerState == Player.PlayerState.HOLDING) 
            {
                SetHoldingScreenVisibility(i, true);
            }
            else 
            {
                SetHoldingScreenVisibility(i, false);
            }
        }
    }

    // Iterate over active players and increment idle timers
    public void UpdateIdleTimers(float deltaTime)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].playerState == Player.PlayerState.ACTIVE)
            {
                if (players[i].idling)
                {
                    players[i].idleTimer += deltaTime;

                    if (!players[i].showingCountdown)
                    {
                        SetCountdownScreenVisibility(i, true);
                    }
                    string countdownValue = Mathf.CeilToInt(MAX_IDLE_TIME - players[i].idleTimer).ToString();
                    countdownText[i].SetText(countdownValue);

                    if (players[i].idleTimer >= MAX_IDLE_TIME)
                    {
                        players[i].Disconnect();
                        SetCountdownScreenVisibility(i, false);
                        CheckIfAllIdle();
                    }
                }
                else
                {
                    players[i].idleTimer = 0.0f;
                }
            }
        }
    }

    public void PlayerIdling(int playerIndex, bool idling) 
    {
        players[playerIndex].idling = idling;
    }

    // Attempt to connect all players in holding state
    public void ConnectHoldingPlayers()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].playerState == Player.PlayerState.HOLDING)
            {
                players[i].Connect();
            }
        }
    }

    // Check if all players are in idle state and if so return to idle state
    public void CheckIfAllIdle()
    {
        bool activeFlag = false;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].playerState == Player.PlayerState.ACTIVE)
            {
                activeFlag = true;
            }
        }

        if (!activeFlag)
        {
            gameManager.ChangeGameState(GameManager.GameState.IDLE);
        }
    }

    // Set idle screen visiblity for input player index
    public void SetIdleScreenVisibility(int index, bool visibility)
    {
        playerIdleScreens[index].SetActive(visibility);
    }

    // Set countdown screen visibility for input player index
    // TODO: this should probably be refactored so that all countdown instances are unified through the VFX Manager
    // the current system is functional but has needless duplication
    public void SetCountdownScreenVisibility(int index, bool visibility)
    {


        playerCountdownScreens[index].SetActive(visibility);
        players[index].showingCountdown = visibility;

        if (visibility)
        {
            playerCountdownAnimations[index].Play();
        }
        else
        {
            playerCountdownAnimations[index].Stop();
        }
    }

    public void SetHoldingScreenVisibility(int index, bool visibility) 
    {
        playerHoldingScreens[index].SetActive(visibility);
        players[index].showingCountdown = visibility;
    }

    public void ForceDisconnectAllPlayers() 
    {
        for (int i = 0; i < 4; i++) 
        {
            players[i].Disconnect();
        }
    }

    public void ForceDisconnectAllActivePlayers() 
    {
        for (int i = 0; i < 4; i++)
        {
            if (players[i].playerState == Player.PlayerState.ACTIVE)
            {
                players[i].Disconnect();
            }
        }
    }
}
