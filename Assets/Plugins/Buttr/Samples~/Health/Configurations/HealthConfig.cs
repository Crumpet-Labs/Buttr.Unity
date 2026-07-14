using UnityEngine;

namespace Buttr.Samples.Health {
    [CreateAssetMenu(fileName = "HealthConfig", menuName = "Buttr Samples/Health/Config", order = 0)]
    public sealed class HealthConfig : ScriptableObject {
        [SerializeField] private int m_MaxHealth = 100;

        public int MaxHealth => m_MaxHealth;
    }
}
