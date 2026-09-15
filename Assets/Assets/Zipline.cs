using UnityEngine;

public class Zipline : MonoBehaviour
{
    [Header("Zipline Points")]
    public Transform startPoint;
    public Transform endPoint;

    public bool IsValid()
    {
        return startPoint != null && endPoint != null && GetLength() > 0.001f;
    }

    public Vector3 GetPoint(float progress)
    {
        if (startPoint == null || endPoint == null)
        {
            return transform.position;
        }

        progress = Mathf.Clamp01(progress);
        return Vector3.Lerp(startPoint.position, endPoint.position, progress);
    }

    public float GetClosestProgress(Vector3 position)
    {
        if (!IsValid())
        {
            return 0f;
        }

        Vector3 line = endPoint.position - startPoint.position;
        return Mathf.Clamp01(Vector3.Dot(position - startPoint.position, line) / line.sqrMagnitude);
    }

    public float GetLength()
    {
        if (startPoint == null || endPoint == null)
        {
            return 0f;
        }

        return Vector3.Distance(startPoint.position, endPoint.position);
    }

    public Vector3 GetDirection()
    {
        if (!IsValid())
        {
            return Vector3.zero;
        }

        return (endPoint.position - startPoint.position).normalized;
    }
}
