using System.Collections;
using IsotopesStats.Domain.Models;

namespace IsotopesStats.Website.Services;

public static class LineupEncoder
{
    private const int BitsPerPlayer = 6;
    private const int StarterCount = 10;

    public static string Encode(List<int> playerIds, List<Player> roster)
    {
        if (playerIds == null || !playerIds.Any() || roster == null) return string.Empty;

        // 1. Build a 10-bit mask for the starters
        ushort mask = 0;
        for (int i = 0; i < StarterCount && i < playerIds.Count; i++)
        {
            if (playerIds[i] > 0)
            {
                mask |= (ushort)(1 << i);
            }
        }

        // 2. Collect indices for filled starters and all bench players
        List<int> indices = new List<int>();
        
        // Add starters if present in mask
        for (int i = 0; i < StarterCount && i < playerIds.Count; i++)
        {
            if (playerIds[i] > 0)
            {
                indices.Add(GetRosterIndex(playerIds[i], roster));
            }
        }

        // Add bench players (indices 10+)
        for (int i = StarterCount; i < playerIds.Count; i++)
        {
            if (playerIds[i] > 0)
            {
                indices.Add(GetRosterIndex(playerIds[i], roster));
            }
        }

        // 3. Bit pack: [10-bit mask] + [N * 6-bit indices]
        int totalBits = StarterCount + (indices.Count * BitsPerPlayer);
        BitArray bitArray = new BitArray(totalBits);
        
        int bitIndex = 0;
        
        // Write mask bits
        for (int i = 0; i < StarterCount; i++)
        {
            bitArray.Set(bitIndex++, (mask & (1 << i)) != 0);
        }

        // Write player indices
        foreach (int indexValue in indices)
        {
            for (int bit = 0; bit < BitsPerPlayer; bit++)
            {
                bool bitValue = (indexValue & (1 << bit)) != 0;
                bitArray.Set(bitIndex++, bitValue);
            }
        }

        byte[] bytes = new byte[(bitArray.Length + 7) / 8];
        bitArray.CopyTo(bytes, 0);

        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    public static List<int> Decode(string? token, List<Player> roster)
    {
        if (string.IsNullOrEmpty(token) || roster == null) return new List<int>();

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
            
            List<int> ids = new List<int>(new int[StarterCount]); // Pre-fill with 10 empty spots
            int bitIndex = 0;

            // 1. Read 10-bit mask
            ushort mask = 0;
            for (int i = 0; i < StarterCount; i++)
            {
                if (bitIndex < bitArray.Length && bitArray.Get(bitIndex++))
                {
                    mask |= (ushort)(1 << i);
                }
            }

            // 2. Read starters based on mask
            for (int i = 0; i < StarterCount; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    int playerIndex = Read6Bits(bitArray, ref bitIndex);
                    ids[i] = GetPlayerIdFromIndex(playerIndex, roster);
                }
            }

            // 3. Read remaining as bench players
            while (bitIndex + BitsPerPlayer <= bitArray.Length)
            {
                int playerIndex = Read6Bits(bitArray, ref bitIndex);
                int playerId = GetPlayerIdFromIndex(playerIndex, roster);
                if (playerId > 0)
                {
                    ids.Add(playerId);
                }
            }

            return ids;
        }
        catch
        {
            return new List<int>();
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

    private static int Read6Bits(BitArray bitArray, ref int bitIndex)
    {
        int value = 0;
        for (int bit = 0; bit < BitsPerPlayer; bit++)
        {
            if (bitIndex < bitArray.Length && bitArray.Get(bitIndex++))
            {
                value |= (1 << bit);
            }
        }
        return value;
    }
}
