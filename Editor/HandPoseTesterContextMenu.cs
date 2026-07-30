using UnityEditor;
using UnityEngine;

namespace ECDA.VRTutorialKit.EditorTools
{
    static class HandPoseTesterContextMenu
    {
        const string SaveMenuPath = "CONTEXT/HandPoseTester/Save Curls As HandPose Asset";
        const string LoadMenuPath = "CONTEXT/HandPoseTester/Load Pose Preview Into Sliders";
        const string LivePoseField = "livePose";

        /// <summary>
        /// Serialized curl field names, identical on HandPoseTester and HandPose,
        /// ordered to match <see cref="FingerType"/>.
        /// </summary>
        static readonly string[] CurlFields =
        {
            "index",
            "middle",
            "ring",
            "pinky",
            "thumb",
        };

        static readonly FingerType[] Fingers =
        {
            FingerType.Index,
            FingerType.Middle,
            FingerType.Ring,
            FingerType.Pinky,
            FingerType.Thumb,
        };

        [MenuItem(SaveMenuPath)]
        static void SaveCurlsAsHandPose(MenuCommand command)
        {
            var tester = (HandPoseTester)command.context;
            var testerObject = new SerializedObject(tester);

            var livePose = testerObject.FindProperty(LivePoseField);
            if (livePose != null && livePose.objectReferenceValue != null)
            {
                var proceed = EditorUtility.DisplayDialog(
                    "Pose Preview Assigned",
                    "A Pose Preview asset is assigned, which overrides the sliders on the driver. " +
                    "The exported asset will contain the slider values, not the previewed pose.\n\n" +
                    "Export the slider values anyway?",
                    "Export Sliders",
                    "Cancel");

                if (!proceed)
                    return;
            }

            var curls = new float[CurlFields.Length];
            for (var i = 0; i < CurlFields.Length; i++)
            {
                var property = testerObject.FindProperty(CurlFields[i]);
                if (property == null || property.propertyType != SerializedPropertyType.Float)
                {
                    Debug.LogError(
                        $"HandPoseTester has no float field '{CurlFields[i]}'. " +
                        "Update CurlFields in HandPoseTesterContextMenu.", tester);
                    return;
                }

                curls[i] = property.floatValue;
            }

            var path = EditorUtility.SaveFilePanelInProject(
                "Save Hand Pose",
                $"{tester.name}_Pose",
                "asset",
                "Choose where to save the generated HandPose asset.");

            if (string.IsNullOrEmpty(path))
                return;

            // Overwriting in place preserves every reference to the asset, and leaves
            // Transition Speed untouched since only the curl fields are written.
            var existing = AssetDatabase.LoadAssetAtPath<HandPose>(path);
            var pose = existing != null ? existing : ScriptableObject.CreateInstance<HandPose>();

            var poseObject = new SerializedObject(pose);
            for (var i = 0; i < CurlFields.Length; i++)
            {
                var property = poseObject.FindProperty(CurlFields[i]);
                if (property == null || property.propertyType != SerializedPropertyType.Float)
                {
                    Debug.LogError(
                        $"HandPose has no float field '{CurlFields[i]}'. " +
                        "Update CurlFields in HandPoseTesterContextMenu.", pose);
                    return;
                }

                property.floatValue = curls[i];
            }

            poseObject.ApplyModifiedPropertiesWithoutUndo();

            if (existing == null)
                AssetDatabase.CreateAsset(pose, path);

            EditorUtility.SetDirty(pose);
            AssetDatabase.SaveAssets();

            EditorGUIUtility.PingObject(pose);
            Selection.activeObject = pose;

            Debug.Log(
                $"{(existing == null ? "Created" : "Updated")} hand pose at '{path}' " +
                $"(index {curls[0]:F3}, middle {curls[1]:F3}, ring {curls[2]:F3}, " +
                $"pinky {curls[3]:F3}, thumb {curls[4]:F3}).", pose);
        }

        [MenuItem(LoadMenuPath)]
        static void LoadPoseIntoSliders(MenuCommand command)
        {
            var tester = (HandPoseTester)command.context;
            var testerObject = new SerializedObject(tester);

            var livePoseProperty = testerObject.FindProperty(LivePoseField);
            var pose = livePoseProperty != null
                ? livePoseProperty.objectReferenceValue as HandPose
                : null;

            if (pose == null)
            {
                EditorUtility.DisplayDialog(
                    "No Pose Preview Assigned",
                    "Assign a HandPose to the Pose Preview field first. Its curls will be copied " +
                    "into the sliders and the field will be cleared so the sliders take effect.",
                    "OK");
                return;
            }

            for (var i = 0; i < CurlFields.Length; i++)
            {
                var property = testerObject.FindProperty(CurlFields[i]);
                if (property == null || property.propertyType != SerializedPropertyType.Float)
                {
                    Debug.LogError(
                        $"HandPoseTester has no float field '{CurlFields[i]}'. " +
                        "Update CurlFields in HandPoseTesterContextMenu.", tester);
                    return;
                }

                property.floatValue = pose.GetCurl(Fingers[i]);
            }

            // Cleared so the sliders drive the hand again; otherwise the preview keeps overriding.
            livePoseProperty.objectReferenceValue = null;

            testerObject.ApplyModifiedProperties();

            Debug.Log($"Loaded '{pose.name}' into the sliders and cleared Pose Preview.", tester);
        }

        [MenuItem(LoadMenuPath, true)]
        static bool ValidateLoadPoseIntoSliders(MenuCommand command)
        {
            var tester = command.context as HandPoseTester;
            if (tester == null)
                return false;

            var property = new SerializedObject(tester).FindProperty(LivePoseField);
            return property != null && property.objectReferenceValue != null;
        }
    }
}
