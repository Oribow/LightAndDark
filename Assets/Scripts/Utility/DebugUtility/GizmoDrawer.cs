using UnityEngine;
using System.Collections;

public class GizmoDrawer : MonoBehaviour
{
#if UNITY_EDITOR
    public string icon;
    public float range;
    public Color gizmosColor;
    public bool onlyDrawWhenSelected = true;

    void OnDrawGizmos()
    {
        if (onlyDrawWhenSelected)
            return;
        DrawGizmos();
    }

    void OnDrawGizmosSelected()
    {
        if (!onlyDrawWhenSelected)
            return;
        DrawGizmos();
    }

    void DrawGizmos()
    {
        if (!icon.IsNullOrEmptyAfterTrimmed())
            Gizmos.DrawIcon(transform.position, icon);
        if (range > 0)
            DebugExtension.DrawCircle(transform.position, Vector3.forward, gizmosColor, range);
    }
#endif
}
