using System.Collections.Generic;
using UnityEngine;

namespace MakeMeLaugh
{
    public class ObjectManager : MonoBehaviour
    {
        public static ObjectManager Instance { get; private set; }

        public float linkDistance = 0f;
        public AudioClip clearAudio = null;

        private List<LinkScript> m_links = new List<LinkScript>();

        private int m_pickingType = 0;
        private List<ObjectScript> m_pickedObjects = new List<ObjectScript>();
        private int m_clearCount = 0;

        private void Awake()
        {
            Instance = this;
        }

        public void beginPick(ObjectScript obj)
        {
            if (m_pickingType == 0)
            {
                m_pickingType = obj.type;
                m_pickedObjects.Clear();
                m_pickedObjects.Add(obj);
            }
            else
            {
                addPick(obj);
            }
        }

        public void addPick(ObjectScript obj)
        {
            if (obj.type != m_pickingType) return;
            int index = m_pickedObjects.IndexOf(obj);
            if (index != -1)
            {
                if (index == m_pickedObjects.Count - 2)
                {
                    int lastIndex = m_pickedObjects.Count - 1;
                    m_pickedObjects.RemoveAt(lastIndex);
                    deactiveLinkAt(index);
                }
                return;
            }
            if (m_pickedObjects.Count > 0)
            {
                ObjectScript lastObj = m_pickedObjects[m_pickedObjects.Count - 1];
                float dist = Vector3.Distance(lastObj.transform.position, obj.transform.position);
                if (dist > linkDistance)
                {
                    return;
                }
            }
            index = m_pickedObjects.Count;
            m_pickedObjects.Add(obj);
            if (index <= 0)
            {
                activeLinkAt(index, m_pickedObjects[index].transform, null);
            }
            else
            {
                activeLinkAt(index - 1, m_pickedObjects[index - 1].transform, m_pickedObjects[index].transform);
            }
        }

        public void endPick()
        {
            if (m_pickedObjects.Count > 1)
            {
                clearPicks(m_pickedObjects);
            }
            m_pickedObjects.Clear();
            m_pickingType = 0;
            clearLinks();
        }

        private void clearPicks(List<ObjectScript> objs)
        {
            int count = objs.Count;
            for (int i = 0; i < count; i++)
            {
                CoreManager.Instance.addScoreAt(objs[i].transform.position);
                GameObject.Destroy(objs[i].gameObject);
                m_clearCount++;
            }
            AudioManager.Instance.play(clearAudio);
        }

        public int getClearCount()
        {
            return m_clearCount;
        }

        private void activeLinkAt(int index, Transform linkFrom, Transform linkTo)
        {
            if (index >= m_links.Count)
            {
                GameObject link = ResourceManager.Instance.requestInstance("Link");
                m_links.Add(link.GetComponent<LinkScript>());
            }
            m_links[index].initLink(linkFrom, linkTo);
        }

        private void deactiveLinkAt(int index)
        {
            if (index < 0 || index >= m_links.Count) return;
            m_links[index].uninitLink();
        }

        private void clearLinks()
        {
            int count = m_links.Count;
            for (int i = 0; i < count; i++)
            {
                m_links[i].uninitLink();
            }
        }

        public void clear()
        {
            destroyLinks();
            m_pickedObjects.Clear();
            m_pickingType = 0;
            m_clearCount = 0;
        }

        public void destroyLinks()
        {
            int count = m_links.Count;
            for (int i = 0; i < count; i++)
            {
                GameObject.Destroy(m_links[i].gameObject);
            }
            m_links.Clear();
        }
    }
}
