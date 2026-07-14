namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrProfileTemplate {
        private readonly string m_Ns;
        private readonly string m_Name;

        public ButtrProfileTemplate(string ns, string name) {
            m_Ns = ns;
            m_Name = name;
        }

        public string Generate() {
            return $@"using UnityEngine;

namespace {m_Ns} {{
    public abstract class {m_Name}Profile : ScriptableObject {{

        // Below is an example of a potential profile method => concrete subclasses bake in
        // their own [SerializeField] data and interpret it through overrides like this
        public abstract void Resolve();
    }}
}}
";
        }
    }
}
