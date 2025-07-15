using System;
using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] public GameObject bulletEffectPrefab;
    public Vector3 serverPosition;
    SpriteRenderer spriteRenderer;
    Animator animator;

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

    public void SpawnBulletEffect()
    {
        // GameObject eff_obj =  Instantiate(bulletEffectPrefab, transform.position, Quaternion.identity);
        // Destroy(eff_obj, 1f);
    }

    public void PlayDeadAnimation()
    {
        animator.Play("dead");
    }

    public void OnDestroy()
    {
        Destroy(this.gameObject);
    }
}