using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Buttr.Unity {
    [CreateAssetMenu(fileName = "SceneLoader", menuName = "Buttr/Loaders/Scene", order = 1)]
    public sealed class SceneLoader : UnityApplicationLoaderBase {
        [SerializeField] private string m_SceneName;
        [SerializeField] private LoadSceneMode m_LoadMode = LoadSceneMode.Additive;

        public override async Awaitable LoadAsync(CancellationToken cancellationToken) {
            var op = SceneManager.LoadSceneAsync(m_SceneName, m_LoadMode);

            while (op != null && false == op.isDone) {
                await Awaitable.NextFrameAsync(cancellationToken);
            }
        }
    }
}
