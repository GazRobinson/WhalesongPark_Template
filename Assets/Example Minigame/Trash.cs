using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    private Vector2 m_Velocity = Vector2.zero;
    private bool m_InWater = false;
    public float m_Gravity = -9.8f;
    public float m_WaterGravityMultiplier = 0.3f;
    public float m_XVelDecay = 1.0f;
    public int m_ScreenID { get; private set; } = -1;
    private float m_WaterLevel = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if (!m_InWater)
        {
            m_Velocity.y += m_Gravity * Time.deltaTime;
        }
        else
        {
            m_Velocity.y += m_Gravity * 0.3f * Time.deltaTime;
            m_Velocity.x = Mathf.MoveTowards(m_Velocity.x, 0.0f, Time.deltaTime * m_XVelDecay);
        }

        transform.position += (Vector3)m_Velocity * Time.deltaTime;

        if(!m_InWater && transform.position.y < m_WaterLevel)
        {
            HitWater();
        }

        if(!ScreenUtility.IsOnScreen(transform.position, -1))
        {
            gameObject.SetActive(false);
        }
    }

    public void Launch(Vector2 position, int screenID)
    {
        m_ScreenID = screenID;

        m_WaterLevel = position.y;
        transform.position = position + Vector2.up * 0.25f;
        m_Velocity = new Vector2(Random.Range(-2.0f, 2.0f), 2.0f);
        m_InWater = false;
        gameObject.SetActive(true);
    }
    public void HitWater()
    {
        m_InWater = true;
        m_Velocity.y = m_Velocity.y * 0.1f;
    }
}
