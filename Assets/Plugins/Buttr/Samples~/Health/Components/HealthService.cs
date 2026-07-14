using UnityEngine;

namespace Buttr.Samples.Health {
    public sealed class HealthService : IHealthService {
        private readonly HealthModel m_Model;

        public HealthService(HealthModel model, HealthConfig config) {
            m_Model = model;
            m_Model.Max = config.MaxHealth;
            m_Model.Current = config.MaxHealth;
        }

        public void TakeDamage(int amount) {
            m_Model.Current = Mathf.Clamp(m_Model.Current - amount, 0, m_Model.Max);
        }

        public void Heal(int amount) {
            m_Model.Current = Mathf.Clamp(m_Model.Current + amount, 0, m_Model.Max);
        }
    }
}
