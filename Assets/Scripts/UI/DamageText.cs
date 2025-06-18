using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UI
{
    public class DamageText : MonoBehaviour
    {
        private Text damageText;

        public void Awake()
        {
            damageText = GetComponent<Text>();
        }

        public void Init(int damage)
        {
            damageText.text = damage.ToString();
            damageText.gameObject.SetActive(true);

            // 간단한 애니메이션: 위로 이동 + 페이드 아웃
            Vector3 targetPos = transform.localPosition + new Vector3(0, 0.3f, 0); // Y로 위로 띄우기

            StartCoroutine(PlayDamageAnim(targetPos));
        }

        private IEnumerator PlayDamageAnim(Vector3 targetPos)
        {
            Vector3 startPos = transform.localPosition;
            float duration = 0.5f;
            float elapsed = 0f;
            
            Color originalColor = damageText.color;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // 위치 이동
                transform.localPosition = Vector3.Lerp(startPos, targetPos, t);

                // 알파 조절
                Color c = originalColor;
                c.a = Mathf.Lerp(1f, 0f, t * 0.5f);
                damageText.color = c;

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
