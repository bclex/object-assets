using OA.Ultima.Data;
using OA.Ultima.Player.Partying;

namespace OA.Ultima.Player
{
    class PlayerState
    {
        static readonly PlayerState _instance;

        JournalData _journal;
        SkillData _skills;
        StatLockData _statLocks;
        PartySystem _partying;
        Features _features;

        static PlayerState()
        {
            _instance = new PlayerState();
            _instance._journal = new JournalData();
            _instance._skills = new SkillData();
            _instance._statLocks = new StatLockData();
            _instance._partying = new PartySystem();
            _instance._features = new Features();
        }

        public static JournalData Journaling => _instance._journal;
        public static SkillData Skills => _instance._skills;
        public static StatLockData StatLocks => _instance._statLocks;
        public static PartySystem Partying => _instance._partying;
        public static Features ClientFeatures => _instance._features;
        public static AssistantFeatures DisabledFeatures = AssistantFeatures.None;
    }
}
