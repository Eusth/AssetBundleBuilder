using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundles
{
    public static void BuildAllAssetBundles()
    {
        // Create the array of bundle build details.
        AssetBundleBuild[] buildMap = new AssetBundleBuild[1];


        var assets = new List<string>();
        var path = Path.Combine(Application.dataPath, @"{ASSETS_PATH}");
        foreach (var file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
        {
            var filePath = file.Substring(Application.dataPath.Length - "Assets".Length);
            assets.Add(filePath);
        }

        buildMap[0].assetBundleName = @"{BUNDLE_NAME}";
        buildMap[0].assetNames = assets.ToArray();

        BuildPipeline.BuildAssetBundles(@"{OUTPUT_PATH}", buildMap, BuildAssetBundleOptions.ForceRebuildAssetBundle, BuildTarget.StandaloneWindows64);
    }
}