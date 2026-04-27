namespace IsotopesStats.Website.Services
{
    public static class StatDefinitions
    {
        private static readonly Dictionary<string, string> Definitions = new Dictionary<string, string>
        {
            { "1B", "Singles" },
            { "2B", "Doubles" },
            { "3B", "Triples" },
            { "IPHR", "Inside the Park Home Runs" },
            { "AB", "At Bats" },
            { "AVG", "Batting Average" },
            { "BB", "Base on Balls (Walk)" },
            { "BO", "Batting Order" },
            { "FC", "Fielder's Choice" },
            { "FO", "Fly Outs" },
            { "GO", "Ground Outs" },
            { "H", "Hits" },
            { "HR", "Home Runs" },
            { "K", "Strike Outs" },
            { "KF", "Strike Out on Fouls" },
            { "O", "Outs" },
            { "OBP", "On Base Percentage" },
            { "OPS", "On Base Plus Slugging" },
            { "GP", "Games Played" },
            { "PA", "Plate Appearances" },
            { "R", "Runs" },
            { "RBI", "Runs Batted In" },
            { "SF", "Sacrifice Flies" },
            { "SLG", "Slugging" },
            { "TB", "Total Bases" }
        };

        public static string GetDefinition(string abbreviation)
        {
            return Definitions.TryGetValue(abbreviation, out var definition) ? definition : string.Empty;
        }
    }
}
