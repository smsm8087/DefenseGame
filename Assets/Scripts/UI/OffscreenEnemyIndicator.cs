using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class OffscreenEnemyIndicator : MonoBehaviour
{
    [Header("UI References")]
    public GameObject leftIndicator;
    public GameObject rightIndicator;
    public TextMeshProUGUI leftCountText;
    public TextMeshProUGUI rightCountText;
    
    [Header("Settings")]
    public float updateInterval = 0.1f;
    public float screenPadding = 50f;
    
    private Camera mainCamera;
    private Dictionary<string, GameObject> enemies;
    private float lastUpdateTime;
    
    // 외부에서 적 딕셔너리를 설정하는 메서드
    public void SetEnemiesDictionary(Dictionary<string, GameObject> enemiesDict)
    {
        enemies = enemiesDict;
    }
    
    void Start()
    {
        mainCamera = Camera.main;
        
        if (leftIndicator) leftIndicator.SetActive(false);
        if (rightIndicator) rightIndicator.SetActive(false);
    }
    
    void Update()
    {
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            UpdateOffscreenIndicators();
            lastUpdateTime = Time.time;
        }
    }
    
    void UpdateOffscreenIndicators()
    {
        if (enemies == null || mainCamera == null) return;
        
        int leftCount = 0;
        int rightCount = 0;
        
        // 활성화된 적들만 필터링
        var activeEnemies = enemies.Values.Where(enemy => enemy != null && enemy.activeInHierarchy);
        
        foreach (var enemy in activeEnemies)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(enemy.transform.position);
            
            // 화면 밖에 있는지 체크
            bool isOffscreen = screenPos.x < -screenPadding || 
                              screenPos.x > Screen.width + screenPadding ||
                              screenPos.y < -screenPadding || 
                              screenPos.y > Screen.height + screenPadding ||
                              screenPos.z < 0; // 카메라 뒤쪽도 고려
            
            if (isOffscreen)
            {
                if (screenPos.x < Screen.width * 0.5f)
                {
                    leftCount++;
                }
                else
                {
                    rightCount++;
                }
            }
        }
        
        UpdateIndicatorUI(leftIndicator, leftCountText, leftCount);
        UpdateIndicatorUI(rightIndicator, rightCountText, rightCount);
    }
    
    void UpdateIndicatorUI(GameObject indicator, TextMeshProUGUI countText, int count)
    {
        if (indicator == null) return;
        
        bool shouldShow = count > 0;
        indicator.SetActive(shouldShow);
        
        if (shouldShow && countText != null)
        {
            countText.text = count.ToString();
        }
    }
    
    // 디버그용 - 현재 오프스크린 적 수를 반환
    public (int left, int right) GetOffscreenEnemyCounts()
    {
        if (enemies == null) return (0, 0);
        
        int leftCount = 0;
        int rightCount = 0;
        
        var activeEnemies = enemies.Values.Where(enemy => enemy != null && enemy.activeInHierarchy);
        
        foreach (var enemy in activeEnemies)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(enemy.transform.position);
            
            bool isOffscreen = screenPos.x < -screenPadding || 
                              screenPos.x > Screen.width + screenPadding ||
                              screenPos.y < -screenPadding || 
                              screenPos.y > Screen.height + screenPadding ||
                              screenPos.z < 0;
            
            if (isOffscreen)
            {
                if (screenPos.x < Screen.width * 0.5f)
                    leftCount++;
                else
                    rightCount++;
            }
        }
        
        return (leftCount, rightCount);
    }
}