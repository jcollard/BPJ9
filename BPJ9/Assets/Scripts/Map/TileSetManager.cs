#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;

[RequireComponent(typeof(TileSet))]
public class TileSetManager : MonoBehaviour
{
    public TileSet TileSet => this.GetComponent<TileSet>();


}

[CustomEditor(typeof(TileSetManager))]
public class TileSetManagerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        TileSetManager manager = (TileSetManager)target;
        EditorGUILayout.ObjectField("Tile Set", manager.TileSet, typeof(TileSet), false);

        if (GUILayout.Button("Find Tiles"))
        {
            manager.TileSet.FindTiles();
        }

        if (EditorGUI.EndChangeCheck())
        {
            // This code will unsave the current scene if there's any change in the editor GUI.
            // Hence user would forcefully need to save the scene before changing scene
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage != null)
        {
            EditorSceneManager.MarkSceneDirty(prefabStage.scene);
        }
    }

}

#endif