
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject projectilePrefab;
    public Transform shootPoint;

    void Update()
    {
        Move();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 moveDir = new Vector2(h, v).normalized;
        transform.Translate(moveDir * moveSpeed * Time.deltaTime);
    }

    void Shoot()
    {
        Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
    }
}
