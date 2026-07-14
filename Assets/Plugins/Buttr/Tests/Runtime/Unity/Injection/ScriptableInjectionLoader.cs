using System.Threading;
using Buttr.Core;
using Buttr.Unity;
using Buttr.Unity.Injection;
using UnityEngine;

namespace Buttr.Tests.Editor.Unity.Injection {
    internal sealed class ScriptableInjectionLoader : UnityApplicationLoaderBase {
        [SerializeField] private ScriptableInjector m_Injector;

        private ApplicationContainer m_Container;

        public override Awaitable LoadAsync(CancellationToken cancellationToken) {
            var builder = new ApplicationBuilder();
            builder.Resolvers.AddSingleton<IClock, SystemClock>();
            m_Container = builder.Build();
            m_Injector.InjectAll();
            return AwaitableUtility.CompletedTask;
        }

        public override Awaitable UnloadAsync() {
            m_Container?.Dispose();
            return AwaitableUtility.CompletedTask;
        }
    }
}
