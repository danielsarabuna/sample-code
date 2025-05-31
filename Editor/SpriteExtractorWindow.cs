using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class SpriteExtractorWindow : EditorWindow
{
    private Texture2D sourceTexture;
    private List<SpriteData> spriteDataList = new List<SpriteData>();
    private Vector2 scrollPosition;
    private string savePath = "Assets/Sprites";

    [MenuItem("Tools/Sprite Extractor")]
    public static void ShowWindow()
    {
        GetWindow<SpriteExtractorWindow>("Sprite Extractor");
    }

    public class SpriteData
    {
        public string name;
        public Rect rect;
        public Vector2 pivot;
    }

    private void OnEnable()
    {
        // Убедимся, что у исходной текстуры включено Read/Write
        if (sourceTexture != null)
        {
            string path = AssetDatabase.GetAssetPath(sourceTexture);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null && !importer.isReadable)
            {
                importer.isReadable = true;
                importer.SaveAndReimport();
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Sprite Extractor", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        sourceTexture =
            (Texture2D)EditorGUILayout.ObjectField("Source Texture", sourceTexture, typeof(Texture2D), false);
        if (EditorGUI.EndChangeCheck())
        {
            LoadSprites();
        }

        savePath = EditorGUILayout.TextField("Save Path", savePath);

        if (GUILayout.Button("Extract Selected Sprites"))
        {
            ExtractSelectedSprites();
        }
    }

    private void LoadSprites()
    {
        spriteDataList.Clear();

        if (sourceTexture == null) return;

        string path = AssetDatabase.GetAssetPath(sourceTexture);
        Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(path);

        foreach (Object obj in sprites)
        {
            if (obj is Sprite sprite)
            {
                SpriteData spriteData = ExtractSpriteData(sprite);
                spriteDataList.Add(spriteData);
            }
        }
    }

    private SpriteData ExtractSpriteData(Sprite sprite)
    {
        return new SpriteData
        {
            name = sprite.name,
            rect = sprite.rect,
            pivot = sprite.pivot / sprite.rect.size, // Конвертируем pivot в относительные координаты
        };
    }

    private void DrawSpriteList()
    {
        if (spriteDataList.Count == 0)
        {
            EditorGUILayout.HelpBox("No sprites loaded", MessageType.Info);
            return;
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < spriteDataList.Count; i++)
        {
            SpriteData spriteData = spriteDataList[i];
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(spriteData.name);

            // Отображение информации о pivot
            EditorGUILayout.Vector2Field("Pivot", spriteData.pivot);

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    private void ExtractSelectedSprites()
    {
        if (!CheckTextureSettings())
        {
            EditorUtility.DisplayDialog("Error", "No source texture selected", "OK");
            return;
        }

        List<SpriteData> selectedSprites = spriteDataList;
        if (selectedSprites.Count == 0)
        {
            EditorUtility.DisplayDialog("Error", "No sprites selected", "OK");
            return;
        }

        // Создаем папку, если она не существует
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
            AssetDatabase.Refresh();
        }

        string baseTextureName = Path.GetFileNameWithoutExtension(sourceTexture.name);

        foreach (var spriteData in selectedSprites)
        {
            // Создаем новую текстуру для каждого спрайта
            Texture2D newTexture = new Texture2D((int)spriteData.rect.width, (int)spriteData.rect.height);

            // Получаем пиксели из исходной текстуры
            var pixels = sourceTexture.GetPixels(
                (int)spriteData.rect.x,
                (int)spriteData.rect.y,
                (int)spriteData.rect.width,
                (int)spriteData.rect.height
            );

            // Применяем пиксели к новой текстуре
            newTexture.SetPixels(pixels);
            newTexture.Apply();

            // Конвертируем текстуру в bytes
            byte[] bytes = newTexture.EncodeToPNG();

            string newFileName = $"{baseTextureName}-{spriteData.name}.png";
            string finalPath = Path.Combine(savePath, newFileName);

            // Сохраняем файл
            File.WriteAllBytes(finalPath, bytes);

            // Очищаем временную текстуру
            DestroyImmediate(newTexture);

            // Настраиваем импортер для нового спрайта
            AssetDatabase.Refresh();
            string assetPath = finalPath.Replace(Application.dataPath, "Assets");
            TextureImporter newImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (newImporter != null)
            {
                newImporter.textureType = TextureImporterType.Sprite;
                newImporter.spriteImportMode = SpriteImportMode.Single;
                newImporter.spritePixelsPerUnit = 100;

                // Устанавливаем pivot
                newImporter.spritePivot = spriteData.pivot;
                // newImporter.spriteAlignment = (int)SpriteAlignment.Custom;

                // Применяем настройки чтения/записи
                newImporter.isReadable = true;

                // Отключаем сжатие для лучшего качества
                newImporter.textureCompression = TextureImporterCompression.Uncompressed;

                newImporter.SaveAndReimport();
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"Successfully extracted {selectedSprites.Count} sprites to {savePath}");
    }

    private bool CheckTextureSettings()
    {
        if (sourceTexture == null) return false;

        string path = AssetDatabase.GetAssetPath(sourceTexture);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

        if (importer == null) return false;

        bool settingsCorrect = true;

        if (!importer.isReadable)
        {
            Debug.LogWarning("Read/Write is not enabled for the source texture. Enabling...");
            importer.isReadable = true;
            settingsCorrect = false;
        }

        if (importer.textureType != TextureImporterType.Sprite)
        {
            Debug.LogWarning("Texture Type is not set to Sprite. Changing...");
            importer.textureType = TextureImporterType.Sprite;
            settingsCorrect = false;
        }

        if (!settingsCorrect)
        {
            importer.SaveAndReimport();
        }

        return true;
    }

    private bool ValidatePivot(Vector2 pivot)
    {
        return pivot.x >= 0f && pivot.x <= 1f &&
               pivot.y >= 0f && pivot.y <= 1f;
    }

    private Vector2 ConvertPixelToPivot(Vector2 pixelPivot, Vector2 spriteSize)
    {
        return new Vector2(
            pixelPivot.x / spriteSize.x,
            pixelPivot.y / spriteSize.y
        );
    }

    private Vector2 CalculatePivot(Rect spriteRect, Vector2 pivotPoint)
    {
        return new Vector2(
            pivotPoint.x / spriteRect.width,
            pivotPoint.y / spriteRect.height
        );
    }
}