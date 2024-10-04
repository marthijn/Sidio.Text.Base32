using System.Collections;
using System.Text;

namespace Sidio.Text.Base32.Tests;

public partial class Base32Tests
{
    [Fact]
    public void DecodeHex_WithNull_ThrowsException()
    {
        // arrange
        string? input = null;

        // act
        var action = () => Base32.DecodeHex(input!);

        // assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [ClassData(typeof(Base32HexTestVectors))]
    public void DecodeHex_ReturnsByteArray(string input, string base32)
    {
        // act
        var result = Base32.DecodeHex(base32);

        // assert
        result.Should().NotBeNull();
        var stringResult = Encoding.UTF8.GetString(result);
        stringResult.Should().BeEquivalentTo(input);
    }
    
    [Fact]
    public void EncodeHex_WithNull_ThrowsException()
    {
        // arrange
        byte[]? input = null;

        // act
        var action = () => Base32.EncodeHex(input!);

        // assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [ClassData(typeof(Base32HexTestVectors))]
    public void EncodeHex_ReturnsBase32HexString(string input, string base32)
    {
        // arrange
        var bytes = Encoding.UTF8.GetBytes(input);

        // act
        var result = Base32.EncodeHex(bytes);

        // assert
        result.Should().NotBeNull();
        result.Should().Be(base32);
    }

    private sealed class Base32HexTestVectors : IEnumerable<object[]>
    {
        private readonly List<object?[]> _data = [];

        public Base32HexTestVectors()
        {
            // RFC test vectors https://datatracker.ietf.org/doc/html/rfc4648#page-8
            _data.Add(["", ""]);
            _data.Add(["f", "CO======"]);
            _data.Add(["fo", "CPNG===="]);
            _data.Add(["foo", "CPNMU==="]);
            _data.Add(["foob", "CPNMUOG="]);
            _data.Add(["fooba", "CPNMUOJ1"]);
            _data.Add(["foobar", "CPNMUOJ1E8======"]);
        }

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}