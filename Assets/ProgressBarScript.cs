using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarScript : MonoBehaviour
{
    RectTransform[] TravelPoints;
    RectTransform[] Ships;

    [SerializeField]
    float TestPercent;

    private void Update()
    {
        if (TravelPoints != null)
        {
            SetShipPositionPercent(TestPercent, 0);
            SetShipPositionPercent(TestPercent * 0.75f, 3);
            SetShipPositionPercent(TestPercent * 0.5f, 2);
            SetShipPositionPercent(TestPercent * 0.25f, 1);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Transform PointsParent = transform.GetChild(0);

        TravelPoints = new RectTransform[PointsParent.childCount];

        for (int i = 0; i < PointsParent.childCount; i++)
        {
            TravelPoints[i] = PointsParent.GetChild(i).gameObject.GetComponent<RectTransform>();
        }

        Debug.Log("Travel Points: " + TravelPoints.Length);

        Transform ShipsParent = transform.GetChild(1);

        Ships = new RectTransform[4];

        for (int i = 0; i < 4; i++)
        {
            Ships[i] = ShipsParent.GetChild(i).gameObject.GetComponent<RectTransform>();
        }
    }
    
    public void SetShipPositionPercent(float PercentIn, int ShipIDIn)
    {
        PercentIn = Mathf.Clamp(PercentIn, 0.0f, 1.0f - 0.01f);

        int Paths = TravelPoints.Length - 1;

        float PercentCut = 1.0f / (float)Paths;

        float CPathDiv = PercentIn / PercentCut;

        int CPath = Mathf.FloorToInt(CPathDiv);

        float PathPercent = CPathDiv % 1.0f;

        Ships[ShipIDIn].position = Vector3.Lerp(TravelPoints[CPath].position, TravelPoints[CPath + 1].position, PathPercent);
    }

}
