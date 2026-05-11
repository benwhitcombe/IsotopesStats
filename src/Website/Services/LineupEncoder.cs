using System.Collections;
using IsotopesStats.Domain.Models;

namespace IsotopesStats.Website.Services;

public static class LineupEncoder
{
    private const int BitsPerPlayer = 6;
    private const int BitsPerPosition = 4;

    public enum Position { None = 0, P = 1, C = 2, _1B = 3, _2B = 4, _3B = 5, SS = 6, LF = 7, LC = 8, RC = 9, RF = 10, B = 11 }

    public static string Encode(List<(int PlayerId, string Position, Dictionary<int, string>? InningPositions)> lineup, List<int> benchIds, List<Player> roster)
    {
        if ((lineup == null || !lineup.Any()) && (benchIds == null || !benchIds.Any())) return string.Empty;
        if (roster == null) return string.Empty;

        int lineupCount = lineup?.Count ?? 0;
        int benchCount = benchIds?.Count ?? 0;

        // Base bits + 28 bits per player for 7 innings (4 bits per position)
        int totalBits = 6 + 6 + (lineupCount * (BitsPerPlayer + BitsPerPosition)) + (benchCount * BitsPerPlayer) + (lineupCount * 7 * BitsPerPosition);
        BitArray bitArray = new BitArray(totalBits);
        int bitIndex = 0;

        WriteBits(bitArray, ref bitIndex, lineupCount, 6);
        WriteBits(bitArray, ref bitIndex, benchCount, 6);

        if (lineup != null)
        {
            foreach ((int PlayerId, string Position, Dictionary<int, string>? InningPositions) item in lineup)
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

        // Add inning data at the end (Sparse Overrides)
        if (lineup != null)
        {
            foreach ((int PlayerId, string Position, Dictionary<int, string>? InningPositions) item in lineup)
            {
                for (int i = 1; i <= 7; i++)
                {
                    string? pos = item.InningPositions?.GetValueOrDefault(i);
                    WriteBits(bitArray, ref bitIndex, string.IsNullOrEmpty(pos) ? 0 : (int)AbbrToPosition(pos), BitsPerPosition);
                }
            }
        }

        byte[] bytes = new byte[(bitIndex + 7) / 8];
        for (int i = 0; i < bitIndex; i++)
        {
            if (bitArray[i]) bytes[i / 8] |= (byte)(1 << (i % 8));
        }

        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    public static (List<(int PlayerId, string Position, Dictionary<int, string> InningPositions)> Lineup, List<int> Bench) Decode(string? token, List<Player> roster)
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

            List<(int PlayerId, string Position, Dictionary<int, string> InningPositions)> lineup = new();
            for (int i = 0; i < lineupCount; i++)
            {
                int playerIdx = ReadBits(bitArray, ref bitIndex, BitsPerPlayer);
                int posIdx = ReadBits(bitArray, ref bitIndex, BitsPerPosition);
                lineup.Add((GetPlayerIdFromIndex(playerIdx, roster), PositionToAbbr((Position)posIdx), new Dictionary<int, string>()));
            }

            List<int> bench = new List<int>();
            for (int i = 0; i < benchCount; i++)
            {
                int playerIdx = ReadBits(bitArray, ref bitIndex, BitsPerPlayer);
                bench.Add(GetPlayerIdFromIndex(playerIdx, roster));
            }

            // Check if we have inning data (28 bits per lineup player)
            if (bitIndex + (lineupCount * 7 * BitsPerPosition) <= bitArray.Length)
            {
                for (int i = 0; i < lineupCount; i++)
                {
                    for (int j = 1; j <= 7; j++)
                    {
                        int innPosVal = ReadBits(bitArray, ref bitIndex, BitsPerPosition);
                        if (innPosVal > 0)
                        {
                            lineup[i].InningPositions[j] = PositionToAbbr((Position)innPosVal);
                        }
                    }
                }
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
