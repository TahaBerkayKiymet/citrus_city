# Citrus City — Unity 6 Prototype
## Playable Demo: Magnetic Tap Puzzle

---

## Quick Start

### 1. Requirements
- Unity Hub (latest)
- Unity Editor **6000.0.x LTS** (Unity 6 LTS)
- Xcode 15+ for iOS builds
- Android SDK API 34 for Android builds

### 2. Open the Project
1. Open Unity Hub → Add → Add project from disk
2. Navigate to the `CitrusCity/` folder → Open
3. When prompted, choose **Unity 6 LTS**
4. Wait for package import (~5 minutes first time for URP)

---

## Scene Setup (One-Time, Required)

After the project opens and compiles:

### Create MainMenu Scene
1. File → New Scene → Empty → Save as `Assets/Scenes/MainMenu.unity`
2. Top menu: **CitrusCity → Setup → Create MainMenu Objects**
3. Save scene

### Create GameScene
1. File → New Scene → Empty → Save as `Assets/Scenes/GameScene.unity`
2. Top menu: **CitrusCity → Setup → Create GameScene Objects**
3. Save scene

*(Note: `Level_01`, `Level_02`, `Level_03` and all `FruitData` assets have already been pre-generated in `Assets/Resources/Levels/` and `Assets/Resources/FruitData/`, so `LevelLoader` links them automatically!)*

### Assets & Models Included
- **High-Res 3D Stylized Sprites**: Transparent PNGs in `Assets/Sprites/Fruits/` (Orange, Lemon, Lime, Cherry, Grape) and `Assets/Sprites/Boosters/` (Rocket, Bomb, Vortex, Arrows).
- **3D Mesh Models (.obj + .mtl)**: Ready in `Assets/Models/` (Orange, Lemon, Lime, Cherry, Grape, Bomb, Rocket).
- **Board & UI**: Glass board cell tiles in `Assets/Sprites/Board/Board_Tile.png` and UI icons in `Assets/Sprites/UI/`.
- **Preconfigured ScriptableObjects**: Ready in `Assets/Resources/FruitData/` and `Assets/Resources/Levels/`.

### Add to Build Settings
File → Build Settings → Add Open Scenes for both (MainMenu=0, GameScene=1)

---

## Running in the Editor

1. Open GameScene
2. Press Play
3. Use mouse (mouse = finger)

| Action | Mouse |
|--------|-------|
| Select group | Click |
| Preview booster | Hold + move |
| Confirm | Release inside group |
| Cancel | Drag outside, release |
| Tap booster | Click booster cell |

---

## Tests (No Play Mode)

CitrusCity → Board Debug Window → Run Logic Tests
All 7 tests should show PASS.

---

## Gameplay: Magnetic Tap

### Boosters
| Size | Booster | Effect |
|------|---------|--------|
| 5-6  | Rocket  | Row or column |
| 7-8  | Bomb    | 3×3 area |
| 9+   | Vortex  | All of one color |

### Direction Rule
Touch the left/right edge of the group → Horizontal rocket
Touch the top/bottom edge → Vertical rocket

### Combos
- Rocket + Rocket → row AND column
- Rocket + Bomb → 5×5 blast
- Bomb + Bomb → 5×5 blast
- Bomb + Vortex → color becomes bombs
- Vortex + Vortex → clear entire board

---

## Building

### iOS
1. Build Settings → iOS → Switch Platform
2. Build → opens Xcode
3. Set Development Team → Run

### Android
1. Build Settings → Android → Switch Platform
2. Player Settings → IL2CPP, ARM64, minAPI=24
3. Build → install APK

Bundle ID: com.citruscity.prototype

---

## Architecture

```
Scripts/
├── Core/       GameManager, EventBus, Constants, ProceduralSprites
├── Board/      BoardState*, GravitySystem*, BoardController, BoardConfig
├── Fruit/      FruitType, FruitData, FruitView, FruitViewPool
├── Selection/  GroupFinder*, SelectionState*, SelectionController
├── Boosters/   BoosterType, BoosterFactory*, BoosterEffect*, BoosterCombo*, BoosterPreviewView
├── Objectives/ ObjectiveTracker*
├── Levels/     LevelData, LevelLoader
├── UI/         GameHUD, ObjectiveView, VictoryPanel, FailurePanel, MainMenuUI
├── Effects/    FruitExplosionFX, BoosterActivateFX
└── Editor/     BoardEditorWindow, SceneBootstrap
```
*Pure C# — no Unity dependencies, fully unit-testable

---

## Acceptance Criteria

- [ ] AC-01: 8×9 grid with 5 fruit colors
- [ ] AC-02: Click highlights group with bounce
- [ ] AC-03: Booster preview + arrow appears while held
- [ ] AC-04: Drag outside group = cancel (no move consumed)
- [ ] AC-05: Release inside = remove fruits, spawn booster
- [ ] AC-06: Fruits fall with smooth animation
- [ ] AC-07: Booster tap = flash + particle effect
- [ ] AC-08: Rocket clears row or column
- [ ] AC-09: Bomb clears 3×3
- [ ] AC-10: Vortex clears all of one color
- [ ] AC-11: Vortex+Vortex clears entire board
- [ ] AC-12: Move counter decrements per move
- [ ] AC-13: Objectives tracked correctly
- [ ] AC-14: Victory panel on completion
- [ ] AC-15: Failure panel when moves run out
- [ ] AC-16: Restart reloads level

---

## Known Limitations (Prototype Scope)
- Dynamic Re-Tap: not implemented
- Aurora Festival bar: not implemented  
- Audio: not implemented
- Economy/monetization: intentionally excluded
- Custom art: procedural sprites used
