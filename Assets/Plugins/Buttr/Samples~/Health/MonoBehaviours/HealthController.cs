using Buttr.Injection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Buttr.Samples.Health {
    public sealed partial class HealthController : MonoBehaviour {
        [Inject] private IHealthService i_Service;
        [Inject] private HealthModel i_Model;

        [SerializeField] private UIDocument m_Document;
        [SerializeField] private int m_AmountPerHit = 10;

        private HealthView m_View;

        private void Start() {
            m_View = new HealthView(m_Document, i_Model);
            m_View.Render();
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                i_Service.TakeDamage(m_AmountPerHit);
                m_View.Render();
            }
            else if (Input.GetKeyDown(KeyCode.H)) {
                i_Service.Heal(m_AmountPerHit);
                m_View.Render();
            }
        }
    }
}
