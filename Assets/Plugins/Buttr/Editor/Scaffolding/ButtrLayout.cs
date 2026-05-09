using System.IO;
using UnityEngine;

namespace Buttr.Editor.Scaffolding {
    /// <summary>
    /// Single source of truth for the Buttr project convention layout — folder names,
    /// scene/asset names, editor-pref keys, and path helpers. All scaffolding,
    /// validation, and menu code reads from here rather than hardcoding string literals.
    /// </summary>
    internal static class ButtrLayout {
        // ── Folder names ─────────────────────────────────────────────────
        public const string RootFolder = "_Project";
        public const string CoreFolder = "Core";
        public const string FeaturesFolder = "Features";
        public const string SharedFolder = "Shared";
        public const string CatalogFolder = "Catalog";
        public const string LoadersFolder = "Loaders";
        public const string UIFolder = "UI";

        // ── File names ───────────────────────────────────────────────────
        public const string SceneName = "Main.unity";
        public const string ProgramScriptName = "Program.cs";
        public const string ProgramLoaderScriptName = "ProgramLoader.cs";
        public const string ProgramLoaderAssetName = "ProgramLoader.asset";

        // ── Editor preference keys ───────────────────────────────────────
        public const string SetupVersionKey = "Buttr.SetupVersion";
        public const string PendingAssetCreationKey = "Buttr.PendingAssetCreation";
        public const string PendingAssetsKey = "Buttr.PendingAssets";

        // ── Layout ───────────────────────────────────────────────────────
        public static readonly string[] ConventionFolders = {
            LoadersFolder,
            CoreFolder,
            FeaturesFolder,
            SharedFolder,
            CatalogFolder,
        };

        // ── Path helpers ─────────────────────────────────────────────────

        /// <summary>Absolute disk path to <c>Assets/_Project</c>.</summary>
        public static string RootPath() 
            => Path.Combine(Application.dataPath, RootFolder);

        /// <summary>Absolute disk path to a subfolder under <c>Assets/_Project</c>.</summary>
        public static string RootSubpath(string subfolder) 
            => Path.Combine(Application.dataPath, RootFolder, subfolder);

        // ── State checks ─────────────────────────────────────────────────

        /// <summary>True if the project has been set up — has <c>_Project/</c> with at least one asmdef.</summary>
        public static bool HasConventionStructure() {
            var root = RootPath();

            return Directory.Exists(root)
                && Directory.GetFiles(root, "*.asmdef", SearchOption.TopDirectoryOnly).Length > 0;
        }

        /// <summary>True if the given absolute path resolves to <c>Assets/_Project</c>.</summary>
        public static bool IsProjectRoot(string folder) {
            return false == string.IsNullOrEmpty(folder)
                && Path.GetFullPath(folder) == Path.GetFullPath(RootPath());
        }
    }
}
