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
2. Add the `Outline Fill` and `Outline Mask` renderer features to the existing render
   pipeline (`Assets/Settings/Project Configuration/Android Preset`). The two feature assets
   ship at `Runtime/Toolkit/Art/`.

## Samples

Import **Tutorial Assets** from the Package Manager for a working tutorial scene.


## Session telemetry

`SessionLogger` writes one JSON Lines file per session to `Application.persistentDataPath/sessions`. Add
`SessionUploader` to the same GameObject to also POST finished sessions to a telemetry server.
Uploaded files move to `sessions/uploaded/`. A request that gets no response at all is retried
once after a second. Files that still fail stay where they are and are retried on the next launch
or session end.

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

### Carrying state across scenes

The tutorial and the experience after it each keep their own log. `ParticipantProfile` carries
what the tutorial learned about the participant into the experience.

- **Tutorial scene:** a `SessionLogger` with `Persist Across Scenes` off, so it only logs the
  tutorial, and a `ParticipantProfile` on its own GameObject.
- **Experience's first scene:** its own `SessionLogger`, with `Persist Across Scenes` on if the
  experience spans several scenes, and another `ParticipantProfile`. Coming from the tutorial,
  the tutorial's profile survives and this one removes itself; run on its own, the experience
  still has a blank profile.

Every session's `session_start` line carries the profile, and a `participant_profile` line is
written whenever it changes. `linkId` is random and joins the tutorial and experience logs;
`participantNumber` is what the participant typed, to match against a survey.

Session ids are `yyyyMMdd_HHmmss_<8 random hex>`. A non-persistent uploader holds a
`SceneTransitionController` scene change, for up to `Timeout Seconds`, while an upload is in
flight.

### What gets logged

Two components write the events below. Neither needs wiring; add them and they find what they
listen to.

- **`TutorialSessionLog`** (tutorial scene, next to `TutorialManager`): the tutorial's flow. It
  ends the session when the tutorial is exited with Finish or Skip, so the tutorial's log runs
  from start to that button and nothing after it.
- **`InteractionSessionLog`** (any scene with a `SessionLogger`, including the experience):
  what the participant does in the scene. Each group of events has its own checkbox
  (interactions, locomotion, conditions, respawns), checked when the event happens, so they can
  also be switched at runtime. Disabling the component mutes all of them.

`SessionLogger` itself writes the session, scene and language events, so every session has them
whichever components are present.

`SessionLogEvent` stays available for one-off moments that neither covers. It accepts a string,
bool, int or float, passed from the event or typed in the Inspector, and stores every value in
`detail` as text.

Every line is `{"t": <seconds since app start>, "utc": "<ISO 8601>", "event": "<name>", "data": {...}}`.
Names are GameObject or asset names. Durations are in seconds; `-1` means the start was not
seen.

| Event | `data` | Written by |
|---|---|---|
| `session_start` | `profile`, `project`, `appVersion`, `device`, `scene`, `locale`, `isEditor` | `SessionLogger` |
| `session_end`, `app_paused`, `app_resumed` | none | `SessionLogger` |
| `scene_loaded` | `scene` | `SessionLogger` |
| `locale_changed` | `locale` (e.g. `en`, `nl`), only when the language is switched | `SessionLogger` |
| `participant_profile` | `participantNumber`, `linkId` | `ParticipantProfile` |
| `tutorial_started` | `config`, `stepCount` | `TutorialSessionLog` |
| `config_selected` | `config`, `stepCount` | `TutorialSessionLog` |
| `step_started` | `index`, `step`, `direction` (`first`, `next`, `previous`, `skip`, `config_changed`) | `TutorialSessionLog` |
| `step_completed` | `index`, `step`, `seconds` on the step | `TutorialSessionLog` |
| `substep_active` | `stepIndex`, `subStep` | `TutorialSessionLog` |
| `substep_completed` | `stepIndex`, `subStep`, `seconds` active | `TutorialSessionLog` |
| `tutorial_all_steps_completed` | `seconds` since `tutorial_started` | `TutorialSessionLog` |
| `tutorial_exited` | `reason` (`finished`, `skipped`), `lastStepIndex`, `lastStep`, `seconds` | `TutorialSessionLog` |
| `grab` | `target`, `hand` (`left`, `right`, `none`), `via` (`near`, `far`, `poke`, `gaze`) | `InteractionSessionLog` |
| `release` | `target`, `hand`, `via`, `seconds` held, `canceled` | `InteractionSessionLog` |
| `socket_placed` | `target`, `socket` | `InteractionSessionLog` |
| `socket_removed` | `target`, `socket`, `seconds` in socket, `canceled` | `InteractionSessionLog` |
| `activate`, `deactivate` | `target`, `hand` | `InteractionSessionLog` |
| `respawn` | `target`, `cause` (`left_volume`, `idle`, `direct`) | `InteractionSessionLog` |
| `condition_met`, `condition_lost` | `source`, `type` (e.g. `ActionOnGaze`) | `InteractionSessionLog` |
| `button_pressed` | `source`, `action` (input action name) | `InteractionSessionLog` |
| `trigger_entered` | `trigger`, `matched` | `InteractionSessionLog` |
| `recenter` | `distance` the camera jumped | `InteractionSessionLog` |
| `move_started`, `turn_started` | `provider`, `position` | `InteractionSessionLog` |
| `teleport`, `snap_turn`, `move_ended`, `turn_ended`, `locomotion` | `provider`, `position` after, `distance`, `angle`, `seconds` | `InteractionSessionLog` |

`canceled` is true when the selection ended because something was disabled or destroyed, e.g. a
step's interaction prefab being replaced, rather than let go. `locomotion` covers providers
without their own name, such as grab-move and climb. Teleports don't also log a grab of the
floor or anchor.
