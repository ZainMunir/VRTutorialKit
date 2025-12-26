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

## v0.2.0
First versions of many vr interactions with placeholder video visuals, tested in other project

### Commits
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

## v0.1.0
Basic package structure following unity documentation and import tested in other project via git url

### Commits
b7c169a change: version and add author
a181c37 add: more structure from unity website
decbc73 init: structure