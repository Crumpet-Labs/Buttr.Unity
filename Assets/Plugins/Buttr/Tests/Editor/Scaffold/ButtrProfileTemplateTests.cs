using Buttr.Editor.Scaffolding;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrProfileTemplateTests {
        [Test]
        public void Generate_IsAbstract() {
            var result = new ButtrProfileTemplate("MyGame.Features.Movement", "Movement").Generate();
            Assert.That(result, Does.Contain("public abstract class MovementProfile"));
        }

        [Test]
        public void Generate_ExtendsScriptableObject() {
            var result = new ButtrProfileTemplate("MyGame.Features.Movement", "Movement").Generate();
            Assert.That(result, Does.Contain("ScriptableObject"));
        }
    }
}
