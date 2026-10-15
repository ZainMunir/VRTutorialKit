using System;
using UnityEngine;

namespace ECDA.VRTutorialKit
{

    public class ParticipantProfile : SingletonBehaviour<ParticipantProfile>
    {
        public string ParticipantNumber { get; private set; } = string.Empty;
        public string LinkId { get; private set; }
        public event Action Changed;
        protected override bool PersistAcrossScenes => true;

        protected override void OnSingletonAwake()
        {
            LinkId = Guid.NewGuid().ToString("N");
            LogSnapshot();
        }

        public void SetParticipantNumber(string number)
        {
            ParticipantNumber = number?.Trim() ?? string.Empty;
            Changed?.Invoke();
            LogSnapshot();
        }

        public Snapshot GetSnapshot() => new Snapshot { participantNumber = ParticipantNumber, linkId = LinkId };

        private void LogSnapshot()
        {
            if (SessionLogger.Instance != null) SessionLogger.Instance.Log("participant_profile", GetSnapshot());
        }

        [Serializable]
        public struct Snapshot
        {
            public string participantNumber, linkId;
        }

        public bool HasParticipantNumber => !string.IsNullOrWhiteSpace(ParticipantNumber);
    }
}
