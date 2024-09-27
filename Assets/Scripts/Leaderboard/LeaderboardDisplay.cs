using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardDisplay : MonoBehaviour
{
    public LeaderBoardManager leaderboardManager;

    [SerializeField]
    GameObject[] scoreGroups;
    TMPro.TMP_Text[] teamRankText;
    TMPro.TMP_Text[] teamNameText;
    TMPro.TMP_Text[] teamScoreText;
    Animator[] scoreAnimators;

    private void Awake()
    {
        teamRankText = new TMPro.TMP_Text[scoreGroups.Length];
        teamNameText = new TMPro.TMP_Text[scoreGroups.Length];
        teamScoreText = new TMPro.TMP_Text[scoreGroups.Length];
        scoreAnimators = new Animator[scoreGroups.Length];

        for (int i = 0; i < scoreGroups.Length; i++)
        {
            scoreAnimators[i] = scoreGroups[i].GetComponent<Animator>();

            for (int j = 0; j < scoreGroups[i].transform.childCount; j++)
            {
                string childName = scoreGroups[i].transform.GetChild(j).name;
                switch (childName)
                {
                    case "TeamRank":
                        teamRankText[i] = scoreGroups[i].transform.GetChild(j).gameObject.GetComponent<TMPro.TMP_Text>();
                        break;
                    case "TeamName":
                        teamNameText[i] = scoreGroups[i].transform.GetChild(j).gameObject.GetComponent<TMPro.TMP_Text>();
                        break;
                    case "TeamScore":
                        teamScoreText[i] = scoreGroups[i].transform.GetChild(j).gameObject.GetComponent<TMPro.TMP_Text>();
                        break;
                }
            }
        }
    }

    public void UpdateLeaderboardDisplay(string currentTeamName) 
    {
        int teamIndex = leaderboardManager.GetIndexOfTeam(currentTeamName);
        Debug.Log("Team index: " + teamIndex);
        if (teamIndex != -1) 
        {
            if (teamIndex <= 8)
            {
                // Player is in top 9 scores so we can just display the top 9 scores
                for (int i = 0; i < scoreGroups.Length; i++)
                {
                    if (i < leaderboardManager.leaderboardEntries.Count)
                    {
                        scoreGroups[i].SetActive(true);

                        teamRankText[i].text = (i + 1).ToString();
                        teamNameText[i].text = leaderboardManager.leaderboardEntries[i].teamName;
                        teamScoreText[i].text = leaderboardManager.leaderboardEntries[i].score.ToString();

                        if (i == teamIndex)
                            scoreAnimators[i].SetBool("Pulse", true);
                        else
                            scoreAnimators[i].SetBool("Pulse", false);

                    }
                    else
                    {
                        scoreGroups[i].SetActive(false);
                    }
                }
            }
            else 
            {
                for (int i = 0; i < scoreGroups.Length; i++)
                {
                    int leaderboardIndex = teamIndex - 4 + i;

                    if (leaderboardIndex < leaderboardManager.leaderboardEntries.Count)
                    {
                        scoreGroups[i].SetActive(true);

                        teamRankText[i].text = (leaderboardIndex + 1).ToString();
                        teamNameText[i].text = leaderboardManager.leaderboardEntries[leaderboardIndex].teamName;
                        teamScoreText[i].text = leaderboardManager.leaderboardEntries[leaderboardIndex].score.ToString();

                        if (leaderboardIndex == teamIndex)
                            scoreAnimators[i].SetBool("Pulse", true);
                        else
                            scoreAnimators[i].SetBool("Pulse", false);

                    }
                    else
                    {
                        scoreGroups[i].SetActive(false);
                    }
                }
            }
        }
    }
}
