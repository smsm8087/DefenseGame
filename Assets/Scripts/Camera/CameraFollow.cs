using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance;
    public float lerpSpeed = 1.5f;
    
    [Header("Camera Bounds")]
    public float minX = -20f; 
    public float maxX = 20f;  
    public float minY = 0f;   
    public float maxY = 10f;  
    
    private Transform target;

    public void setTarget(Transform _target)
    {
        target = _target;
    }
    
    void Start()
    {
        Instance = this;
    }

    private void Update()
    {
        if (target == null) return;
        
        // 기본 타겟 위치 계산
        float targetX = target.position.x;
        float targetY = Mathf.Max(target.position.y, minY);
        
        // 경계 제한 적용
        targetX = Mathf.Clamp(targetX, minX, maxX);
        targetY = Mathf.Clamp(targetY, minY, maxY);
        
        Vector3 targetPos = new Vector3(targetX, targetY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
    }
    
    // Scene 뷰에서 경계선 표시
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, transform.position.z);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0);
        Gizmos.DrawWireCube(center, size);
    }
}