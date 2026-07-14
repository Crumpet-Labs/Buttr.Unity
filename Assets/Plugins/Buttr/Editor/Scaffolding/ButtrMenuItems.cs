using UnityEditor;

namespace Buttr.Editor.Scaffolding {
    internal static class ButtrMenuItems {
        private const int BasePriority = 100;

        [MenuItem("Assets/Buttr/Packages/New Feature", false, BasePriority)]
        private static void NewFeature() {
            var parent = ButtrMenuItemsUtility.GetSelectedFolder();
            var featuresFolder = ButtrLayout.RootSubpath(ButtrLayout.FeaturesFolder);

            if (false == parent.Replace('\\', '/').Contains($"/{ButtrLayout.FeaturesFolder}"))
                parent = featuresFolder;

            ButtrNewPackagePopup.Show(parent, PackageType.Feature);
        }

        [MenuItem("Assets/Buttr/Packages/New Feature", true)]
        private static bool NewFeatureValidation() {
            return ButtrMenuItemsUtility.HasConventionStructure();
        }

        [MenuItem("Assets/Buttr/Packages/New Core", false, BasePriority + 1)]
        private static void NewCorePackage() {
            var parent = ButtrMenuItemsUtility.GetSelectedFolder();
            var coreFolder = ButtrLayout.RootSubpath(ButtrLayout.CoreFolder);

            if (false == parent.Replace('\\', '/').Contains($"/{ButtrLayout.CoreFolder}"))
                parent = coreFolder;

            ButtrNewPackagePopup.Show(parent, PackageType.Core);
        }

        [MenuItem("Assets/Buttr/Packages/New Core", true)]
        private static bool NewCorePackageValidation() {
            return ButtrMenuItemsUtility.HasConventionStructure();
        }
        
        [MenuItem("Assets/Buttr/Packages/New UI", false, BasePriority + 1)]
        private static void NewUIPackage() {
            var parent = ButtrMenuItemsUtility.GetSelectedFolder();
            var uiFolder = ButtrLayout.RootSubpath(ButtrLayout.UIFolder);

            if (false == parent.Replace('\\', '/').Contains($"/{ButtrLayout.UIFolder}"))
                parent = uiFolder;

            ButtrNewPackagePopup.Show(parent, PackageType.UI);
        }

        [MenuItem("Assets/Buttr/Packages/New UI", true)]
        private static bool NewUIPackageValidation() {
            return ButtrMenuItemsUtility.HasConventionStructure();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Unity/Controller", false, BasePriority)]
        private static void AddController() {
            new AddControllerCommand(ButtrMenuItemsUtility.FindPackageRoot(), true).Execute();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Unity/Controller", true)]
        private static bool AddControllerValidation() {
            return ButtrMenuItemsUtility.HasConventionStructure() && ButtrMenuItemsUtility.IsInsidePackage();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Unity/View", false, BasePriority + 1)]
        private static void AddView() {
            new AddViewCommand(ButtrMenuItemsUtility.FindPackageRoot(), true).Execute();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Unity/View", true)]
        private static bool AddViewValidation() {
            return ButtrMenuItemsUtility.HasConventionStructure() && ButtrMenuItemsUtility.IsInsidePackage();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Data/Model", false, BasePriority)]
        private static void AddModel() {
            new AddModelCommand(ButtrMenuItemsUtility.FindPackageRoot(), true).Execute();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Data/Model", true)]
        private static bool AddModelValidation() {
            return ButtrMenuItemsUtility.HasConventionStructure() && ButtrMenuItemsUtility.IsInsidePackage();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Data/Identifier", false, BasePriority + 1)]
        private static void AddIdentifier() {
            new AddIdentifierCommand(ButtrMenuItemsUtility.FindPackageRoot(), true).Execute();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Data/Identifier", true)]
        private static bool AddIdentifierValidation() {
            return ButtrMenuItemsUtility.HasConventionStructure() && ButtrMenuItemsUtility.IsInsidePackage();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Data/Definition", false, BasePriority + 2)]
        private static void AddDefinition() {
            new AddDefinitionCommand(ButtrMenuItemsUtility.FindPackageRoot(), true).Execute();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Data/Definition", true)]
        private static bool AddDefinitionValidation() {
            return ButtrMenuItemsUtility.HasConventionStructure() && ButtrMenuItemsUtility.IsInsidePackage();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Data/Configuration", false, BasePriority + 3)]
        private static void AddConfiguration() {
            new AddConfigurationCommand(ButtrMenuItemsUtility.FindPackageRoot(), true).Execute();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Data/Configuration", true)]
        private static bool AddConfigurationValidation() {
            return ButtrMenuItemsUtility.HasConventionStructure() && ButtrMenuItemsUtility.IsInsidePackage();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Logic/System", false, BasePriority + 1)]
        private static void AddSystem() {
            new AddSystemCommand(ButtrMenuItemsUtility.FindPackageRoot(), true).Execute();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Logic/System", true)]
        private static bool AddSystemValidation() {
            return ButtrMenuItemsUtility.HasConventionStructure() && ButtrMenuItemsUtility.IsInsidePackage();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Logic/Mediator", false, BasePriority + 2)]
        private static void AddMediator() {
            new AddMediatorCommand(ButtrMenuItemsUtility.FindPackageRoot(), true).Execute();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Logic/Mediator", true)]
        private static bool AddMediatorValidation() {
            return ButtrMenuItemsUtility.HasConventionStructure() && ButtrMenuItemsUtility.IsInsidePackage();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Logic/Handler", false, BasePriority + 3)]
        private static void AddHandler() {
            new AddHandlerCommand(ButtrMenuItemsUtility.FindPackageRoot(), true).Execute();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Logic/Handler", true)]
        private static bool AddHandlerValidation() {
            return ButtrMenuItemsUtility.HasConventionStructure() && ButtrMenuItemsUtility.IsInsidePackage();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Logic/Profile", false, BasePriority + 4)]
        private static void AddProfile() {
            new AddProfileCommand(ButtrMenuItemsUtility.FindPackageRoot(), true).Execute();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Logic/Profile", true)]
        private static bool AddProfileValidation() {
            return ButtrMenuItemsUtility.HasConventionStructure() && ButtrMenuItemsUtility.IsInsidePackage();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Logic/Behaviour", false, BasePriority + 5)]
        private static void AddBehaviour() {
            new AddBehaviourCommand(ButtrMenuItemsUtility.FindPackageRoot(), true).Execute();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Logic/Behaviour", true)]
        private static bool AddBehaviourValidation() {
            return ButtrMenuItemsUtility.HasConventionStructure() && ButtrMenuItemsUtility.IsInsidePackage();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Infrastructure/Service + Contract", false, BasePriority)]
        private static void AddServiceAndContract() {
            new AddServiceAndContractCommand(ButtrMenuItemsUtility.FindPackageRoot(), true).Execute();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Infrastructure/Service + Contract", true)]
        private static bool AddServiceAndContractValidation() {
            return ButtrMenuItemsUtility.HasConventionStructure() && ButtrMenuItemsUtility.IsInsidePackage();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Infrastructure/Repository", false, BasePriority + 1)]
        private static void AddRepository() {
            new AddRepositoryCommand(ButtrMenuItemsUtility.FindPackageRoot(), true).Execute();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Infrastructure/Repository", true)]
        private static bool AddRepositoryValidation() {
            return ButtrMenuItemsUtility.HasConventionStructure() && ButtrMenuItemsUtility.IsInsidePackage();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Infrastructure/Registry", false, BasePriority + 2)]
        private static void AddRegistry() {
            new AddRegistryCommand(ButtrMenuItemsUtility.FindPackageRoot(), true).Execute();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Infrastructure/Registry", true)]
        private static bool AddRegistryValidation() {
            return ButtrMenuItemsUtility.HasConventionStructure() && ButtrMenuItemsUtility.IsInsidePackage();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Infrastructure/Loader", false, BasePriority + 3)]
        private static void AddLoader() {
            new AddLoaderCommand(ButtrMenuItemsUtility.FindPackageRoot(), ButtrMenuItemsUtility.InferPackageType(), true).Execute();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Infrastructure/Loader", true)]
        private static bool AddLoaderValidation() {
            return ButtrMenuItemsUtility.HasConventionStructure() && ButtrMenuItemsUtility.IsInsidePackage();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Structure/Extensions", false, BasePriority)]
        private static void AddExtensions() {
            new AddExtensionsCommand(ButtrMenuItemsUtility.FindPackageRoot(), true).Execute();
        }

        [MenuItem("Assets/Buttr/Packages/Add to Package/Structure/Extensions", true)]
        private static bool AddExtensionsValidation() {
            return ButtrMenuItemsUtility.HasConventionStructure() && ButtrMenuItemsUtility.IsInsidePackage();
        }
    }
}