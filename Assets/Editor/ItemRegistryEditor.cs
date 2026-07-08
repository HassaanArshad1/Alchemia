using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.IO;

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
            
            GUILayout.Space(5);
            if (GUILayout.Button("Generate Missing Placeholder Sprites"))
                GeneratePlaceholderSprites();
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
        
        private void GeneratePlaceholderSprites()
        {
            ItemRegistry registry = (ItemRegistry)target;
            SerializedObject registrySO = new SerializedObject(registry);
            SerializedProperty itemsProp = registrySO.FindProperty("items");

            int generated = 0;
            for (int i = 0; i < itemsProp.arraySize; i++)
            {
                MergeItem item = itemsProp.GetArrayElementAtIndex(i).objectReferenceValue as MergeItem;
                if (item == null || item.Sprite != null) continue;

                string soPath = AssetDatabase.GetAssetPath(item).Replace('\\', '/');
                string soFolder = Path.GetDirectoryName(soPath).Replace('\\', '/');
                string leafFolderName = Path.GetFileName(soFolder); // e.g. "Potions"

                string spriteFolder = $"Assets/Sprites/{leafFolderName}";
                if (!Directory.Exists(spriteFolder))
                    Directory.CreateDirectory(spriteFolder);

                string pngPath = $"{spriteFolder}/{item.name}_icon.png";

                Sprite sprite = PlaceholderSpriteGenerator.CreateSprite(item.ChainId, item.Tier, pngPath);

                SerializedObject itemSO = new SerializedObject(item);
                itemSO.FindProperty("sprite").objectReferenceValue = sprite;
                itemSO.ApplyModifiedProperties();
                generated++;
            }

            Debug.Log($"ItemRegistry: generated {generated} placeholder sprites.");
        }
    }
}