using UnityEngine;

namespace MakeMeLaugh
{
    public class ScoreScript : ResourceScript
    {
        public int point = 0;
        public float speed = 0f;
        public float curveWeight = 0.5f;
        public Vector2 curveAngleRange = new Vector2(90f, 100f);

        private bool m_isPlaying = false;
        private Vector3 m_destPoint = Vector3.zero;
        private float m_distance = 0;
        private float m_side = 0;
        private float m_movedDistance = 0f;
        private float m_startAngle = 0f;

        public void play(Vector3 point)
        {
            m_isPlaying = true;
            m_destPoint = point;
            m_distance = Vector3.Distance(m_destPoint, transform.position);
            m_side = Random.Range(0, 100) % 2 == 0 ? 1 : -1;
            m_movedDistance = 0f;
            m_startAngle = Random.Range(curveAngleRange.x, curveAngleRange.y);
        }

        public void stop()
        {
            if (!m_isPlaying) return;
            m_isPlaying = false;
            GameObject.Destroy(gameObject);
        }

        private void Update()
        {
            if (!m_isPlaying) return;
            Vector3 pos = transform.position;
            Vector3 dir = m_destPoint - pos;
            float percent = m_movedDistance / m_distance;
            if (percent <= curveWeight)
            {
                percent = percent / curveWeight;
                float angle = Mathf.Lerp(m_startAngle, 0f, percent) * m_side;
                if (angle % 360 != 0)
                {
                    float dist = dir.magnitude;
                    float centerAngle = Vector2.SignedAngle(dir.normalized, Vector2.up);
                    float targetAngle = centerAngle + angle;
                    dir.x = Mathf.Sin(targetAngle * Mathf.Deg2Rad) * dist;
                    dir.y = Mathf.Cos(targetAngle * Mathf.Deg2Rad) * dist;
                }
            }
            float speedNow = speed * Time.deltaTime;
            m_movedDistance += speedNow;
            if (dir.magnitude < speedNow)
            {
                pos = m_destPoint;
                transform.position = pos;
                stop();
            }
            else
            {
                pos += speedNow * dir.normalized;
                transform.position = pos;
            }
        }
    }
}
