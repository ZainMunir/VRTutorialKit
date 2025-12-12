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

## v0.1.0
Basic package structure following unity documentation and import tested in other project via git url

### Commits
b7c169a change: version and add author
a181c37 add: more structure from unity website
decbc73 init: structure