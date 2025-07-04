using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class SceneHighlighter
{
    private static GameObject targetToHighlight;

    static SceneHighlighter()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    public static void SetTarget(GameObject target)
    {
        targetToHighlight = target;
        SceneView.RepaintAll();
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        if (targetToHighlight == null) return;

        Handles.color = Color.cyan;
        Bounds bounds = GetBounds(targetToHighlight);
        Handles.DrawWireCube(bounds.center, bounds.size);
        Handles.Label(bounds.center + Vector3.up * 1.5f, targetToHighlight.name, EditorStyles.boldLabel);
    }

    private static Bounds GetBounds(GameObject go)
    {
        Renderer renderer = go.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds;
        }
        else
        {
            return new Bounds(go.transform.position, Vector3.one * 0.5f);
        }
    }
}