using System.Collections;
using System.Collections.Generic;
using Enemy;
using TMPro;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public string guid;
    public Vector3 serverPosition;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject outlineObj;
    [SerializeField] private Transform footPos;
    [SerializeField] private Transform facePos;
    [SerializeField] private Transform bodyPos;
    [SerializeField] private Transform textShowPos;
    void Awake()
    {
        serverPosition = transform.position;
    }

    void Start()
    {
        //ChangeState(moveState);
    }

    void Update()
    {
        //currentState?.Update(this);
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void PlayIntroCoroutine()
    {
        StartCoroutine(PlayBossIntro());
    }
    private IEnumerator PlayBossIntro()
    {
        GameManager.Instance.PauseGame();
        //카메라 팔로우
        // 1. 발 → 줌인
        yield return StartCoroutine(CameraFollow.Instance.MoveCamera(footPos.position, 2f, 0.8f));

        // 2. 얼굴 → 유지
        yield return StartCoroutine(CameraFollow.Instance.MoveCamera(facePos.position, 2f, 0.8f));

        // 3. 전체 → 줌아웃
        yield return StartCoroutine(CameraFollow.Instance.MoveCamera(bodyPos.position, 4f, 1.2f));

        StartCoroutine(CameraFollow.Instance.MoveCamera(textShowPos.position, 5f, 0.2f));
        yield return StartCoroutine(PlayBossIntroText());   

        //resume 하면 카메라도 알아서 update 됨.
        GameManager.Instance.ResumeGame();
    }
    IEnumerator PlayBossIntroText()
    {
        // 등장 연출 예시 (페이드 인)
        GameObject introUI = UIManager.Instance.getIntroUICanvas();
        CanvasGroup canvasGroup = introUI.GetComponent<CanvasGroup>();
        TextMeshProUGUI titleText = introUI.transform.Find("BossTitle").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI descText =  introUI.transform.Find("BossDesc").GetComponent<TextMeshProUGUI>();
        RectTransform titleRT = titleText.GetComponent<RectTransform>();
        RectTransform descRT = descText.GetComponent<RectTransform>();
        if(canvasGroup == null || titleText == null || descText == null) yield break;
        
        canvasGroup.alpha = 0f;
        UIManager.Instance.setActiveGameUICanvas(false);
        UIManager.Instance.setActiveIntroUICanvas(true);

        Vector2 titleStartPos = titleRT.anchoredPosition;
        Vector2 descStartPos = descRT.anchoredPosition;

        Vector2 titleDestPos = new Vector2(370f, -195f);
        Vector2 descDestPos = new Vector2(382f, -340f);
        float t = 0f;
        while (t < 0.3f)
        {
            t += Time.unscaledDeltaTime * 2f;
            canvasGroup.alpha = Mathf.Clamp01(t); // 페이드 인
            yield return null;
        }
        canvasGroup.alpha = 1f;
        
        float titleTime = 0f;
        while (titleTime < 1f)
        {
            titleTime += Time.unscaledDeltaTime;
            float easeT = Utils.EaseOutCubic(titleTime);
            titleRT.anchoredPosition = Vector2.Lerp(titleStartPos, titleDestPos, easeT);
            descRT.anchoredPosition = Vector2.Lerp(descStartPos, descDestPos, easeT);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(1f);
        canvasGroup.alpha = 0f;
        UIManager.Instance.setActiveGameUICanvas(true);
        UIManager.Instance.setActiveIntroUICanvas(false);
    }
    public void ShowOutline(float duration = 3f)
    {
        if (outlineObj == null) return;

        outlineObj.SetActive(true);
        CancelInvoke(nameof(HideOutline));
        Invoke(nameof(HideOutline), duration);
    }

    private void HideOutline()
    {
        outlineObj.SetActive(false);
    }
}