using System;
using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public Vector3 serverPosition;
    SpriteRenderer spriteRenderer;
    Animator animator;

    private string bullet_id; 
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator =  GetComponent<Animator>();
    }

    public void SyncFromServer(float posX, float posY)
    {
        serverPosition = new Vector3(posX, posY,transform.position.z);
    }
    void Update()
    {
        //삭제할때 id값 할당됨.
        if (!string.IsNullOrEmpty(bullet_id)) return;
        
        Vector3 dir = serverPosition - transform.position;
        if (dir.sqrMagnitude > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, serverPosition, Time.deltaTime * 5f);
        }

        transform.Rotate(0f, 0f, 360f * Time.deltaTime);
    }
    public void StartFadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }
    private IEnumerator FadeInCoroutine()
    {
        float elapsed = 0f;
        Color color = spriteRenderer.color;
        float duration = 0.2f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
    }

    public void PlayBulletExplosionAnim()
    {
        
    }

    public void PlayDeadAnimation(string pid)
    {
        bullet_id = pid;
        transform.rotation = Quaternion.identity;
        animator.Play("explosion");
    }

    public void OnDestroy()
    {
        if (string.IsNullOrEmpty(bullet_id)) return;
        Destroy(this.gameObject);
        NetworkManager.Instance.RemoveBullet(bullet_id);
        Debug.Log($"[BulletSpawnHandler] 총알 삭제됨: {bullet_id}");
    }
}