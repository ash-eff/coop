using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapManager))]
public class MapManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MapManager mapManager = (MapManager)target;

        GUILayout.BeginVertical();

        if(GUILayout.Button("Generate Grid"))
        {
            mapManager.GenerateGrid();
        }

        if (GUILayout.Button("Clear Grid"))
        {
            mapManager.ClearGrid();
        }

        GUILayout.EndVertical();
    }
}
