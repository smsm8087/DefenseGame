using System.Collections;
using System.Collections.Generic;
using DataModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    
    public class CenterText : MonoBehaviour
    {
        TextMeshProUGUI centerText;
        private void Awake()
        {
            centerText = GetComponent<TextMeshProUGUI>();
        }

        public void UpdateText(int count, string start_msg)
        {
            gameObject.SetActive(count > 0 || !string.IsNullOrEmpty(start_msg));
            
            if (count > 0)
            {
                centerText.text = count.ToString();
            }
            else if (!string.IsNullOrEmpty(start_msg))
            {
                centerText.text = start_msg;
            }
        }

        public void startDurationAnimation(int duration, List<CardData> cards)
        {
            gameObject.SetActive(duration > 0);
            StartCoroutine(DurationCoroutin(duration, cards));
        }
        public IEnumerator DurationCoroutin(int duration, List<CardData> cards)
        {
            float elapsedTime = 0;
            while (elapsedTime < duration * 1000)
            {
                elapsedTime++;
                centerText.text = $"카드선택까지 남은시간 : {(duration - elapsedTime).ToString()}\n " +
                                  $"카드 : {TextManager.Instance.GetText(cards[0].title)}{cards[0].value}%\n" +
                                  $"카드 : {TextManager.Instance.GetText(cards[1].title)}{cards[1].value}%\n" +
                                  $"카드 : {TextManager.Instance.GetText(cards[2].title)}{cards[2].value}%\n";
                yield return new WaitForSeconds(1);
            }
            gameObject.SetActive(duration > 0);
        }
    }
}
