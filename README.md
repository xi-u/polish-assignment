# 💅 Polish Assignment

In this assignment you'll take a functional but rough game and make it *feel* good. The mechanics are all there: enemies spawn, flock, shoot, blink and explode. What's missing is the layer of polish that turns "it works" into "it feels great". Your job is to add that layer.

## The Game

The game is a bullet hell (of sorts), with one important twist: **you cannot attack**. You control a small green ship, and the only thing you can do is dodge. Enemies and their bullets, mines, homing missiles and blast waves come at you from all sides, and all you can do is weave through them and survive for as long as possible. Touch anything and you're dead. After a short pause the arena resets and you try again.

Enemies are introduced one at a time, easiest to hardest. The full roster is only active after roughly five minutes, so the pressure keeps ramping up the longer you stay alive.

### Controls

| Input | Action |
|---|---|
| `WASD` / Arrow keys | Move (the only thing you can do!) |

### The Enemy Roster

All visuals are procedurally generated primitive shapes. Each enemy is recognizable by its color and behaviour:

| Enemy | Color | Behaviour |
|---|---|---|
| **TailGator** | Red | Chases the player and fires bullets |
| **SwarmSparrow** | Orange | Hunts in flocks, swarming toward the player |
| **WebWeaver** | Pink | Roams the arena, dropping floating mines |
| **SeekerHawk** | Blue | Pursues the player and launches homing missiles |
| **BlinkWolf** | Cyan | Hunts in packs, periodically shrinks away and *blinks* right next to you, then charges |
| **MirrageManta** | Purple | Roams and periodically splits off a clone. Which one is real? |
| **HiveRaptor** | Yellow | Flocks toward the player and kamikaze-dives when close |
| **BlastBadger** | Red-violet | Closes in and periodically releases a blast wave |

(There is also a **ShieldRhino**, an enemy with a frontal shield, in the codebase. It is not currently part of the spawn rotation.)

## The Assignment: Add Polish

The game is intentionally unpolished. Quite a few things don't work, or don't *feel*, the way they should. The most obvious one:

- **The camera doesn't smooth its movement at all.** It's supposed to lerp toward the player (and look ahead in the direction of movement), but in practice it doesn't, giving the whole game a jittery, rigid feel. Start in `Assets/Scripts/Behaviours/CameraBehaviour.cs`.

Beyond fixing what's broken, think about everything that makes a game feel alive: smooth camera work, screen shake, hit feedback, particles, sound, telegraphing enemy spawns and attacks, death and restart sequences, transitions, UI feedback. Play the game, notice everything that feels abrupt, stiff or unclear, and polish it.

### One Rule

Before you ask: no, you cannot change the game or its mechanics in any meaningful way. This is not the moment to build the game *you* want to make. You work with what's provided: the player stays defenseless, the enemies keep their behaviour, and the game stays what it is. Polish is about making an existing game feel better, not about making a different game. Learning to work within someone else's design is part of the job.

## Getting Started

1. Install Unity `6000.4.6f1` (or open the project and let Unity Hub fetch the matching editor version).
2. Clone this repository and open the project folder in Unity.
3. Open `Assets/Scenes/SampleScene.unity` and press Play.

---

## Project Structure

```
Assets/
├── Scenes/
│   └── SampleScene.unity          <-- The one and only game scene
├── Scripts/
│   ├── Behaviours/                <-- MonoBehaviours that give entities their behaviour
│   │   ├── Collision/             <-- Trigger-based collision handling + handler registry
│   │   ├── Flocking/              <-- Boids: Cohesion, Separation, Alignment, FollowTransform
│   │   ├── Projectiles/           <-- Mine & homing missile behaviours
│   │   ├── CameraBehaviour.cs     <-- Follow camera (jittery! see The Assignment)
│   │   ├── PlayerBehaviour.cs     <-- Input, movement & rotation
│   │   ├── ShootBehaviour.cs      <-- Generic "stop, shoot, resume" attack component
│   │   └── ...                    <-- Per-enemy behaviours (BlinkWolf, MirrageManta, ...)
│   ├── GameObjects/               <-- Entity classes: Player, enemies, stars
│   │   └── Projectiles/           <-- Bullet, Mine, HomingMissile
│   ├── ObjectPool/                <-- Generic object pooling (PoolManager, Pool<T>)
│   ├── PrimitiveShape/            <-- Procedural shapes (Triangle, Circle, ship variants)
│   └── Utility/                   <-- ServiceLocator, UnitStats, GenerateWorldBehaviour
```

### How It Fits Together

- **Entities are plain C# classes, not MonoBehaviours.** `Entity` (in `GameObjects/`) creates and owns its `GameObject` in code. Subclasses like `Player`, `TailGator` and `WebWeaver` build themselves out of procedural `PrimitiveShape`s. There are no prefabs or sprites.
- **Behaviour is composed, not inherited.** Entities attach reusable `MonoBehaviour` components from `Behaviours/` (movement, flocking, shooting, collision) to get their personality. A `HiveRaptor`, for example, is a triangular ship + flocking + kamikaze behaviour.
- **Everything is pooled.** `PoolManager` builds a `Pool<T>` per enemy and projectile type (via reflection on the `PoolableType` enum) and recycles entities instead of destroying them. Request entities with `PoolManager.Instance.GetEntity(...)`.
- **`ServiceLocator`** provides global access to shared services. The most important one is the `Player`, which nearly every enemy behaviour needs to find its target.
- **`GenerateWorldBehaviour`** builds the arena (walls + play area), spawns the player and runs the escalating enemy spawn coroutines. It also listens for `Player.OnPlayerDied` to reset the game.

---

## Hand-in

Submit via the next available assignment on the DLO:

- A .zip of your project folder, **or** a link to your repository
- A short list of the polish you added (what, where, and why it improves the feel)
