using UnityEngine;

namespace MakeMeLaugh
{
    public class CombineObjectScript : ResourceScript
    {
        public float animateSpeed = 0f;
        public float baseRadius = 0f;
        public float radiusLimit = 0f;
        public float growRadius = 1f;
        public float eliminateRadius = 1f;
        public AudioClip growAudio = null;
        public float growPitch = 1f;
        public AudioClip eliminateAudio = null;

        private float m_currentRadius = 0f;
        private float m_nextRadius = 0f;

        private bool m_full = false;

        private void Start()
        {
            transform.localScale = Vector3.zero;
            m_currentRadius = 0f;
            m_nextRadius = 0f;
        }

        private void Update()
        {
            if (m_full)
            {
                return;
            }

            if (m_currentRadius != m_nextRadius)
            {
                if (m_currentRadius < m_nextRadius)
                {
                    m_currentRadius += Time.deltaTime * animateSpeed;
                    if (m_currentRadius >= m_nextRadius)
                    {
                        m_currentRadius = m_nextRadius;

                        if (radiusLimit > 0f)
                        {
                            if (m_currentRadius >= radiusLimit)
                            {
                                m_full = true;
                                CoreManager.Instance.endingReady();
                            }
                        }
                    }
                }
                else
                {
                    m_currentRadius -= Time.deltaTime * animateSpeed;
                    if (m_currentRadius <= m_nextRadius)
                    {
                        m_currentRadius = m_nextRadius;
                    }
                }
                transform.localScale = new Vector3(m_currentRadius, m_currentRadius, m_currentRadius);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            ObjectScript obj = collision.GetComponentInParent<ObjectScript>();
            if (obj != null)
            {
                modifyNextRadiusByObject();
                GameObject.Destroy(obj.gameObject);
                return;
            }
            ScoreScript score = collision.GetComponentInParent<ScoreScript>();
            if (score != null)
            {
                modifyNextRadiusByScore(score.point);
                GameObject.Destroy(score.gameObject);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {

        }

        private void modifyNextRadiusByObject()
        {
            if (m_nextRadius < baseRadius)
            {
                m_nextRadius = baseRadius;
            }
            else
            {
                m_nextRadius += growRadius;
            }
            AudioManager.Instance.play(growAudio, growPitch);
        }

        private void modifyNextRadiusByScore(int point)
        {
            m_nextRadius -= eliminateRadius * point;
            if (m_nextRadius < 0f)
            {
                m_nextRadius = 0f;
            }
            //AudioManager.Instance.play(eliminateAudio);
        }
    }
}
