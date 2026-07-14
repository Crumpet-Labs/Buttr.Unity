using System.Threading;
using Buttr.Core;
using Buttr.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Buttr.Samples.Health {
    [CreateAssetMenu(fileName = "HealthLoader", menuName = "Buttr Samples/Health/Loader", order = 0)]
    public sealed class HealthLoader : UnityApplicationLoaderBase {
        [SerializeField] private HealthConfig m_Config;
        [SerializeField] private string m_HealthSceneName = "Health";

        private ApplicationContainer m_Container;

        public override async Awaitable LoadAsync(CancellationToken cancellationToken) {
            var builder = new ApplicationBuilder();

            builder.UseHealth(m_Config);

            m_Container = builder.Build();

            var op = SceneManager.LoadSceneAsync(m_HealthSceneName, LoadSceneMode.Additive);

            while (op != null && false == op.isDone) {
                await Awaitable.NextFrameAsync(cancellationToken);
            }
        }

        public override Awaitable UnloadAsync() {
            m_Container?.Dispose();
            return AwaitableUtility.CompletedTask;
        }
    }
}
