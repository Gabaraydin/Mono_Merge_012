# MonoMerge

A monochrome hyper-casual merge-puzzle mobile game built with Unity (2D template, C#).

Players drag numbered tiles onto a 5x5 grid; three or more adjacent tiles of the same
value merge into the next tier. The game ends when the grid is full. Monetization is
ad-only (Google AdMob: banner, interstitial, rewarded) — no backend, no in-app purchases.

## Tech stack

- Unity 2022.3 LTS, 2D project template
- C# (no third-party gameplay frameworks; Google Mobile Ads Unity SDK for ads)
- Local persistence only: `PlayerPrefs` for high score and mute preference — no server,
  no database
- Target platforms: Android + iOS, target build size < 40 MB

## Project structure

```
Assets/
  Scripts/
    Core/    game state machine, save/load, game-over check
    Grid/    5x5 board data model
    Tiles/   tile data + spawn tray
    Input/   drag & drop, snap-to-grid
    Merge/   adjacency detection + merge resolution
    Score/   scoring
    Undo/    single-slot move snapshot (rewarded-ad "undo" feature)
    UI/      HUD, main menu, game-over panel, settings
    Audio/   sound effects
    VFX/     merge particle burst, camera shake
    Ads/     AdMob wrappers (banner/interstitial/rewarded)
  Sprites/   placeholder monochrome tile/cell/particle art
  Icons/     placeholder app icon
  Plugins/Android/  AndroidManifest.xml (AdMob App ID + permissions)
```

Every non-trivial class documents the specific game-design rule it implements, so the
source doubles as a spec reference.

## Status

Core gameplay, scoring, UI, audio/VFX and the ad-integration code are implemented.
Still needed before a store release (all manual, Unity Editor work):

- Build the actual scenes/prefabs and wire component references in the Inspector
  (no `.unity` or `.prefab` files exist yet — code only so far)
- Install the Google Mobile Ads Unity SDK (see `Assets/Scripts/Ads/AdsManager.cs` for
  exact steps) and add the `GOOGLE_MOBILE_ADS` scripting define symbol — until then, all
  ad code compiles but no-ops
- Replace the AdMob test IDs in `Assets/Scripts/Ads/AdUnitIds.cs` and the test App ID in
  `Assets/Plugins/Android/AndroidManifest.xml` with real ones from the AdMob console
- Set Player Settings (bundle ID, company/product name, store icons) and verify the
  final build size

## Opening the project

Open the repository root in Unity Hub (`Add project from disk`) with Unity 2022.3 LTS.
`Packages/manifest.json` and `ProjectSettings/` are checked in; Unity will generate any
remaining default settings files on first open.
