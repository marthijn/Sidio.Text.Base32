﻿namespace Sidio.Text.Base32;

public static partial class Base32
{
    private static readonly char[] Base32HexTable =
    [
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F',
        'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V'
    ];

    private static readonly int[] Base32HexDecodeMap = CreateDecodeMap(Base32HexTable);

    /// <summary>
    /// Converts a base32 hex string to a byte array.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>A <see cref="byte"/> array.</returns>
    public static byte[] DecodeHex(string input)
    {
        ArgumentNullException.ThrowIfNull(input);
        return input.Length == 0 ? [] : Decode(input.AsSpan(), Base32HexDecodeMap);
    }

    /// <summary>
    /// Converts a byte array to a base32 hex string.
    /// </summary>
    /// <param name="inArray">The input array.</param>
    /// <returns>A base32 <see cref="string"/>.</returns>
    public static string EncodeHex(byte[] inArray)
    {
        ArgumentNullException.ThrowIfNull(inArray);
        return inArray.Length == 0 ? string.Empty : Encode(new ReadOnlySpan<byte>(inArray), Base32HexTable);
    }
}