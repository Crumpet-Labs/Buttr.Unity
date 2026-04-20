#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Buttr.Core.Examples.Aliasing {
    public interface IGreeter {
        string Greet();
    }

    public interface IFarewell {
        string Farewell();
    }

    public sealed class EnglishSpeaker : IGreeter, IFarewell {
        public string Greet() => "Hello";
        public string Farewell() => "Goodbye";
    }

    public sealed class FrenchSpeaker : IGreeter, IFarewell {
        public string Greet() => "Bonjour";
        public string Farewell() => "Au revoir";
    }

    public static class AliasingSmokeTest {
        [MenuItem("Tools/Buttr/Aliasing Smoke Test")]
        public static void Run() {
            var builder = new ApplicationBuilder();
            // EnglishSpeaker is aliased as IGreeter + IFarewell; FrenchSpeaker is a plain concrete.
            // All<IGreeter>() should return both — EnglishSpeaker via its alias key, FrenchSpeaker via assignable-to.
            builder.Resolvers.AddSingleton<EnglishSpeaker>().As<IGreeter>().As<IFarewell>();
            builder.Resolvers.AddSingleton<FrenchSpeaker>();

            var app = builder.Build();

            try {
                var greeters = 0;
                foreach (var g in app.All<IGreeter>()) {
                    Debug.Log($"[AliasingSmokeTest] IGreeter -> {g.GetType().Name}: {g.Greet()}");
                    greeters++;
                }
                Debug.Log($"[AliasingSmokeTest] All<IGreeter>() count = {greeters} (expected 2)");

                var english = app.Get<EnglishSpeaker>();
                var englishViaGreeter = app.Get<IGreeter>();
                var englishViaFarewell = app.Get<IFarewell>();
                var shared = ReferenceEquals(english, englishViaGreeter)
                          && ReferenceEquals(english, englishViaFarewell);
                Debug.Log(shared
                    ? "[AliasingSmokeTest] PASS: aliases share the singleton instance across all keys"
                    : "[AliasingSmokeTest] FAIL: alias resolved a different instance from the concrete key");
            } finally {
                app.Dispose();
            }
        }

#if BUTTR_ANALYZER_SMOKE
        // Enable BUTTR_ANALYZER_SMOKE in Project Settings -> Player -> Scripting Define Symbols
        // to compile these lines and verify BUTTR013/BUTTR014 fire from Buttr.Core.Analyzers.
        // Expected: two compile errors — one BUTTR013, one BUTTR014.
        public interface IUnrelated { }

        public static void AnalyzerTripLines() {
            var builder = new ApplicationBuilder();

            // BUTTR013: EnglishSpeaker does not implement IUnrelated — alias is not a supertype.
            builder.Resolvers.AddSingleton<EnglishSpeaker>().As<IUnrelated>();

            // BUTTR014: IGreeter claimed twice — duplicate alias key across registrations.
            builder.Resolvers.AddSingleton<EnglishSpeaker>().As<IGreeter>();
            builder.Resolvers.AddSingleton<FrenchSpeaker>().As<IGreeter>();
        }
#endif
    }
}
#endif
