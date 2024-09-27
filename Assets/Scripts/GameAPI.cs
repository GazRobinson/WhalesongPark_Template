using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAPI : MonoBehaviour
{
    private static GameAPI instance;

    public TransitionManager transitionManager;
    [SerializeField] private ScorecardGenerator scorecardGenerator;
    [SerializeField] private LoseStateController loseStateController;
    [SerializeField] private CreditAnimation creditController;
    [SerializeField] private GameObject comPortLoadingCanvas;
    [SerializeField] private GameObject[] timerParents;
    [SerializeField] private GameObject[] playerPlayPrompts;
    [SerializeField] private IntroController introController;

    public static TransitionManager Transitions { get { return instance.transitionManager; } }
    public static ScorecardGenerator Scorecard { get { return instance.scorecardGenerator; } }
    public static LoseStateController LoseScreen { get { return instance.loseStateController; } }
    public static CreditAnimation CreditController { get { return instance.creditController; } }
    public static GameObject ComPortLoader { get { return instance.comPortLoadingCanvas; } }
    public static GameObject[] TimerParents { get { return instance.timerParents; } }
    public static GameObject[] PlayPrompts { get { return instance.playerPlayPrompts; } }
    public static IntroController IntroController { get { return instance.introController; } }

    private void Awake()
    {
        instance = this;
    }
}
