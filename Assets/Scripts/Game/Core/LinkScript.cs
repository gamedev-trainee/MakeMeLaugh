using UnityEngine;

namespace MakeMeLaugh
{
    public class LinkScript : ResourceScript
    {
        public Transform startPoint = null;
        public Transform endPoint = null;
        public Transform line = null;

        private int m_linkStyle = 0;
        private Transform m_linkFrom = null;
        private Transform m_linkTo = null;

        private Vector3 m_lineScale = Vector3.one;

        private void Awake()
        {
            m_lineScale = line.localScale;
        }

        public void initLink(Transform linkFrom, Transform linkTo)
        {
            m_linkFrom = linkFrom;
            m_linkTo = linkTo;
            m_linkStyle = 0;
            startPoint.localPosition = Vector3.zero;
            endPoint.localPosition = Vector3.zero;
            if (m_linkFrom != null)
            {
                m_linkStyle = 1;
                startPoint.gameObject.SetActive(true);
            }
            if (m_linkTo != null)
            {
                m_linkStyle = 2;
                endPoint.gameObject.SetActive(true);
                line.gameObject.SetActive(true);
            }
            enabled = true;
        }

        public void uninitLink()
        {
            enabled = false;
            m_linkFrom = null;
            m_linkTo = null;
            startPoint.gameObject.SetActive(false);
            endPoint.gameObject.SetActive(false);
            line.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (m_linkStyle == 1)
            {
                transform.position = m_linkFrom.position;
            }
            else if (m_linkStyle == 2)
            {
                Vector3 startPosition = m_linkFrom.position;
                Vector3 endPosition = m_linkTo.position;
                Vector3 offset = endPosition - startPosition;
                Vector3 center = startPosition + offset * 0.5f;
                transform.position = center;
                startPoint.localPosition = startPosition - center;
                endPoint.localPosition = endPosition - center;
                float length = offset.magnitude;
                float angle = Vector2.SignedAngle(Vector2.up, new Vector2(offset.x, offset.y));
                line.localEulerAngles = new Vector3(0f, 0f, angle);
                line.localScale = new Vector3(m_lineScale.x, length, m_lineScale.z);
            }
        }
    }
}
