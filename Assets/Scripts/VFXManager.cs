using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VFXManager : MonoBehaviour
{
    private static VFXManager instance;

    public AudioClip fireworkPop;

    public Transform[] originalFireworkPositions;
    public ParticleSystem firework;

    private List<GameObject> fireworkInstances = new List<GameObject>();

    public List<Color> playerColours;



    List<GameObject> instantTutorials = new List<GameObject>();
    List<GameObject> instantCountdowns = new List<GameObject>();
    List<int> instantIndexes = new List<int>();
    [SerializeField] List<GameObject> playerZones = new List<GameObject>();

    [SerializeField] private GameObject countdownPrefab;
    bool countdownOn = false;


    private int currentCountdown = 0;

    [SerializeField] AudioClip countdownClip;


    // Start is called before the first frame update
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        //spawn 20 fireworks add to list
        for (int i = 0; i < 20; i++)
        {
            GameObject fireworkInstance = Instantiate(firework.gameObject, originalFireworkPositions[0].position, Quaternion.identity);
            fireworkInstance.transform.rotation = Quaternion.Euler(-90, 0, 0);
            fireworkInstance.GetComponent<ParticleSystem>().Stop();
            fireworkInstance.transform.parent = transform;
            fireworkInstance.SetActive(false);
            fireworkInstances.Add(fireworkInstance);
        }
    }

    

    private GameObject GetFireworkInstance()
    {
        foreach (GameObject fireworkInstance in fireworkInstances)
        {
            if (!fireworkInstance.activeInHierarchy)
            {
                return fireworkInstance;
            }
        }
        return null;
    }

    public static void PlayFireworks(int playerIndex, Vector2 pos = default(Vector2)) 
    {
        if (playerIndex < 4 && playerIndex >= 0)
        {
            // instantiate firework
            GameObject fireworkInstance = instance.GetFireworkInstance();

            if (fireworkInstance == null)
            {
                return;
            }
            
            instance.CreateFireworks(playerIndex, fireworkInstance, (Vector2)instance.originalFireworkPositions[playerIndex].position + pos);
        }
        else{

            for (int i = 0; i < 4; i++)
            {
                // instantiate firework
                GameObject fireworkInstance = instance.GetFireworkInstance();

                if (fireworkInstance == null)
                {
                    break;
                }

                instance.CreateFireworks(i, fireworkInstance, (Vector2)instance.originalFireworkPositions[playerIndex].position + pos);
            }
        }
        
    }

    public static void PlayFireworksAtGlobalPosition(int playerIndex, Vector2 pos = default(Vector2)) 
    {
        if (playerIndex < 4 && playerIndex >= 0)
        {
            // instantiate firework
            GameObject fireworkInstance = instance.GetFireworkInstance();

            if (fireworkInstance == null)
            {
                return;
            }

            instance.CreateFireworks(playerIndex, fireworkInstance, pos);
        }
        
        
    }

    public static void PlayInstantTutorial(int playerIndex, GameObject go, Vector2 pos = default(Vector2)) 
    {
        // debug 
        Debug.Log("Instant tutorial called");

        if (playerIndex < 4 && playerIndex >= 0)
        {
            // instantiate game object
            GameObject goInstance = Instantiate(go, (Vector2)instance.originalFireworkPositions[playerIndex].position + pos, Quaternion.identity);

            if (goInstance == null)
            {
                return;
            }

            // set parent
            goInstance.transform.SetParent(instance.playerZones[playerIndex].transform);
            goInstance.transform.SetAsFirstSibling();
            goInstance.transform.localScale = (Vector3.one);

            if (goInstance.GetComponent<UIColourFlip>() != null){
                UIColourFlip cf = goInstance.GetComponent<UIColourFlip>();
                cf.flipColour = instance.playerColours[playerIndex];
                cf.rightButton.color = cf.flipColour;
                cf.rightHand.color = cf.flipColour;
            }

            else if (goInstance.GetComponentInChildren<Image>() != null){
                foreach (Image sr in goInstance.GetComponentsInChildren<Image>())
                {
                    sr.color = instance.playerColours[playerIndex];
                }
            }
            

            instance.instantTutorials.Add(goInstance);
            instance.instantIndexes.Add(playerIndex);
        }
        
        
    }

    public static void PlayInstantCountdown(int time){
        if (!instance.countdownOn){
            for (int i = 0; i < 4; i++){
            // instantiate game object
            GameObject goInstance = Instantiate(instance.countdownPrefab, (Vector2)instance.originalFireworkPositions[i].position, Quaternion.identity);

            if (goInstance == null)
            {
                return;
            }

            // set parent
            goInstance.transform.SetParent(instance.playerZones[i].transform);
            goInstance.transform.SetAsFirstSibling();
            goInstance.transform.localScale = (Vector3.one);

            // set countdown text
            goInstance.GetComponentInChildren<TMPro.TMP_Text>().SetText(time.ToString());
            instance.currentCountdown = time;

            // play audio
            PlayerAudioManager.PlayOneShot(i, instance.countdownClip, 1, 1.0f + (time/10.0f));

            instance.instantCountdowns.Add(goInstance);

            
        }

        instance.countdownOn = true;
        }
        
    }

    public static void ClearInstantCountdown(){
        foreach (GameObject go in instance.instantCountdowns)
        {
            Destroy(go);
        }
        instance.instantCountdowns.Clear();
        instance.countdownOn = false;
    }

    public static int GetCurrentCountdown(){
        return instance.currentCountdown;
    }
    
        

    public static void ClearSpecificInstantTutorials(int playerIndex) 
    {
        for (int i = 0; i < instance.instantIndexes.Count;)
        {
            if (instance.instantIndexes[i] == playerIndex)
            {
                Destroy(instance.instantTutorials[i]);
                instance.instantTutorials.RemoveAt(i);
                instance.instantIndexes.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
    }

    public static void ClearInstantTutorials() 
    {
        foreach (GameObject go in instance.instantTutorials)
        {
            Destroy(go);
        }
        instance.instantTutorials.Clear();
    }

    public static void PlayFishParticlesAtGlobalPosition(int playerIndex, Vector2 pos = default(Vector2)) 
    {
        if (playerIndex < 4 && playerIndex >= 0)
        {
            // instantiate firework
            GameObject fireworkInstance = instance.GetFireworkInstance();

            if (fireworkInstance == null)
            {
                return;
            }

            instance.CreateFireworks(playerIndex, fireworkInstance, pos);
        }
        
        
    }

    private void CreateFireworks(int playerIndex, GameObject fireworkInstance, Vector2 pos){
        fireworkInstance.transform.position = pos;
        fireworkInstance.SetActive(true);

        // set firework colour
        ParticleSystem.MainModule main = fireworkInstance.GetComponent<ParticleSystem>().main;
        main.startColor = instance.playerColours[playerIndex];

        // play audio
        PlayerAudioManager.PlayOneShot(playerIndex, instance.fireworkPop);
        // play
        fireworkInstance.GetComponent<ParticleSystem>().Play();
    }

    
}
