using System.Collections.Generic;
using UnityEngine;

namespace MakeMeLaugh
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }

        public List<GameObject> prefabs = new List<GameObject>();

        private Dictionary<string, GameObject> m_prefabMap = new Dictionary<string, GameObject>();
        private List<ResourceScript> m_instances = new List<ResourceScript>();

        private void Awake()
        {
            Instance = this;

            int count = prefabs.Count;
            for (int i = 0; i < count; i++)
            {
                m_prefabMap.Add(prefabs[i].name, prefabs[i]);
            }
        }

        public GameObject requestInstance(string name)
        {
            GameObject prefab;
            if (!m_prefabMap.TryGetValue(name, out prefab))
            {
                return null;
            }
            GameObject inst = prefab.Clone();
            ResourceScript script = inst.GetComponent<ResourceScript>();
            if (script == null)
            {
                script = inst.AddComponent<ResourceScript>();
            }
            m_instances.Add(script);
            return inst;
        }

        public void removeInstance(ResourceScript script)
        {
            m_instances.Remove(script);
        }

        public void destroyInstances()
        {
            int count = m_instances.Count;
            for (int i = 0; i < count; i++)
            {
                m_instances[i].setInvalid();
                GameObject.Destroy(m_instances[i].gameObject);
            }
            m_instances.Clear();
        }
    }
}
