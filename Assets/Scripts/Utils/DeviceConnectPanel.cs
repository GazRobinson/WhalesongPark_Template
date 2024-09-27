using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeviceConnectPanel : MonoBehaviour
{
    public ComPortController comPortController;

    public Image[] rollerballLights;
    public Image[] buttonLights;

    public Color disconnectedColour;
    public Color connectedColour;

    public GameObject panel;

    bool[] rollOn = new bool[4];
    bool[] buttonOn = new bool[4];

    public TMPro.TMP_Text iterText;

    private void Awake()
    {
        rollOn = new bool[]{ false, false, false, false };
        buttonOn = new bool[] { false, false, false, false };
    }

    private void Update()
    {
        if (comPortController != null) 
        {
            iterText.SetText(comPortController.searchIterations.ToString());

            for (int i = 0; i < 4; i++) 
            {
                if (comPortController.mouseSerialPorts != null)
                {
                    if (i < comPortController.mouseSerialPorts.Length)
                    {
                        if (!rollOn[i])
                        {
                            if (comPortController.mouseSerialPorts[i] != null && comPortController.mouseSerialPorts[i].ReadMessage() != null)
                            {
                                rollOn[i] = true;
                            }
                        }
                    }
                }

                if (rollOn[i])
                    rollerballLights[i].color = connectedColour;
                else
                    rollerballLights[i].color = disconnectedColour;

                if (comPortController.buttonSerialPorts != null)
                {
                    if (i < comPortController.buttonSerialPorts.Length)
                    {
                        if (!buttonOn[i])
                        {
                            if (comPortController.buttonSerialPorts[i] != null && comPortController.buttonSerialPorts[i].ReadMessage() != null)
                            {
                                buttonOn[i] = true;
                            }
                        }
                    }
                }

                if (buttonOn[i])
                    buttonLights[i].color = connectedColour;
                else
                    buttonLights[i].color = disconnectedColour;
            }
        }

        if (Input.GetKeyDown(KeyCode.D)) 
        {
            panel.SetActive(!panel.activeInHierarchy);
        }
    }
}
