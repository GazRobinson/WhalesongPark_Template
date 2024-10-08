using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using WhaleInput;

//Leaving this in for future maintainers
//I think the input system for this should mirror Unity's static "Input." for ease of use

public static class WhalesongInput
{
    private static WhaleInput.InputManager inputManager;
    public static void Initialise(WhaleInput.InputManager inMan)
    {
        if (inputManager == null)
        {
            inputManager = inMan;
        }
    }
    public enum WhaleButton
    {
        Up,
        Down,
        Left,
        Right,
        L_Button,
        R_Button
    }


    public static bool GetButtonDown(int PlayerID, WhaleButton btn)
    {
        if (PlayerID > 3)
        {
            Debug.LogError("PlayerID is " + PlayerID + ". It must be from 0-3.");
            return false;
        }
        return inputManager.GetButtonDown(PlayerID, btn);
    }
    public static bool GetButtonUp(int PlayerID, WhaleButton btn)
    {
        if (PlayerID > 3)
        {
            Debug.LogError("PlayerID is " + PlayerID + ". It must be from 0-3.");
            return false;
        }
        return inputManager.GetButtonUp(PlayerID, btn);
    }
    public static bool GetButton(int PlayerID, WhaleButton btn)
    {
        if (PlayerID > 3)
        {
            Debug.LogError("PlayerID is " + PlayerID + ". It must be from 0-3.");
            return false;
        }
        return inputManager.GetButton(PlayerID, btn);
    }
}

namespace WhaleInput
{
    public struct InputActions
    {
        public System.Action<int> APressed;
        public System.Action<int> ADown;
        public System.Action<int> AReleased;

        public System.Action<int> BPressed;
        public System.Action<int> BDown;
        public System.Action<int> BReleased;

        public System.Action<int, Vector2> DirectionInput;
    }

    public class InputManager : MonoBehaviour
    {
        [SerializeField] private PlayerInputController[] playerInput;
        private SingleComPortController Comtroller;

        public System.Action<bool[]> OnPortsVerified;

        public bool GetButtonDown(int player, WhalesongInput.WhaleButton button)
        {
            return playerInput[player].GetButtonDown(button);
        }
        public bool GetButtonUp(int player, WhalesongInput.WhaleButton button)
        {
            return playerInput[player].GetButtonUp(button);
        }
        public bool GetButton(int player, WhalesongInput.WhaleButton button)
        {
            return playerInput[player].GetButton(button);
        }

        public void Initialise(InputActions actions, bool debug)
        {
            WhalesongInput.Initialise(this);
            bool good = true;
            int[] btns = new int[24];
            string path = "";
#if !UNITY_EDITOR
            path = Application.dataPath + "/../../Buttons.txt";
#else
            path = Application.dataPath + "/Buttons.txt";
#endif
            string truePath = "";
            if (File.Exists(path))
            {
                truePath = path;
            }
            else if (File.Exists(Application.dataPath + "/../Buttons.txt"))
            {
                truePath = Application.dataPath + "/../Buttons.txt";
            }

            if (File.Exists(truePath))
            {
                var sr = new StreamReader(truePath);

                for (int i = 0; i < 24; i++)
                {
                    int res = -1;
                    good = int.TryParse(sr.ReadLine(), out res);

                    if (!good)
                        break;
                    else
                    {
                        if (res < 0 || res > 32)
                        {
                            good = false;
                            break;
                        }
                    }
                    btns[i] = res;
                }
                sr.Close();

            }
            else
            {
                good = false;
            }
            if (good)
            {
                Debug.Log("Inputs loaded successfully from " + truePath);
            }
            else
            {
                Debug.LogWarning("Inputs NOT loaded from " + truePath);
            }
            for (int i = 0; i < playerInput.Length; i++)
            {
                if (good)
                {
                    int[] bt = { btns[i * 6], btns[(i * 6) + 1], btns[(i * 6) + 2], btns[(i * 6) + 3], btns[(i * 6) + 4], btns[(i * 6) + 5] };
                    playerInput[i].Initialise(bt);
                }
                else
                {
                    print("Default input init");
                }
            }

            Comtroller = GetComponent<SingleComPortController>();
            for (int i = 0; i < playerInput.Length; i++)
            {
                playerInput[i].SetComtroller(ref Comtroller);
                playerInput[i].SetListeners(actions, i);
            }
            //TODO: Do this properly!

            bool[] playerPortsVerified = new bool[4] {true, true, true, true};
            OnPortsVerified(playerPortsVerified);

            if (debug)
            {
                InputDebugger ID = Instantiate<InputDebugger>(Resources.Load<InputDebugger>("InputDebugger"));
                ID.comtroller = Comtroller;
            }
        }

        private void Update()
        {
            for (int i = 0; i < playerInput.Length; i++)
            {
                playerInput[i].Update();
            }
        }
    }
}
