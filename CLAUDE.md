# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Stella** is a 3D first-person roguelike dungeon crawler built in **Unity 6 (6000.3.11f1)**. It is a team project with multiple developers (Jihoon, Minji, JungHo, SeoJun, Nuri) each working in their own scene subdirectories under `Assets/!_Stella/Scenes/<name>/`.

## Unity-Specific Workflow

There is no build CLI — all builds, scene testing, and Play Mode happen inside the Unity Editor. To work on this project:

- Open the project in Unity 6 (6000.3.11f1)
- Enter Play Mode from the Editor to test
- Use **Window > General > Test Runner** for any Editor/Play Mode tests
- Scripts are compiled automatically on save; check the Console for compile errors

No `npm`, `make`, or terminal build commands apply here.

## Architecture

All game code lives under `Assets/!_Stella/Scripts/`. There are no C# namespaces — all scripts are in the global namespace.

### Core Systems

**Player** (`Scripts/Player/`)
- `PlayerController.cs` — Rigidbody-based first-person movement (WASD + mouse look + jump). Also owns the player's health and `TakeDamage`/`Die` methods.
- `PlayerCombat.cs` — Attack input (LMB = light attack, 0.5s cooldown; RMB = heavy attack, 1.2s cooldown). Animation integration is stubbed out (TODO).
- `SkillInput.cs` — Dash skill (LeftShift, 5s cooldown). Requires `PlayerController.IsMoving == true`. Drives a cooldown fill-bar UI via serialized references.

**Interfaces** (`Scripts/Interfaces/`)
- `IDamageable.cs` — `IsDead` property + `TakeDamage(int)` method. All damageable entities (player, enemies, boss) implement this.

**Enemies** (`Scripts/Enemies/`)
- `EnemyBase.cs` — Abstract base implementing `IDamageable`. Provides health/shield system, Rigidbody movement helpers (`MoveTowardTarget`, `MoveAwayFromTarget`), target-finding by `"Player"` tag, and `LookAtTarget`. Abstract methods: `OnDied()`, `ExecutePattern()`.
- `EnemyAI.cs` — Melee chaser. Detection range 12, attack range 1.6, 30 HP, 10 damage per hit, 1s cooldown. Checks `PlayerHealth.IsDead` before attacking.
- `RangedEnemy.cs` — Ranged attacker inheriting `EnemyBase`. Maintains keep-distance (6 units), retreats if too close, fires `EnemyProjectile` prefabs. Falls back to direct damage if no prefab assigned.
- `TankerEnemy.cs` — Tank inheriting `EnemyBase`. One-time shield burst at ≤8 units: applies 25 HP shield + visual to nearby enemies via `Physics.OverlapSphere`.
- `EnemyProjectile.cs` — Rigidbody projectile. Initialized via `Initialize(direction, speed, damage, owner)`. Hits `IDamageable` targets, excludes owner/owner's children, auto-destroys after 5s.
- `EnemyEliteVisual.cs` — Scale multiplier (1.5×) and aura prefab attachment for elite enemies. `SetElite(bool)` at runtime; editor preview via `[ContextMenu]`.

**Scene Management** (`Scripts/Managers/SceneController.cs`)
- Singleton (`DontDestroyOnLoad`). Async scene loading with a scene-history stack.
- Key methods: `LoadScene(name)`, `ReloadCurrentScene()`, `LoadPreviousScene()`, `QuitGame()`.
- `StartInput.cs` triggers `LoadScene` on any key press (used on the Start scene).

**Map / Dungeon Generation** (`Scripts/Map/RandomSpawner.cs`)
- Object-pooled room spawner. Flow: Lobby → 4 random normal rooms → 1 random event room → Boss.
- Prevents consecutive duplicate room types. Runtime keys: **R** = reset, **N** = spawn next room.

**Boss** (`Scripts/Boss/`)
- `IBoss.cs` — Interface: `BossName`, `MaxHealth`, `CurrentHealth`, `CurrentPhase`, `IsDead`, `TakeDamage()`, `ChangePhase()`.
- `BossBase.cs` — Abstract base implementing `IBoss`. Phase transitions at configurable HP thresholds (default 50%). Abstract: `OnPhaseChanged()`, `Die()`, `ExecutePattern()`.
- `Boss.cs` — Concrete boss inheriting `EnemyBase` (not `BossBase`). Three attack patterns: **Rush** (14-speed charge, 20 damage), **Ranged Burst** (5-shot spread), **Radial Shot** (12 projectiles in circle). Pattern cooldown 6s with 0.6s telegraph. Includes `OnValidate()` range clamping and Gizmo debug visualization.

**UI** (`Scripts/UI/`)
- `UiManger.cs` — Sequential fade-out of child then parent UI elements plus button-rise animation.
- `UISelectManager.cs` — Mode-selection panel: slides current UI down, stagger-fades in theme items, then hands off to `ThemeSwitch`.
- `ThemeSwitch.cs` — Swaps non-clickable elements for clickable ones with per-item fade and delay. Dynamically adds `CanvasGroup` if missing.
- `ButtonMoveDown.cs` — Hover handler (`IPointerEnterHandler/Exit`) with sine-wave oscillating selection indicator.
- All UI transitions use `CanvasGroup.alpha` via coroutines; no third-party tween library.

### Scene Layout

| Scene | Purpose |
|-------|---------|
| `Start.unity` | Entry point; any-key to proceed |
| `Lobby.unity` | Hub / mode selection |
| `Dungeons/Battle.unity` | Combat encounters |
| `Dungeons/Boss.unity` | Boss fight |
| `Scenes/<dev>/` | Per-developer sandbox scenes |

### Key Patterns

- **Singleton**: `SceneController` — access via `SceneController.Instance`.
- **Abstract base + subclass**: `EnemyBase` → `EnemyAI / RangedEnemy / TankerEnemy`; `BossBase` implements `IBoss`.
- **Coroutines** for all timed async work (cooldowns, UI animations, scene loading progress).
- **Rigidbody velocity** for movement (not `Transform.Translate`).
- Tag-based lookups: `"Player"` and `"Floor"` are hard-coded strings used in `FindWithTag`.

### Packages in Use

- Universal Render Pipeline (URP) 17.3.0
- Input System 1.19.0
- TextMesh Pro 2.0.0
- NavMesh AI 2.0.11
- Timeline 1.8.11
- Visual Scripting 1.9.10

## Developer Scene Convention

Each team member has a personal subdirectory under `Assets/!_Stella/Scenes/<name>/` for sandbox scenes. Scripts, prefabs, and materials from those scenes are promoted into the shared `Assets/!_Stella/Scripts|Prefabs|Art/` directories once ready. The `Scenes/<dev>/Scenes/` subfolder retains the developer's test `.unity` files.
