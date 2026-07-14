# ScriptableObjects

ScriptableObjects are first-class in Buttr's architecture — they're how designer-facing data and strategy enter the DI graph. This guide covers the four idiomatic ScriptableObject roles (Configuration, Definition, Handler, Profile) and the two helpers that connect ScriptableObjects to the container: `ScriptableRegistrar` and `ScriptableInjector`.

For the suffix-level rules see [Conventions](Conventions.md).

## The ScriptableObject roles

| Role | Purpose | Stateful? |
|---|---|---|
| **Configuration** | Editable settings tuned by designers | No |
| **Definition** | Extensible "what is this" entries (replaces enums) | No |
| **Handler** | Stateless, designer-swappable strategy logic | No |
| **Profile** | Self-contained bundle of data and its own interpretation logic | No |

All four are **stateless** — they describe, not execute. Stateful strategy belongs in a `Behaviour` (plain C# class).

## Two helpers, two directions

| Helper | Direction | What it does |
|---|---|---|
| `ScriptableRegistrar` | ScriptableObjects **into** the container | Registers `.asset` references as singleton sources, keyed by concrete type |
| `ScriptableInjector` | Container **into** ScriptableObjects | Resolves `[Inject]` fields on ScriptableObject assets at boot |

> **Renamed in 3.0.0:** `ScriptableRegistrar` is the class previously called `Buttr.Unity.ScriptableInjector`. The `ScriptableInjector` name now belongs to the injection driver. See the [CHANGELOG](https://github.com/Crumpet-Labs/Buttr.Unity/blob/main/Assets/Plugins/Buttr/CHANGELOG.md) migration note.

## ScriptableRegistrar — ScriptableObjects as sources

`ScriptableRegistrar` is a `[Serializable]` helper class (not a MonoBehaviour) that bulk-registers ScriptableObject assets into a Buttr container. Expose it on a Loader as a `[SerializeField]`, drag your `.asset` references into its list in the Inspector, and call `Inject(builder)` from `LoadAsync`. Internally it builds an Expression-tree-compiled `AddSingleton<TConcrete>().WithFactory(() => instance)` registration for each entry, keyed by each asset's concrete type.

Typical flow inside a Loader:

```csharp
public sealed class CombatLoader : UnityApplicationLoaderBase {
    [SerializeField] private ScriptableRegistrar m_Registrar;

    private ApplicationContainer m_Container;

    public override Awaitable LoadAsync(CancellationToken ct) {
        var builder = new ApplicationBuilder();

        m_Registrar.Inject(builder);   // Registers each ScriptableObject under its concrete type
        builder.Resolvers.AddSingleton<ICombatService, CombatService>();

        m_Container = builder.Build();
        return AwaitableUtility.CompletedTask;
    }

    public override Awaitable UnloadAsync() {
        m_Container?.Dispose();
        return AwaitableUtility.CompletedTask;
    }
}
```

`ScriptableRegistrar` overloads `Inject(ApplicationBuilder)` and `Inject(IDIBuilder)` — same pattern works inside a `ScopeBuilder` or `DIBuilder` Loader too.

If you need fine-grained control — registering individual assets, applying configuration, or providing a different concrete type — skip `ScriptableRegistrar` and use `WithFactory` directly:

```csharp
[SerializeField] private CombatConfiguration m_Config;

public override Awaitable LoadAsync(CancellationToken ct) {
    var builder = new ApplicationBuilder();

    builder.Resolvers.AddSingleton<CombatConfiguration>().WithFactory(() => m_Config);

    m_Container = builder.Build();
    return AwaitableUtility.CompletedTask;
}
```

The `WithFactory` lambda short-circuits Buttr's constructor scan — perfect for ScriptableObjects since they're created by Unity, not by Buttr.

`[SerializeField]` slots on the Loader expose drag-targets in the Inspector. The `.asset` files themselves live in `Catalog/` (see [Conventions](Conventions.md)).

## ScriptableInjector — injecting into ScriptableObjects

Since 3.0.0, ScriptableObjects can be `[Inject]` **targets**, exactly like MonoBehaviours: mark the class `partial`, add `[Inject]` fields, and the source generator emits the `IInjectable` half.

```csharp
public sealed partial class GameplaySettings : ScriptableObject {
    [Inject] private IClockService i_Clock;
}
```

`ScriptableInjector` is a `[Serializable]` helper that drives injection over a fixed list of ScriptableObject assets at boot. Expose it on a Loader, drag the injectable `.asset` references into its list, and call `InjectAll()` **after** the application container is built:

```csharp
public sealed class CoreLoader : UnityApplicationLoaderBase {
    [Header("Scriptable Objects")]
    [SerializeField] private ScriptableRegistrar m_Registrar;
    [SerializeField] private ScriptableInjector m_Injector;

    private ApplicationContainer m_Container;

    public override Awaitable LoadAsync(CancellationToken ct) {
        var builder = new ApplicationBuilder();

        m_Registrar.Inject(builder);
        builder.UseCore();

        m_Container = builder.Build();
        m_Injector.InjectAll();       // Resolves [Inject] fields on the listed assets
        return AwaitableUtility.CompletedTask;
    }

    public override Awaitable UnloadAsync() {
        m_Container?.Dispose();
        return AwaitableUtility.CompletedTask;
    }
}
```

This is the shape the Core Loader scaffolding template generates.

`InjectAll()` resets each asset's `IInjectable.Injected` flag before injecting. A ScriptableObject asset persists across editor play sessions when domain reload is disabled (and across boots on CoreCLR players), so without the reset a second boot would silently skip re-injection and leave stale references.

For a single asset on demand, use the adapter directly:

```csharp
InjectionProcessorUnityExtensions.Inject(scriptableObject);
```

It throws an `InjectionException` if the asset is `null` or has no generated injector (no `[Inject]` fields / missing `partial`).

### Application lifetime only

ScriptableObjects resolve from the **application container only** — scoped injection is a compile-time error (**BUTTR020**, with a code fix that removes the scope argument):

```csharp
public sealed partial class GameplaySettings : ScriptableObject {
    [Inject("combat")] private ICombatService i_Combat;   // BUTTR020: error
}
```

The reason is lifetime: a ScriptableObject asset outlives every scene scope. A scoped dependency injected into a persistent asset would dangle the moment its scope is disposed. Keep scoped dependencies in MonoBehaviours owned by the scope's GameObject; give ScriptableObjects application-lifetime dependencies only.

## Configurations

A Configuration is a ScriptableObject with editable settings that tune a package's behaviour.

```csharp
[CreateAssetMenu(fileName = "AudioConfiguration",
                 menuName = "Game/Audio/Configuration")]
public sealed class AudioConfiguration : ScriptableObject {
    [SerializeField] private float m_MasterVolume = 1.0f;
    [SerializeField] private AudioMixer m_Mixer;

    public float MasterVolume => m_MasterVolume;
    public AudioMixer Mixer => m_Mixer;
}
```

Consumers inject the concrete Configuration type directly:

```csharp
public sealed class AudioService : IAudioService {
    private readonly AudioConfiguration m_Config;

    public AudioService(AudioConfiguration config) {
        m_Config = config;
        m_Config.Mixer.SetFloat("Master", m_Config.MasterVolume);
    }
}
```

The Configuration **script** lives in the feature's `Configurations/` folder. The Configuration **asset** lives in `Catalog/{Feature}/`.

## Definitions

A Definition is an extensible enum-replacement. Instead of a `WeaponType.Sword` enum entry that requires code changes to extend, designers create new `SwordDefinition.asset` files.

```csharp
[CreateAssetMenu(fileName = "WeaponDefinition",
                 menuName = "Game/Combat/Weapon Definition")]
public sealed class WeaponDefinition : ScriptableObject {
    [SerializeField] private string m_DisplayName;
    [SerializeField] private int m_BaseDamage;
    [SerializeField] private float m_AttackSpeed;

    public string DisplayName => m_DisplayName;
    public int BaseDamage => m_BaseDamage;
    public float AttackSpeed => m_AttackSpeed;
}
```

Definitions are usually referenced directly from gameplay code (via serialised fields on MonoBehaviours, or looked up by ID through a Registry). They don't always need to be in the DI container — but if they're part of a fixed set loaded at boot, register them via `ScriptableRegistrar`.

## Handlers

Handlers are abstract ScriptableObjects providing a **feature-specific public API** for strategic logic. The pattern pairs well with `DIBuilder<TKey>` for designer-assignable strategies.

```csharp
public abstract class AttackHandler : ScriptableObject {
    public abstract DamageResult Execute(CombatContext ctx);
}

[CreateAssetMenu(menuName = "Game/Combat/Handlers/Melee Attack")]
public sealed class MeleeAttackHandler : AttackHandler {
    [SerializeField] private int m_Damage = 10;

    public override DamageResult Execute(CombatContext ctx) {
        return new DamageResult(m_Damage, DamageType.Physical);
    }
}
```

Register each Handler under its **concrete** type and resolve them polymorphically via `All<T>()`. Buttr's `All<T>` walks every registration whose concrete type is assignable to `T`, so the abstract `AttackHandler` doesn't need its own registration:

```csharp
[SerializeField] private MeleeAttackHandler m_Melee;
[SerializeField] private RangedAttackHandler m_Ranged;

builder.Resolvers.AddSingleton<MeleeAttackHandler>().WithFactory(() => m_Melee);
builder.Resolvers.AddSingleton<RangedAttackHandler>().WithFactory(() => m_Ranged);

// Later:
foreach (var handler in Application<AttackHandler>.All()) {
    // Fan-out to every registered handler
}
```

(Or drop both into a single `ScriptableRegistrar` and call `m_Registrar.Inject(builder)` for the same result.)

Or key them by ID with `DIBuilder<TKey>` for designer-assigned strategy selection:

```csharp
var builder = new DIBuilder<AbilityId>();
builder.AddSingleton<MeleeAttackHandler>(MeleeId).WithFactory(() => m_Melee);
builder.AddSingleton<RangedAttackHandler>(RangedId).WithFactory(() => m_Ranged);

var container = builder.Build();
var chosen = container.Get<AttackHandler>(currentAbilityId);
chosen.Execute(ctx);
```

## Profiles

Profiles are abstract ScriptableObjects that **bundle data and behaviour into a single self-contained unit**. Where a Handler is the "how" paired with a Definition's "what", a Profile collapses both into one asset — the parameters baked into the Profile *are* its identity, and only this Profile's logic knows how to interpret them.

```csharp
public abstract class GaitProfile : ScriptableObject {
    public abstract GaitResolution Resolve(float stickMagnitude);
}

public sealed class BandedGaitProfile : GaitProfile {
    [SerializeField] private float m_LowSpeed = 1.4f;
    [SerializeField] private float m_HighSpeed = 2.5f;
    [SerializeField, Range(0f, 1f)] private float m_SplitPoint = 0.5f;

    public override GaitResolution Resolve(float mag) { /* ... */ }
}
```

Profiles are typically consumed by **direct asset reference** — a serialized field on the component or configuration that uses them — rather than through the container, since each asset is a unique self-contained unit with no pairing to resolve. For the "Handler or Profile?" decision test, see [Conventions](Conventions.md).

## Where assets live

The rule is simple: **scripts live with the feature, `.asset` instances live in `Catalog/`.**

```
Features/Combat/
├── Configurations/
│   └── CombatConfiguration.cs       # script
├── Handlers/
│   └── MeleeAttackHandler.cs        # script
└── Loaders/
    ├── CombatLoader.cs              # script
    └── CombatLoader.asset           # exception: loader assets live here

Catalog/Combat/
├── CombatConfiguration.asset
├── Definitions/
│   └── SwordDefinition.asset
├── Handlers/
│   └── MeleeAttackHandler.asset
└── Profiles/
    └── AggressiveCombatProfile.asset
```

The right-click scaffolding (`Buttr > Packages > Add to Package`) handles this layout for you. See [Editor Tooling](EditorTooling.md).
