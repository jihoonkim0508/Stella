# Repository Guidelines

## Project Structure & Module Organization

Stella is a Unity 6 first-person roguelike dungeon crawler. Shared game content lives under `Assets/!_Stella/`: `Scripts/` for C# logic, `Prefabs/` for reusable objects, and `Art/`, `Audio/`, `Data/`, and `Settings/` for assets and configuration. Runtime scenes are in `Assets/!_Stella/Scenes/`, including `Start.unity`, `Lobby.unity`, and `Scenes/Dungeons/`. Developer sandboxes live in `Assets/!_Stella/Scenes/<Name>/`; promote reusable work into shared folders.

Core script modules include `Player/`, `Enemies/`, `Boss/`, `Managers/`, `Map/`, `UI/`, and `Interfaces/`. Scripts currently use the global namespace, so avoid introducing namespaces unless the team agrees on a broader migration.

## Build, Test, and Development Commands

There is no repository build CLI. Open the project with Unity `6000.3.11f1`.

- Unity Editor Play Mode: run local gameplay and scene checks.
- `Window > General > Test Runner`: run Editor or Play Mode tests.
- Unity Console: check compile errors after saving C# files.

Do not add `npm`, `make`, or custom terminal build workflows unless they are intentionally adopted by the project.

## Coding Style & Naming Conventions

Use C# conventions consistent with existing scripts: `PascalCase` for classes, public methods, properties, and intentionally visible serialized fields; `camelCase` for locals and private fields where already used. Keep MonoBehaviour files named after their primary class, for example `PlayerController.cs`. Prefer serialized references over hard-coded scene lookups, but preserve current tags such as `"Player"` and `"Floor"`.

Movement and timed behavior generally use Rigidbody APIs and coroutines. Keep comments brief and focused on non-obvious behavior.

## Testing Guidelines

Use Unity Test Framework (`com.unity.test-framework`) via the Test Runner. Place new tests in Unity-recognized Editor or Play Mode test folders. Name tests after behavior, for example `EnemyProjectile_DoesNotDamageOwner`. At minimum, verify changed scenes in Play Mode and confirm the Console has no compile errors.

## Commit & Pull Request Guidelines

Recent history uses short Korean commit summaries focused on the changed feature or system. Keep commits concise and behavior-focused, for example `UI animation update` or `enemy behavior draft` if writing in English. For pull requests, include a short description, affected scenes or systems, manual test notes, and screenshots or clips for UI, animation, or visual changes. Link issues when applicable and call out Unity version or package changes.

## Agent-Specific Instructions

Read `CLAUDE.md` before making architectural changes. Avoid editing generated Unity folders such as `Library/`, `Temp/`, `Logs/`, and `UserSettings/`. Preserve `.meta` files when moving or adding Unity assets.
