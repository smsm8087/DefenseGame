using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class NetworkCharacterFollower : MonoBehaviour
{
    [SerializeField] private float lerpSpeed = 10f;

    private Vector3 targetPosition;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool lastFacingRight = true;
    private bool firstSync = true;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        targetPosition = transform.position;
    }

    public void SetTargetPosition(Vector3 newPos)
    {
        float dx = newPos.x - transform.position.x;

        if (Mathf.Abs(dx) > 0.01f)
        {
            lastFacingRight = dx > 0;
        }

        spriteRenderer.flipX = lastFacingRight;
        targetPosition = newPos;
    }

    public void SetJumping(bool isJumping)
    {
        if (animator != null)
        {
            animator.SetBool("isJumping", isJumping);
        }
    }

    public void SetRunning(bool isRunning)
    {
        if (animator != null)
        {
            animator.SetFloat("isRunning", isRunning ? 1f : 0f);
        }
    }

    private void Update()
    {
        if (firstSync)
        {
            transform.position = targetPosition;
            firstSync = false;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.deltaTime);
        }
    }
}