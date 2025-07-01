using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterAnimator : MonoBehaviour
{
    [Header("UI Components")]
    public Image targetImage;

    [Header("Animation Data List")]
    public List<JobIdleAnimationData> jobAnimationDataList;

    private List<FrameData> frames;
    private int currentIndex = 0;
    private float timer = 0f;
    
    [SerializeField] private Vector2 baseAnchoredPosition;

    void Start()
    {
        // 내가 원하는 UI 기준 위치 (예: 화면 아래쪽 중앙)
        Vector2 desiredPosition = new Vector2(0, -148.6f);

        // Sprite Pivot 기준에 맞게 RectTransform Pivot 설정
        targetImage.rectTransform.pivot = new Vector2(0.47f, 0f);

        // 위치 설정
        targetImage.rectTransform.anchoredPosition = desiredPosition;

        // basePosition 저장 (만약 흔들림 줄 거면)
        baseAnchoredPosition = desiredPosition;
        
        float worldScale = 2.5f; // 플레이어가 스케일 3일 경우
        Vector2 originalSize = targetImage.sprite.rect.size;

        targetImage.SetNativeSize();
        targetImage.rectTransform.sizeDelta = originalSize * worldScale;
    }
    private void Update()
    {
        if (frames == null || frames.Count == 0 || targetImage == null) return;

        timer += Time.deltaTime;

        if (timer >= frames[currentIndex].duration)
        {
            timer = 0f;
            currentIndex = (currentIndex + 1) % frames.Count;
            Sprite nextSprite = frames[currentIndex].sprite;

            // Sprite가 같더라도 강제로 갱신시키기 위해 임시 null 처리
            if (targetImage.sprite == nextSprite)
            {
                targetImage.sprite = null;
            }

            targetImage.sprite = nextSprite;
            Vector2 pivotOffset = new Vector2(0.47f, 0f);
            targetImage.rectTransform.anchoredPosition = baseAnchoredPosition + pivotOffset;
        }
    }
    public void SetJob(string jobType)
    {
        targetImage.sprite = null;
        var data = jobAnimationDataList.FirstOrDefault(j => j.jobType.ToLower() == jobType.ToLower());

        if (data == null)
        {
            Debug.LogWarning($"[SetJob] 해당 직업({jobType})의 애니메이션 데이터가 없습니다.");
            frames = null;
            return;
        }

        frames = data.idleFrames;
        currentIndex = 0;
        timer = 0f;

        if (frames.Count > 0 && targetImage != null)
            targetImage.sprite = frames[0].sprite;
    }
}