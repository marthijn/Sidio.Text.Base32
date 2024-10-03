namespace Sidio.Text.Base32;

/// <summary>
/// The base32 encoding class.
/// </summary>
public static partial class Base32Encoding
{
    private const char UnitSeparator = (char)0x1F;
    private const char MaxByte = (char)0xFF;

    private static readonly char[] Base32Table =
    [
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O',
        'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '2', '3', '4', '5', '6', '7', '='
    ];

    private static readonly int[] Base32DecodeMap = CreateDecodeMap(Base32Table);

    /// <summary>
    /// Converts a base32 string to a byte array.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>A <see cref="byte"/> array.</returns>
    public static byte[] FromString(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        // remove padding '=' characters from the end of the input
        return input.Length == 0 ? [] : FromString(input.AsSpan().TrimEnd('='), Base32DecodeMap);
    }

    /// <summary>
    /// Converts a byte array to a base32 string.
    /// </summary>
    /// <param name="inArray">The input array.</param>
    /// <returns>A base32 <see cref="string"/>.</returns>
    public static string ToString(byte[] inArray)
    {
        ArgumentNullException.ThrowIfNull(inArray);
        return inArray.Length == 0 ? string.Empty : ToString(new ReadOnlySpan<byte>(inArray), Base32Table);
    }

    private static byte[] FromString(ReadOnlySpan<char> inputSpan, int[] decodeMap)
    {
        // calculate the expected output byte array length
        // each 8 characters of Base32 results in 5 bytes
        var outputLength = (inputSpan.Length * 5) / 8;

        // stack allocation for small arrays
        Span<byte> outputSpan = stackalloc byte[outputLength];

        var bitBuffer = 0;
        var bitCount = 0;
        var outputIndex = 0;

        // process each character in the Base32 input
        foreach (var c in inputSpan)
        {
            // get the index value of the character from the Base32 alphabet
            var index = decodeMap[c];
            if (index == -1)
            {
                throw new ArgumentException($"Invalid character '{c}' in Base32 string.");
            }

            // shift the buffer to the left by 5 bits and add the index value
            bitBuffer = (bitBuffer << 5) | index;
            bitCount += 5;

            // extract 8 bits (1 byte) whenever we have enough bits (>= 8)
            if (bitCount < 8)
            {
                continue;
            }

            outputSpan[outputIndex++] = (byte)((bitBuffer >> (bitCount - 8)) & MaxByte);
            bitCount -= 8;
        }

        // convert the span to a byte array and return it
        return outputSpan.ToArray();
    }

    private static string ToString(ReadOnlySpan<byte> inArray, char[] base32Table)
    {
        // calculate the length of the output: Base32 encoding is 8/5 times the size of the input
        var outputLength = ((inArray.Length * 8) + 4) / 5;

        // output length should be padded to a multiple of 8
        var paddingLength = (outputLength + 7) / 8 * 8;

        // use stackalloc for small memory allocation
        Span<char> outputSpan = stackalloc char[paddingLength];

        // declare bit buffer and bit count
        var bitBuffer = 0;
        var bitCount = 0;
        var outputIndex = 0;

        // process each byte in inputSpan
        foreach (var b in inArray)
        {
            // shift the buffer left by 8 bits and add the byte value to the buffer
            bitBuffer = (bitBuffer << 8) | b;
            bitCount += 8;

            // while we have 5 or more bits in the buffer, process a Base32 character
            while (bitCount >= 5)
            {
                // extract the top 5 bits from the buffer
                var index = (bitBuffer >> (bitCount - 5)) & UnitSeparator;
                outputSpan[outputIndex++] = base32Table[index];
                bitCount -= 5;
            }
        }

        // if there are leftover bits (less than 5), pad with zeros
        if (bitCount > 0)
        {
            var index = (bitBuffer << (5 - bitCount)) & UnitSeparator;
            outputSpan[outputIndex++] = base32Table[index];
        }

        // add padding '=' to make the result length a multiple of 8
        while (outputIndex % 8 != 0)
        {
            outputSpan[outputIndex++] = '=';
        }

        // return the result as a string
        return new string(outputSpan[..outputIndex]);
    }
    
    private static int[] CreateDecodeMap(char[] table)
    {
        var map = new int[256];

        for(var i = map.Length; i-- > 0;)
        {
            // initialize with -1 (invalid character)
            map[i] = -1;
        }

        for(var i = table.Length; i-- > 0;)
        {
            map[table[i]] = i;
        }

        return map;
    }
}