# Changelog
All notable changes to this project will be documented in this file.

Tag commits with 
```
git tag vX.X.X [HASH]
```
where hash is optional (takes current commit without it) like
```
git tag v0.0.0 decbc73d833677b4e0b04b8e9964533bcfd87b24
```

Get commits since tag with 
```
git log vX.X.X..HEAD --oneline --no-decorate
```
like 
```
git log v0.0.0..HEAD --oneline --no-decorate
```

## v0.4.0
Localisation with English, Dutch. Version used (mostly) in Outsourced during course

```
f87abbb add: hider uses onEnable and onDisable instead of Start
dcae6f2 add: dutch translations and make it the default
af1b339 add: prevent teleport under tables
6aafb70 fix: grabmovement arrows flipping multiple times
df69084 add: all text uses localisation table now
4e494ae fix: add dependency
7280e58 add: localization package to test
c018e90 fix: int to float size, startOnEnable false
e0265a9 add: ability to set bouncing arrow size
80ee265 add: progressprovider to actiononallconditions
fe03fa0 fix: actionafterdelay multiple uses
698063c add: stacked conditional events and orientation action
cc67b27 add: general gaze action
3c5ef71 add: tags to respawn functionality
644995e add: bouncing arrow mechanism and targets, action after delay
097de98 add: ability to run something before scene change
95ff5d6 fix: disabling socketinteractabletag prevents mount
7b2974a add: separate out tutorial controls from rigcontroller
6ad8e9c add: flip arrows in grab movement + set placement as target after delay
6b77ac0 change: cuboid -> cube
0186a15 add: limit teleportable area
f25a613 add: change some wording and plane backing to UI
778b4a9 add: changes to rig
469c020 add: revert to old shader with new method
4c32e1b add: potential alternative to inconsistent outlining
2533342 update: make triggerenterevents reusable elsewhere
6fdef66 add: use new door prefab for doors task too
b23dfa7 add: hidden objects exploring task
f973f6b add: unhighlight with helper
e82f2a7 add: new way of handling socket filtering
909cdaf add: dynamic update to interactionHighlighter
020f353 add: allow setting linkedobjects in highlighter
7c6c9db add: variations of steps with teleport on both hands
74f8df5 add: fix attempts for scene transitions breaking
95d874e fix: hovervisual and highlightobject conflict
fe8fe57 add: improve hover visual to use all materials
a1eee30 add: improve hinge animator
8f2d62d fix: layering of arrow when not in overlay
faa69e6 add: videos for each step
c7b0466 add: hide controller button
157ba6d add: guide arrows in grabfarmove and different second door
c896709 fix: teleport anchor and gaze target bugs
98471c1 changelog: entry v0.3.0
```

## v0.3.0
Guiding arrows, new steps (gaze, recenter, variations of movement and grab) and reworked tooltips

### Commits
```
13b3e0d add: color interpolation effect for gaze step
c676a46 add: grab movement step
e522f28 add: change how tooltips are handled
a0862a8 add: make all tooltips right handed and flip for left instead
7667336 add: basic both controller movement steps
2da800c add: re-center step
18dd70f add: recenter tooltip
3fdbd70 add: far grab step
7c755d7 add: changed navigation between scenes
10421fd add: tutorial config selector ui
41926f0 refactor: file locations
1b97e68 add: gaze interaction step
d25705b add: guiding arrows based on target position
98075c5 add: locomotion endings closer to table
463bbba add: new table
193f6f9 fix: outline stuff unconnected
06ca15a changelog: entry v0.2.0
```

## v0.2.0
First versions of many vr interactions with placeholder video visuals, tested in other project

### Commits
```
6825a9c fix: regenerate guids for quickoutline
157a842 refactor: move quickoutline into package
224664f add: respawn objects if far away
b3c6ded add: outlined objects interaction v1
9f0da93 update: prefabs and grab interaction
c19732f update: separate collider trigger
cf98f2d add: door interaction v1
e89a69c add: dynamic text ui
793bfb8 move: ui into runtime
71a9c51 wip: door interaction
0f3aac4 init: quick outline unity package
4a4bb60 fix: held or socketed objects not removed fully
8261923 add: socket interaction v1
bde5f62 add: skip tutorial option
bcdaaaf add: smooth movement interaction v1 similar to teleport
b3f5e06 add: teleport interaction v1
a319e3e add: ui interaction step v1 complete
5d859d0 fix: video performance issues
e393bd6 wip: ui interaction step
759c78f wip: grab tutorial step
bb86c75 fix: missing material
17782cc add: substep system for managing step completion
3a7af5e wip: rig controller to choose active movement controls
631c98f fix: missing model
371e82b fix: missing theme.tss
44958b1 fix: missing panel settings
4e7fcd5 add: exit scene UI and fix missing prefab
a4b2f42 add: fading transition when exiting
d617390 fix: sample path
05e15b5 add: basic interaction prefab for grab
526d8ef add: video in ui
bb75e8e add: move tooltip logic into tooltipcontroller
d85e434 add: UI interactions
2de6a55 change: de-duplicate template scene assets
a00d8c1 fix: namespace curly brace wrong indentation
5913b0e fix: move dynamic portions into samples
5b3b954 add: all tooltips and build tested
aeabee4 add: dynamic tooltip based on tutorial step
5ad4f3c init: main scene
ce9b9b1 changelog: entry v0.1.0
```

## v0.1.0
Basic package structure following unity documentation and import tested in other project via git url

### Commits
```
b7c169a change: version and add author
a181c37 add: more structure from unity website
decbc73 init: structure
```