using UnityEngine;
using UnityEditor;
using System.IO;

namespace Alchemia.Data.Editor
{
    public static class PlaceholderSpriteGenerator
    {
        private const int TextureSize = 128;

        public static Sprite CreateSprite(string chainId, int tier, string savePath)
        {
            Color chainColor = ColorForChain(chainId);
            Texture2D tex = new Texture2D(TextureSize, TextureSize, TextureFormat.RGBA32, false);

            FillTransparent(tex);
            DrawFilledCircle(tex, TextureSize / 2, TextureSize / 2, TextureSize * 0.42f, chainColor);
            DrawRingOutline(tex, TextureSize / 2, TextureSize / 2, TextureSize * 0.42f, Color.white, 3f);
            DrawTierPips(tex, tier);
            tex.Apply();

            File.WriteAllBytes(savePath, tex.EncodeToPNG());
            AssetDatabase.ImportAsset(savePath);

            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(savePath);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.filterMode = FilterMode.Bilinear;
            importer.SaveAndReimport();

            return AssetDatabase.LoadAssetAtPath<Sprite>(savePath);
        }

        private static Color ColorForChain(string chainId)
        {
            uint hash = Fnv1aHash(chainId);
            float hue = (hash % 360) / 360f;
            return Color.HSVToRGB(hue, 0.55f, 0.9f);
        }

        // Manual stable hash — string.GetHashCode() isn't guaranteed
        // consistent across runs/processes, and we need reproducible colors.
        private static uint Fnv1aHash(string s)
        {
            uint hash = 2166136261;
            foreach (char c in s)
            {
                hash ^= c;
                hash *= 16777619;
            }
            return hash;
        }

        private static void FillTransparent(Texture2D tex)
        {
            Color clear = new Color(0, 0, 0, 0);
            Color[] pixels = new Color[tex.width * tex.height];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = clear;
            tex.SetPixels(pixels);
        }

        private static void DrawFilledCircle(Texture2D tex, int cx, int cy, float radius, Color color)
        {
            int r = Mathf.CeilToInt(radius);
            for (int y = -r; y <= r; y++)
            for (int x = -r; x <= r; x++)
            {
                if (x * x + y * y <= radius * radius)
                {
                    int px = cx + x, py = cy + y;
                    if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                        tex.SetPixel(px, py, color);
                }
            }
        }

        private static void DrawRingOutline(Texture2D tex, int cx, int cy, float radius, Color color, float thickness)
        {
            int r = Mathf.CeilToInt(radius);
            for (int y = -r; y <= r; y++)
            for (int x = -r; x <= r; x++)
            {
                float dist = Mathf.Sqrt(x * x + y * y);
                if (dist <= radius && dist >= radius - thickness)
                {
                    int px = cx + x, py = cy + y;
                    if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                        tex.SetPixel(px, py, color);
                }
            }
        }

        private static void DrawTierPips(Texture2D tex, int tier)
        {
            float pipRadius = TextureSize * 0.045f;
            float spacing = pipRadius * 2.6f;
            float totalWidth = (tier - 1) * spacing;
            float startX = (TextureSize / 2f) - (totalWidth / 2f);
            float y = TextureSize * 0.14f;

            for (int i = 0; i < tier; i++)
            {
                float x = startX + i * spacing;
                DrawFilledCircle(tex, Mathf.RoundToInt(x), Mathf.RoundToInt(y), pipRadius, Color.white);
            }
        }
    }
}