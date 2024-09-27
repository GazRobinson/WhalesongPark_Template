using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipZoneDetection : MonoBehaviour
{
    [SerializeField]
    NetMinigame minigame;

    [SerializeField] AudioClip shipZoneAudio;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.name) 
        {
            case "ShipZone1":
                minigame.shipZone = 0;
                PlayerAudioManager.PlayOneShot(0, shipZoneAudio);
                break;
            case "ShipZone2":
                minigame.shipZone = 1;
                PlayerAudioManager.PlayOneShot(1, shipZoneAudio);
                break;
            case "ShipZone3":
                minigame.shipZone = 2;
                PlayerAudioManager.PlayOneShot(2, shipZoneAudio);
                break;
            case "ShipZone4":
                minigame.shipZone = 3;
                PlayerAudioManager.PlayOneShot(3, shipZoneAudio);
                break;
        }
    }
}
