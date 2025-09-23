using UnityEditor;
using UnityEngine;

public class SpriteModeChanger : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        TextureImporter textureImporter = (TextureImporter)assetImporter;

        // Kiểm tra nếu texture là sprite
        if (textureImporter.textureType == TextureImporterType.Sprite)
        {
            // Đặt Sprite Mode thành Single
            textureImporter.spriteImportMode = SpriteImportMode.Single;
        }
    }
}