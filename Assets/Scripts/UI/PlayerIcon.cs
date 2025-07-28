using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIcon : MonoBehaviour
{
    [SerializeField] private Image readyIcon;
    [SerializeField] private Image baseIcon;
    [SerializeField] private TextMeshProUGUI nickNameText;
    public string playerId;
    public void SetInfo(string playerId, string nickName)
    {
        this.playerId = playerId;
        this.nickNameText.text = nickName;
    }

    public void SetJobIcon(string job_tpye)
    {
        string capitalJob = FirstCharToUpper(job_tpye);
        string spritePath = $"Character/{capitalJob}/PROFILE_{capitalJob}";

        Sprite overrideSprite = Resources.Load<Sprite>(spritePath);
        if (overrideSprite != null)
        {
            baseIcon.enabled = true;
            baseIcon.sprite = overrideSprite;
        }
        else
        {
            baseIcon.enabled = false;
        }
    }
    private string FirstCharToUpper(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";
        return char.ToUpper(input[0]) + input.Substring(1).ToLower();
    }
}
