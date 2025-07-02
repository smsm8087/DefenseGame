
using System;
using System.Collections.Generic;
using DataModels;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private WaveManager waveManager;
    [SerializeField] private Transform crystalRoot;
    [SerializeField] private List<SpriteRenderer> backGroundImages;
    [SerializeField] private HPBar hpBar;
    [SerializeField] private HPText hpText;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        GameDataManager.Instance.LoadAllData();
    }

    private void Start()
    {
        NetworkManager.Instance.SetOnGamveOverAction( ()=> TriggerGameOver());
    }
    public void TriggerGameOver()
    {
        NetworkManager.Instance.RemoveAllEnemies();
    }
    public void UpdateHPBar(float currentHp, float maxHp)
    {
        hpBar?.UpdateHP(currentHp, maxHp);
        hpText?.UpdateHP(currentHp, maxHp);
    }
    
    public void InitializeGame(int wave_id)
    {
        var waveData = GameDataManager.Instance.GetData<WaveData>("wave_data", wave_id);
        if (waveData == null)
        {
            Debug.LogError("wave data not found");
            return;
        }
        GameObject crystalPrefab = Resources.Load<GameObject>(waveData.shared_hp);
        if (crystalPrefab != null)
        {
            GameObject go = Instantiate(crystalPrefab, crystalRoot);
        }
        else
        {
            Debug.LogError($"프리팹 로드 실패: {waveData.shared_hp}");
        }
        Sprite backGroundSprite = Resources.Load<Sprite>(waveData.background);
        if (backGroundSprite != null)
        {
            foreach (SpriteRenderer sprite in backGroundImages)
            {
                sprite.sprite = backGroundSprite;
            }
        }
    }
}
