using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Buttr.Editor.Scaffolding {
    internal static class ButtrPackageScaffolderUtility {
        /// <summary>
        /// Builds a namespace from the folder path relative to _Project/.
        /// e.g. _Project/Features/Inventory → ProjectName.Features.Inventory
        /// </summary>
        public static string ResolveNamespace(this string parentFolder, string packageName) {
            var projectFolder = ButtrLayout.RootPath().Replace('\\', '/');
            parentFolder = parentFolder.Replace('\\', '/');

            if (false == parentFolder.StartsWith(projectFolder)) {
                return packageName;
            }

            var relative = parentFolder[projectFolder.Length..]
                .TrimStart('/', '\\');

            var rootNs = GetRootNamespace();

            if (string.IsNullOrEmpty(relative))
                return $"{rootNs}.{packageName}";


            var safeName = SanitiseTypeName(packageName);
            var middle = relative.Replace('/', '.').Replace('\\', '.');
            var safeMiddle = SanitiseNamespace(middle);

            return $"{rootNs}.{safeMiddle}.{safeName}";
        }

        /// <summary>
        /// Reads the root namespace from the project's asmdef in <c>Assets/_Project</c>.
        /// Falls back to the project folder name.
        /// </summary>
        public static string GetRootNamespace() {
            var root = ButtrLayout.RootPath();

            if (Directory.Exists(root)) {
                var asmdefs = Directory.GetFiles(root, "*.asmdef", SearchOption.TopDirectoryOnly);

                if (asmdefs.Length > 0) {
                    var content = File.ReadAllText(asmdefs[0]);
                    var key = "\"rootNamespace\":";
                    var index = content.IndexOf(key, StringComparison.Ordinal);

                    if (index >= 0) {
                        var start = content.IndexOf('"', index + key.Length) + 1;
                        var end = content.IndexOf('"', start);

                        if (start > 0 && end > start)
                            return content.Substring(start, end - start);
                    }
                }
            }

            return SanitiseTypeName(
                Path.GetFileName(Path.GetDirectoryName(Application.dataPath))
            ) ?? "Project";
        }

        /// <summary>
        /// Finds the Catalog folder at <c>Assets/_Project/Catalog</c>.
        /// </summary>
        public static string FindCatalogFolder(this string fromPath) {
            var catalogPath = ButtrLayout.RootSubpath(ButtrLayout.CatalogFolder);
            return Directory.Exists(catalogPath) ? catalogPath : null;
        }

        public static string CreateSubFolder(this string parent, string name) {
            var path = Path.Combine(parent, name);
            Directory.CreateDirectory(path);
            return path;
        }

        public static string EnsureSubFolder(this string parent, string name) {
            var path = Path.Combine(parent, name);

            if (false == Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        public static void WriteFile(this string folder, string fileName, string content) {
            File.WriteAllText(Path.Combine(folder, fileName), content);
        }

        public static void WriteFileIfNew(this string folder, string fileName, string content, bool refresh = false) {
            var path = Path.Combine(folder, fileName);

            if (File.Exists(path)) {
                Debug.Log($"[Buttr] {fileName} already exists — skipping");
                return;
            }

            File.WriteAllText(path, content);
            if(refresh) AssetDatabase.Refresh();
            Debug.Log($"[Buttr] Created {fileName}");
        }

        /// <summary>
        /// Infers the package name and namespace from a folder path.
        /// Walks up to find the package root (folder containing a *.asmdef that isn't the project root).
        /// </summary>
        public static (string ns, string name) InferPackage(this string folderPath) {
            var name = Path.GetFileName(folderPath);
            var packageRoot = folderPath;

            var current = folderPath;

            while (false == string.IsNullOrEmpty(current)) {
                var asmdefFiles = Directory.GetFiles(current, "*.asmdef", SearchOption.TopDirectoryOnly);

                if (asmdefFiles.Length > 0 && false == ButtrLayout.IsProjectRoot(current)) {
                    name = Path.GetFileName(current);
                    packageRoot = current;
                    break;
                }

                if (ButtrLayout.IsProjectRoot(current))
                    break;

                var parent = Path.GetDirectoryName(current);

                if (parent == current) break;

                current = parent;
            }

            var ns = Path.GetDirectoryName(packageRoot).ResolveNamespace(name);
            return (SanitiseNamespace(ns), SanitiseTypeName(name));
        }

        /// <summary>
        /// Queues a ScriptableObject asset for creation after the next domain reload.
        /// Stored as a semicolon-delimited list of typeName|assetPath pairs in EditorPrefs.
        /// </summary>
        public static void QueuePendingAsset(this string typeName, string assetPath) {
            var existing = EditorPrefs.GetString(ButtrLayout.PendingAssetsKey, string.Empty);
            var entry = $"{typeName}|{assetPath}";

            if (existing.Contains(entry)) return;

            var updated = string.IsNullOrEmpty(existing) ? entry : $"{existing};{entry}";
            EditorPrefs.SetString(ButtrLayout.PendingAssetsKey, updated);
        }

        /// <summary>
        /// Sanitises a string into a valid C# identifier.
        /// Strips invalid characters, PascalCases on word boundaries (hyphens, spaces, underscores).
        /// </summary>
        private static readonly Regex InvalidIdentifierChars = new(@"[^\p{L}\p{Nd}_]", RegexOptions.Compiled);
        private static readonly Regex WordBoundary = new(@"[-_\s.]+(.)", RegexOptions.Compiled);
        private static readonly Regex LeadingInvalid = new(@"^[^a-zA-Z_]+", RegexOptions.Compiled);

        /// <summary>
        /// Sanitises a raw string into a valid PascalCase C# type name.
        /// </summary>
        internal static string SanitiseTypeName(string raw) {
            if (string.IsNullOrWhiteSpace(raw)) return "Default";

            var pascalCased = WordBoundary.Replace(raw, m => m.Groups[1].Value.ToUpper());
            var stripped = InvalidIdentifierChars.Replace(pascalCased, string.Empty);
            stripped = LeadingInvalid.Replace(stripped, string.Empty);

            if (stripped.Length == 0) return "Default";

            return char.ToUpper(stripped[0]) + stripped[1..];
        }

        /// <summary>
        /// Sanitises a dotted namespace string by sanitising each segment individually.
        /// </summary>
        internal static string SanitiseNamespace(string raw) {
            if (string.IsNullOrWhiteSpace(raw)) return "Project";

            var segments = raw.Split('.');
            var results = new List<string>(segments.Length);

            foreach (var segment in segments) {
                var sanitised = SanitiseTypeName(segment);
                if (sanitised != "Default" || segment.Trim().Length > 0)
                    results.Add(sanitised);
            }

            return results.Count == 0 ? "Project" : string.Join(".", results);
        }
    }
}