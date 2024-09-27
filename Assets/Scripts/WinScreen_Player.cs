using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen_Player : MonoBehaviour
{
    [SerializeField] private Image    m_ScoreIcon;
    [SerializeField] private TMP_Text   m_ScoreText;

    [SerializeField] private Image      m_PlayerIcon;
    [SerializeField] private TMP_Text   m_PlayerTimeText;

    [SerializeField] private Image      m_TeamIcon;
    [SerializeField] private TMP_Text   m_TeamTimeText;

    public void ShowWinScreen(Sprite ScoreIcon, GameScoreData scoreData, int index)
    {
        m_ScoreIcon.sprite = ScoreIcon;
        m_ScoreText.text = scoreData.PlayerScores[index].ToString() + scoreData.ScoreSuffix;

        //m_PlayerIcon;
        m_PlayerTimeText.text = scoreData.PlayerTimes[index].ToString() + "s";

        //m_TeamIcon;
        m_TeamTimeText.text = scoreData.TeamTime.ToString() + "s";

    }

}
