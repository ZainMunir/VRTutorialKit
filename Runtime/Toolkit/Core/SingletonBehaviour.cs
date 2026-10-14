using UnityEngine;

namespace ECDA.VRTutorialKit
{
    // Subclasses must not declare Awake (it hides this one and Instance is never set); use OnSingletonAwake.
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        private static T instance;

        public static T Instance
        {
            get
            {
#if UNITY_EDITOR
                // Awake has not run in edit mode, so fall back to a search there only.
                if (instance == null && !Application.isPlaying)
                    instance = FindAnyObjectByType<T>();
#endif
                return instance;
            }
        }

        protected virtual bool PersistAcrossScenes => false;

        protected virtual void OnSingletonAwake() { }

        // Called on the duplicate just before its GameObject is destroyed.
        protected virtual void OnDuplicateDestroyed() { }

        protected void Awake()
        {
            if (instance != null && instance != this)
            {
                OnDuplicateDestroyed();
                Destroy(gameObject);
                return;
            }

            instance = (T)this;
            if (PersistAcrossScenes) DontDestroyOnLoad(gameObject);

            OnSingletonAwake();
        }

        protected virtual void OnDestroy()
        {
            if (instance == this) instance = null;
        }
    }
}