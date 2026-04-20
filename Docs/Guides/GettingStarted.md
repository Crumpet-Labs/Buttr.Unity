# Getting Started

This guide walks you from an empty Unity project to a running, injected application.

## 1. Install the package

See [Installation](Installation.md).

## 2. Run the setup menu

`Tools > Buttr > Setup Project`.

This scaffolds the following under `Assets/_Project/`:

```
Assets/
└── _Project/
    ├── {ProjectName}.asmdef
    ├── Main.unity               # Boot scene (added to build settings at index 0)
    ├── Program.cs               # Application composition entry point
    ├── ProgramLoader.cs         # Bridge between Unity lifecycle and Program.Main()
    ├── Loaders/
    │   └── ProgramLoader.asset  # Scene-referenced instance of ProgramLoader
    ├── Core/                    # Engine-agnostic packages reusable across projects
    ├── Features/                # Game-specific feature packages
    ├── Shared/                  # Assets and scripts shared across Core and Features
    └── Catalog/                 # ScriptableObject data assets, organised by feature
```

The boot scene (`Main.unity`) contains a single `Boot` GameObject with the `UnityApplicationBoot` component pointing at `ProgramLoader.asset`. Hit Play — nothing happens yet, but the boot pipeline is wired.

## 3. Register your first service

Open `Program.cs`:

```csharp
using System.Collections.Generic;
using Buttr.Unity;

namespace YourGame {
    public static class Program {
        public static ApplicationContainer Main() => Main(CMDArgs.Read());

        private static ApplicationContainer Main(IDictionary<string, string> args) {
            var builder = new ApplicationBuilder();

            builder.Resolvers.AddSingleton<IGreeter, Greeter>();

            return builder.Build();
        }
    }
}
```

Add a simple service:

```csharp
namespace YourGame {
    public interface IGreeter {
        string Greet(string name);
    }

    public sealed class Greeter : IGreeter {
        public string Greet(string name) => $"Hello, {name}!";
    }
}
```

## 4. Inject it into a MonoBehaviour

```csharp
using Buttr.Injection;
using UnityEngine;

namespace YourGame {
    public partial class Welcome : MonoBehaviour {
        [Inject] private IGreeter m_Greeter;

        private void Start() {
            Debug.Log(m_Greeter.Greet("world"));
        }
    }
}
```

Drop the `Welcome` component on a GameObject in any scene, attach a `SceneInjector` to the scene's boot GameObject (or to an object that lives before `Awake`), and press Play. Console shows `Hello, world!`.

For details on injection setup see [MonoBehaviour Injection](MonoBehaviourInjection.md).

## 5. Next steps

- **Organise by feature**, not by file type. See [Conventions](Conventions.md).
- **Scaffold new packages** via right-click → `Buttr > Packages > New Feature`. See [Editor Tooling](EditorTooling.md).
- **Inject data** from ScriptableObjects. See [ScriptableObjects](ScriptableObjects.md).
- **Add boot-time setup** beyond the single `Program.cs`. See [Loaders](Loaders.md).

## Core concepts in one page

```csharp
var builder = new ApplicationBuilder();

// Singletons — one instance, shared
builder.Resolvers.AddSingleton<IFoo, Foo>();

// Transients — new instance each resolve
builder.Resolvers.AddTransient<IBar, Bar>();

// Hidden — available for constructor injection, not via Application<T>.Get()
builder.Hidden.AddSingleton<IInternal, Internal>();

// Aliasing — one resolver, many type keys
builder.Resolvers.AddSingleton<AuthService>()
    .As<IAuthService>()
    .As<ISessionProvider>();

// Factories and post-build configuration
builder.Resolvers.AddTransient<Widget>()
    .WithFactory(() => new Widget())
    .WithConfiguration(w => { w.Label = "Hello"; return w; });

var app = builder.Build();

// Resolve from anywhere
var foo = Application<IFoo>.Get();

// Bulk resolve — every registration assignable to IFoo, zero-alloc
foreach (var item in Application.All<IFoo>()) { /* ... */ }

// Dispose — cleans up all resolved IDisposables
app.Dispose();
```

For aliasing, bulk resolution, and `Hidden` in depth, see the [Buttr.Core docs](https://github.com/Crumpet-Labs/Buttr.Core/tree/main/Docs).
