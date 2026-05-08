using System.Collections;
using IsotopesStats.Domain.Models;

namespace IsotopesStats.Website.Services;

public static class LineupEncoder
{
    private const int BitsPerPlayer = 6;
    private const int BitsPerPosition = 4;

    public enum Position { None = 0, P = 1, C = 2, _1B = 3, _2B = 4, _3B = 5, SS = 6, LF = 7, LC = 8, RC = 9, RF = 10, B = 11 }

    public static string Encode(List<(int PlayerId, string Position)> lineup, List<int> benchIds, List<Player> roster)
    {
        if ((lineup == null || !lineup.Any()) && (benchIds == null || !benchIds.Any())) return string.Empty;
        if (roster == null) return string.Empty;

        // Format: [LineupCount: 6 bits] [BenchCount: 6 bits]
        // For each lineup: [PlayerIndex: 6 bits] [Position: 4 bits]
        // For each bench: [PlayerIndex: 6 bits]

        int lineupCount = lineup?.Count ?? 0;
        int benchCount = benchIds?.Count ?? 0;

        int totalBits = 6 + 6 + (lineupCount * (BitsPerPlayer + BitsPerPosition)) + (benchCount * BitsPerPlayer);
        BitArray bitArray = new BitArray(totalBits);
        int bitIndex = 0;

        WriteBits(bitArray, ref bitIndex, lineupCount, 6);
        WriteBits(bitArray, ref bitIndex, benchCount, 6);

        if (lineup != null)
        {
            foreach ((int PlayerId, string Position) item in lineup)
            {
                WriteBits(bitArray, ref bitIndex, GetRosterIndex(item.PlayerId, roster), BitsPerPlayer);
                WriteBits(bitArray, ref bitIndex, (int)AbbrToPosition(item.Position), BitsPerPosition);
            }
        }

        if (benchIds != null)
        {
            foreach (int id in benchIds)
            {
                WriteBits(bitArray, ref bitIndex, GetRosterIndex(id, roster), BitsPerPlayer);
            }
        }

        byte[] bytes = new byte[(bitArray.Length + 7) / 8];
        bitArray.CopyTo(bytes, 0);

        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    public static (List<(int PlayerId, string Position)> Lineup, List<int> Bench) Decode(string? token, List<Player> roster)
    {
        if (string.IsNullOrEmpty(token) || roster == null) return (new(), new());

        try
        {
            string base64 = token.Replace('-', '+').Replace('_', '/');
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            byte[] bytes = Convert.FromBase64String(base64);
            BitArray bitArray = new BitArray(bytes);
            int bitIndex = 0;

            if (bitArray.Length < 12) return (new(), new());

            int lineupCount = ReadBits(bitArray, ref bitIndex, 6);
            int benchCount = ReadBits(bitArray, ref bitIndex, 6);

            List<(int PlayerId, string Position)> lineup = new List<(int PlayerId, string Position)>();
            for (int i = 0; i < lineupCount; i++)
            {
                int playerIdx = ReadBits(bitArray, ref bitIndex, BitsPerPlayer);
                int posIdx = ReadBits(bitArray, ref bitIndex, BitsPerPosition);
                lineup.Add((GetPlayerIdFromIndex(playerIdx, roster), PositionToAbbr((Position)posIdx)));
            }

            List<int> bench = new List<int>();
            for (int i = 0; i < benchCount; i++)
            {
                int playerIdx = ReadBits(bitArray, ref bitIndex, BitsPerPlayer);
                bench.Add(GetPlayerIdFromIndex(playerIdx, roster));
            }

            return (lineup, bench);
        }
        catch
        {
            return (new(), new());
        }
    }

    private static int GetRosterIndex(int playerId, List<Player> roster)
    {
        int index = roster.FindIndex(p => p.Id == playerId);
        return (index != -1 && index < 63) ? index : 63;
    }

    private static int GetPlayerIdFromIndex(int rosterIndex, List<Player> roster)
    {
        if (rosterIndex >= 0 && rosterIndex < roster.Count && rosterIndex != 63)
        {
            return roster[rosterIndex].Id;
        }
        return 0;
    }

    private static void WriteBits(BitArray bitArray, ref int bitIndex, int value, int bitCount)
    {
        for (int i = 0; i < bitCount; i++)
        {
            if (bitIndex < bitArray.Length)
            {
                bitArray.Set(bitIndex++, (value & (1 << i)) != 0);
            }
        }
    }

    private static int ReadBits(BitArray bitArray, ref int bitIndex, int bitCount)
    {
        int value = 0;
        for (int i = 0; i < bitCount; i++)
        {
            if (bitIndex < bitArray.Length && bitArray.Get(bitIndex++))
            {
                value |= (1 << i);
            }
        }
        return value;
    }

    private static Position AbbrToPosition(string abbr) => abbr switch
    {
        "P" => Position.P, "C" => Position.C, "1B" => Position._1B, "2B" => Position._2B,
        "3B" => Position._3B, "SS" => Position.SS, "LF" => Position.LF, "LC" => Position.LC,
        "RC" => Position.RC, "RF" => Position.RF, "B" => Position.B, _ => Position.None
    };

    private static string PositionToAbbr(Position pos) => pos switch
    {
        Position.P => "P", Position.C => "C", Position._1B => "1B", Position._2B => "2B",
        Position._3B => "3B", Position.SS => "SS", Position.LF => "LF", Position.LC => "LC",
        Position.RC => "RC", Position.RF => "RF", Position.B => "B", _ => ""
    };
}
