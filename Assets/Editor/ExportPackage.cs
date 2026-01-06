using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExportPackage : MonoBehaviour
{
    [MenuItem("Export/Export with tags and layers, Input settings")]
    public static void Eexport()
    {
        string[] projectContent = new string[] { "Assets", "ProjectSettings/TagManager.asset", "ProjectSettings/InputManager.asset", "ProjectSettings/ProjectSettings.asset" };
        AssetDatabase.ExportPackage(projectContent, "Project3D.unitypackage", ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
        Debug.Log("Project Exported");
    }
}
