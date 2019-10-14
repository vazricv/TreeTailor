#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExportAssetBundle : MonoBehaviour
{

    [MenuItem("Assets/Build AssetBundle")]
    static void ExportResource()
    {
        Debug.Log(Selection.activeObject.name);
        string path = "Assets/AssetBundles/" + Selection.activeObject.name + ".unity3d";
        Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        foreach (var c in selection)
            Debug.Log(c.name);
        BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path,
                                       BuildAssetBundleOptions.CollectDependencies |
                                       BuildAssetBundleOptions.CompleteAssets, BuildTarget.WebGL);
        // BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.WebGL);
    }

}
#endif