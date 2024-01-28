using UnityEngine;

namespace MakeMeLaugh
{
    public class ResourceScript : MonoBehaviour
    {
        private bool m_isValid = true;

        public void setInvalid()
        {
            m_isValid = false;
        }

        protected virtual void OnDestroy()
        {
            if (m_isValid)
            {
                ResourceManager.Instance.removeInstance(this);
            }
            m_isValid = false;
        }
    }
}
