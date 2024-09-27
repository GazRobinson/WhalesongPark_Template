using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO.Ports;
using System.Threading;
using System.Text.RegularExpressions;
using System.Linq;

using UnityEngine.Events;

public class ComPortController : MonoBehaviour
{
    const int expectedPlayerCount = 4;

    public SerialThreadLines[] mouseSerialPorts;
    Thread[] mouseSerialThreads;

    public SerialThreadLines[] buttonSerialPorts;
    Thread[] buttonSerialThreads;

    bool[] buttonsAOn;
    bool[] buttonsBOn;

    List<SerialThreadLines> allComPorts;
    List<Thread> allComThreads;

    // [Player index][Event index]
    public List<System.Action<int>> onButtonAPressedEvents;
    public List<System.Action<int>> onButtonADownEvents;
    public List<System.Action<int>> onButtonAReleasedEvents;

    public List<System.Action<int>> onButtonBPressedEvents;
    public List<System.Action<int>> onButtonBDownEvents;
    public List<System.Action<int>> onButtonBReleasedEvents;

    public System.Action<bool[]> OnPortsVerified;

    public List<float> mouseData = new List<float>();

    public Vector2[] deltaMouse;

    public int searchIterations = 0;
    const int MAX_SEARCH_ITERATIONS = 1000;
    bool foundAllPorts = false;
    bool verifiedPorts = false;

    bool[] mousePortsFound;
    bool[] buttonPortsFound;


    private void Awake()
    {
        verifiedPorts = false;

        GetAllComPorts();
    }

    void GetAllComPorts()
    {
        mousePortsFound = new bool[4];
        buttonPortsFound = new bool[4];

        mouseSerialPorts = new SerialThreadLines[expectedPlayerCount];
        mouseSerialThreads = new Thread[expectedPlayerCount];

        buttonSerialPorts = new SerialThreadLines[expectedPlayerCount];
        buttonSerialThreads = new Thread[expectedPlayerCount];

        deltaMouse = new Vector2[expectedPlayerCount];

        buttonsAOn = new bool[expectedPlayerCount];
        buttonsBOn = new bool[expectedPlayerCount];

        allComPorts = new List<SerialThreadLines>();
        allComThreads = new List<Thread>();

        onButtonAPressedEvents = new List<System.Action<int>>();
        onButtonADownEvents = new List<System.Action<int>>();
        onButtonAReleasedEvents = new List<System.Action<int>>();

        onButtonBPressedEvents = new List<System.Action<int>>();
        onButtonBDownEvents = new List<System.Action<int>>();
        onButtonBReleasedEvents = new List<System.Action<int>>();

        foreach (string portName in SerialPort.GetPortNames())
        {
            Debug.Log("Found system serial port: " + portName);
            SerialThreadLines devicePort = new SerialThreadLines(portName, 115200, 1000, 10);
            Thread thread = new Thread(new ThreadStart(devicePort.RunForever));
            thread.Start();

            allComPorts.Add(devicePort);
            allComThreads.Add(thread);
        }
    }
    private void Update()
    {
        if (!foundAllPorts && searchIterations <= MAX_SEARCH_ITERATIONS)
        {
            foundAllPorts = CheckIfAllInputPortsFound();
            for (int i = 0; i < allComPorts.Count; i++)
            {
                if (!mouseSerialPorts.Contains(allComPorts[i]) && !buttonSerialPorts.Contains(allComPorts[i]))
                {
                    string message = (string)allComPorts[i].ReadMessage();
                    //Debug.Log(message);
                    if (message != null)
                    {
                        switch (message[0])
                        {
                            case 'M':
                                {
                                    // Mouse input COM port
                                    int playerNum = -1;
                                    int.TryParse(message[1].ToString(), out playerNum);
                                    Debug.Log("Player num found: " + playerNum);
                                    if (playerNum != -1 && playerNum > 0 && playerNum <= expectedPlayerCount)
                                    {
                                        Debug.Log("Found mouse");
                                        mouseSerialPorts[playerNum - 1] = allComPorts[i];
                                        mouseSerialThreads[playerNum - 1] = allComThreads[i];
                                        mousePortsFound[playerNum - 1] = true;
                                    }
                                }
                                break;
                            case 'B':
                                {
                                    // Button input COM port
                                    int playerNum = -1;
                                    int.TryParse(message[1].ToString(), out playerNum);
                                    if (playerNum != -1 && playerNum > 0 && playerNum <= expectedPlayerCount)
                                    {
                                        Debug.Log("Found buttons");
                                        buttonSerialPorts[playerNum - 1] = allComPorts[i];
                                        buttonSerialThreads[playerNum - 1] = allComThreads[i];
                                        buttonPortsFound[playerNum - 1] = true;
                                    }
                                }
                                break;
                        }
                    }
                }
            }

            searchIterations++;
        }
        else
        {
            for (int i = 0; i < mouseSerialPorts.Length; i++)
            {
                if (mouseSerialPorts[i] != null)
                {
                    string message = (string)mouseSerialPorts[i].ReadMessage();
                    if (message != null)
                    {
                        //Debug.Log("Mouse message: " + message);
                        int indicatorIndex = message.IndexOf('X');
                        int deltaX = 0;
                        string sampleMessage = message.Substring(indicatorIndex + 2, 4);
                        sampleMessage = Regex.Replace(sampleMessage, "[^0-9.+-]", "");
                        bool parsedSuccessfully = int.TryParse(sampleMessage, out deltaX);
                        if (!parsedSuccessfully)
                        {
                            Debug.Log("Parse failed: " + sampleMessage);
                        }

                        indicatorIndex = message.IndexOf('Y');
                        int deltaY = 0;
                        int dstToEnd = message.Length - (indicatorIndex + 2);
                        sampleMessage = message.Substring(indicatorIndex + 2, dstToEnd);
                        //sampleMessage = Regex.Replace(sampleMessage, "[^0-9.+-]", "");
                        parsedSuccessfully = int.TryParse(sampleMessage, out deltaY);
                        if (!parsedSuccessfully)
                        {
                            Debug.Log("Parse failed: " + sampleMessage);
                        }

                        //Debug.Log("DeltaX: " + deltaX + ", DeltaY: " + deltaY);

                        float diff = deltaX - deltaMouse[i].x;
                        mouseData.Add(diff);

                        //if (deltaX > 10)
                        //    deltaX = 1;
                        //else if (deltaX < -10)
                        //    deltaX = -1;

                        //if (deltaY > 10)
                        //    deltaY = 1;
                        //else if (deltaY < -10)
                        //    deltaY = -1;

                        deltaMouse[i] = new Vector2(deltaX / 127.0f, deltaY / 127.0f);
                    }
                    //else 
                    //{
                    //    deltaMouse[i] = Vector2.zero;
                    //}
                }
                else 
                {
                    deltaMouse[i] = Vector2.zero;
                }
            }

            for (int i = 0; i < buttonSerialPorts.Length; i++)
            {
                if (buttonSerialPorts[i] != null)
                {
                    string message = (string)buttonSerialPorts[i].ReadMessage();
                    if (message != null && message.Length == 8)
                    {
                        int indicatorIndex = message.IndexOf('C');
                        int buttonAValue = 0;
                        string sampleMessage = message[indicatorIndex + 2].ToString();
                        bool parsedSuccessfully = int.TryParse(sampleMessage, out buttonAValue);
                        if (!parsedSuccessfully)
                        {
                            Debug.Log("Parse failed: " + sampleMessage);
                        }

                        if (buttonAValue == 1 && parsedSuccessfully)
                        {
                            // Button A down
                            for (int j = 0; j < onButtonADownEvents.Count; j++)
                                onButtonADownEvents[j].Invoke(i);

                            if (!buttonsAOn[i])
                            {
                                // Button A on click
                                for (int j = 0; j < onButtonAPressedEvents.Count; j++)
                                    onButtonAPressedEvents[j].Invoke(i);
                            }
                        }
                        else if (buttonsAOn[i])
                        {
                            // Button A released
                            for (int j = 0; j < onButtonAReleasedEvents.Count; j++)
                                onButtonAReleasedEvents[j].Invoke(i);
                        }

                        buttonsAOn[i] = (buttonAValue == 1);


                        indicatorIndex = message.IndexOf('D');
                        int buttonBValue = 0;
                        sampleMessage = message[indicatorIndex + 2].ToString();
                        parsedSuccessfully = int.TryParse(sampleMessage, out buttonBValue);
                        if (!parsedSuccessfully)
                        {
                            Debug.Log("Parse failed: " + sampleMessage);
                        }

                        Debug.Log(message);
                        Debug.Log("Button b value: " + buttonBValue);
                        if (buttonBValue == 1 && parsedSuccessfully)
                        {
                            // Button  B down
                            for (int j = 0; j < onButtonBDownEvents.Count; j++)
                                onButtonBDownEvents[j](i);

                            if (!buttonsBOn[i])
                            {
                                // Button B on click
                                for (int j = 0; j < onButtonBPressedEvents.Count; j++)
                                    onButtonBPressedEvents[j](i);
                            }
                        }
                        else if (buttonsBOn[i])
                        {
                            // Button B released
                            for (int j = 0; j < onButtonBReleasedEvents.Count; j++)
                                onButtonBReleasedEvents[j](i);
                        }

                        buttonsBOn[i] = (buttonBValue == 1);
                    }
                }
            }

            if (!verifiedPorts) 
            {
                verifiedPorts = true;
                VerifyAllPlayerPorts();
                Debug.Log("Verified all ports");
            }

            //if (!verifiedPorts)
            //{
            //    bool[] playerPortStates = new bool[4];
            //    for (int j = 0; j < 4; j++)
            //    {
            //        playerPortStates[j] = CheckPortsForPlayer(j);
            //    }
            //    gameManager.VerifyPorts(playerPortStates);
            //}

        }
    }

    private void VerifyAllPlayerPorts() 
    {
        bool[] playerPortsVerified = new bool[4];
        for (int i = 0; i < 4; i++) 
        {
            playerPortsVerified[i] = (mousePortsFound[i] && buttonPortsFound[i]);
        }
        if (OnPortsVerified != null)
        {
            OnPortsVerified(playerPortsVerified);
        }
    }

    private void OnDestroy()
    {
        Debug.Log("Destroyed script");
        for (int i = 0; i < allComPorts.Count; i++)
        {
            if (allComPorts[i] != null)
                CloseSerialPort(allComPorts[i]);
            if (allComThreads[i] != null)
                CloseThread(allComThreads[i]);
        }
    }

    bool CheckPortsForPlayer(int index) 
    {
        if (mouseSerialPorts[index] == null)
        {
            return false;
        }
        else if ((string)mouseSerialPorts[index].ReadMessage() == null)
        {
            return false;
        }

        if (buttonSerialPorts[index] == null)
        {
            return false;
        }
        else if ((string)buttonSerialPorts[index].ReadMessage() == null)
        {
            return false;
        }

        return true;
    }

    bool CheckIfAllInputPortsFound()
    {
        bool allPortsFound = true;
        for (int i = 0; i < mouseSerialPorts.Length; i++)
        {
            if (mouseSerialPorts[i] == null)
            {
                allPortsFound = false;
            }
            else if ((string)mouseSerialPorts[i].ReadMessage() == null)
            {
                allPortsFound = false;
            }
        }

        for (int i = 0; i < buttonSerialPorts.Length; i++)
        {
            if (buttonSerialPorts[i] == null)
            {
                allPortsFound = false;
            }
            else if ((string)buttonSerialPorts[i].ReadMessage() == null)
            {
                allPortsFound = false;
            }
        }
        //Debug.Log("All ports found: " + allPortsFound);
        return allPortsFound;
    }

    bool FoundPortsForPlayer(int playerIndex) 
    {
        bool allPortsFound = true;
        if (mouseSerialPorts[playerIndex] == null)
        {
            allPortsFound = false;
        }
        else if ((string)mouseSerialPorts[playerIndex].ReadMessage() == null)
        {
            allPortsFound = false;
        }

        if (buttonSerialPorts[playerIndex] == null)
        {
            allPortsFound = false;
        }
        else if ((string)buttonSerialPorts[playerIndex].ReadMessage() == null)
        {
            allPortsFound = false;
        }

        return allPortsFound;
    }

    void CloseSerialPort(SerialThreadLines serialThread)
    {
        if (serialThread != null)
        {
            serialThread.RequestStop();
            serialThread = null;
        }
    }

    void CloseThread(Thread thread)
    {
        if (thread != null)
        {
            thread.Join();
            thread = null;
        }
    }
}
