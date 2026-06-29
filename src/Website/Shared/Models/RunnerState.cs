namespace IsotopesStats.Website.Shared.Models
{
    public class RunnerState
    {
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public bool IsBatter { get; set; }
        public int StartBase { get; set; }
        public int Destination { get; set; }
        public bool ShowOutOptions { get; set; }
    }
}
