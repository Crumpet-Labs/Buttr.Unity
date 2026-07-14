using Buttr.Core;

namespace Buttr.Samples.Health {
    public static class HealthPackage {
        public static IConfigurableCollection UseHealth(this ApplicationBuilder builder, HealthConfig config) {
            return new ConfigurableCollection()
                .Register(builder.Resolvers.AddSingleton<HealthConfig>().WithFactory(() => config))
                .Register(builder.Resolvers.AddSingleton<IHealthService, HealthService>())
                .Register(builder.Resolvers.AddSingleton<HealthModel>());
        }
    }
}
