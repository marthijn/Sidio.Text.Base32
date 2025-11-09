namespace Sidio.Text.Base32;

/// <summary>
/// The base32 encoding class.
/// </summary>
public static partial class Base32
{
    private const char UnitSeparator = (char)0x1F;

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
    public static byte[] Decode(string input)
    {
        ArgumentNullException.ThrowIfNull(input);
        return input.Length == 0 ? [] : Decode(input.AsSpan(), Base32DecodeMap);
    }

    /// <summary>
    /// Converts a byte array to a base32 string.
    /// </summary>
    /// <param name="inArray">The input array.</param>
    /// <returns>A base32 <see cref="string"/>.</returns>
    public static string Encode(byte[] inArray)
    {
        ArgumentNullException.ThrowIfNull(inArray);
        return inArray.Length == 0 ? string.Empty : Encode(new ReadOnlySpan<byte>(inArray), Base32Table);
    }

    private static byte[] Decode(ReadOnlySpan<char> inputSpan, int[] decodeMap)
    {
        // manually find the end position without padding '=' characters
        var inputLength = inputSpan.Length;
        while (inputLength > 0 && inputSpan[inputLength - 1] == '=')
        {
            inputLength--;
        }

        // calculate the expected output byte array length
        // each 8 characters of base32 results in 5 bytes
        var outputLength = (inputLength * 5) / 8;

        // use stack allocation for small arrays (<=256 bytes), otherwise heap
        Span<byte> outputSpan = outputLength <= 256
            ? stackalloc byte[outputLength]
            : new byte[outputLength];

        var bitBuffer = 0;
        var bitCount = 0;
        var outputIndex = 0;

        // process each character in the Base32 input
        for (var i = 0; i < inputLength; i++)
        {
            var c = inputSpan[i];

            // bounds check before array access
            if (c > 255)
            {
                throw new ArgumentException("Invalid character in Base32 string.");
            }

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
            if (bitCount >= 8)
            {
                outputSpan[outputIndex++] = (byte)(bitBuffer >> (bitCount - 8));
                bitCount -= 8;
            }
        }

        // convert the span to a byte array and return it
        return outputSpan.ToArray();
    }

    private static string Encode(ReadOnlySpan<byte> inArray, char[] base32Table)
    {
        // calculate the length of the output: Base32 encoding is 8/5 times the size of the input
        var outputLength = ((inArray.Length * 8) + 4) / 5;

        // output length should be padded to a multiple of 8
        var paddingLength = (outputLength + 7) & ~7; // faster than / 8 * 8

        // use stack allocation for small arrays (<=256 bytes), otherwise heap
        Span<char> outputSpan = paddingLength <= 256
            ? stackalloc char[paddingLength]
            : new char[paddingLength];

        // declare bit buffer and bit count
        var bitBuffer = 0;
        var bitCount = 0;
        var outputIndex = 0;

        // process each byte in inArray
        for (var i = 0; i < inArray.Length; i++)
        {
            // shift the buffer left by 8 bits and add the byte value to the buffer
            bitBuffer = (bitBuffer << 8) | inArray[i];
            bitCount += 8;

            // while we have 5 or more bits in the buffer, process a base32 character
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
        while (outputIndex < paddingLength)
        {
            outputSpan[outputIndex++] = '=';
        }

        // return the result as a string
        return new string(outputSpan[..paddingLength]);
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