# Buttr for Unity — Guides

Practical walkthroughs for using Buttr in a Unity 6+ project. For the engine-agnostic core (aliasing, `All<T>()`, `DIBuilder`, `Hidden`, analyzer catalogue), see the [Buttr.Core docs](https://github.com/Crumpet-Labs/Buttr.Core/tree/main/Docs).

## Contents

| Guide | What it covers |
|---|---|
| [Installation](Installation.md) | UPM install, updating, version pinning |
| [Getting Started](GettingStarted.md) | First project — `Program.cs`, `ProgramLoader`, boot scene |
| [Conventions](Conventions.md) | Suffix-driven architecture: Models, Presenters, Services, Handlers, etc. |
| [MonoBehaviour Injection](MonoBehaviourInjection.md) | `[Inject]`, `partial`, `SceneInjector`, `MonoInjector` |
| [ScriptableObjects](ScriptableObjects.md) | `ScriptableInjector`, Configurations, Definitions, Handlers |
| [Editor Tooling](EditorTooling.md) | `Tools > Buttr > Setup Project`, right-click scaffolding |
| [Loaders](Loaders.md) | `UnityApplicationBoot`, `UnityApplicationLoaderBase`, the boot pipeline |

## When to read what

- **First project with Buttr?** Installation → Getting Started → Editor Tooling.
- **Adding a feature to an existing Buttr project?** Conventions → Editor Tooling.
- **Injecting into a MonoBehaviour for the first time?** MonoBehaviour Injection.
- **Injecting data from a ScriptableObject?** ScriptableObjects.
- **Writing a boot-time loader?** Loaders.
