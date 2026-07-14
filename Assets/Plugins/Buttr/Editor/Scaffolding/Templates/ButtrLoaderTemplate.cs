namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrLoaderTemplate {
        private readonly string m_ProjectName;
        private readonly string m_Ns;
        private readonly string m_Name;
        private readonly PackageType m_Type;

        public ButtrLoaderTemplate(string projectName, string ns, string name, PackageType type) {
            m_ProjectName = projectName;
            m_Ns = ns;
            m_Name = name;
            m_Type = type;
        }

        public string Generate() {
            return m_Type switch {
                PackageType.Feature => $@"using System.Threading;
using Buttr.Core;
using Buttr.Unity;
using Buttr.Unity.Injection;
using UnityEngine;

namespace {m_Ns} {{
    [CreateAssetMenu(fileName = ""{m_Name}Loader"", menuName = ""{m_ProjectName}/Loaders/{m_Name}"", order = 0)]
    public sealed class {m_Name}Loader : UnityApplicationLoaderBase {{
        [SerializeField] private ScriptableRegistrar m_Registrar;

        private IDIContainer m_Container;

        public override Awaitable LoadAsync(CancellationToken cancellationToken) {{
            var builder = new ScopeBuilder({m_Name}Package.Scope);

            m_Registrar.Inject(builder);

            builder.Use{m_Name}();

            m_Container = builder.Build();
            return AwaitableUtility.CompletedTask;
        }}

        public override Awaitable UnloadAsync() {{
            m_Container?.Dispose();
            return AwaitableUtility.CompletedTask;
        }}
    }}
}}
",
                PackageType.Core => $@"using System.Threading;
using Buttr.Core;
using Buttr.Unity;
using Buttr.Unity.Injection;
using UnityEngine;

namespace {m_Ns} {{
    [CreateAssetMenu(fileName = ""{m_Name}Loader"", menuName = ""{m_ProjectName}/Loaders/{m_Name}"", order = 0)]
    public sealed class {m_Name}Loader : UnityApplicationLoaderBase {{
        [Header(""Scriptable Objects"")]
        [SerializeField] private ScriptableRegistrar m_Registrar;
        [SerializeField] private ScriptableInjector m_Injector;

        private ApplicationContainer m_Container;

        public override Awaitable LoadAsync(CancellationToken cancellationToken) {{
            var builder = new ApplicationBuilder();

            m_Registrar.Inject(builder);

            builder.Use{m_Name}();

            m_Container = builder.Build();

            m_Injector.InjectAll();

            return AwaitableUtility.CompletedTask;
        }}

        public override Awaitable UnloadAsync() {{
            m_Container?.Dispose();
            return AwaitableUtility.CompletedTask;
        }}
    }}
}}
",
                _ => string.Empty
            };
        }
    }

}