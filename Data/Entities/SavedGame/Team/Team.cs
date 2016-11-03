using System.ComponentModel.DataAnnotations;

namespace Data.Entities.SavedGame.Team
{
    public interface ITeam
    {
        // Department1 = Design
        // Department2 = Production
        // Department3 = Aerodynamics
        // Department4 = Assembly
        // Department5 = Race

        // Happiness
        // Motivation
        // Pressure

        int Id { get; set; }
        string Name { get; set; }

        int Department1Motivation { get; set; }
        int Department1Happiness { get; set; }
        int Department2Motivation { get; set; }
        int Department2Happiness { get; set; }
        int Department3Motivation { get; set; }
        int Department3Happiness { get; set; }
        int Department4Motivation { get; set; }
        int Department4Happiness { get; set; }
        int Department5Motivation { get; set; }
        int Department5Happiness { get; set; }
    }

    public class Team : ITeam
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Display(Name = "Design Department Motivation")]
        public int Department1Motivation { get; set; }
        [Display(Name = "Design Department Happiness")]
        public int Department1Happiness { get; set; }
        [Display(Name = "Production Department Motivation")]
        public int Department2Motivation { get; set; }
        [Display(Name = "Production Department Happiness")]
        public int Department2Happiness { get; set; }
        [Display(Name = "Aerodynamics Department Motivation")]
        public int Department3Motivation { get; set; }
        [Display(Name = "Aerodynamics Department Happiness")]
        public int Department3Happiness { get; set; }
        [Display(Name = "Assembly Department Motivation")]
        public int Department4Motivation { get; set; }
        [Display(Name = "Assembly Department Happiness")]
        public int Department4Happiness { get; set; }
        [Display(Name = "Race Department Motivation")]
        public int Department5Motivation { get; set; }
        [Display(Name = "Race Department Happiness")]
        public int Department5Happiness { get; set; }
    }
}
