using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    
    public class CountDownText : MonoBehaviour
    {
        Text countdownText;
        private void Awake()
        {
            this.gameObject.SetActive(false);
            countdownText = GetComponent<Text>();
        }

        public void UpdateText(int count, string start_msg)
        {
            this.gameObject.SetActive(true);
            if (count > 0)
            {
                countdownText.text = count.ToString();
            }
            else if (string.Empty != start_msg)
            {
                countdownText.text = start_msg;
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
