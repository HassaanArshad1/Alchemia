using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Alchemia.Data.Editor
{
    [CustomEditor(typeof(ItemRegistry))]
    public class ItemRegistryEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);
            if (GUILayout.Button("Auto-Populate From Project"))
                PopulateFromProject();
        }

        private void PopulateFromProject()
        {
            ItemRegistry registry = (ItemRegistry)target;
            SerializedObject so = new SerializedObject(registry);
            SerializedProperty itemsProp = so.FindProperty("items");

            string[] guids = AssetDatabase.FindAssets("t:MergeItem");
            List<MergeItem> found = guids
                .Select(g => AssetDatabase.LoadAssetAtPath<MergeItem>(AssetDatabase.GUIDToAssetPath(g)))
                .Where(item => item != null)
                .OrderBy(item => item.ChainId)
                .ThenBy(item => item.Tier)
                .ToList();

            itemsProp.ClearArray();
            for (int i = 0; i < found.Count; i++)
            {
                itemsProp.InsertArrayElementAtIndex(i);
                itemsProp.GetArrayElementAtIndex(i).objectReferenceValue = found[i];
            }

            so.ApplyModifiedProperties();
            Debug.Log($"ItemRegistry: auto-populated with {found.Count} MergeItem assets.");
        }
    }
}