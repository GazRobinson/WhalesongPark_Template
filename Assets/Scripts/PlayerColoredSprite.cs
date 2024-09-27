using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerColoredSprite : MonoBehaviour
{
    // Start is called before the first frame update
    public void SetColour(int player)
    {
        Color c = PlayerUtilities.GetPlayerColor(player);
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr)
            {
                sr.color = c;
                Destroy(this);
                return;
            }
            UnityEngine.UI.Image img = GetComponent<UnityEngine.UI.Image>();
            if (img)
            {
                img.color = c;
                Destroy(this);
                return;
            }
            TextMeshPro tmp = GetComponent<TextMeshPro>();
            if (tmp)
            {
                tmp.color = c;
                Destroy(this);
                return;
            }
            TextMeshProUGUI tmpUI = GetComponent<TextMeshProUGUI>();
            if (tmpUI)
            {
                tmpUI.color = c;
                Destroy(this);
                return;
            }
        }
    }
}
