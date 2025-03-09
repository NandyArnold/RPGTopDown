using UnityEngine;

public class CameraController: MonoBehaviour
{

    [Header("Target Settings")]
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Boundary Settings")]
    public bool useBoundaries = true;
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -10f;
    public float maxY = 10f;

    [Header("Advanced Settings")]
    public float snapDistance = 0.05f;

    private void LateUpdate()
    {
        if (!target)
            return;
        Vector3 desiredPosition = target.position + offset;

        float distance = Vector3.Distance(transform.position, desiredPosition);

        if(distance <snapDistance)
        {
            transform.position = desiredPosition;
            return;
        }

        Vector3 smoothedPosition = Vector3.Lerp(transform.position,desiredPosition,smoothSpeed*Time.deltaTime);

        if(useBoundaries)
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.y, minX, maxX);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY, maxY);
        }

        transform.position = smoothedPosition;

    }


    public void SetBoundaries(float minX, float maxX, float minY, float maxY)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;

    }



}
