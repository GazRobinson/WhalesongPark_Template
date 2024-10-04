using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class MinigameBase : MonoBehaviour
{
    [SerializeField] 
    private TutorialController tutorial;

    [SerializeField] public Sprite m_ScoreSprite;
    public bool m_IsMinigameActive
    {
        get;
        private set;
    } = false;

    //  Universal Timer
    [Tooltip("How long should the game last for each attempt.")] [SerializeField] 
    private float[] GameDurationPerLevel = new float[4] { 0, 0, 0, 0 };

    public int Difficulty
    {
        get;
        private set;
    } = 0;

    public System.Action<bool> OnGameComplete;    //The gameplay is complete and the player has won or lost, so we report it to the MinigameManager
    public System.Action OnMinigameFinished;

    public System.Action<float> OnIncreaseTimer;
    public System.Action<bool> OnTimerChange;

    public System.Action PauseTimer;
    public System.Action UnpauseTimer;

    [Header("Events")]
    public UnityEvent MinigameLoaded;   //Anything that needs to be done on game load
    public UnityEvent MinigameStart;    //Anything that needs to start when the player gains control
    public UnityEvent MinigameFinished; //Gameplay has ceased. Get things to stop doing things with this event.
    
    public float GetGameDuration()
    {
        return GameDurationPerLevel[Mathf.Clamp(Difficulty, 0, GameDurationPerLevel.Length-1)];
    }

    private void Start()
    {
        if(tutorial == null)
        {
            Debug.LogError("No Tutorial found! Please ensure the tutorial has a TutorialController script attached");
        }
    }

    /// <summary>
    /// Perform any functionality you need to do to reset the game.
    /// </summary>
    protected abstract void OnResetGame();
    /// <summary>
    /// What should happen when the player presses the left hand button?
    /// </summary>
    /// <param name="playerIndex">Which player (0-3) pressed the button</param>
    public abstract void OnPrimaryFire(int playerIndex);
    /// <summary>
    /// What should happen when the player presses the right hand button?
    /// </summary>
    /// <param name="playerIndex">Which player (0-3) pressed the button</param>
    public abstract void OnSecondaryFire(int playerIndex);

    /// <summary>
    /// How do you want to handle input from the four directional buttons?
    /// </summary>
    /// <param name="playerIndex">Which player (0-3) pressed the button</param>
    /// <param name="direction">Which direction(s) are they pressing</param>
    public abstract void OnDirectionalInput(int playerIndex, Vector2 direction);

    //TODO - put this in the game load?
    public void ResetMinigame()
    {
        Difficulty = 0;
        OnResetGame();
    }

    public virtual void LoadMiniGame() 
    {
        Debug.Log("Load new minigame");
        gameObject.SetActive(true);

        //DataManager.AddEvent(new MinigameStarted(gameObject.name, gameObject.name, 1, PlayerUtilities.GetActivePlayerCount()));

        m_IsMinigameActive = false;
        MinigameLoaded.Invoke();
    }

    public virtual void UnloadMinigame()
    {

    }

    public virtual void PlayTutorial(System.Action callback)
    {
        tutorial.Begin(callback);
    }

    /// <summary>
    /// Begins the playing of the minigame
    /// </summary>
    public virtual void StartMinigame() 
    {
        Debug.Log("Starting Mini game");
        m_IsMinigameActive = true;
        Difficulty++;

        MinigameStart.Invoke();
    }

    public virtual void Update(){
        OnUpdate();
    }

    protected virtual void OnUpdate(){

    }

    /// <summary>
    /// This function is called to end the game and decide win or loss.
    /// </summary>
    public abstract void TimeUp();

    public void Shutdown()
    {
        gameObject.SetActive(false);
    }
    public void EndGame() 
    {        
        Debug.Log("Setting game to inactive and doing and endgame events...");
        m_IsMinigameActive = false;
        MinigameFinished.Invoke();
    }
    /// <summary>
    /// This function is called at the end of the game so that it knows what to display on the score screen.
    /// You give it information about what each players score was, how much time they earned individually, and also how much time they've earned together
    /// </summary>
    /// <returns>A class that contains all the necessary information to display the score page</returns>
    public abstract GameScoreData GetScoreData();

    //Here's an example!
    /*{
        Debug.LogWarning("GetScoreData not implemented in your Minigame, using a placeholder for now!");
        GameScoreData gsd = new GameScoreData();
        int teamTime = 0;
        for (int i = 0; i < 4; i++)
        {
            if (PlayerUtilities.GetPlayerState(i) == Player.PlayerState.ACTIVE)
            {
                gsd.PlayerScores[i] = 100;
                gsd.PlayerTimes[i] = 7;
                teamTime += gsd.PlayerTimes[i];
            }
        }
        gsd.ScoreSuffix = "%";
        gsd.TeamTime = teamTime;
        return gsd;
    }*/
}
