#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
using System.Linq;

[RequireComponent(typeof(GridTile))]
public class GridTileManager : MonoBehaviour
{
    public GridTile Model => this.GetComponent<GridTile>();
    public NeighborSpace[] Criteria;

    public void SimplifyCriteria()
    {
        // TODO: Remove duplicates???
        HashSet<NeighborSpace> CriteriaSet = new HashSet<NeighborSpace>(this.Criteria);
        Model.CriteriaSet = CriteriaSet;
    }
}

[CustomEditor(typeof(GridTileManager))]
public class GridTileManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        GridTileManager manager = (GridTileManager)target;
        EditorGUILayout.ObjectField("Tile", manager.Model, typeof(TileSet), false);

        serializedObject.Update();
        NeighborSpace[] orig = (NeighborSpace[])manager.Criteria.Clone();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Criteria"), true);
        serializedObject.ApplyModifiedProperties();

        if (!Enumerable.SequenceEqual(orig, manager.Criteria))
        {
            manager.SimplifyCriteria();
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