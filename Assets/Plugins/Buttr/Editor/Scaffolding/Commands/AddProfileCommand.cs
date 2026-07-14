namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddProfileCommand {
        private readonly string m_PackageFolder;
        private readonly bool m_RefreshAssetDatabase;

        public AddProfileCommand(string packageFolder, bool refreshAssetDatabase) {
            m_PackageFolder = packageFolder;
            m_RefreshAssetDatabase = refreshAssetDatabase;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var folder = m_PackageFolder.EnsureSubFolder("Profiles");
            folder.WriteFileIfNew($"{name}Profile.cs", new ButtrProfileTemplate(ns, name).Generate(), m_RefreshAssetDatabase);
        }
    }
}
