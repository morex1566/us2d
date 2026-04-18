using UnityEngine;
using UnityEngine.InputSystem;

public static class Utls
{
    // Current, Parent, Children 전체 시도
    public static T FindComponent<T>(GameObject obj) where T : Component
    {
        T comp = obj.GetComponent<T>();
        if (comp != null) return comp;
            
        comp = obj.GetComponentInParent<T>();
        if (comp != null) return comp;

        return obj.GetComponentInChildren<T>();
    }

    public static bool IsNearlyZero(this Vector2 value)
    {
        return value.sqrMagnitude < 0.0001f;
    }

    public static bool IsNotNearlyZero(this Vector2 value)
    {
        return value.sqrMagnitude >= 0.0001f;
    }

    public static Vector3 GetMouseWorldPosition(Camera camera = null)
    {
        if (camera == null) camera = Camera.main;

        Vector3 screenPos = Mouse.current.position.ReadValue();
        screenPos.z = -camera.transform.position.z;

        Vector3 worldPos = camera.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;

        return worldPos;
    }
}