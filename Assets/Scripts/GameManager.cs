using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using static UnityEngine.Networking.UnityWebRequest;

public class GameManager : MonoBehaviour
{
    //Please let us add this here for the Discovery Game.
    public bool AlwaysPassInputToGame = false;

    //TODO: Move this into its own state class
    public enum GameState { IDLE, INTRO, MINIGAME, ENDSTATE, NULL };
    private GameState gameState = GameState.IDLE;
    private GameState previousState = GameState.IDLE;

    //Timer
    public GameTimer m_GameTimer;
    private float m_LastFrameTime = 0.0f;

    //Input
    private Vector2[] playerAxisInput = new Vector2[4];
    private bool[] m_AnyPlayerInput;

    public AudioClip playerButtonPressClip;
    public AudioClip playerButton2PressClip;

    private int[] buttonASuccessiveInputs;
    private int[] buttonBSuccessiveInputs;

    private bool[] inputBlocked;
    private bool[] portsFound;

    private bool verifiedPorts = false;

    //private float stateTimer = 0.0f;  //TODO: Build a proper state timer class that can manage the game hanging and rest to idle

    //TODO: These could move to GameAPI
    private WhaleInput.InputManager inputManager;
    private PlayerManager       playerManager;
    private MusicManager        musicManager;
    private PlayerAudioManager  playerAudioManager;
    private MinigameManager     minigameManager;

    //State info
    private bool creditsPlaying = false; //TODO: Move into Credit Controller
    int runCount = 0;

    // Debug Stuff
    //public GameAutomaton GA;
    [SerializeField] private bool DoTutorial = true;
    [SerializeField] private bool DoGameTutorials = true;
    public bool skipCredits = false;
    public static bool SkipTutorials = false;
    public bool disableIdle = false;

    public bool CanPlayerJoinGame()
    {
        return (gameState == GameManager.GameState.MINIGAME && !(minigameManager.MinigameActive)) || gameState == GameManager.GameState.IDLE || gameState == GameManager.GameState.INTRO;
    }
    public bool CanPlayerHoldGame()
    {
        return gameState != GameState.ENDSTATE;
    }
    public string GetCurrentGameName()
    {
        return minigameManager.CurrentGameName;
    }

    private void ProcessCommandLine()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-SkipTutorial")
            {
                DoTutorial = false;
            }
            else
            {
                DoTutorial = true;
            }
        }
    }
    bool ArgExists(ref string[] args, string argToCheck)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == argToCheck)
            {
                return true;
            }
        }
        return false;
    }
    bool GetArg(ref string[] args, string argToCheck, out string result)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == argToCheck && args.Length > i + 1)
            {
                result = args[i + 1];
                return true;
            }
        }
        result = "";
        return false;
    }

    private void Awake()
    {
        // if not in unity editor, then process command line arguments
#if !UNITY_EDITOR
            ProcessCommandLine();
#else
        SkipTutorials = !DoTutorial;
#endif
        ScreenUtility.CorrectOrthographicSize(Camera.main);

        inputManager = GetComponent<WhaleInput.InputManager>();
        if(inputManager == null)
        {
            inputManager = Instantiate<WhaleInput.InputManager>(Resources.Load<WhaleInput.InputManager>("Input Manager"));
        }
        inputManager.OnPortsVerified += VerifyPorts;


        buttonASuccessiveInputs = new int[4];
        buttonBSuccessiveInputs = new int[4];
        inputBlocked = new bool[4];
        portsFound = new bool[4];

        Debug.Log("Audio cap: " + AudioSettings.driverCapabilities.ToString());
        Debug.Log("Audio speaker mode: " + AudioSettings.speakerMode.ToString());

        musicManager = GetComponentInChildren<MusicManager>();
        playerAudioManager = GetComponentInChildren<PlayerAudioManager>();

        playerManager = GetComponent<PlayerManager>();
        playerManager.Initialise(this);
        m_AnyPlayerInput = new bool[4];

        m_GameTimer.ResetClock();
        m_LastFrameTime = m_GameTimer.m_TimerMax;
        minigameManager = GameObject.FindGameObjectWithTag("MinigameManager").GetComponent<MinigameManager>();
        minigameManager.OnMinigameQuit = PlayerCompletedMinigame; //New and good
        minigameManager.OnIncreaseTimer += IncreaseTimer;
        minigameManager.UnpauseTimer = () => m_GameTimer.ResumeTimer();
        minigameManager.PauseTimer = () => m_GameTimer.PauseTimer();

        // set mouse cursor to invisible
        Cursor.visible = false;
    }

    private void IncreaseTimer(float time){
        m_GameTimer.AddTime(time);
    }

    private void Start()
    {
        GameAPI.ComPortLoader.SetActive(true);
        verifiedPorts = false;

        string[] cmdArgs = System.Environment.GetCommandLineArgs();
        bool InputDebugMode = ArgExists(ref cmdArgs, "-INPUTDEBUG");

        inputManager.Initialise(new WhaleInput.InputActions
        {
            APressed = OnPrimaryFire,
            ADown = OnPrimaryHold,
            AReleased = OnPrimaryReleased,

            BPressed = OnSecondaryFire,
            BDown = OnSecondaryHold,
            BReleased = OnSecondaryReleased,

            DirectionInput = OnDirectionalInput
        }, InputDebugMode);

        ChangeGameState(GameState.IDLE);
    }

    private void PlayerCompletedMinigame(){
        GameAPI.Transitions.PlayRandomTransition(() =>
        {
            ChangeGameState(GameState.MINIGAME);
        });
    }

    // On game state changed, call exit function for previous state and entry functions for new state
    public void ChangeGameState(GameState state)
    {
        if(state!= GameState.MINIGAME && gameState == state) //TODO: Wee hack for now. Maybe we go to a transition state on transition?
		{
            Debug.LogWarning("Attempting to transition into the current state! " + gameState.ToString() + " into " + state.ToString());
            return;
		}
        if (gameState != GameState.ENDSTATE || state == GameState.IDLE)
        {
            previousState = gameState;
            gameState = state;

            Debug.Log("Going from state: " + previousState.ToString() + ", to: " + gameState.ToString());

            if (state != GameState.IDLE)
            {
                //TODO: This sucks we should have a switch for exiting the current state regardless
                OnExitState(previousState);
            }

            //Enter new state
            switch (state)
            {
                case GameState.IDLE:
                    // Cabinet is completely idle and awaiting any player input
                    OnIdleState();
                    minigameManager.ResetProgress();
                    m_GameTimer.ResetClock();
                    SetTeamTimerVisibility(true);
                    break;
                case GameState.INTRO:
                    // Cabinet is playing the tutorial intro before loading the first minigame
                    // Start Intro Sequence
                    SetTeamTimerVisibility(false);
                    GameAPI.IntroController.Begin(()=> {
                        ChangeGameState(GameState.MINIGAME);
                    });
                    break;
                case GameState.MINIGAME:
                    for (int i = 0; i < 4; i++)
                    {
                        if (playerManager.GetPlayerState(i) == Player.PlayerState.ACTIVE && !m_AnyPlayerInput[i])
                        {
                            playerManager.players[i].idling = true;
                        }
                    }

                    // Cabinet presents minigame tutorial over fixed time
                    minigameManager.GetAndLoadNextMinigame();
                        //TODO - Probably want a break in here in case loading is async in future - Gaz
                    ShowTutorialForCurrentGame();
                    break;
                case GameState.ENDSTATE:
                    // Present end card and leaderboard after time has run out before idling
                    OnShowEndState();
                    minigameManager.ForceShutdown();
                    break;
                case GameState.NULL:
                    // This shouldn't happen
                    break;
            }
        }
    }

    // Exit functions upon leaving a state
    void OnExitState(GameState state)
    {
        Debug.Log("Exit state: " + state.ToString());
        switch (state)
        {
            case GameState.IDLE:
                //musicManager.OnMinigamesStart
                break;
            case GameState.INTRO:
                SetTeamTimerVisibility(false);
                UIManager.SetWaitBannerVisible(true);
                break;
            case GameState.MINIGAME:
                UIManager.SetTimerVisible(false);
                // TODO re-enable random scorecards
                GameAPI.Scorecard.GenerateRandomScorecard();
                VFXManager.ClearInstantTutorials();
                VFXManager.ClearInstantCountdown();


                break;
            case GameState.ENDSTATE:
                break;
            case GameState.NULL:
                break;
        }

        
    }

    private void Update()
    {
        playerManager.UpdateIdleTimers(Time.deltaTime);
        playerManager.UpdateHoldingScreens();

        if (gameState == GameState.MINIGAME && m_GameTimer.IsRunning)
        {            
            for (int i = 0; i < 4; i++){
                if (playerManager.GetPlayerState(i) == Player.PlayerState.ACTIVE)
                {
                    UIManager.UpdatePlayerUI(i, true);
                }
                else{
                    UIManager.UpdatePlayerUI(i, false);
                    
                }
                        
            }
            
            float timeLeft = m_GameTimer.Tick(Time.deltaTime);            
            
            
            if (Mathf.Approximately(0.0f, timeLeft))
            {
                VFXManager.ClearInstantCountdown();
                ChangeGameState(GameState.ENDSTATE);
            }
            int iTimeLeft = Mathf.CeilToInt(timeLeft);
            //We have ticked past a second boundary
            if (iTimeLeft != Mathf.CeilToInt(m_LastFrameTime))
            {
                if(iTimeLeft < 4)
                {
                    VFXManager.ClearInstantCountdown();
                    VFXManager.PlayInstantCountdown(iTimeLeft);
                }
            }
            m_LastFrameTime = timeLeft;
        }

        for (int i = 0; i < 4; i++) 
        {
            bool canPlay = portsFound[i] && !inputBlocked[i];
            GameAPI.PlayPrompts[i].SetActive(canPlay);
        }
    }
    //A player has provided a directional input
    public void OnDirectionalInput(int playerIndex, Vector2 direction)
    {
        playerAxisInput[playerIndex] = new Vector2(direction.x, direction.y);
        if (playerAxisInput[playerIndex] != Vector2.zero)
        {
            m_AnyPlayerInput[playerIndex] = true;
            playerManager.players[playerIndex].idling = false;

            if (playerManager.GetPlayerState(playerIndex) == Player.PlayerState.IDLE)
            {
                GameAPI.Transitions.PlayBubbleTransition(playerIndex, playerManager.players[playerIndex].Connect);
            }
        }
        minigameManager.PassDirectionalInput(playerIndex, direction);
    }
    // Primary fire event for all players
    public void OnPrimaryFire(int playerIndex)
    {
        if (!inputBlocked[playerIndex])
        {
            Debug.Log("THEIRS: Primary Fire Index: " + playerIndex);
            playerManager.players[playerIndex].OnPrimaryFire();
            // If player is active and a minigame is currently playing, trigger the primary fire event for the current minigame
            if ((gameState == GameState.MINIGAME && playerManager.GetPlayerState(playerIndex) == Player.PlayerState.ACTIVE) || AlwaysPassInputToGame)
            {
                minigameManager.PassButtonInput(playerIndex, 0);
            }

            m_AnyPlayerInput[playerIndex] = true;
            playerManager.players[playerIndex].idling = false;

            PlayerAudioManager.PlayOneShot(playerIndex, playerButtonPressClip);
        }
    }

    // Secondary fire event for all players
    public void OnSecondaryFire(int playerIndex)
    {
        if (!inputBlocked[playerIndex])
        {
            //Debug.Log("Secondary Fire Index: " + playerIndex);
            playerManager.players[playerIndex].OnSecondaryFire();
            // If player is active and a minigame is currently playing, trigger the secondary fire event for the current minigame
            if ((gameState == GameState.MINIGAME && playerManager.GetPlayerState(playerIndex) == Player.PlayerState.ACTIVE) || AlwaysPassInputToGame)
            {
                minigameManager.PassButtonInput(playerIndex, 1);
            }
            // TODO remove this line as a debug for completing the swap effect
            //if (playerIndex == 3)
            //{
            //    SwapCompleted();
            //}
            m_AnyPlayerInput[playerIndex] = true;
            playerManager.players[playerIndex].idling = false;

            PlayerAudioManager.PlayOneShot(playerIndex, playerButton2PressClip, 1, 1.1f);
        }
    }

    public void OnPrimaryHold(int playerIndex) 
    {
        if (buttonASuccessiveInputs[playerIndex] > 1000)
        {
            inputBlocked[playerIndex] = true;
        }
        else
        {
            buttonASuccessiveInputs[playerIndex] += 1;
        }
    }

    public void OnSecondaryHold(int playerIndex) 
    {
        if (buttonBSuccessiveInputs[playerIndex] > 1000)
        {
            inputBlocked[playerIndex] = true;
        }
        else
        {
            buttonBSuccessiveInputs[playerIndex] += 1;
        }
    }

    public void OnPrimaryReleased(int playerIndex)
    {
        buttonASuccessiveInputs[playerIndex] = 0;
        if (buttonASuccessiveInputs[playerIndex] < 1000 && buttonBSuccessiveInputs[playerIndex] < 1000) 
        {
            inputBlocked[playerIndex] = false;
        }
    }

    public void OnSecondaryReleased(int playerIndex)
    {
        buttonBSuccessiveInputs[playerIndex] = 0;
        if (buttonASuccessiveInputs[playerIndex] < 1000 && buttonBSuccessiveInputs[playerIndex] < 1000)
        {
            inputBlocked[playerIndex] = false;
        }
    }


    // Vertical axis event for player input with given index
    public void GetVerticalAxis(int playerIndex, float verticalValue)
    {
        
    }

    // Horizontal axis event for player input with given index
    public void GetHorizontalAxis(int playerIndex, float horizontalValue)
    {
        
    }

    public void VerifyPorts(bool[] playerPortsFound) 
    {
        for (int i = 0; i < 4; i++) 
        {
            portsFound[i] = playerPortsFound[i];
            GameAPI.PlayPrompts[i].SetActive(portsFound[i]);
        }

        GameAPI.ComPortLoader.SetActive(false);
        verifiedPorts = true;
        /* if(GA != null)
            GA.IsActive = true; */
        ForceRevertToIdle();
    }

    void ShowTutorialForCurrentGame()
    {
        // If any players are currently holding, set them to active
        playerManager.ConnectHoldingPlayers();
        for (int i = 0; i < 4; i++)
            m_AnyPlayerInput[i] = false;

        if (DoGameTutorials)
        {
            SetTeamTimerVisibility(true);
            UIManager.SetWaitBannerVisible(true);
            minigameManager.PlayTutorial();
        }
        else
        {
            minigameManager.MinigameStart();
        }
    }

    // Event to trigger upon all players idling
    void OnIdleState()
    {
        // Reset flags and team timer
        if (minigameManager.MinigameLoaded)
        {
            minigameManager.ForceShutdown();
        }
        m_GameTimer.ResetClock();
        //musicManager.OnIdleStart();

        runCount++;
        if (runCount >= 10) 
        {
            runCount = 0;
            DataManager.OutputEventsToFile();
            DataManager.OutputErrorLog();
        }

        // If any players are currently holding, set them to active
        playerManager.ConnectHoldingPlayers();
        
        Debug.Log("Scene Reset");
        if (previousState != GameState.IDLE){
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

        StartCoroutine(DelayForTime(2.0f, () =>
        {
            Resources.UnloadUnusedAssets();
        }));
    }

    void OnShowEndState()
    {
        m_GameTimer.PauseTimer();
        SetTeamTimerVisibility(false);
        GameAPI.LoseScreen.ShowLoseState(minigameManager.MinigamesCompleted);
    }

    // Utility function to delay for a custom time before executing a callback function
    IEnumerator DelayForTime(float delay, Action callback = null)
    {
        yield return new WaitForSeconds(delay);
        if (callback != null)
        {
            callback();
        }
    }

    public void PlayCredits()
    {
        GameAPI.CreditController.gameObject.SetActive(true);
        GameAPI.CreditController.PlayCredits();
        creditsPlaying = true;

        //TODO: Coroutine is bad bad here.
        StartCoroutine(DelayForTime(skipCredits ? 4.0f : 25.0f, () =>
        {
            GameAPI.Transitions.PlayRandomTransition(() =>
            {
                GameAPI.CreditController.HideCredits();
                creditsPlaying = false;
                ForceRevertToIdlePostCredits();
            });

        }));
    }

    public void ForceRevertToIdle() 
    {
        playerManager.ForceDisconnectAllPlayers();
        creditsPlaying = false;
        ChangeGameState(GameState.IDLE);
    }

    public void ForceRevertToIdlePostCredits() 
    {
        playerManager.ForceDisconnectAllActivePlayers();
        creditsPlaying = false;
        ChangeGameState(GameState.IDLE);
    }

    public void TryExitIdleState()
    {
        if (gameState == GameState.IDLE && verifiedPorts)
        {
            minigameManager.Initialise();
            ChangeGameState(DoTutorial ? GameState.INTRO : GameState.MINIGAME);
        }
    }

   /* public void SkipToNextMinigame() 
    {
        StopAllCoroutines();
        GameAPI.Transitions.PlayRandomTransition(() =>
        {
            ChangeGameState(GameState.MINIGAME);
        });
    }*/

    private void SetTeamTimerVisibility(bool visibility) 
    {
        for (int i = 0; i < GameAPI.TimerParents.Length; i++) 
        {
            GameAPI.TimerParents[i].SetActive(visibility);
        }
    }

    private void OnApplicationQuit()
    {
        DataManager.OutputEventsToFile();
    }

    public void GoToCredits()
    {
        GameAPI.Transitions.PlayRandomTransition(() =>
        {
            GameAPI.LoseScreen.Deactivate();
            PlayCredits();
        });
    }
}
