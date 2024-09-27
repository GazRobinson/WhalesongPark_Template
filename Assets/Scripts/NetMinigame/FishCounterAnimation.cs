using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishCounterAnimation : MonoBehaviour
{
    private NetMinigame minigame;

    [SerializeField]
    TMP_Text[] fishCounterText;
    Animator[] fishCounterAnimators;

    [SerializeField]
    GameObject winStateCanvas;

    float countingDelay = 0.02f;

    [SerializeField]
    float[] fishCounts = { 0, 0, 0, 0 };
    float[] fishPercentages = { 0, 0, 0, 0 };
    public bool playersDone = false;

    [SerializeField]
    TMP_Text[] timeAddedSingleText;
    [SerializeField] Color[] playerColors;
    [SerializeField] Material[] playerMaterials;
    Animator[] timeAddedSingleAnimations;
    [SerializeField]
    Image[] singleIcons;
    Animator[] singleIconAnimators;

    [SerializeField]
    TextMeshProUGUI[] teamTimeAddedText;
    Animator[] teamTimeAddedAnimators;
    [SerializeField]
    Image[] teamIcons;
    Animator[] teamIconsAnimators;

    public AudioClip countAudioTick;
    public AudioClip positiveAudioDing;

    public AudioClip fireworkPop;

    float[] fireworkTimers;
    float[] fireworkTime;
    Vector2[] originalFireworkPositions;
    public ParticleSystem[] fireworks;

    public float minFireworkTime;
    public float maxFireworkTime;

    private System.Action AllDoneCallback = null;

    private void Awake()
    {
        fireworks = GetComponentsInChildren<ParticleSystem>();

        fireworkTimers = new float[fireworks.Length];
        fireworkTime = new float[fireworks.Length];
        originalFireworkPositions = new Vector2[fireworks.Length];

        for (int i = 0; i < fireworkTime.Length; i++)
        {
            fireworkTime[i] = Random.Range(minFireworkTime, maxFireworkTime);
            originalFireworkPositions[i] = fireworks[i].gameObject.transform.position;
        }

        fishCounterAnimators = new Animator[4];
        timeAddedSingleAnimations = new Animator[4];
        singleIconAnimators = new Animator[4];
        teamTimeAddedAnimators = new Animator[4];
        teamIconsAnimators = new Animator[4];

        // Get references to scorecard animators
        for (int i = 0; i < fishCounterText.Length; i++) 
        {
            fishCounterAnimators[i] = fishCounterText[i].gameObject.GetComponent<Animator>();
            timeAddedSingleAnimations[i] = timeAddedSingleText[i].gameObject.GetComponent<Animator>();
            singleIconAnimators[i] = singleIcons[i].gameObject.GetComponent<Animator>();
            teamTimeAddedAnimators[i] = teamTimeAddedText[i].gameObject.GetComponent<Animator>();
            teamIconsAnimators[i] = teamIcons[i].gameObject.GetComponent<Animator>();
        }
    }

    private void Start()
    { 
        minigame = transform.parent.GetComponent<NetMinigame>();
        minigame.MinigameLoaded.AddListener(ResetCount);
    }

    public void ResetCount()
    {
        // Hide canvas and reset animation values
        winStateCanvas.SetActive(false);
        fishCounts = new float[] { 0, 0, 0, 0 };
        fishPercentages = new float[] { 0, 0, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            if (timeAddedSingleAnimations[i] != null)
                timeAddedSingleAnimations[i].SetTrigger("Reset");
            if (singleIconAnimators[i] != null)
                singleIconAnimators[i].SetTrigger("Reset");
            if (teamTimeAddedAnimators[i] != null)
                teamTimeAddedAnimators[i].SetTrigger("Reset");
            if (teamIconsAnimators[i] != null)
                teamIconsAnimators[i].SetTrigger("Reset");
        }

        playersDone = false;
    }

    private void Update()
    {
        // Check if player counters have all completed before playing next animations
        if (fishCounts.Length >= 4 && minigame.fishLeft.Length >= 4)
        {
            bool playerOneTwoDone = (fishCounts[0] == fishPercentages[0]) && (fishCounts[1] == fishPercentages[1]);
            bool playerThreeFourDone = (fishCounts[2] == fishPercentages[2]) && (fishCounts[3] == fishPercentages[3]);
            playersDone = (playerOneTwoDone && playerThreeFourDone);
        }
    }

    public void ShowWinStateAnimation(System.Action callback) 
    {
        AllDoneCallback = callback;
        winStateCanvas.SetActive(true);

        for (int i = 0; i < fishCounterText.Length; i++)
            fishCounterText[i].SetText("0%");

        for (int i = 0; i < 4; i++) 
        {
            // Calculate percentage of fish for each player
            fishPercentages[i] = minigame.fishLeft[i] / 32.0f * 100.0f;
            
            //singleIcons[i].material = playerMaterials[i];

            // Fill canvas time text fields
            if (timeAddedSingleText[i] != null){
                timeAddedSingleText[i].SetText("+" + minigame.playerTimeWon[i].ToString() + "s");
                timeAddedSingleText[i].faceColor = playerColors[i];
            }
                
            if (teamTimeAddedText[i] != null){
                teamTimeAddedText[i].SetText("+" + minigame.teamTimeWon.ToString() + "s");

                // generate a random value between zero and three
                int randomIcon = Random.Range(0, 3);
                // regenerate if value is equal to i
                if (randomIcon == i)
                {
                    if (randomIcon == 3)
                        randomIcon = 0;
                    else{
                        randomIcon = i + 1;
                    }
                    
                }
                teamTimeAddedText[i].faceColor = playerColors[randomIcon];
            }

            StartCoroutine(CountDelay(i));
        }
        // Pause until all counters have completed increment animations
        StartCoroutine(WaitForCountersComplete());
    }

    IEnumerator CountDelay(int playerIndex) 
    {
        if (minigame != null && playerIndex < minigame.fishLeft.Length)
        {
            if (PlayerUtilities.GetPlayerState(playerIndex) == Player.PlayerState.ACTIVE)
            {
                for (int i = 0; i < fishPercentages[playerIndex]; i++)
                {
                    yield return new WaitForSeconds(countingDelay);
                    //Increment fish count value and update canvas text
                    fishCounts[playerIndex]++;
                    if (playerIndex < fishCounterText.Length && fishCounterText[playerIndex] != null)
                    {
                        fishCounterText[playerIndex].SetText(fishCounts[playerIndex].ToString() + "%");
                    }

                    if (i % 2 == 0)
                    {
                        PlayerAudioManager.PlayOneShot(playerIndex, countAudioTick, 0.75f);
                    }

                    if (i == 25 || i == 50 || i == 75 || i == 99)
                    {
                        VFXManager.PlayFireworks(playerIndex, new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)));
                    }

                }
                // When counter reaches target value, play pulse animation
                if (playerIndex < fishCounterAnimators.Length && fishCounterAnimators[playerIndex] != null)
                {
                    fishCounterAnimators[playerIndex].SetTrigger("Pulse");
                    PlayerAudioManager.PlayOneShot(playerIndex, positiveAudioDing, 0.75f, 1.0f);
                }
            }
        }
    }

    // Wait until all counters have completed before playing time gained animations
    IEnumerator WaitForCountersComplete()
    { 
        yield return new WaitForSeconds(3.0f);

        ShowPlayerTimes();
    }

    void ShowPlayerTimes() 
    {
        for (int i = 0; i < 4; i++)
        {
            // Play fade in animations for single player time gained UI
            if (timeAddedSingleAnimations[i] != null)
                timeAddedSingleAnimations[i].SetTrigger("FadeIn");
            if (singleIconAnimators[i] != null)
                singleIconAnimators[i].SetTrigger("FadeIn");

            PlayerAudioManager.PlayOneShot(i, positiveAudioDing, 0.75f, 1.1f);
        }
        // Pause for time then play team time animations
        StartCoroutine(CustomDelay(2.0f, ShowTeamTimes));
    }

    // Generic utility function for delaying for input time before executing callback
    IEnumerator CustomDelay(float timeDelay, System.Action callback = null) 
    {
        yield return new WaitForSeconds(timeDelay);
        if (callback != null)
            callback();
    }

    void ShowTeamTimes()
    {
        for (int i = 0; i < 4; i++)
        {
            // Play team time fade in animations and pause before next animations
            if (teamTimeAddedAnimators[i] != null)
                teamTimeAddedAnimators[i].SetTrigger("FadeIn");
            if (teamIconsAnimators[i] != null)
                teamIconsAnimators[i].SetTrigger("FadeIn");

            PlayerAudioManager.PlayOneShot(i, positiveAudioDing, 0.75f, 1.2f);
        }

        StartCoroutine(CustomDelay(2.0f, PlayFlyAnimation));
    }

    void PlayFlyAnimation() 
    {
        for (int i = 0; i < 4; i++)
        {
            // Trigger UI flying animations
            if (teamTimeAddedAnimators[i] != null)
                teamTimeAddedAnimators[i].SetTrigger("FlyToTimer");
        }

        StartCoroutine(CustomDelay(1.0f, () =>
        {
            StartCoroutine(StartIncrementTimer());
        }));
    }

    // Increment team game timer by won time
    IEnumerator StartIncrementTimer() 
    {
        minigame.OnTimerChange(true);
        for (int i = 0; i < minigame.teamTimeWon; i++) 
        {
            yield return new WaitForSeconds(0.05f);
            minigame.OnIncreaseTimer.Invoke(1.0f);

            if (i % 2 == 0)
            {
                PlayerAudioManager.PlayGlobalOneShot(countAudioTick, 0.75f);
            }
        }
        minigame.OnTimerChange(false);

        //We're done???
        Debug.Log("All done with the win");
        AllDoneCallback();
    }

    public void PlayFireworks()
    {
       for (int i = 0; i < fireworks.Length; i++) 
        {
            int playerFireworkIndex = 0;
            if (i >= 15)
                playerFireworkIndex = 3;
            else if (i >= 10)
                playerFireworkIndex = 2;
            else if (i >= 5)
                playerFireworkIndex = 1;

            fireworkTimers[i] += Time.deltaTime;
            if (fireworkTimers[i] >= fireworkTime[i]) 
            {
                fireworks[i].gameObject.transform.position = originalFireworkPositions[i] + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
                fireworks[i].Play();
                PlayerAudioManager.PlayOneShot(playerFireworkIndex, fireworkPop, 0.2f, 1.0f);
                fireworkTime[i] = Random.Range(minFireworkTime, maxFireworkTime);
                fireworkTimers[i] = 0.0f;               
            }
        }
    }
}
