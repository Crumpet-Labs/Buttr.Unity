# Loaders

Loaders are how Buttr bridges Unity's scene lifecycle to your `Program.cs` composition. The boot scene runs a `UnityApplicationBoot` component, which walks a list of `UnityApplicationLoaderBase` ScriptableObjects in sequence, awaiting each one's `LoadAsync` before starting the next.

This gives you a clean async/await pipeline for bootstrapping your application — no dependency on scene load order, no scattered `[RuntimeInitializeOnLoadMethod]` calls, no guessing what ran when.

## The boot sequence

1. Unity loads `Main.unity` (the boot scene, added to build settings at index 0).
2. `UnityApplicationBoot.Start()` fires, using the GameObject's `destroyCancellationToken` as the cancellation source.
3. It iterates `m_ApplicationLoaders` in list order, `await`ing `LoadAsync(CancellationToken)` on each.
4. Control returns. The application is fully composed. From here, your own code takes over — typically by loading the next scene.
5. When the boot GameObject is destroyed (or on application quit), `OnDestroy()` iterates the same list in the same order, awaiting each `UnloadAsync()`.

A `[SerializeField] bool m_DontDestroyOnLoad` option keeps the boot GameObject alive across scene loads, which is what you want if `UnloadAsync()` should fire at application quit rather than when leaving the boot scene.

## UnityApplicationLoaderBase

The base class:

```csharp
public abstract class UnityApplicationLoaderBase : ScriptableObject {
    public abstract Awaitable LoadAsync(CancellationToken cancellationToken);
    public virtual Awaitable UnloadAsync() => AwaitableUtility.CompletedTask;
}
```

`UnloadAsync` has a no-op default — override it only when you need to dispose the container or clean up. Every loader is a ScriptableObject, so the actual `.asset` file can be dragged into the `UnityApplicationBoot` component's loader list in the Inspector and configured with serialized fields.

## ProgramLoader — the generated default

`Tools > Buttr > Setup Project` generates a `ProgramLoader` that calls your `Program.Main()`:

```csharp
[CreateAssetMenu(fileName = "ProgramLoader",
                 menuName = "Buttr/Loaders/Program", order = 0)]
public sealed class ProgramLoader : UnityApplicationLoaderBase {
    private ApplicationContainer m_Container;

    public override Awaitable LoadAsync(CancellationToken ct) {
        m_Container = Program.Main();
        return AwaitableUtility.CompletedTask;
    }

    public override Awaitable UnloadAsync() {
        m_Container?.Dispose();
        return AwaitableUtility.CompletedTask;
    }
}
```

The generated `Program.cs` is where all your composition lives:

```csharp
public static class Program {
    public static ApplicationContainer Main() => Main(CMDArgs.Read());

    private static ApplicationContainer Main(IDictionary<string, string> args) {
        var builder = new ApplicationBuilder();

        builder.UseConsole();
        builder.UseAudio();
        builder.UseNetworking();

        return builder.Build();
    }
}
```

This separation — composition in pure C# (`Program.cs`), lifecycle bridge in a ScriptableObject (`ProgramLoader`) — lets you test composition without Unity, and swap loader behaviour without touching composition.

## Custom loaders

For features that need their own boot-time setup beyond `Program.cs`, inherit from `UnityApplicationLoaderBase`:

```csharp
[CreateAssetMenu(fileName = "SceneLoader",
                 menuName = "Game/Scene/Loader")]
public sealed class SceneLoader : UnityApplicationLoaderBase {
    [SerializeField] private SceneConfiguration m_Config;

    private ApplicationContainer m_Container;

    public override Awaitable LoadAsync(CancellationToken ct) {
        var builder = new ApplicationBuilder();

        builder.Resolvers.AddSingleton(m_Config);
        builder.Resolvers.AddSingleton<ISceneService, SceneService>();

        m_Container = builder.Build();
        return AwaitableUtility.CompletedTask;
    }

    public override Awaitable UnloadAsync() {
        m_Container?.Dispose();
        return AwaitableUtility.CompletedTask;
    }
}
```

Create the asset (`Right-click > Create > Game > Scene > Loader`), drop it into the Boot GameObject's loader list after `ProgramLoader`.

## Loader ordering

Loaders run **in list order**, both for load and for unload. Order matters when later loaders depend on registrations from earlier ones — e.g., a feature loader that reads a `GameConfiguration` registered by `ProgramLoader`.

Because unload iterates in the same order (not reversed), an early loader that teardown-depends on a later loader's registrations needs to defend against it. In practice, each loader owns and disposes its own `ApplicationContainer` — there's no implicit dependency graph between them at teardown.

## Where loader assets live

**Exception to the Catalog rule:** loader `.asset` files live alongside their script in the package's `Loaders/` folder, not in `Catalog/`.

```
Features/Combat/
└── Loaders/
    ├── CombatLoader.cs
    └── CombatLoader.asset
```

This is because a loader is part of the package's machinery — not data that designers manage. It's closer to a script file than to a Configuration.

## Cancellation

`LoadAsync` receives a `CancellationToken`. If the boot is cancelled (application quit during boot, for example), the token signals cancellation. Respect it in long-running loaders:

```csharp
public override async Awaitable LoadAsync(CancellationToken ct) {
    var data = await LoadRemoteConfigAsync(ct);  // If cancelled, LoadRemoteConfigAsync should throw
    var builder = new ApplicationBuilder();
    builder.Resolvers.AddSingleton(data);
    m_Container = builder.Build();
}
```

For simple synchronous loaders, `return AwaitableUtility.CompletedTask;` is the right pattern — there's nothing to cancel.

## After boot

The boot scene handles initial application setup. After bootstrapping, scene management is your responsibility — load/unload scenes through your own Core features (Addressables, SceneManager, or whatever fits your project). Buttr provides the boot pipeline and the patterns (Loaders, Services, Scopes) but doesn't prescribe how scenes are managed past boot.

A common pattern is to load the first "real" scene from within a post-`ProgramLoader` loader, or from a script attached to the Boot GameObject that runs after `UnityApplicationBoot` finishes.
