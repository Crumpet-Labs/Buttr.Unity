using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Buttr.Tests.Editor.Unity.Injection {
    public sealed class ScriptableInjectionTests {
        private ScriptableInjectionLoader m_Loader;
        private InjectableSettings m_Settings;

        [UnitySetUp]
        public IEnumerator Setup() {
            m_Loader = Resources.Load<ScriptableInjectionLoader>("ScriptableInjectionLoader");
            m_Settings = Resources.Load<InjectableSettings>("InjectableSettings");
            m_Settings.Clock = null;
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown() {
            yield return m_Loader.UnloadAsync();

            m_Loader = null;
            m_Settings = null;
        }

        [UnityTest] public IEnumerator ScriptableObject_FieldInjectedAfterBoot() {
            yield return m_Loader.LoadAsync(default);

            Assert.IsNotNull(m_Settings.Clock);
            Assert.IsInstanceOf<SystemClock>(m_Settings.Clock);
        }
    }
}
