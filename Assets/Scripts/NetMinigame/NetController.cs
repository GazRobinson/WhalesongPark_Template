using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetController : MonoBehaviour
{
    [SerializeField]
    NetMinigame minigameController;

    public float minY;
    public float maxY;

    public float netSpeed = 1.0f;

    float timeValue = 0.0f;
    bool paused = false;

    [SerializeField]
    bool duplicate = false;
    [SerializeField]
    Transform duplicateMatchHeight;

    float randomOffset;
    public Vector2 randomPauseOccurenceRange = new Vector2(1.0f, 4.0f);
    public Vector2 randomPauseLengthRange = new Vector2(1.0f, 3.0f);

    public AudioClip fishCollectAudio;

    [SerializeField]private SpriteRenderer upArrow;
    [SerializeField]private SpriteRenderer downArrow;

    private Vector2 prevPos;

    float minWaterHeight = -1.175f;
    float maxWaterHeight = -0.35f;
    

    private void Start()
    {
        timeValue = 0.0f;
        randomOffset = Random.Range(0, 5.0f);

        minigameController.MinigameStart.AddListener(OnMinigameStart);
        minigameController.MinigameFinished.AddListener(OnMinigameCompleted);
    }

    void OnMinigameStart() 
    {
        timeValue = 0.0f;
        randomOffset = Random.Range(0, 5.0f);

        upArrow.enabled = false;
        downArrow.enabled = false;
    }

    void OnMinigameCompleted() 
    {
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        if (!duplicate)
        {
            if (!paused)
            {
                timeValue += Time.deltaTime;
            }

            // perlin noise
            float heightValue = Mathf.PerlinNoise(timeValue * netSpeed * 0.8f, 0.0f) * (maxY - minY) + minY;

            // difficulty scaling
            if (minigameController.Difficulty > 2){
                heightValue = Mathf.PerlinNoise(timeValue * netSpeed * 1.5f, 0.0f) * (maxY - minY) + minY;
            }
            else if (minigameController.Difficulty > 1){
                heightValue = Mathf.PerlinNoise(timeValue * netSpeed * 1.1f, 0.0f) * (maxY - minY) + minY;
            }
            
            // this is the top and bottom of the water
            heightValue = Mathf.Clamp(heightValue, minWaterHeight, maxWaterHeight);

            transform.localPosition = new Vector3(transform.localPosition.x, heightValue, 0.0f);

        }
        else 
        {
            transform.localPosition = new Vector3(transform.localPosition.x, duplicateMatchHeight.localPosition.y, 0.0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if (collision.gameObject.tag == "Fish") 
        {
            if (collision.GetComponent<FishBounce>().IsOriginFish()){
                collision.GetComponent<FishBounce>().SetOriginFish(false);
            }
            collision.gameObject.SetActive(false);
            // TODO update this with another method as this is very hardcoded
            // Already using tag for detecting and using a script component would be less performant
            switch (collision.gameObject.name) 
            {
                case "PlayerFish1":
                    minigameController.fishLeft[0] -= 1;
                    PlayerAudioManager.PlayOneShot(0, fishCollectAudio, 0.5f, 0.75f);
                    VFXManager.PlayFishParticlesAtGlobalPosition(0, collision.transform.position);
                    break;
                case "PlayerFish2":
                    minigameController.fishLeft[1] -= 1;
                    PlayerAudioManager.PlayOneShot(1, fishCollectAudio, 0.5f, 0.75f);
                    VFXManager.PlayFishParticlesAtGlobalPosition(1, collision.transform.position);
                    break;
                case "PlayerFish3":
                    minigameController.fishLeft[2] -= 1;
                    PlayerAudioManager.PlayOneShot(2, fishCollectAudio, 0.5f, 0.75f);
                    VFXManager.PlayFishParticlesAtGlobalPosition(2, collision.transform.position);
                    break;
                case "PlayerFish4":
                    minigameController.fishLeft[3] -= 1;
                    PlayerAudioManager.PlayOneShot(3, fishCollectAudio, 0.5f, 0.75f);
                    VFXManager.PlayFishParticlesAtGlobalPosition(3, collision.transform.position);
                    break;
            }
        }
    }

    private void GenerateArrow(){
        
        if (prevPos.y < transform.localPosition.y)
        {
            upArrow.enabled = true;
            downArrow.enabled = false;
        }
        else if (prevPos.y > transform.localPosition.y)
        {
            upArrow.enabled = false;
            downArrow.enabled = true;
        }
        else
        {
            upArrow.enabled = false;
            downArrow.enabled = false;
        }

        prevPos = transform.localPosition;
    }
}
