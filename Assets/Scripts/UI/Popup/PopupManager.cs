using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    [SerializeField] private Transform popupRoot; // 팝업이 붙을 부모 (Canvas 안에 있어야 함)

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public T ShowPopup<T>(GameObject popupPrefab) where T : BasePopup
    {
        var popupObj = Instantiate(popupPrefab, popupRoot);
        var popup = popupObj.GetComponent<T>();
        popup.Open();
        return popup;
    }
}