using UnityEngine;

public class BulletController : MonoBehaviour
{
    public Vector3 serverPosition;
    
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
}