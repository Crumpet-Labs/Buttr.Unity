using Buttr.Editor.Scaffolding;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrLoaderTemplateTests {
        [Test]
        public void Generate_Feature_UsesScopeBuilder() {
            var result = new ButtrLoaderTemplate("MyGame", "MyGame.Features.Inventory", "Inventory", PackageType.Feature).Generate();
            Assert.That(result, Does.Contain("new ScopeBuilder(InventoryPackage.Scope)"));
        }

        [Test]
        public void Generate_Feature_StoresScopeContainer() {
            var result = new ButtrLoaderTemplate("MyGame", "MyGame.Features.Inventory", "Inventory", PackageType.Feature).Generate();
            Assert.That(result, Does.Contain("IDIContainer m_Container"));
        }

        [Test]
        public void Generate_Core_UsesApplicationBuilder() {
            var result = new ButtrLoaderTemplate("MyGame", "MyGame.Core.Audio", "Audio", PackageType.Core).Generate();
            Assert.That(result, Does.Contain("new ApplicationBuilder()"));
        }

        [Test]
        public void Generate_Core_StoresApplicationContainer() {
            var result = new ButtrLoaderTemplate("MyGame", "MyGame.Core.Audio", "Audio", PackageType.Core).Generate();
            Assert.That(result, Does.Contain("ApplicationContainer m_Container"));
        }

        [Test]
        public void Generate_UsesProjectNameInMenuPath() {
            var result = new ButtrLoaderTemplate("MyGame", "MyGame.Features.Inventory", "Inventory", PackageType.Feature).Generate();
            Assert.That(result, Does.Contain("menuName = \"MyGame/Loaders/Inventory\""));
        }

        [Test]
        public void Generate_CallsUsePackageExtension() {
            var result = new ButtrLoaderTemplate("MyGame", "MyGame.Features.Inventory", "Inventory", PackageType.Feature).Generate();
            Assert.That(result, Does.Contain("builder.UseInventory()"));
        }

        [Test]
        public void Generate_DisposesOnUnload() {
            var result = new ButtrLoaderTemplate("MyGame", "MyGame.Features.Inventory", "Inventory", PackageType.Feature).Generate();
            Assert.That(result, Does.Contain("m_Container?.Dispose()"));
        }

        [Test]
        public void Generate_Feature_IncludesInjectionUsing() {
            var result = new ButtrLoaderTemplate("MyGame", "MyGame.Features.Inventory", "Inventory", PackageType.Feature).Generate();
            Assert.That(result, Does.Contain("using Buttr.Unity.Injection;"));
        }

        [Test]
        public void Generate_Feature_HasRegistrarField() {
            var result = new ButtrLoaderTemplate("MyGame", "MyGame.Features.Inventory", "Inventory", PackageType.Feature).Generate();
            Assert.That(result, Does.Contain("ScriptableRegistrar m_Registrar"));
        }

        [Test]
        public void Generate_Feature_CallsRegistrarInject() {
            var result = new ButtrLoaderTemplate("MyGame", "MyGame.Features.Inventory", "Inventory", PackageType.Feature).Generate();
            Assert.That(result, Does.Contain("m_Registrar.Inject(builder)"));
        }

        [Test]
        public void Generate_Feature_DoesNotHaveInjectorField() {
            var result = new ButtrLoaderTemplate("MyGame", "MyGame.Features.Inventory", "Inventory", PackageType.Feature).Generate();
            Assert.That(result, Does.Not.Contain("ScriptableInjector m_Injector"));
        }

        [Test]
        public void Generate_Core_IncludesInjectionUsing() {
            var result = new ButtrLoaderTemplate("MyGame", "MyGame.Core.Audio", "Audio", PackageType.Core).Generate();
            Assert.That(result, Does.Contain("using Buttr.Unity.Injection;"));
        }

        [Test]
        public void Generate_Core_HasRegistrarField() {
            var result = new ButtrLoaderTemplate("MyGame", "MyGame.Core.Audio", "Audio", PackageType.Core).Generate();
            Assert.That(result, Does.Contain("ScriptableRegistrar m_Registrar"));
        }

        [Test]
        public void Generate_Core_CallsRegistrarInject() {
            var result = new ButtrLoaderTemplate("MyGame", "MyGame.Core.Audio", "Audio", PackageType.Core).Generate();
            Assert.That(result, Does.Contain("m_Registrar.Inject(builder)"));
        }

        [Test]
        public void Generate_Core_HasInjectorField() {
            var result = new ButtrLoaderTemplate("MyGame", "MyGame.Core.Audio", "Audio", PackageType.Core).Generate();
            Assert.That(result, Does.Contain("ScriptableInjector m_Injector"));
        }

        [Test]
        public void Generate_Core_CallsInjectAll() {
            var result = new ButtrLoaderTemplate("MyGame", "MyGame.Core.Audio", "Audio", PackageType.Core).Generate();
            Assert.That(result, Does.Contain("m_Injector.InjectAll()"));
        }

        [Test]
        public void Generate_Core_InjectAllAfterBuild() {
            var result = new ButtrLoaderTemplate("MyGame", "MyGame.Core.Audio", "Audio", PackageType.Core).Generate();
            var buildIndex = result.IndexOf("m_Container = builder.Build();");
            var injectAllIndex = result.IndexOf("m_Injector.InjectAll();");
            Assert.That(injectAllIndex, Is.GreaterThan(buildIndex));
        }
    }
}