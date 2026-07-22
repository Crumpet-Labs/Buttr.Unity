<p align="center">
  <img src="Assets/Docs/Images/buttr-wordmark.svg" alt="Buttr" width="400"/>
</p>

<p align="center">
  <strong>The Unity bridge for Buttr — source-generated injection, scene walking, and editor scaffolding on top of the engine-agnostic DI container.</strong>
</p>

<p align="center">
  <a href="https://github.com/Crumpet-Labs/Buttr.Unity/releases"><img src="https://img.shields.io/github/v/release/Crumpet-Labs/Buttr.Unity?style=flat-square" alt="Release"></a>
  <a href="https://github.com/Crumpet-Labs/Buttr.Unity/blob/main/LICENSE"><img src="https://img.shields.io/github/license/Crumpet-Labs/Buttr.Unity?style=flat-square" alt="License"></a>
  <a href="https://unity.com"><img src="https://img.shields.io/badge/Unity-6+-black?style=flat-square&logo=unity" alt="Unity 6+"></a>
</p>

Buttr adds Unity-specific integration on top of the engine-agnostic [Buttr.Core](https://github.com/Crumpet-Labs/Buttr.Core) DI container:

- **Source-generated `[Inject]`** — `partial` MonoBehaviours and ScriptableObjects get their injection plumbing at compile time; the field-set path runs zero runtime reflection.
- **Scene walking** — `SceneInjector` injects every `[Inject]` object in a scene; `MonoInjector` targets a single object or prefab instance.
- **ScriptableObject wiring** — register assets as container sources with `ScriptableRegistrar`, or make an asset an `[Inject]` target with `ScriptableInjector`.
- **Scoped injection** — `[Inject("key")]` resolves from a named scope instead of the application container.
- **Boot pipeline** — `UnityApplicationBoot` builds your container from `Loader` assets; the built-in `SceneLoader` enters gameplay scenes once it's ready.
- **Nine compile-time analyzers** — `BUTTR001`/`002`/`003`/`005`/`007`/`008`/`009`/`011`/`020` flag a missing `partial`, `[Inject]` on a non-MonoBehaviour, magic-string scope keys, and more. Full table: [Buttr.Unity.SourceGeneration README](https://github.com/Crumpet-Labs/Buttr.Unity.SourceGeneration/blob/main/README.md).
- **Editor scaffolding** — `Tools > Buttr > Setup Project` wires the boot pipeline; a right-click `Buttr > Packages` menu adds whole packages (New Feature/Core/UI) and 16 `Add to Package` archetypes (Model, Service, Controller, Handler, …) into them.

**Find the full stack on our website:** [crumpetlabs.co.uk/buttr](https://crumpetlabs.co.uk/buttr)

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

1. `Tools > Buttr > Setup Project` scaffolds `_Project/`, `Main.unity` boot scene, `Program.cs`, `ProgramLoader`.
2. Register a service in `Program.cs`:

   ```csharp
   var builder = new ApplicationBuilder();
   builder.Resolvers.AddSingleton<IGreeter, Greeter>();
   return builder.Build();
   ```

3. Inject it into a `partial` MonoBehaviour:

   ```csharp
   public partial class Welcome : MonoBehaviour {
       [Inject] private IGreeter i_Greeter;
   }
   ```

4. The container is built at boot in `Start`, but a `SceneInjector` injects at `Awake` — so injected objects can't sit in the boot scene. Put `Welcome` and a `SceneInjector` in a **second scene**, then load it after boot: create a `SceneLoader` asset (`Assets > Create > Buttr > Loaders > Scene`) pointing at that scene and add it to the boot object's **Application Loaders** after `ProgramLoader`. Add both scenes to Build Settings (boot first) and press Play.

Full walkthrough: [Docs/Guides/GettingStarted.md](Docs/Guides/GettingStarted.md).

## Samples

Import the **Health** sample from `Window > Package Manager > Buttr for Unity > Samples`. It's the smallest complete feature — a clamped health value with a UI Toolkit bar — and shows the whole stack in one place: a `Model`, a write-owning `Service`, a designer `Configuration`, a `View`, an `[Inject]` `Controller`, and the `Loader` that builds the container at boot. It ships as scripts only; its README walks you through the two-scene setup the quickstart describes.

## Documentation

| Topic | Guide |
|---|---|
| UPM install, updating, version pinning | [Installation](Docs/Guides/Installation.md) |
| First project end-to-end | [Getting Started](Docs/Guides/GettingStarted.md) |
| Suffix-driven architecture | [Conventions](Docs/Guides/Conventions.md) |
| `[Inject]` + injectors | [MonoBehaviour Injection](Docs/Guides/MonoBehaviourInjection.md) |
| Designer-facing data | [ScriptableObjects](Docs/Guides/ScriptableObjects.md) |
| Setup project + scaffolding | [Editor Tooling](Docs/Guides/EditorTooling.md) |
| Boot pipeline | [Loaders](Docs/Guides/Loaders.md) |

Engine-agnostic core (aliasing, `All<T>()`, `DIBuilder`, `Hidden`, analyzer catalogue): [Buttr.Core docs](https://github.com/Crumpet-Labs/Buttr.Core/tree/main/Docs).

## Changelog

Release history ships with the package: [Assets/Plugins/Buttr/CHANGELOG.md](Assets/Plugins/Buttr/CHANGELOG.md). Entries that change the tracked Buttr.Core version name it.

## Contributing

Contributions are welcome. Open an issue first to discuss what you'd like to change. See [CONTRIBUTING.md](CONTRIBUTING.md).

## License

MIT — see [LICENSE](LICENSE).
