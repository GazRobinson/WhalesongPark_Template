using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SimpleExampleGame {

    public class SimpleExampleGame : MinigameBase
    {
        [Header("Game Specific variables")]
        public Bullet pf_Bullet;
        public Trash pf_Trash;

        [SerializeField] private PlayerFish[] m_Players;
        [SerializeField] private List<Enemy> m_Enemies;
        private List<Bullet> m_Bullets;
        private List<Trash> m_Trash;
        private int[] m_Scores;

        private void Awake()
        {
            MinigameLoaded.AddListener(InitialiseGame);
        }

        public override void OnDirectionalInput(int playerIndex, Vector2 direction)
        {
            m_Players[playerIndex].HandleDirectionalInput(direction);
        }

        public override void OnPrimaryFire(int playerIndex)
        {
            m_Players[playerIndex].HandleButtonInput(0);
        }

        public override void OnSecondaryFire(int playerIndex)
        {
            m_Players[playerIndex].HandleButtonInput(1);
        }

        public override void TimeUp()
        {
            OnGameComplete(true);
        }

        protected override void OnResetGame()
        {
            //InitialiseGame();
        }

        public override GameScoreData GetScoreData()
        {
            int teamTime = 0;
            GameScoreData gsd = new GameScoreData();
            for (int i = 0; i < 4; i++)
            {
                if (PlayerUtilities.GetPlayerState(i) == Player.PlayerState.ACTIVE)
                {
                    gsd.PlayerScores[i] = m_Scores[i];
                    gsd.PlayerTimes[i] = Mathf.Min(m_Scores[i]/2, 1);
                    teamTime += gsd.PlayerTimes[i];
                }
            }
            gsd.ScoreSuffix = " cleaned";
            gsd.TeamTime = teamTime;
            return gsd;
        }

        public void InitialiseGame()
        {
            Debug.Log("Initialising mini game");

            m_Scores = new int[4]{0,0,0,0};


            //Build pool of bullets
            m_Bullets = new List<Bullet>();
            Bullet b;
            for (int i =0; i < 50; i++)
            {
                b = Instantiate<Bullet>(pf_Bullet);
                b.gameObject.SetActive(false);
                m_Bullets.Add(b);
            }

            //Build pool of trash
            m_Trash = new List<Trash>();
            Trash trash;
            for (int i = 0; i < 10; i++)
            {
                trash = Instantiate<Trash>(pf_Trash);
                trash.gameObject.SetActive(false);
                m_Trash.Add(trash);
            }

            for (int i = 0; i < m_Players.Length; i++)
            {
                m_Players[i].GetBullet = GetBullet;
                m_Players[i].m_ScreenID = i;
            }
            for (int i = 0; i < m_Enemies.Count; i++)
            {
                m_Enemies[i].GetTrash = GetTrash;
                m_Enemies[i].m_ScreenID = i;
            }
        }

        Bullet GetBullet()
        {
            Bullet returnBullet = null;
            for (int i = 0; i < m_Bullets.Count; i++)
            {
                if (m_Bullets[i].gameObject.activeSelf)
                    continue;
                returnBullet = m_Bullets[i];
            }
            return returnBullet;
        }
        Trash GetTrash()
        {
            Trash returnTrash = null;
            for (int i = 0; i < m_Trash.Count; i++)
            {
                if (m_Trash[i].gameObject.activeSelf)
                    continue;
                returnTrash = m_Trash[i];
            }
            return returnTrash;
        }

        private void FixedUpdate()
        {
            foreach (Trash t in m_Trash)
            {
                if (!t.gameObject.activeSelf)
                    continue;
                foreach (Bullet b in m_Bullets)
                {
                    if (!b.gameObject.activeSelf)
                        continue;
                    if(Vector3.SqrMagnitude(b.transform.position - t.transform.position) < 0.25f)
                    {
                        m_Scores[t.m_ScreenID]++;
                        b.gameObject.SetActive(false);
                        t.gameObject.SetActive(false);
                        break;
                    }
                }
            }
        }
    }
}
