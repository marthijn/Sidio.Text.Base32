using System.Collections;
using System.Text;
using FluentAssertions;

namespace Sidio.Text.Base32.Tests;

public partial class Base32EncodingTests
{
    [Theory]
    [ClassData(typeof(Base32TestVectors))]
    public void FromString_ReturnsByteArray(string input, string base32)
    {
        // act
        var result = Base32Encoding.FromString(base32);

        // assert
        result.Should().NotBeNull();
        var stringResult = Encoding.UTF8.GetString(result);
        stringResult.Should().BeEquivalentTo(input);
    }

    [Theory]
    [ClassData(typeof(Base32TestVectors))]
    public void ToString_ReturnsBase32String(string input, string base32)
    {
        // arrange
        var bytes = Encoding.UTF8.GetBytes(input);

        // act
        var result = Base32Encoding.ToString(bytes);

        // assert
        result.Should().NotBeNull();
        result.Should().Be(base32);
    }

    private sealed class Base32TestVectors : IEnumerable<object[]>
    {
        private readonly List<object?[]> _data = [];

        public Base32TestVectors()
        {
            // RFC test vectors https://datatracker.ietf.org/doc/html/rfc4648#page-8
            _data.Add(["", ""]);
            _data.Add(["f", "MY======"]);
            _data.Add(["fo", "MZXQ===="]);
            _data.Add(["foo", "MZXW6==="]);
            _data.Add(["foob", "MZXW6YQ="]);
            _data.Add(["fooba", "MZXW6YTB"]);
            _data.Add(["foobar", "MZXW6YTBOI======"]);

            // additional test vectors
            _data.Add(["hello, world!", "NBSWY3DPFQQHO33SNRSCC==="]);
        }

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}