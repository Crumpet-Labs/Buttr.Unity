namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrMediatorTemplate {
        private readonly string m_Ns;
        private readonly string m_Name;

        public ButtrMediatorTemplate(string ns, string name) {
            m_Ns = ns;
            m_Name = name;
        }

        public string Generate() {
            return $@"using System;

namespace {m_Ns} {{
    public sealed class {m_Name}Mediator : IDisposable {{
        private readonly I{m_Name}Service m_Service;
        private readonly {m_Name}Model m_Model;

        public {m_Name}Mediator(I{m_Name}Service service, {m_Name}Model model) {{
            // todo: subscribe to model changes

            m_Service = service;
            m_Model = model;
        }}

        public void Dispose() {{
            // todo: unsubscribe from model changes
        }}
    }}
}}
";
        }
    }
}