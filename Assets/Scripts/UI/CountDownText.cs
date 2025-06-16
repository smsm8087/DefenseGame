using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    
    public class CountDownText : MonoBehaviour
    {
        Text countdownText;
        private void Awake()
        {
            countdownText = GetComponent<Text>();
        }

        public void UpdateText(int count, string start_msg)
        {
            gameObject.SetActive(count > 0 || !string.IsNullOrEmpty(start_msg));
            
            if (count > 0)
            {
                countdownText.text = count.ToString();
            }
            else if (!string.IsNullOrEmpty(start_msg))
            {
                countdownText.text = start_msg;
            }
        }
    }
}
