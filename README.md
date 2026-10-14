# VR Tutorial Kit

A modular VR tutorial package for basic interactions, built on the XR Interaction Toolkit.

## Structure

The package ships two assemblies. The tutorial depends on the toolkit; never the reverse.

| Assembly | Location | What it is |
|---|---|---|
| `ecda.vrtoolkit` | `Runtime/Toolkit/` | Reusable VR building blocks: sockets, grabs, respawn, hinges, sliders and transform tweens, highlights, guidance arrows, conditions and progress, rig and fade, haptics, scene transitions, hands, callouts, UI Toolkit helpers |
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


## Session telemetry

`SessionLogger` writes one JSON Lines file per session to `Application.persistentDataPath/sessions`. Add
`SessionUploader` to the same GameObject to also POST finished sessions to a telemetry server.
Uploaded files move to `sessions/uploaded/`. Files that fail to upload stay where they are and
are retried on the next launch or session end.

### API key

The uploader sends `X-Api-Key`, which must match the server's `INGEST_KEY`. Keep it out of
version control with a key file in the consuming project:

1. Copy `Assets/Resources/SessionUploaderKey.example.txt` to `Assets/Resources/SessionUploaderKey.txt`
   (create both if your project has neither; any `Resources` folder works).
2. Replace the contents with the key. The whole file is the key: no quotes, no comments.
   Surrounding whitespace and newlines are trimmed.
3. Add the key file to the project's `.gitignore`:
   ```
   /Assets/Resources/SessionUploaderKey.txt
   /Assets/Resources/SessionUploaderKey.txt.meta
   ```

The key file is the only place the key is read from; there is no inspector field, so it can't end
up in a scene or prefab. Without the file, the uploader logs a warning at startup and uploads
nothing. Sessions stay on disk and are uploaded on the first launch after the file is added.

The key still ships inside the build, so anyone with the APK can extract it. It keeps random
traffic out, nothing more. A `401 unauthorized` in the console means the key doesn't match
`INGEST_KEY` on the server.

### Grab directly from headset

- `adb pull /sdcard/Android/data/com.ECDA.VRTutorial/files/sessions LOCAL_PATH`

### Other settings

- **Endpoint:** use `https://`. An `http://` endpoint (e.g. a laptop server on the LAN) also needs
  `Player Settings > Other Settings > Allow downloads over HTTP` set to `Always allowed`.
- **Live Upload:** also uploads the running session every `Live Upload Interval` seconds
  (minimum 10), so a dashboard can follow it. The session is uploaded again when it ends, and
  that final upload is the one the server keeps.
