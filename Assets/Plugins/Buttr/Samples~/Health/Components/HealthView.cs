using UnityEngine.UIElements;

namespace Buttr.Samples.Health {
    public sealed class HealthView {
        private readonly HealthModel m_Model;
        private readonly VisualElement m_Fill;
        private readonly Label m_Label;

        public HealthView(UIDocument document, HealthModel model) {
            m_Model = model;

            var root = document.rootVisualElement;
            m_Fill = root.Q<VisualElement>("health-fill");
            m_Label = root.Q<Label>("health-label");
        }

        public void Render() {
            var ratio = m_Model.Max > 0 ? (float)m_Model.Current / m_Model.Max : 0f;
            m_Fill.style.width = Length.Percent(ratio * 100f);
            m_Label.text = $"{m_Model.Current} / {m_Model.Max}";
        }
    }
}
