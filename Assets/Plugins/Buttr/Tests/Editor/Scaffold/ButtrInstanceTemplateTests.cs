using Buttr.Editor.Scaffolding;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrInstanceTemplateTests {
        [Test]
        public void Generate_UI_UsesScopeBuilder() {
            var result = new ButtrInstanceTemplate("MyGame", "MyGame.Features.UI", "UI", PackageType.UI).Generate();
            Assert.That(result, Does.Contain("new ScopeBuilder(UIPackage.Scope)"));
        }

        [Test]
        public void Generate_UI_StoresScopeContainer() {
            var result = new ButtrInstanceTemplate("MyGame", "MyGame.Features.UI", "UI", PackageType.UI).Generate();
            Assert.That(result, Does.Contain("IDIContainer m_Container"));
        }

        [Test]
        public void Generate_UI_IncludesInjectionUsing() {
            var result = new ButtrInstanceTemplate("MyGame", "MyGame.Features.UI", "UI", PackageType.UI).Generate();
            Assert.That(result, Does.Contain("using Buttr.Unity.Injection;"));
        }

        [Test]
        public void Generate_UI_HasRegistrarField() {
            var result = new ButtrInstanceTemplate("MyGame", "MyGame.Features.UI", "UI", PackageType.UI).Generate();
            Assert.That(result, Does.Contain("ScriptableRegistrar m_Registrar"));
        }

        [Test]
        public void Generate_UI_CallsRegistrarInject() {
            var result = new ButtrInstanceTemplate("MyGame", "MyGame.Features.UI", "UI", PackageType.UI).Generate();
            Assert.That(result, Does.Contain("m_Registrar.Inject(builder)"));
        }

        [Test]
        public void Generate_UI_DoesNotHaveInjectorField() {
            var result = new ButtrInstanceTemplate("MyGame", "MyGame.Features.UI", "UI", PackageType.UI).Generate();
            Assert.That(result, Does.Not.Contain("ScriptableInjector m_Injector"));
        }

        [Test]
        public void Generate_UI_CallsUsePackageExtension() {
            var result = new ButtrInstanceTemplate("MyGame", "MyGame.Features.UI", "UI", PackageType.UI).Generate();
            Assert.That(result, Does.Contain("builder.UseUI()"));
        }

        [Test]
        public void Generate_UI_DisposesOnDestroy() {
            var result = new ButtrInstanceTemplate("MyGame", "MyGame.Features.UI", "UI", PackageType.UI).Generate();
            Assert.That(result, Does.Contain("m_Container?.Dispose()"));
        }

        [Test]
        public void Generate_UI_WiresUIDocument() {
            var result = new ButtrInstanceTemplate("MyGame", "MyGame.Features.UI", "UI", PackageType.UI).Generate();
            Assert.That(result, Does.Contain("UIDocument m_UIDocument"));
        }

        [Test]
        public void Generate_Unknown_ReturnsEmpty() {
            var result = new ButtrInstanceTemplate("MyGame", "MyGame.Features.Inventory", "Inventory", PackageType.Feature).Generate();
            Assert.That(result, Is.Empty);
        }
    }
}