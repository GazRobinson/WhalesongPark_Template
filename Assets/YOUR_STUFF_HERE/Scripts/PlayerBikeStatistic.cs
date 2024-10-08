using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerBikeStatistic")]
public class PlayerBikeStatistic : ScriptableObject
{
    public float maxBikePower = 5;
    public float buttonPressPower = 1f;
    public float bikePowerDecayRate = 0.01f;
}
