namespace Buttr.Samples.Health {
    public interface IHealthService {
        void TakeDamage(int amount);
        void Heal(int amount);
    }
}
