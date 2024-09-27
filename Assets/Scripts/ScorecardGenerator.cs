using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorecardGenerator : MonoBehaviour
{
    public GameObject[] simulatedScorecards;

    private int scorecardIndex = 0;
    private int previousCardIndex = 0;

    public void GenerateRandomScorecard() 
    {
        if (simulatedScorecards.Length > 1) 
        {
            previousCardIndex = scorecardIndex;

            int iterations = 0;
           // do 
           // {
                scorecardIndex = Random.Range(0, simulatedScorecards.Length - 1);
                iterations++;

           // } while (previousCardIndex == scorecardIndex || iterations > 100);

            for (int i = 0; i < simulatedScorecards.Length; i++) 
            {
                if (i == scorecardIndex)
                    simulatedScorecards[i].SetActive(true);
                else
                    simulatedScorecards[i].SetActive(false);
            }
        }
    }
}
