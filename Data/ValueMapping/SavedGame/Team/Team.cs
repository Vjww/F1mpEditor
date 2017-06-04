namespace Data.ValueMapping.SavedGame.Team
{
    public class Team
    {
        // Offset values
        private const int BaseOffset = 373052;
        private const int LocalOffset = 1212;

        private const int NameOffset = 0;
        // private const int Department1Staff1Count = 836;
        // private const int Department1StaffSalary= 840;
        // private const int Department1Staff2Count = 848;
        // private const int Department1Staff2Salary = 852;
        private const int Department1MotivationOffset = 896;
        private const int Department1HappinessOffset = 900;
        private const int Department1PressureOffset = 904;
        private const int Department2MotivationOffset = 968;
        private const int Department2HappinessOffset = 972;
        private const int Department2PressureOffset = 976;
        private const int Department3MotivationOffset = 1040;
        private const int Department3HappinessOffset = 1044;
        private const int Department3PressureOffset = 1048;
        private const int Department4MotivationOffset = 1112;
        private const int Department4HappinessOffset = 1116;
        private const int Department4PressureOffset = 1120;
        private const int Department5MotivationOffset = 1184;
        private const int Department5HappinessOffset = 1188;
        private const int Department5PressureOffset = 1192;

        // Lengths
        public const int NameLength = 40;

        // Properties
        public int Name { get; set; }
        public int Department1Motivation { get; set; }
        public int Department1Happiness { get; set; }
        public int Department2Motivation { get; set; }
        public int Department2Happiness { get; set; }
        public int Department3Motivation { get; set; }
        public int Department3Happiness { get; set; }
        public int Department4Motivation { get; set; }
        public int Department4Happiness { get; set; }
        public int Department5Motivation { get; set; }
        public int Department5Happiness { get; set; }

        public Team(int id)
        {
            // Calculate step offset from zero based index
            var stepOffset = LocalOffset * id;

            Name = BaseOffset + stepOffset + NameOffset;
            Department1Motivation = BaseOffset + stepOffset + Department1MotivationOffset;
            Department1Happiness = BaseOffset + stepOffset + Department1HappinessOffset;
            Department2Motivation = BaseOffset + stepOffset + Department2MotivationOffset;
            Department2Happiness = BaseOffset + stepOffset + Department2HappinessOffset;
            Department3Motivation = BaseOffset + stepOffset + Department3MotivationOffset;
            Department3Happiness = BaseOffset + stepOffset + Department3HappinessOffset;
            Department4Motivation = BaseOffset + stepOffset + Department4MotivationOffset;
            Department4Happiness = BaseOffset + stepOffset + Department4HappinessOffset;
            Department5Motivation = BaseOffset + stepOffset + Department5MotivationOffset;
            Department5Happiness = BaseOffset + stepOffset + Department5HappinessOffset;
        }
    }
}
