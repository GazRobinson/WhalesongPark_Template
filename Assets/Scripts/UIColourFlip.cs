using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIColourFlip : MonoBehaviour
{
    [SerializeField] private Image leftHand;
    [SerializeField] public Image rightHand;
    [SerializeField] private Image leftButton;
    [SerializeField] public Image rightButton;

    [SerializeField] public Color flipColour;
    private Color blankColour;

    void Awake(){
        blankColour = GetComponent<Image>().color;

        leftHand = gameObject.transform.GetChild(2).GetComponent<Image>();
        rightHand = gameObject.transform.GetChild(3).GetComponent<Image>();
        leftButton = gameObject.transform.GetChild(0).GetComponent<Image>();
        rightButton = gameObject.transform.GetChild(1).GetComponent<Image>();

        rightButton.color = flipColour;
        rightHand.color = flipColour;
    }

    public void FlipLeftButton()
    {
        if (leftButton.color == flipColour)
            leftButton.color = blankColour;
        else
            leftButton.color = flipColour;
    }

    public void FlipLeftHand()
    {
        if (leftHand.color == flipColour)
            leftHand.color = blankColour;
        else
            leftHand.color = flipColour;
    }

    public void FlipRightButton()
    {
        if (rightButton.color == flipColour)
            rightButton.color = blankColour;
        else
            rightButton.color = flipColour;
    }

    public void FlipRightHand()
    {
        if (rightHand.color == flipColour)
            rightHand.color = blankColour;
        else
            rightHand.color = flipColour;
    }

    public void FlipAll()
    {
        FlipLeftButton();
        FlipLeftHand();
        FlipRightButton();
        FlipRightHand();
    }
}
