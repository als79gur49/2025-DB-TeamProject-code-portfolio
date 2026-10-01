using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public static class Utils
{
    //radius범위 내 랜덤 position반환
    public static Vector3 RandomPositionFromRadius(int radius, Vector3 point = default)
    {
        int distance = Random.Range(0, radius + 1);
        int angle = Random.Range(0, 360);

        float rad = angle * Mathf.Deg2Rad;

        float x = Mathf.Cos(rad) * distance;
        float z = Mathf.Sin(rad) * distance;

        return new Vector3(x, 0, z) + point;
    }

    public static bool IsNearTarget(Vector3 target, Vector3 current, int length)
    {
        float distance = Vector3.Distance(target, current);

        if (distance < length)
        {
            return true;
        }

        return false;
    }
}
