using IsotopesStats.Domain.Models;

namespace IsotopesStats.Website.Shared.Models
{
    public class SandboxRecordedPlay
    {
        public string Result { get; set; } = "";
        public int Inning { get; set; }
        public int RunsThisInning { get; set; }
        public int IsotopesScore { get; set; }
        public int Outs { get; set; }
        public string? RunnerOn1B { get; set; }
        public string? RunnerOn2B { get; set; }
        public string? RunnerOn3B { get; set; }
        public int BatterIndex { get; set; }
        public string? IsotopesBatterName { get; set; }
        public PlateAppearance PA { get; set; } = new();
    }
}
