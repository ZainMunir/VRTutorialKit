using System.Collections.Generic;
using UnityEngine;

namespace ECDA.VRTutorialKit
{

    public class ObjectHider : MonoBehaviour
    {
        [SerializeField] private List<GameObject> objectsToHide = new List<GameObject>();


        [SerializeField] private bool isHidden = false;

        private Dictionary<GameObject, Vector3> originalPositions;

        void OnEnable()
        {
            originalPositions = new Dictionary<GameObject, Vector3>();
            foreach (var obj in objectsToHide)
            {
                if (obj != null)
                {
                    originalPositions[obj] = obj.transform.position;
                }
            }
            UpdateObjectVisibility();
        }

        void UpdateObjectVisibility()
        {
            foreach (var obj in objectsToHide)
            {
                if (obj != null)
                {
                    if (isHidden)
                    {
                        obj.transform.position = originalPositions[obj] + Vector3.down * 10;
                    }
                    else
                    {
                        obj.transform.position = originalPositions[obj];
                    }
                }
            }
        }

        public void ToggleVisibility()
        {
            isHidden = !isHidden;
            UpdateObjectVisibility();
        }
    }
}