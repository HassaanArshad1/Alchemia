using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace Alchemia.Data.Editor
{
    public class MergeChainGeneratorWindow : EditorWindow
    {
        private string chainId = "";
        private string namePrefix = "";
        private List<string> tierNames = new List<string> { "" };

        [MenuItem("Alchemia/Merge Chain Generator")]
        public static void ShowWindow()
        {
            GetWindow<MergeChainGeneratorWindow>("Merge Chain Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Generate a Merge Chain", EditorStyles.boldLabel);
            chainId = EditorGUILayout.TextField("Chain Id (lowercase)", chainId);
            namePrefix = EditorGUILayout.TextField("Name Prefix (e.g. \"Herb\")", namePrefix);

            string subfolder = string.IsNullOrEmpty(namePrefix) ? "" : $"{namePrefix}s";
            EditorGUILayout.LabelField("Output", $"Assets/ScriptableObjects/{subfolder}/");

            GUILayout.Label("Tiers (in order, tier 1 first)");
            for (int i = 0; i < tierNames.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                tierNames[i] = EditorGUILayout.TextField($"Tier {i + 1}", tierNames[i]);
                if (GUILayout.Button("-", GUILayout.Width(24)))
                {
                    tierNames.RemoveAt(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+ Add Tier"))
                tierNames.Add("");

            GUILayout.Space(10);
            if (GUILayout.Button("Generate MergeItem Assets"))
                Generate();
        }

        private void Generate()
        {
            if (string.IsNullOrWhiteSpace(chainId) || string.IsNullOrWhiteSpace(namePrefix))
            {
                Debug.LogError("MergeChainGenerator: Chain Id and Name Prefix are both required.");
                return;
            }

            string folder = $"Assets/ScriptableObjects/{namePrefix}s";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            for (int i = 0; i < tierNames.Count; i++)
            {
                int tier = i + 1;
                string displayName = string.IsNullOrWhiteSpace(tierNames[i])
                    ? $"{namePrefix} Tier {tier}"
                    : tierNames[i];

                MergeItem item = ScriptableObject.CreateInstance<MergeItem>();
                SerializedObject so = new SerializedObject(item);
                so.FindProperty("chainId").stringValue = chainId;
                so.FindProperty("tier").intValue = tier;
                so.FindProperty("displayName").stringValue = displayName;
                so.ApplyModifiedPropertiesWithoutUndo();

                string assetPath = $"{folder}/{namePrefix}_Tier{tier}.asset";
                AssetDatabase.CreateAsset(item, assetPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"MergeChainGenerator: created {tierNames.Count} assets in {folder}");
        }
    }
}