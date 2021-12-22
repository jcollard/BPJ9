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

    public bool IsFloorOverride;
    public bool IsCenterOverride;

    public void SimplifyCriteria()
    {
        // TODO: Remove duplicates???
        HashSet<NeighborSpace> CriteriaSet = new HashSet<NeighborSpace>(this.Criteria);
        Model.CriteriaSet = CriteriaSet;
    }

    public void DiscoverCriteria()
    {
        if (this.IsFloorOverride && this.IsCenterOverride) throw new System.Exception("Cannot be both floor and center wall.");

        this.Model.IsFloor = false;

        if (this.IsFloorOverride)
        {
            this.Criteria = new NeighborSpace[] { };
            this.Model.IsFloor = true;
        }
        else if (this.IsCenterOverride)
        {
            this.Criteria = NeighborSpaceUtil.Spaces;
        }
        else
        {
            HashSet<NeighborSpace> newCriteria = new HashSet<NeighborSpace>();
            Transform parent = this.Model.transform.parent;
            for (int ix = 0; ix < parent.childCount; ix++)
            {
                Transform siblingTransform = parent.GetChild(ix);
                GameObject sibling = siblingTransform.gameObject;
                if (sibling == this.Model.gameObject) continue;
                Vector2 diff = siblingTransform.position - this.transform.position;
                if(NeighborSpaceUtil
                    .SpaceLookup
                    .TryGetValue(((int)diff.x, (int)diff.y), out NeighborSpace space))
                {
                    newCriteria.Add(space);
                }
            }
            this.Criteria = newCriteria.ToArray();
        }

        this.SimplifyCriteria();
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
        manager.IsFloorOverride = EditorGUILayout.Toggle("Is Floor Override", manager.IsFloorOverride);
        manager.IsCenterOverride = EditorGUILayout.Toggle("Is Center Override", manager.IsCenterOverride);

        serializedObject.Update();
        NeighborSpace[] orig = (NeighborSpace[])manager.Criteria.Clone();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Criteria"), true);
        serializedObject.ApplyModifiedProperties();

        if (!Enumerable.SequenceEqual(orig, manager.Criteria))
        {
            manager.SimplifyCriteria();
        }

        if (GUILayout.Button("Discover Criteria"))
        {
            manager.DiscoverCriteria();
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