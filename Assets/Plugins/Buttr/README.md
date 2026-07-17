# Buttr for Unity

A lightweight dependency-injection container with an opinionated architecture for Unity 6+. Adds MonoBehaviour, ScriptableObject, and scene-walking integration on top of the engine-agnostic [Buttr.Core](https://github.com/Crumpet-Labs/Buttr.Core) library.

- **Source-generated injection** — `[Inject]` fields on `partial` MonoBehaviours and ScriptableObjects. The field-injection path runs zero reflection at runtime; container build itself uses minimal reflection (constructor scanning, alias mapping).
- **Roslyn analyzers** — 18 compile-time rules (9 in Buttr.Core, 9 Unity-specific) for duplicate registrations, missing dependencies, alias mismatches, `[Inject]` on a non-`partial` class, and more — half of them with one-click code fixes.
- **Suffix-driven architecture** — Models, Services, Handlers, Behaviours. Every class name tells you what it does.
- **Editor scaffolding** — one-click project setup, right-click to scaffold conventions-compliant packages.

## Installation

Buttr.Unity depends on Buttr.Core. UPM doesn't auto-resolve git-URL dependencies, so install Core **first**, then Unity. In `Window > Package Manager` → `+` → **Install package from git URL**:

1. Install Buttr.Core:

   ```
   https://github.com/Crumpet-Labs/Buttr.Core.git?path=package
   ```

2. Install Buttr.Unity:

   ```
   https://github.com/Crumpet-Labs/Buttr.Unity.git?path=Assets/Plugins/Buttr
   ```

Pin versions by appending a tag (e.g. `#v1.4.1` for Core, `#v3.0.1` for Unity). Requires Unity 6.0+.

## Getting started

1. `Tools > Buttr > Setup Project` — scaffolds `_Project/`, a `Main.unity` boot scene, `Program.cs`, and `ProgramLoader`.
2. Register a service in `Program.cs`. `Main` receives the parsed launch arguments from `CMDArgs.Read()` — ignore the parameter until you need to branch on them:

   ```csharp
   public interface IGreeter { string Greet(string name); }

   public sealed class Greeter : IGreeter {
       public string Greet(string name) => $"Hello, {name}!";
   }

   public static class Program {
       public static ApplicationContainer Main() => Main(CMDArgs.Read());

       private static ApplicationContainer Main(IDictionary<string, string> args) {
           var builder = new ApplicationBuilder();
           builder.Resolvers.AddSingleton<IGreeter, Greeter>();
           return builder.Build();
       }
   }
   ```

3. Inject it into a `partial` MonoBehaviour:

   ```csharp
   public partial class Welcome : MonoBehaviour {
       [Inject] private IGreeter i_Greeter;

       private void Start() => Debug.Log(i_Greeter.Greet("world"));
   }
   ```

4. Put `Welcome` in a **second scene**, not the boot scene. Buttr injects `[Inject]` fields at `Awake`, but the boot scene doesn't build the container until `Start` — so an injected object has to be entered *after* boot. Add a `SceneInjector` to the second scene (it fills `[Inject]` fields on entry), create a `SceneLoader` asset (`Assets > Create > Buttr > Loaders > Scene`) set to that scene's name, and add it to `UnityApplicationBoot`'s **Application Loaders** list after `ProgramLoader`. Put both scenes in Build Settings, boot scene first, and press Play from the boot scene — the Console prints `Hello, world!`.

Full walkthrough: [Docs/Guides/GettingStarted.md](https://github.com/Crumpet-Labs/Buttr.Unity/blob/main/Docs/Guides/GettingStarted.md).

## Common patterns

- **Runtime-spawned prefabs** — put a `MonoInjector` on the prefab root and pick a `MonoInjectStrategy` (`Mono` / `GameObject` / `GameObjectAndChildren`); it fills the `[Inject]` fields on `Instantiate` → `Awake`, then removes itself.
- **Entering gameplay scenes after boot** — a `SceneLoader` asset in `UnityApplicationBoot`'s loader list, after `ProgramLoader`, loads a build-settings scene once the container is built, so its injectors run against a ready container.
- **Scoped injection** — `[Inject(Scopes.Inventory)]` resolves from a named scope instead of the application container. Define scope keys as `const string`; the `BUTTR009` analyzer flags magic-string keys.

## Samples

Import via `Window > Package Manager` → **Buttr for Unity** → **Samples** → **Import**.

**Health** — the smallest *complete* Buttr feature, and the conventions applied end-to-end: a Model, a Service that owns the writes, a designer-tunable Configuration, a View that only reads, an `[Inject]` Controller, and a Loader that builds the container at boot. Read it after Getting Started, when you want the whole **boot → inject → Service mutates Model → View reads** loop in one place rather than a snippet at a time. Requires a short two-scene setup — see the sample's README.

## Documentation

Unity-specific guides live in [Docs/Guides/](https://github.com/Crumpet-Labs/Buttr.Unity/tree/main/Docs/Guides):

| Guide | What it covers |
|---|---|
| [Installation](https://github.com/Crumpet-Labs/Buttr.Unity/blob/main/Docs/Guides/Installation.md) | UPM install, updating, version pinning |
| [Getting Started](https://github.com/Crumpet-Labs/Buttr.Unity/blob/main/Docs/Guides/GettingStarted.md) | First project — `Program.cs`, `ProgramLoader`, boot scene |
| [Conventions](https://github.com/Crumpet-Labs/Buttr.Unity/blob/main/Docs/Guides/Conventions.md) | Suffix-driven architecture |
| [MonoBehaviour Injection](https://github.com/Crumpet-Labs/Buttr.Unity/blob/main/Docs/Guides/MonoBehaviourInjection.md) | `[Inject]`, `partial`, `SceneInjector`, `MonoInjector` |
| [ScriptableObjects](https://github.com/Crumpet-Labs/Buttr.Unity/blob/main/Docs/Guides/ScriptableObjects.md) | `ScriptableRegistrar`, `ScriptableInjector`, Configurations, Definitions, Handlers, Profiles |
| [Editor Tooling](https://github.com/Crumpet-Labs/Buttr.Unity/blob/main/Docs/Guides/EditorTooling.md) | Setup project, right-click scaffolding |
| [Loaders](https://github.com/Crumpet-Labs/Buttr.Unity/blob/main/Docs/Guides/Loaders.md) | `UnityApplicationBoot`, boot pipeline |

Engine-agnostic core (aliasing, `All<T>()`, `DIBuilder`, `Hidden`, analyzer catalogue): [Buttr.Core docs](https://github.com/Crumpet-Labs/Buttr.Core/tree/main/Docs).

## Changelog

[CHANGELOG.md](https://github.com/Crumpet-Labs/Buttr.Unity/blob/main/Assets/Plugins/Buttr/CHANGELOG.md) ships with the package — each entry names the Buttr.Core version it tracks. Upgrading from 2.x? 3.0.0 renamed `ScriptableInjector` → `ScriptableRegistrar`; see its migration note in the changelog.

## License

MIT — see [LICENSE.md](https://github.com/Crumpet-Labs/Buttr.Unity/blob/main/Assets/Plugins/Buttr/LICENSE.md).
