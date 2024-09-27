using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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

        public void Initialise(InputActions actions, bool debug)
        {            
            print(Application.dataPath);
            bool good = true;
            int[] btns = new int[24];
            if (File.Exists(Application.dataPath + "/Buttons.txt"))
            {
                var sr = new StreamReader(Application.dataPath + "/Buttons.txt");

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
                Debug.Log("Inputs loaded successfully!");
            }
            else
            {
                Debug.Log("Inputs NOT loaded!");
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
