using UnityEngine;

namespace MakeMeLaugh
{
    public class CoreManager : MonoBehaviour
    {
        public static CoreManager Instance { get; private set; }

        public enum States
        {
            None,
            Ready,
            Running,
            EndingReady,
            EndingRunning,
            EndingEnd,
            End,
        }

        public Transform alexPoint = null;
        public Transform fallPoint = null;
        public Vector2 fallOffsets = Vector2.zero;
        public float fallInternal = 0f;
        public float fallInternalAdd = 0f;
        public float fallInternalMin = 0f;

        public Transform combinePoint = null;
        public AudioClip explosionAudio = null;

        public float endingDuration = 0f;

        //

        private States m_state = States.None;

        private Animation m_cameraAnimation = null;

        private AlexScript m_alex = null;

        private float m_fallIntervalLeft = 0f;

        private GameObject m_combineObj = null;
        private Animation m_combineAnimation = null;

        private float m_endingPassedTime = 0f;

        private int m_gameCount = 0;

        private void Awake()
        {
            Instance = this;

            m_cameraAnimation = Camera.main.GetComponent<Animation>();

            reset();
        }

        private void Update()
        {
            switch (m_state)
            {
                case States.Ready:
                    {
                        if (m_cameraAnimation == null || !m_cameraAnimation.isPlaying)
                        {
                            run();
                        }
                    }
                    break;
                case States.Running:
                    {
                        updateFall();
                    }
                    break;
                case States.EndingReady:
                    {
                        m_endingPassedTime += Time.deltaTime;
                        if (m_endingPassedTime >= endingDuration)
                        {
                            endingRunning();
                        }
                    }
                    break;
                case States.EndingRunning:
                    {
                        if (m_combineAnimation == null || !m_combineAnimation.isPlaying)
                        {
                            endingFinish();
                        }
                    }
                    break;
            }
        }

        public int getGameCount()
        {
            return m_gameCount;
        }

        public void start()
        {
            m_state = States.Ready;
            m_cameraAnimation.Play("zoom_in");
            m_alex.showInside();
        }

        public void stop()
        {
            m_gameCount++;
            m_state = States.End;
            m_cameraAnimation.Play("zoom_out");
            m_alex.hideInside();
        }

        public void reset()
        {
            m_state = States.None;
            createAlex();
        }

        public void endingReady()
        {
            m_state = States.EndingReady;
            m_endingPassedTime = 0f;
            m_combineAnimation.Play("explosion_start");
            m_cameraAnimation.Play("focus");
        }

        public void endingRunning()
        {
            m_state = States.EndingRunning;
            m_combineAnimation.Play("explosion_end");
            AudioManager.Instance.play(explosionAudio);
            UIManager.Instance.playEnding();
            m_alex.setDead();
        }

        public void endingFinish()
        {
            m_state = States.EndingEnd;
            UIManager.Instance.showResult(m_alex.getID(), m_alex.getAge());
            clear();
        }

        private void run()
        {
            m_state = States.Running;
            createCombine();
        }

        private void createAlex()
        {
            if (m_alex != null) return;
            GameObject inst = ResourceManager.Instance.requestInstance("Alex");
            inst.transform.position = alexPoint.position;
            m_alex = inst.GetComponent<AlexScript>();
        }

        private void destroyAlex()
        {
            if (m_alex == null) return;
            GameObject.Destroy(m_alex.gameObject);
            m_alex = null;
        }

        private void updateFall()
        {
            m_fallIntervalLeft -= Time.deltaTime;
            if (m_fallIntervalLeft > 0f)
            {
                return;
            }
            m_fallIntervalLeft = fallInternal;
            fallInternal -= Time.deltaTime * fallInternalAdd;
            if (fallInternal < fallInternalMin) fallInternal = fallInternalMin;
            createFall();
        }

        private void createFall()
        {
            Vector3 fallPosition = fallPoint.position;
            fallPosition.x += Random.Range(-fallOffsets.x, fallOffsets.x);
            fallPosition.y += Random.Range(-fallOffsets.y, fallOffsets.y);
            createFallAt(fallPosition);
        }

        private void createFallAt(Vector3 position)
        {
            int objID = Random.Range(1, 5);
            GameObject inst = ResourceManager.Instance.requestInstance(string.Format("P_{0}", objID));
            inst.transform.position = position;
        }

        private void createCombine()
        {
            if (m_combineObj != null) return;
            GameObject inst = ResourceManager.Instance.requestInstance("C_0");
            if (m_alex.combineContainer != null)
            {
                inst.transform.SetParent(m_alex.combineContainer);
            }
            inst.transform.position = combinePoint.position;
            m_combineObj = inst;
            m_combineAnimation = m_combineObj.GetComponentInChildren<Animation>();
        }

        private void destroyCombine()
        {
            if (m_combineObj == null) return;
            GameObject.Destroy(m_combineObj);
            m_combineObj = null;
            m_combineAnimation = null;
        }

        public void addScoreAt(Vector3 position)
        {
            GameObject inst = ResourceManager.Instance.requestInstance("Score");
            inst.transform.position = position;
            ScoreScript score = inst.GetComponent<ScoreScript>();
            score.play(m_combineObj.transform.position);
        }

        public void clear()
        {
            destroyCombine();
            destroyAlex();
            ObjectManager.Instance.clear();
            ResourceManager.Instance.destroyInstances();
        }
    }
}
