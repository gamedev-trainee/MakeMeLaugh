using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MakeMeLaugh
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        public enum States
        {
            None,
            Ready,
            Running,
            Ending,
            Result,
            Reset,
        }

        public GameObject startRoot = null;
        public Button startButton = null;
        public GameObject endingRoot = null;
        public Animation endingAnimation = null;
        public GameObject resultRoot = null;
        public Button resultButton = null;
        public TextMeshProUGUI resultLabel = null;
        public GameObject guideRoot = null;
        public float guideCD = 0f;
        public float guideDuration = 0f;

        private States m_state = States.None;

        private bool m_guideShown = false;
        private float m_guideCDLeft = 0f;
        private float m_guidePassedTime = 0f;

        private void Awake()
        {
            Instance = this;

            startRoot.SetActive(false);
            endingRoot.SetActive(false);
            resultRoot.SetActive(false);
            guideRoot.SetActive(false);

            m_guideCDLeft = guideCD;
        }

        private void Start()
        {
            onReady();
        }

        private void Update()
        {
            switch (m_state)
            {
                case States.Running:
                    {
                        if (m_guideShown)
                        {
                            m_guidePassedTime += Time.deltaTime;
                            if (m_guidePassedTime >= guideDuration)
                            {
                                m_guidePassedTime = 0f;
                                hideGuide();
                            }
                        }
                        else
                        {
                            m_guideCDLeft -= Time.deltaTime;
                            if (m_guideCDLeft <= 0f)
                            {
                                m_guideCDLeft = guideCD;
                                showGuide();
                            }
                        }
                    }
                    break;
                case States.Reset:
                    {
                        if (m_state == States.Reset)
                        {
                            if (!endingAnimation.isPlaying)
                            {
                                resultRoot.SetActive(false);
                                onReady();
                            }
                        }
                    }
                    break;
            }
        }

        protected void onReady()
        {
            m_state = States.Ready;
            startRoot.SetActive(true);
            startButton.onClick.AddListener(onStartClick);
        }

        private void onStartClick()
        {
            onGame();
        }

        private void onGame()
        {
            m_state = States.Running;
            startButton.onClick.RemoveListener(onStartClick);
            startRoot.SetActive(false);
            CoreManager.Instance.start();
        }

        private void onReset()
        {
            m_state = States.Reset;
            hideGuide();
            endingAnimation.Play("ending_reset");
            resultButton.onClick.RemoveListener(onResultClick);
            CoreManager.Instance.reset();
        }

        public void playEnding()
        {
            m_state = States.Ending;
            hideGuide();
            endingRoot.SetActive(true);
            endingAnimation.Play("ending");
        }

        public void showResult(int id, string age)
        {
            m_state = States.Result;
            hideGuide();
            resultRoot.SetActive(true);
            resultButton.onClick.AddListener(onResultClick);
            resultLabel.text = string.Format("No.{0} Alexander was gone\nat the age of {1}", id, age);
            CoreManager.Instance.stop();
        }

        private void onResultClick()
        {
            onReset();
        }

        private void showGuide()
        {
            if (CoreManager.Instance.getGameCount() > 0)
            {
                hideGuide();
                return;
            }
            if (ObjectManager.Instance.getClearCount() > 0)
            {
                hideGuide();
                return;
            }
            m_guideShown = true;
            m_guidePassedTime = 0f;
            guideRoot.SetActive(true);
        }

        private void hideGuide()
        {
            m_guideShown = false;
            guideRoot.SetActive(false);
        }
    }
}
