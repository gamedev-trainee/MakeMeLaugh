using System.Collections.Generic;
using UnityEngine;

namespace MakeMeLaugh
{
    public class NumberScript : MonoBehaviour
    {
        public bool useStartNum = false;
        public int startNum = 0;

        public TextAlignment horizonAlign = TextAlignment.Left;
        public TextAlignment verticalAlign = TextAlignment.Left;
        public int orderInLayer = 0;
        public List<Sprite> numSprites = new List<Sprite>();
        public List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

        private void Start()
        {
            if (useStartNum)
            {
                setNumber(startNum);
            }
        }

        public void setNumber(int value)
        {
            string str = value.ToString();
            int count = str.Length;
            initRenderers(count);
            float totalW = 0f;
            float maxH = 0f;
            int num;
            for (int i = 0; i < count; i++)
            {
                int.TryParse(str[i].ToString(), out num);
                spriteRenderers[i].sprite = numSprites[num];
                totalW += spriteRenderers[i].size.x;
                maxH = Mathf.Max(maxH, spriteRenderers[i].size.y);
            }
            float vx = 0f;
            float vy = 0f;
            switch (horizonAlign)
            {
                case TextAlignment.Left: vx = 0; break;
                case TextAlignment.Center: vx = -totalW * 0.5f; break;
                case TextAlignment.Right: vx = -totalW; break;
            }
            switch (verticalAlign)
            {
                case TextAlignment.Left: vy = maxH; break;
                case TextAlignment.Center: vy = maxH * 0.5f; break;
                case TextAlignment.Right: vy = 0; break;
            }
            float vw = 0f;
            Vector3 pos;
            for (int i = 0; i < count; i++)
            {
                pos = spriteRenderers[i].transform.localPosition;
                pos.x = vx + vw + spriteRenderers[i].size.x * 0.5f;
                pos.y = vy + spriteRenderers[i].size.y * 0.5f;
                spriteRenderers[i].transform.localPosition = pos;
                vw += spriteRenderers[i].size.x;
            }
        }

        protected void initRenderers(int count)
        {
            while (count >= spriteRenderers.Count)
            {
                initRenderer();
            }
            for (int i = 0; i < count; i++)
            {
                spriteRenderers[i].gameObject.SetActive(true);
            }
            if (count < spriteRenderers.Count)
            {
                int total = spriteRenderers.Count;
                for (int i = count; i < total; i++)
                {
                    spriteRenderers[i].gameObject.SetActive(false);
                }
            }
        }

        protected void initRenderer()
        {
            int index = spriteRenderers.Count;
            GameObject go = new GameObject(index.ToString());
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = Vector3.zero;
            go.transform.localScale = Vector3.one;
            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = orderInLayer;
            spriteRenderers.Add(renderer);
        }
    }
}
