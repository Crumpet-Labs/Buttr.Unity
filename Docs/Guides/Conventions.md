# Conventions

Buttr is a dependency-injection container first, and the container has no opinion about how you name or organise your code — it works with whatever conventions you already use. What follows is a *separate*, optional layer: an architecture I've leaned on across real projects because it pays for itself.

The idea is simple: every type carries a **suffix that says what it does** — `Service`, `View`, `Registry`, `Handler`. That single rule buys two things worth more than they look:

- **It kills analysis paralysis.** You stop staring at a new file wondering where it belongs or what to call it. The front door other features call through? A `Service`. Something that reads the model and renders it? A `View`. The suffix decides, so your attention goes to the code instead of the shape of the folder.
- **It scales to a team.** Anyone can open a feature they've never seen and read its structure at a glance. Onboarding, review, and handoffs stop leaning on tribal knowledge, because the shape is legible on its own.

None of it is required, and you don't adopt it all at once — reach for the handful of suffixes a feature actually needs (`Service`, `Model`, `View`, `Controller`) and pull in the rest when a problem calls for them. The table below is the full vocabulary, not a checklist.

For a quick tour, see [Getting Started](GettingStarted.md).

---

## Project structure

Buttr organises by feature, not by file type. Everything lives under `Assets/_Project/`.

```
Assets/_Project/
├── {ProjectName}.asmdef
├── Main.unity                  # Boot scene
├── Program.cs                  # Application composition entry point
├── ProgramLoader.cs + .asset   # Bridge to Unity's boot lifecycle
├── Core/                       # Game-agnostic packages — travels across projects
├── Features/                   # Game-specific feature packages
├── Shared/                     # Assets and scripts used by both Core and Features
└── Catalog/                    # ScriptableObject data assets, organised by feature
```

**Core** is your reusable library: logging, event systems, save systems, audio management. It's game-agnostic and rarely changes. **Features** are game-specific: inventory, combat, dialogue, crafting. They're built for the game in front of you right now.

Features contain code and feature-specific resources. **All ScriptableObject assets** live in `Catalog/`, mirroring the structure of `Core/` and `Features/`. The script that defines a ScriptableObject lives in the feature; the `.asset` instance lives in `Catalog/`. Loader assets are the one exception — they live in the package's own `Loaders/` folder next to the loader script.

---

## General rules

**Seal your classes.** All classes should be `sealed` unless explicitly designed for inheritance. This communicates intent, prevents unintended inheritance, and allows the compiler to optimise. If a class isn't `sealed`, it should be `abstract`.

**Minimise MonoBehaviours.** If something doesn't need Unity lifecycle hooks or `GetComponent<T>` access, it should be a plain C# class that gets injected. Don't make something a MonoBehaviour just because it's convenient.

**Scope.** Buttr handles feature-level architecture — singleton services, mediators, models, and registries that live once per feature. Per-instance entity management (hundreds of enemies, projectiles, spawned objects each with their own state) is a different domain that lives within the System and Controller layer, using whatever approach fits the project's performance needs. Buttr provides the structure those systems plug into, but doesn't prescribe how individual instances are managed.

---

## Suffix reference

| Layer | Suffix | What it does | Type |
|-------|--------|--------------|------|
| **Unity** | Controller | Coordinates Unity components and systems on a GameObject | MonoBehaviour |
| | Instance | Manages a scoped container's lifecycle on a GameObject (e.g. UI panels) | MonoBehaviour |
| **Data** | Model | Data — no behaviour, no dependencies | Class / Struct |
| | Id | Readonly struct for domain-driven identity | Struct |
| | Definition | ScriptableObject entry point into a feature | ScriptableObject |
| | Configuration | ScriptableObject providing editable settings | ScriptableObject |
| **Logic** | View | Observes and displays Model data — reads only | Class |
| | System | Reads Model state and executes it continuously | Class |
| | Mediator | Listens to events, filters and routes towards Services | Class |
| | Handler | Stateless ScriptableObject — designer-facing logic; pairs with a Definition for data | ScriptableObject |
| | Profile | Self-contained ScriptableObject bundling data and its own interpretation logic | ScriptableObject |
| | Behaviour | Stateful strategy — code-facing, drives a System's update loop | Class |
| **Infrastructure** | Service | Public API of a feature and its write-owner — the only type that mutates Model state; the entry point other features inject | Class |
| | Repository | CRUD operations on local persistent storage | Interface |
| | Registry | Tracks active runtime objects by ID | Class |
| | Loader | ScriptableObject that bootstraps a feature at boot time | ScriptableObject |
| **Structure** | Extensions | Internal stateless functional methods that keep classes lean | Static Class |
| | Contract | Interface defining the public API boundary of a feature | Interface |

---

## Unity layer

These types touch GameObjects and live in Unity's lifecycle. All MonoBehaviours live in the `MonoBehaviours/` folder within a feature — no exceptions. Consumers should never have to hunt for something to drag onto a GameObject.

**Controllers** coordinate Unity components, injected services, and event systems on a GameObject. They manage lifecycle (`OnEnable`, `OnDisable`), own subscriptions, and expose a public API for other systems to interact with. Controllers are always MonoBehaviours. Controllers don't mutate Model state directly — they route actions to the feature's Service or raise events that Mediators handle.

**Instances** are MonoBehaviours that own a scoped container's lifecycle on a GameObject. They build a `ScopeBuilder`, register ScriptableObjects via `ScriptableRegistrar`, register the feature's package, and dispose the container on `OnDestroy`. Used when a feature needs its own isolated scope tied to a specific GameObject — most commonly UI panels backed by UI Toolkit.

---

## Data layer

These types hold or describe data. No behaviour, no dependencies, no awareness of the systems that use them.

**Models** are plain data — state with no logic. A Model doesn't know about Unity, injection, or anything else. Trivially serialisable, testable, and portable.

**IDs** are readonly structs providing domain-driven identity — `EntityId` rather than `int`, `ItemId` rather than `string`. Prevents accidental cross-domain comparisons and makes APIs self-documenting. Implement `IEquatable<T>`. IDs live in an `Identifiers/` folder within the feature.

**Definitions** are ScriptableObjects that serve as extensible entry points into features. Where you might traditionally use an enum (`WeaponType.Sword`), a Definition (`SwordDefinition.asset`) lets designers create new entries without touching code. Describe *what* something is.

**Configurations** are ScriptableObjects providing editable settings for packages and features. Typically registered into the application container via `ScriptableRegistrar`, letting designers tune behaviour without touching code. See [ScriptableObjects](ScriptableObjects.md).

---

## Logic layer

These types make decisions. They contain the rules, strategies, and coordination logic that drive your features.

**Views** observe and display Model data but never write to it. Plain C# classes that receive dependencies via constructor injection. For UI Toolkit features, a View takes a `UIDocument` reference and queries visual elements from it. All mutations flow back through the feature's Service.

```csharp
public sealed class InventoryView {
    private readonly VisualElement m_Root;

    public InventoryView(UIDocument document) {
        m_Root = document.rootVisualElement;
    }
}
```

**Systems** read Model state and execute it continuously. Where a View *renders* the Model visually, a System *acts* on it mechanically — movement, ticking Behaviours, simulation logic. Systems own the update loop and switch/tick the active Behaviour. Typically plain C# classes injected into the MonoBehaviour that hosts them.

**Mediators** are self-contained event listeners. Bind to events in their constructor, apply filtering or transformation, route results to the relevant Service — or exit early if the event isn't applicable. Nothing calls into a Mediator from the outside; they react implicitly. Unbind on disposal. Live in `Components/`, use standard C# events or event buses — not UnityEvents.

The next three suffixes — **Handler**, **Profile**, and **Behaviour** — are all swappable logic, and two questions tell them apart:

- **Does it hold runtime state?** Yes → **Behaviour**: a plain C# strategy your System constructs and ticks. No → it's a designer-facing ScriptableObject; ask the next question.
- **Is its data its own, or in a separate Definition?** Its own → **Profile** (self-contained). A separate Definition → **Handler** (paired with that Definition for its data).

The comparison table at the end of this section is the quick reference; the detail below is the *why*.

**Handlers** are abstract ScriptableObjects providing a **feature-specific public API** for strategic logic. **Stateless** and **designer-facing** — a designer drags a different Handler asset onto a component and behaviour changes without a recompile. Handlers pair with `DIBuilder<TKey>` for runtime resolution by key, and typically pair with a **Definition** that supplies the data. Think of Definitions as the "what" and Handlers as the "how." Parameters on a Handler describe behaviour quirks, not identity — many Definitions can share one Handler.

**Profiles** are abstract ScriptableObjects that **bundle data and behaviour into a single self-contained unit**. Where a Handler is the "how" paired with a Definition's "what", a Profile collapses both into one asset — the parameters baked into the Profile **are** its identity, and only this Profile's logic knows how to interpret them. Use a Profile when data and behaviour are inseparable: a different parameter set would require different interpretation logic, and vice versa. Each concrete Profile subclass defines its own interpretation; designers author each `.asset` instance as a unique self-contained unit, no pairing required.

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

**Behaviours** are lightweight strategy objects providing a **feature-specific Tick method** to drive a System's update loop. **Stateful** and **code-facing** — constructed with a Model reference, switched at runtime through code, ticked by the host System. Only the active Behaviour ticks each frame.

```csharp
public interface IMovementBehaviour {
    void Tick(MovementContext ctx);
}

public sealed class SprintBehaviour : IMovementBehaviour {
    public void Tick(MovementContext ctx) {
        ctx.Velocity = ctx.Direction * ctx.SprintSpeed;
    }
}
```

| | Handler | Profile | Behaviour |
|---|---------|---------|-----------|
| **Data carries identity** | No — data describes behaviour quirks | Yes — data IS the identity | N/A — runtime, not asset |
| **State** | Stateless | Stateless (asset data is fixed) | Stateful — constructed with Model |
| **Audience** | Designer-facing — swappable in editor | Designer-facing — swappable in editor | Code-facing — switched at runtime |
| **Type** | ScriptableObject | ScriptableObject | Plain C# class |
| **API** | Feature-specific public methods | Feature-specific public methods | Feature-specific Tick method |
| **Pairs with** | A Definition for data | Nothing — self-contained | Owned by host System |
| **Resolution** | `DIBuilder<TKey>` by key | Direct asset reference | Owned and ticked by host System |

---

## Infrastructure layer

These types connect features to the outside world — APIs, persistence, runtime state tracking. All infrastructure types are injected and live in `Components/`.

**Services** are the public API of a feature — the entry point other features inject — and its **write-owner**: the only type that mutates Model state. Discrete changes (a command, an event, a user action) flow through a Service method; continuous, per-frame changes belong to a System instead. A Service is the boundary — the front door other features knock on — and because one operation often has to touch several Models at once (spend currency *and* add the item), it's the natural owner of that coordination. Some Services wrap external APIs, asset systems, or remote databases; others wrap Registries and internal logic behind a clean interface. Services pair with Contracts to define their public interface.

```csharp
public sealed class InventoryService : IInventoryService {
    private readonly InventoryModel m_Inventory;
    private readonly CurrencyModel m_Currency;

    public InventoryService(InventoryModel inventory, CurrencyModel currency) {
        m_Inventory = inventory;
        m_Currency = currency;
    }

    public BuyResult Buy(ItemId item, int price) {
        if (m_Currency.Balance < price) return BuyResult.NotEnoughFunds;
        m_Currency.Spend(price);
        m_Inventory.Add(item);
        return BuyResult.Success;
    }
}
```

**Repositories** are interfaces defining CRUD operations on local persistent storage. Own the persistence boundary, expose domain-friendly methods rather than raw storage calls. Contracts live in `Contracts/` alongside Service contracts.

**Registries** are the runtime counterpart to Repositories. Where a Repository persists data to storage, a Registry tracks what's alive and accessible right now — active entities, spawned objects, loaded resources — typically keyed by `EntityId`. Registration returns a disposable handle for automatic deregistration.

**Loaders** are `UnityApplicationLoaderBase` ScriptableObjects that bootstrap a feature at boot time. See [Loaders](Loaders.md).

---

## Structure layer

**Extensions** are a core pattern. Internal extension methods are preferred over private class methods to keep classes focused. Extensions should be functional — take input, return a value, don't require access to private state. If a method needs many values to function, that's a signal to rethink the structure.

```csharp
internal static class InventoryExtensions {
    public static int RemainingSpace(this InventoryModel inventory, ItemId id) {
        var slot = inventory.FindSlot(id);
        return slot == null ? 0 : slot.MaxStack - slot.Count;
    }
}
```

Extensions also serve as the mechanism for Configurable Packages — each package exposes a `builder.UseSomething()` extension that encapsulates its registrations.

**Contracts** are interfaces defining the public API boundary of a feature. The folder name reflects the purpose — an interface is a contract between a feature and its consumers. Live in `Contracts/`.

---

## Feature structure

Every feature follows the same layout. Only the package file and an optional README live at the feature root — everything else is logically grouped.

```
Features/Console/
├── ConsolePackage.cs           # Package entry point — always at root
├── {Namespace}.asmdef
├── README.md                   # Optional
├── Components/                 # Injected types: Services, Models,
│   ├── ConsoleService.cs       #   Repositories, Registries,
│   ├── ConsoleModel.cs         #   Mediators, Systems, Views
│   ├── ConsoleView.cs
│   └── ConsoleMediator.cs
├── Configurations/             # Configuration ScriptableObject scripts
│   └── ConsoleConfiguration.cs
├── Contracts/                  # Interfaces / public API boundaries
│   └── IConsoleService.cs
├── Identifiers/                # Readonly ID structs
│   └── ConsoleLogId.cs
├── MonoBehaviours/             # ALL MonoBehaviours for this feature
│   └── ConsoleController.cs
├── Common/                     # Supporting types that don't fit elsewhere
│   ├── ConsoleLog.cs
│   └── ConsoleCategory.cs
├── Loaders/                    # Boot-time loader scripts and assets
│   ├── ConsoleLoader.cs
│   └── ConsoleLoader.asset
└── Exceptions/                 # Custom exceptions — only if necessary
    └── ConsoleException.cs
```

The `Catalog/` folder mirrors the feature structure for ScriptableObject data assets:

```
Catalog/
├── Console/
│   └── ConsoleConfiguration.asset
├── Combat/
│   ├── CombatConfiguration.asset
│   ├── Definitions/
│   │   ├── MeleeWeaponDefinition.asset
│   │   └── RangedWeaponDefinition.asset
│   └── Handlers/
│       ├── MeleeAttackHandler.asset
│       └── RangedAttackHandler.asset
└── Movement/
    ├── MovementConfiguration.asset
    └── Profiles/
        ├── WalkRunGaitProfile.asset
        └── RunSprintGaitProfile.asset
```

---

## Scaffolding

You don't hand-write any of this structure. `Tools > Buttr > Setup Project` and the right-click `Buttr > Packages` menus generate correctly-shaped feature folders. See [Editor Tooling](EditorTooling.md).