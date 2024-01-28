using System.Collections.Generic;
using UnityEngine;

namespace MakeMeLaugh
{
    public class AlexScript : ResourceScript
    {
        public static readonly string IDKey = "alex_id";
        public static readonly int AgeBase = 35;
        public static readonly long HourMS = (long)60 * 60 * 1000;

        public NumberScript idLabel = null;
        public Animation headAnimation = null;
        public Vector2 turnIntervals = Vector2.zero;
        public Animation eyeAnimation = null;
        public Vector2 blinkInternals = Vector2.zero;
        public GameObject inside = null;
        public List<SpriteRenderer> insideSprites = new List<SpriteRenderer>();
        public List<Animation> insideAnimations = new List<Animation>();
        public HeartBeatScript heartBeat = null;
        public Transform combineContainer = null;

        private int m_id = 0;
        private long m_bornTime = 0;

        private bool m_insideShowing = false;
        private bool m_insideShown = false;
        private float m_turnIntervalLeft = 0f;
        private float m_blinkIntervalLeft = 0f;

        private void Start()
        {
            if (PlayerPrefs.HasKey(IDKey))
            {
                m_id = PlayerPrefs.GetInt(IDKey);
            }
            if (m_id <= 0)
            {
                m_id = 1;
            }
            m_bornTime = getNow();

            int count = insideSprites.Count;
            for (int i = 0; i < count; i++)
            {
                insideSprites[i].color = new Color(1, 1, 1, 0);
            }
            m_turnIntervalLeft = Random.Range(turnIntervals.x, turnIntervals.y);
            m_blinkIntervalLeft = Random.Range(blinkInternals.x, blinkInternals.y);

            idLabel.setNumber(getID());
        }

        private void Update()
        {
            updateTurn();
            updateBlink();
            updateInside();
        }

        public int getID()
        {
            return m_id;
        }

        private long getNow()
        {
            System.TimeSpan passed = System.DateTime.Now - new System.DateTime(1970, 1, 1, 0, 0, 0);
            return (long)passed.TotalMilliseconds;
        }

        public string getAge()
        {
            long now = getNow();
            long passed = now - m_bornTime;
            int day = (int)(passed / HourMS);
            int age = AgeBase;
            if (day > 0)
            {
                age += day;
            }
            long left = passed - HourMS * day;
            float percentF = left / (float)HourMS;
            int percentI = (int)(percentF * 1000);
            string percentS = percentI.ToString();
            while (percentS.Length < 2) percentS = string.Format("0{0}", percentS);
            return string.Format("{0}.{1}", age, percentI);
        }

        private void updateTurn()
        {
            if (m_insideShown) return;
            m_turnIntervalLeft -= Time.deltaTime;
            if (m_turnIntervalLeft > 0f) return;
            m_turnIntervalLeft = Random.Range(turnIntervals.x, turnIntervals.y);
            int rnd = Random.Range(0, 100);
            if (rnd % 2 == 0)
            {
                headAnimation.Play("turn_left");
            }
            else
            {
                headAnimation.Play("turn_right");
            }
        }

        private void updateBlink()
        {
            m_blinkIntervalLeft -= Time.deltaTime;
            if (m_blinkIntervalLeft > 0f) return;
            m_blinkIntervalLeft = Random.Range(blinkInternals.x, blinkInternals.y);
            int rnd = Random.Range(0, 100);
            if (rnd % 3 == 0)
            {
                eyeAnimation.Play("blink_twice");
            }
            else
            {
                eyeAnimation.Play("blink");
            }
        }

        public void showInside()
        {
            if (!inside.activeSelf) inside.SetActive(true);
            m_insideShowing = true;
            m_insideShown = true;
            int count = insideAnimations.Count;
            for (int i = 0; i < count; i++)
            {
                insideAnimations[i].Play("show");
            }
        }

        public void hideInside()
        {
            heartBeat.stop();
            m_insideShown = false;
            int count = insideAnimations.Count;
            for (int i = 0; i < count; i++)
            {
                insideAnimations[i].Play("hide");
            }
        }

        private void updateInside()
        {
            if (m_insideShowing)
            {
                bool done = true;
                int count = insideAnimations.Count;
                for (int i = 0; i < count; i++)
                {
                    if (insideAnimations[i].isPlaying)
                    {
                        done = false;
                        break;
                    }
                }
                if (done)
                {
                    m_insideShowing = false;
                    heartBeat.play();
                }
            }
        }

        public void setDead()
        {
            PlayerPrefs.SetInt(IDKey, m_id + 1);
            heartBeat.stop();
        }
    }
}
