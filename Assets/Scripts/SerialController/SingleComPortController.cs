using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO.Ports;
using System.Threading;
using System.Text.RegularExpressions;
using System.Linq;

using UnityEngine.Events;

//Btn - COM
// 00 -  37  
// 01 -  36
// 02 -  35
// 03 -  34
// 04 -  33
// 05 -  32
// 06 -  31
// 07 -  30
// 08 -  53
// 09 -  52
// 10 -  51
// 11 -  50
// 12 -  10
// 13 -  11
// 14 -  12
// 15 -  13
// 16 -  22
// 17 -  23
// 18 -  24
// 19 -  25
// 20 -  26
// 21 -  27
// 22 -  28
// 23 -  29

public class SingleComPortController : MonoBehaviour
{
    const int expectedPlayerCount = 4;

    public  SerialThreadLines gameSerialPort = null;
    private Thread gameSerialThread = null;

    private List<SerialThreadLines> allComPorts;
    private List<Thread> allComThreads;

    // [Player index][Event index]
    public List<System.Action<int>> onButtonAPressedEvents;
    public List<System.Action<int>> onButtonADownEvents;
    public List<System.Action<int>> onButtonAReleasedEvents;

    public List<System.Action<int>> onButtonBPressedEvents;
    public List<System.Action<int>> onButtonBDownEvents;
    public List<System.Action<int>> onButtonBReleasedEvents;

    public System.Action onControllerVerified;

    public int searchIterations = 0;
    const int MAX_SEARCH_ITERATIONS = 1000;
    bool foundAllPorts = false;
    bool verifiedPorts = false;

    bool[] lastUpdateButtons = new bool[24];
    bool[] buttons = new bool[24];

    public bool ButtonPressed(int button)
    {
        return (buttons[button] && !lastUpdateButtons[button]);
    }
    public bool ButtonReleased(int button)
    {
        return (!buttons[button] && lastUpdateButtons[button]);
    }
    public bool ButtonHeld(int button)
    {
        return buttons[button];
    }
    public bool AnyButton()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] && !lastUpdateButtons[i])
                return true;
        }
        return false;
    }

    public string ButtonTester()
    {
        string returnString = "";
        for(int i=0; i < buttons.Length; i++)
        {
            if (buttons[i])
                returnString += i.ToString() + ", ";
        }
        return returnString;
    }

    //GameManager gameManager;

    private void Awake()
    {
        //gameManager = GetComponent<GameManager>();
        verifiedPorts = false;

        GetAllComPorts();
    }

    void GetAllComPorts()
    {
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
    private bool AttemptGetController()
    {
        for (int i = 0; i < allComPorts.Count; i++)
        {
            string message = (string)allComPorts[i].ReadMessage();
            if (message != null && message.Length == 32 && message.Substring(0, 8) == "00000000")
            {
                Debug.Log("Found the controller");
                gameSerialPort = allComPorts[i];
                gameSerialThread = allComThreads[i];

                //Remove the other Com listeners
                int index = 0;
                for (int j = 0; j < allComPorts.Count;)
                {
                    if (index != i)
                    {
                        if (allComPorts[j] != null)
                            CloseSerialPort(allComPorts[j]);
                        if (allComThreads[j] != null)
                            CloseThread(allComThreads[j]);
                        allComPorts.RemoveAt(j);
                        index++;
                    }
                    else
                    {
                        j++;
                        index++;
                        continue;
                    }
                }
                return true;
            }
        }
        return false;
    }
    private void Update()
    {
        //Copy last frame's buttons for UP/DOWN comparison
        for (int i = 0; i < 24; i++)
        {
            lastUpdateButtons[i] = buttons[i];
        }

        //If the controller (Arduino) has not yet been identified, find it
        if (!foundAllPorts && searchIterations <= MAX_SEARCH_ITERATIONS)
        {
            foundAllPorts = AttemptGetController();            
            searchIterations++;
        }
        else
        {
            if (foundAllPorts)
            {
                bool DoRead = true;
                while (DoRead)
                {
                    string msg = (string)gameSerialPort.ReadMessage();
                    if (msg != null)
                    {
                        if (msg.Length == 32) {
                            System.Int32 value = System.Convert.ToInt32(msg, 2);

                            //Debug.Log(msg + " : " + value.ToString());

                            for (int i = 0; i < 24; i++)
                            {
                                buttons[i] = (value & (1 << i)) > 0;
                            } 
                        }
                    }
                    else
                    {
                        DoRead = false;
                    }
                }
            }

            if (!verifiedPorts)
            {
                VerifyAllPlayerPorts();
            }
        }
    }

    private void VerifyAllPlayerPorts()
    {
        verifiedPorts = true;
        //TODO: This might have to verify four players - check ComPortController for old behaviour
        if (onControllerVerified != null)
        {
            onControllerVerified();
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < allComPorts.Count; i++)
        {
            if (allComPorts[i] != null)
                CloseSerialPort(allComPorts[i]);
            if (allComThreads[i] != null)
                CloseThread(allComThreads[i]);
        }
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
