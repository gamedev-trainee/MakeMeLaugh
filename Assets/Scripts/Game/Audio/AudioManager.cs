using System.Collections.Generic;
using UnityEngine;

namespace MakeMeLaugh
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        private List<AudioSource> m_freePlayers = new List<AudioSource>();
        private List<AudioSource> m_busyPlayers = new List<AudioSource>();

        private void Awake()
        {
            Instance = this;
        }

        private void LateUpdate()
        {
            lock (m_busyPlayers)
            {
                int count = m_busyPlayers.Count;
                for (int i = count - 1; i >= 0; i--)
                {
                    if (m_busyPlayers[i].isPlaying) continue;
                    recyclePlayer(m_busyPlayers[i]);
                    m_busyPlayers.RemoveAt(i);
                }
            }
        }

        public void play(AudioClip clip, float pitch = 1f)
        {
            if (clip == null) return;
            AudioSource player = requestPlayer();
            if (!m_busyPlayers.Contains(player))
            {
                m_busyPlayers.Add(player);
            }
            player.pitch = pitch;
            player.PlayOneShot(clip);
        }

        protected AudioSource createPlayer()
        {
            GameObject go = new GameObject("AudioPlayer");
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localEulerAngles = Vector3.zero;
            AudioSource player = go.AddComponent<AudioSource>();
            return player;
        }

        protected AudioSource requestPlayer()
        {
            if (m_freePlayers.Count > 0)
            {
                AudioSource player = m_freePlayers[m_freePlayers.Count - 1];
                m_freePlayers.RemoveAt(m_freePlayers.Count - 1);
                return player;
            }
            return createPlayer();
        }

        protected void recyclePlayer(AudioSource player)
        {
            if (!m_freePlayers.Contains(player))
            {
                m_freePlayers.Add(player);
            }
        }
    }
}
