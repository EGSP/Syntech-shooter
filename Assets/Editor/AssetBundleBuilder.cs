
using UnityEditor;

public class AssetBundleBuilder
{
    [MenuItem("Assets/BuildAssetBundles")]
    public static void BuildAssetBundles()
    {
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}
