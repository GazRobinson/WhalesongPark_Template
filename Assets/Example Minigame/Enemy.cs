using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public System.Func<Trash> GetTrash;
    public float m_MoveSpeed = 2.0f;
    public float m_MoveWidth = 2.0f;

    public float m_TimeBetweenSpawns = 4.0f;

    public int m_ScreenID = -1;

    private float m_NextSpawn = 4.0f;
    private Vector3 m_InitialPosition = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        m_InitialPosition = transform.position;
        m_NextSpawn = Time.time + m_TimeBetweenSpawns;
    }

    // Update is called once per frame
    void Update()
    {
        //Move left and right
        transform.position = m_InitialPosition + new Vector3( Mathf.Sin(Time.time * m_MoveSpeed) * m_MoveWidth,0.0f, 0.0f );

        if(Time.time > m_NextSpawn)
        {
            LaunchTrash();
        }
    }

    void LaunchTrash()
    {
        Trash trash = GetTrash();
        if (trash != null)
        {
            trash.Launch(transform.position, m_ScreenID);
            m_NextSpawn = Time.time + m_TimeBetweenSpawns;
        }
        else
        {
            //We didn't get a trash. Wait half a second before trying again.
            m_NextSpawn = Time.time + 0.5f;
        }
    }
}
