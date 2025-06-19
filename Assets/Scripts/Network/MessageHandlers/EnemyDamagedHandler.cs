using UnityEngine;
using System.Collections.Generic;
using UI;
using UnityEditor;

public class EnemyDamagedHandler : INetworkMessageHandler
{
    private readonly Dictionary<string, GameObject> Enemies = new ();
    private readonly GameObject DamageTextPrefab;

    public string Type => "enemy_damaged";

    public EnemyDamagedHandler(Dictionary<string, GameObject> Enemies,  GameObject DamageTextPrefab)
    {
        this.Enemies = Enemies;
        this.DamageTextPrefab = DamageTextPrefab;
    }

    public void Handle(NetMsg msg)
    {
        List<EnemyDamageInfo> damagedEnemies = msg.damagedEnemies;
        foreach (var damagedEnemy in damagedEnemies)
        {
            var pid = damagedEnemy.enemyId;
            if (Enemies.TryGetValue(pid, out var enemyObj))
            {
                Debug.Log($"Found enemy: {enemyObj.name}");
            
                // HP바 업데이트 (기존 방식 그대로)
                Transform hpBarFill = enemyObj.transform.Find("UICanvas/HpUI/Health Bar Fill");
                if (hpBarFill != null)
                {
                    var enemyHpBar = hpBarFill.GetComponent<EnemyHPBar>();
                    if (enemyHpBar != null)
                    {
                        Debug.Log("Found EnemyHPBar, updating HP");
                        enemyHpBar.UpdateHP(damagedEnemy.currentHp, damagedEnemy.maxHp);
                    }
                    else
                    {
                        Debug.LogError("EnemyHPBar component not found on Health Bar Fill!");
                    }
                }
                else
                {
                    Debug.LogError("Health Bar Fill not found!");
                }
                
                // 메인 캔버스 찾기
                Canvas mainCanvas = GameObject.Find("Canvas")?.GetComponent<Canvas>();
                if (mainCanvas != null)
                {
                    Debug.Log($"Found main canvas: {mainCanvas.name}");
                    
                    // 몬스터 위치 가져오기
                    Transform damageTextRoot = enemyObj.transform.Find("UICanvas/DamagePos");
                    Vector3 worldPos = damageTextRoot != null ? damageTextRoot.position : enemyObj.transform.position;
                    
                    // 월드 좌표를 스크린 좌표로 변환
                    Camera worldCamera = Camera.main;
                    Vector3 screenPos = worldCamera.WorldToScreenPoint(worldPos);
                    
                    // 스크린 좌표를 UI 좌표로 변환
                    Vector2 canvasPos;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        mainCanvas.transform as RectTransform,
                        screenPos,
                        null, // Overlay 모드
                        out canvasPos
                    );
                    
                    // 메인 캔버스에 직접 생성
                    var dmgTextObj = GameObject.Instantiate(DamageTextPrefab, mainCanvas.transform);
                    Debug.Log($"Created damage text in main canvas at world pos: {worldPos}");
                    
                    // 몬스터 위치에 맞게 설정
                    var rectTransform = dmgTextObj.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        rectTransform.localPosition = new Vector3(canvasPos.x, canvasPos.y, 0f);
                        rectTransform.sizeDelta = new Vector2(200, 100);
                        Debug.Log($"Set position to ({canvasPos.x}, {canvasPos.y}, 0)");
                    }
                    
                    // Text 컴포넌트 강제 설정
                    var textComponent = dmgTextObj.GetComponent<UnityEngine.UI.Text>();
                    if (textComponent != null)
                    {
                        textComponent.text = damagedEnemy.damage.ToString();
                        textComponent.alignment = TextAnchor.MiddleCenter;
                        Debug.Log($"Forced text settings: text='{textComponent.text}', size={textComponent.fontSize}, color=red");
                    }
                    else
                    {
                        Debug.LogError("Text component not found on damage text prefab!");
                    }
                    
                    // DamageText 스크립트 초기화
                    var dmgText = dmgTextObj.GetComponent<DamageText>();
                    if (dmgText != null)
                    {
                        Debug.Log("Initializing DamageText script");
                        dmgText.Init(damagedEnemy.damage);
                    }
                    else
                    {
                        Debug.LogWarning("DamageText script not found, but text should still be visible");
                    }
                }
                else
                {
                    Debug.LogError("Main Canvas not found!");
                }
            }
        }
    }
}