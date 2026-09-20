# Third Party Notices

This package contains third-party software components governed by the licence(s) indicated
below.

---

## SceneAttribute

- Component: `Runtime/Toolkit/ThirdParty/SceneAttribute.cs` (v1.0, modified: namespaced)
- Source: Unity Asset Store, "Scene Attribute — Reference Scenes in Inspector", Aiden Nathan,
  https://github.com/Agent40infinity/
- Licence: MIT

```
MIT License Copyright (c) 2025 Aiden Nathan

Permission is hereby granted, free of charge, to any person obtaining a copy of this software
and associated documentation files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
```

## Guidance Line

- Component: `Runtime/Toolkit/ThirdParty/GuidanceLine/` (v1.0.0, unmodified apart from folder
  location)
- Source: Unity Asset Store, "Guidance Line", CesarG,
  https://assetstore.unity.com/publishers/109534
- Licence: the author's terms, from the bundled `README.txt`: "You may use this asset freely in
  your projects as long as you credit me if you deploy / publish your project."

**Credit is required.** Any shipped experience built on this package must display:

> Guidance Line by CesarG (Unity Asset Store)

## Quick Outline

- Components: `Runtime/Toolkit/ThirdParty/QuickOutline/` (v1.1, modified: namespaced, GUIDs
  regenerated). The shaders and materials under `Resources/` are in active use: the
  `Outline Fill` and `Outline Mask` renderer features in `Runtime/Toolkit/Art/` reference the
  materials, and `SimpleOutline` drives the shaders' `_OutlineColor` and `_OutlineWidth`
  properties. Only `Scripts/Outline.cs` is unused, having been superseded by
  `Runtime/Toolkit/Interaction/Highlight/SimpleOutline.cs`.
- Source: Quick Outline by Chris Nolet, https://github.com/chrisnolet/QuickOutline (also
  distributed on the Unity Asset Store)
- Licence: MIT

```
MIT License

Copyright (c) 2018 Chris Nolet

Permission is hereby granted, free of charge, to any person obtaining a copy of this software
and associated documentation files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
```

## Unity XR Interaction Toolkit samples and VR Template

- Components: `Runtime/Toolkit/Callouts/{Callout,CalloutGazeController,BezierCurve}.cs`
  (modified: `SetHandSide` / `FlipCurve` added) and the prefabs, materials and models under
  `Samples~/TutorialAssets/Assets/`, copied from the XR Interaction Toolkit Starter Assets
  sample and Unity's VR project template.
- Source: Unity Technologies
- Licence: Unity Companion License, https://unity.com/legal/licenses/unity-companion-license
  (the XR Interaction Toolkit package licence applies to the sample assets:
  https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.6/license/LICENSE.html)
