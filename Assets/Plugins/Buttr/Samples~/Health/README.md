# Health — a Buttr sample

The smallest *complete* Buttr feature: a clamped health value, a UI Toolkit bar, and a key to take damage. It shows the whole loop — **boot → inject → Service mutates Model → View reads** — in one place, and it's the code the Buttr guides draw their Unity examples from.

## What it shows

| Suffix | File | Role |
|---|---|---|
| Model | `Components/HealthModel.cs` | Plain data: `Current`, `Max`. |
| Service | `Components/HealthService.cs` (`IHealthService`) | The **write-owner** — `TakeDamage`/`Heal`, clamped to `[0..Max]`. The clamp is *why* writes go through the Service, not the Model. |
| Configuration | `Configurations/HealthConfig.cs` | Designer-tunable `MaxHealth`, passed into `UseHealth` and registered there so the analyzer can see it. |
| View | `Components/HealthView.cs` | Reads the Model, drives the UI Toolkit bar. Never writes. |
| Controller | `MonoBehaviours/HealthController.cs` | An `[Inject]` MonoBehaviour; a key takes damage. |
| Loader | `Loaders/HealthLoader.cs` | Builds the container at boot, **then** enters the injected scene. |
| Package | `HealthPackage.cs` | `UseHealth()` — the one registration seam. |

## Setup (two scenes)

Buttr injects `[Inject]` MonoBehaviours at `Awake`, but the container is built at boot in `Start` — so an injected object must be entered **after** boot. This sample uses two scenes:

**1. Boot scene** — one empty GameObject with `UnityApplicationBoot` (tick *Don't Destroy On Load*).
- Create a `HealthLoader` asset: **Assets → Create → Buttr Samples → Health → Loader**.
- Create a `HealthConfig` asset: **Assets → Create → Buttr Samples → Health → Config**.
- Assign the `HealthConfig` to the loader's **Config** field, and the loader into `UnityApplicationBoot`'s **Application Loaders** list.

**2. Health scene** — name it `Health` (matches the loader's *Health Scene Name*).
- A GameObject with `HealthController`.
- A GameObject with a `UIDocument`: set its **Source Asset** to `UI/HealthBar.uxml` and assign a `PanelSettings`. Drag the `UIDocument` onto the controller's **Document** field.

Add both scenes to **Build Settings**, boot scene first, and press Play from the boot scene.

**Controls:** `Space` takes damage, `H` heals.

## Notes

- Uses the legacy `Input` manager for brevity. If your project is set to the *new* Input System only, swap the two `Input.GetKeyDown` calls in `HealthController`.
- The bar's fill width is driven in code, so the `.uss` is purely cosmetic — the sample works without it.
