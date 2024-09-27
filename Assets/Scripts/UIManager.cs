using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    [SerializeField] private List<UnityEngine.UI.Image> timerImages;

    [SerializeField] private List<GameObject> mainBanners;

    [SerializeField] private List<GameObject> waitBanners;

    // list of images
    [SerializeField] private List<UnityEngine.UI.Image> playerImages = new List<UnityEngine.UI.Image>();

    public Color[] playerColors = new Color[4];

    public bool[] playerActive = new bool[4];

    [SerializeField] private Animator[] goPromptAnimators;

    // Start is called before the first frame update
    void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        foreach (UnityEngine.UI.Image image in instance.playerImages)
        {
            image.gameObject.GetComponent<Animator>().Play("SingleIconIdle");
        }

    }

    public static Color GetPlayerColor(int playerIndex){
        return instance.playerColors[playerIndex];
    }

    public static void SetTimer(float f){
        foreach (UnityEngine.UI.Image image in instance.timerImages)
        {
            image.fillAmount = f;
        }
    }
    public static void SetTimer(float current, float max)
    {
        float f = current / max;
        foreach (UnityEngine.UI.Image image in instance.timerImages)
        {
            image.fillAmount = f;
        }
    }

    public static void SetTimerVisible(bool visible){
        foreach (UnityEngine.UI.Image image in instance.timerImages)
        {
            image.enabled = visible;
        }
    }

    public static void SetWaitBannerVisible(bool visible){
        foreach (GameObject banner in instance.waitBanners)
        {
            banner.SetActive(visible);
        }

        foreach (GameObject banner in instance.mainBanners)
        {
            banner.SetActive(!visible);
        }
    }

    public static void SetSpecificTutorialVisible(int playerIndex, bool visible){
        instance.waitBanners[playerIndex].SetActive(visible);
        instance.mainBanners[playerIndex].SetActive(!visible);
    }



    public static void UpdatePlayerUI(int playerIndex, bool isPlayerActive){
        if (isPlayerActive)
                {
                    for (int j = 0; j < 4; j++){
                        instance.playerImages[playerIndex + (4*j)].color = new Color (1,1,1,1);
                        
                        instance.playerActive[playerIndex] = true;
                    }
                    
                }
                else{
                    for (int j = 0; j < 4; j++){
                        instance.playerImages[playerIndex + (4*j)].color = new Color (0.2f,0.2f,0.2f,1);
                        
                        instance.playerActive[playerIndex] = false;
                    }
                }
    }

    public static Vector2 GetPlayerUIPosition(int playerScreen, int playerIndex){
        return instance.playerImages[playerIndex + (4*playerScreen)].transform.position;
    }

    public static void PlayerUIFlash(int playerScreen, int playerIndex){
        instance.playerImages[playerIndex + (4*playerScreen)].GetComponent<Animator>().Play("Pulse");
    }

    public static void PlayGoPrompt(int playerIndex = -1){
        if (playerIndex >= 0 || playerIndex <= 3){
            instance.goPromptAnimators[playerIndex].SetTrigger("PlayPrompt");
        }
        else{
            for (int i = 0; i < instance.goPromptAnimators.Length; i++) 
            {
                instance.goPromptAnimators[i].SetTrigger("PlayPrompt");
            }
        }
        
    }
}
