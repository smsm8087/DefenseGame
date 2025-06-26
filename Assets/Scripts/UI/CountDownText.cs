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
    }
}
