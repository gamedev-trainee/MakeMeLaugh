using UnityEngine;

namespace MakeMeLaugh
{
    public class HeartBeatScript : MonoBehaviour
    {
        public bool playOnStart = false;

        public float duration = 1f;
        public Vector3 scaleMin = Vector3.one;
        public Vector3 scaleMax = Vector3.one;
        public AnimationCurve curveMinToMax = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public AnimationCurve curveMaxToMin = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public Transform body = null;
        public AudioClip audioClip = null;

        private float m_timePassed = 0f;

        private bool m_isPlaying = false;

        private void Start()
        {
            if (playOnStart)
            {
                play();
            }
        }

        public void play()
        {
            m_isPlaying = true;
        }

        public void stop()
        {
            m_isPlaying = false;
        }

        private void Update()
        {
            if (!m_isPlaying) return;

            m_timePassed += Time.deltaTime;
            if (m_timePassed >= duration)
            {
                m_timePassed = 0f;
                onUpdate(1f);
                AudioManager.Instance.play(audioClip);
            }
            else
            {
                onUpdate(m_timePassed / duration);
            }
        }

        protected void onUpdate(float progress)
        {
            float realProgress;
            Vector3 scaleCurrent;
            if (progress < 0.5f)
            {
                realProgress = progress / 0.5f;
                realProgress = curveMinToMax.Evaluate(realProgress);
                scaleCurrent = Vector3.Lerp(scaleMin, scaleMax, realProgress);
            }
            else
            {
                realProgress = (progress - 0.5f) / 0.5f;
                realProgress = curveMaxToMin.Evaluate(realProgress);
                scaleCurrent = Vector3.Lerp(scaleMax, scaleMin, realProgress);
            }
            body.transform.localScale = scaleCurrent;
        }
    }
}
