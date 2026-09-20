# VR Tutorial Kit

A modular VR tutorial package for basic interactions, built on the XR Interaction Toolkit.

## Structure

The package ships two assemblies. The tutorial depends on the toolkit; never the reverse.

| Assembly | Location | What it is |
|---|---|---|
| `ecda.vrtoolkit` | `Runtime/Toolkit/` | Reusable VR building blocks: sockets, grabs, respawn, hinges and sliders, highlights, guidance arrows, conditions and progress, rig and fade, scene transitions, hands, callouts, UI Toolkit helpers |
| `ecda.vrtutorialkit` | `Runtime/Tutorial/` | The tutorial itself: manager, config and steps, substeps, the step controllers and the step prefabs |
| `ecda.vrtoolkit.Editor` | `Editor/` | Inspector drawers and authoring tools for the toolkit |

A project that only wants the interaction building blocks can reference `ecda.vrtoolkit`
alone and ignore the tutorial half.

## Dependencies

Declared in `package.json`:

- XR Interaction Toolkit 3.6.0
- Input System 1.16.0
- XR Core Utils 2.5.3
- Localization 1.5.9

Also required, and **not** resolvable through `package.json` because it is a sample rather
than a package: the XR Interaction Toolkit **Starter Assets** sample, imported into the
consuming project's `Assets/`. `RigController` needs it for `ControllerInputActionManager`
and `DynamicMoveProvider`.

## Unity Asset Store dependencies

These are vendored into `Runtime/Toolkit/ThirdParty/`. See `Third Party Notices.md` for the
full terms — note that **Guidance Line requires credit** in any published experience.

[SceneAttribute](https://assetstore.unity.com/packages/tools/utilities/scene-attribute-reference-scenes-in-inspector-316227)
- v1.0 is copied with slight modifications into the package, in accordance with the license it offers

[Quick Outline](https://assetstore.unity.com/packages/tools/particles-effects/quick-outline-115488#reviews)
- v1.1 is copied with slight modifications into the package, under the MIT licence of its
  GitHub release
- Its shaders and materials back the two renderer features below; its `Outline.cs` is unused,
  superseded by `SimpleOutline`

[Guidance Line](https://assetstore.unity.com/packages/tools/game-toolkits/guidance-line-303873)
- v1.0 is copied into the package

## Project setup

Two steps, both required before `SimpleOutline` / `HighlightObject` will show anything:

1. Add an **`Outline` layer** under `Project Settings > Tags and Layers`. `SimpleOutline`
   moves objects onto it to mark them for the outline pass, and logs an error if it is absent.
2. Add the `Outline Fill` and `Outline Objects` renderer features to the existing render
   pipeline (`Assets/Settings/Project Configuration/Android Preset`). The two feature assets
   ship at `Runtime/Toolkit/Art/`.

## Samples

Import **Tutorial Assets** from the Package Manager for a working tutorial scene.
