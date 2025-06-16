using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    
    public class CenterText : MonoBehaviour
    {
        Text centerText;
        private void Awake()
        {
            centerText = GetComponent<Text>();
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
    }
}
