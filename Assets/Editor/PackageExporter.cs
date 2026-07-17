using System.IO;
using UnityEditor;
using UnityEngine;

namespace Buttr.Editor {
    public static class PackageExporter {
        [MenuItem("Buttr/Export Unity Package")]
        public static void CreatePackage() {
            var projectContent = new[] {
                "Assets/Plugins/Buttr", 
            };

            var packageName = "Buttr.unitypackage";
            var outputPath = Path.Combine(Application.dataPath, "..", packageName);

            AssetDatabase.ExportPackage(
                projectContent,
                outputPath,
                ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies
            );

            Debug.Log($"Successfully exported Unity package to: {outputPath}");
        }
    }
}