#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
using System.Linq;
using CaptainCoder.Unity;

[RequireComponent(typeof(WallTile))]
public class WallTileManager : MonoBehaviour
{
    public WallTile Model => this.GetComponent<WallTile>();
    public NeighborSpace[] Criteria => Model.CriteriaSet.ToArray();
    public GridTileSet TileSet;
    public SpriteLoader SpriteLoader => TileSet == null ? null : TileSet.Sprites;
    public static int SelectedSpriteSet;
    public static int Row;
    public static int Col;

    public void SimplifyCriteria()
    {
        // TODO: Remove duplicates???
        HashSet<NeighborSpace> CriteriaSet = new HashSet<NeighborSpace>(this.Criteria);
        Model.CriteriaSet = CriteriaSet;
    }

    public void PushChanges()
    {
        GridTileSetManager tileSetManager = TileSet.GetComponent<GridTileSetManager>();
        if (tileSetManager != null)
        {
            tileSetManager.UpdateWallTile(Model);
        }
    }
}

[CustomEditor(typeof(WallTileManager))]
public class WallTileManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        WallTileManager manager = (WallTileManager)target;
        EditorGUILayout.ObjectField("Tile", manager.Model, typeof(WallTile), false);
        EditorGUILayout.ObjectField("Sprite Loader", manager.SpriteLoader, typeof(SpriteLoader), true);

        manager.TileSet = (GridTileSet)EditorGUILayout.ObjectField("Tile Set", manager.TileSet, typeof(GridTileSet), true);

        if (GUILayout.Button("Center Camera"))
        {
            UnityEditorUtils.Instance.CenterSceneCameraOn(manager.gameObject, 3);
        }

        if (manager.SpriteLoader != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Template Adder");
            string[] options = manager.SpriteLoader.Sets.Select(set => set.Name).ToArray();
            WallTileManager.SelectedSpriteSet = EditorGUILayout.Popup(WallTileManager.SelectedSpriteSet, options);
            SpriteSet set = manager.SpriteLoader.Sets[WallTileManager.SelectedSpriteSet];

            EditorGUILayout.BeginHorizontal();
            WallTileManager.Row = Mathf.Clamp(WallTileManager.Row, 0, set.Width);
            WallTileManager.Col = Mathf.Clamp(WallTileManager.Col, 0, set.Height);
            EditorGUILayout.LabelField($"Row: {WallTileManager.Row}");
            EditorGUILayout.LabelField($"Col {WallTileManager.Col}:");
            EditorGUILayout.EndHorizontal();


            for (int r = 0; r < set.Height; r++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int c = 0; c < set.Width; c++)
                {
                    if (set.sprites[r * set.Width + c] == null)
                    {
                        GUILayout.Button("   ");
                    }
                    else
                    {
                        if (GUILayout.Button($"{r},{c}")) {
                            WallTileManager.Row = r;
                            WallTileManager.Col = c;
                            int ix = (WallTileManager.Row * set.Width) + WallTileManager.Col;
                            manager.GetComponent<SpriteRenderer>().sprite = manager.SpriteLoader.GetSpriteTemplate(set, ix).GetSprite();
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Template"))
            {
                int ix = (WallTileManager.Row * set.Width) + WallTileManager.Col;
                manager.Model.Templates.Add(manager.SpriteLoader.GetSpriteTemplate(set, ix));
            }

            if (GUILayout.Button("Replace Template and Push"))
            {
                int ix = (WallTileManager.Row * set.Width) + WallTileManager.Col;
                manager.Model.Templates = new List<SpriteTemplate>();
                manager.Model.Templates.Add(manager.SpriteLoader.GetSpriteTemplate(set, ix));
                manager.GetComponent<SpriteRenderer>().sprite = manager.Model.Templates[0].GetSprite();
                manager.PushChanges();
            }

            if (GUILayout.Button("Center on TemplateSet"))
            {
                UnityEditorUtils.Instance.CenterSceneCameraOn(manager.SpriteLoader.GetSetContainer(set).gameObject, 3);
            }
            EditorGUILayout.Space();
        }
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("Criteria"), true);

        if (GUILayout.Button("Push Changes"))
        {
            manager.PushChanges();
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