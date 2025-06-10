using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float lerpSpeed = 1.5f;

    void Start()
    {
    }

    private void Update()
    {
        if (target == null) return;

        Vector3 targetPos = new Vector3(target.position.x, Mathf.Max(target.position.y, 0), transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
    }
}
