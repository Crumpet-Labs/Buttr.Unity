using System;
using System.Collections.Generic;
using Buttr.Injection;
using UnityEngine;

namespace Buttr.Unity.Injection {
    [Serializable]
    public sealed class ScriptableInjector {
        [SerializeField] private List<ScriptableObject> m_Objects;

        public void InjectAll() {
            if (m_Objects == null) return;

            foreach (var obj in m_Objects) {
                if (obj == null) continue;
                
                InjectionProcessor.Reset(obj);
                InjectionProcessorUnityExtensions.Inject(obj);
            }
        }
    }
}
