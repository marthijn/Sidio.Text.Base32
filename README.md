# Sidio.Text.Base32
A simple .NET package that converts a byte array to a [Base32](https://en.wikipedia.org/wiki/Base32) string and vice versa. Because the latest framework
features are used, .NET 8.0 or higher is required.

[![build](https://github.com/marthijn/Sidio.Text.Base32/actions/workflows/build.yml/badge.svg)](https://github.com/marthijn/Sidio.Text.Base32/actions/workflows/build.yml)
[![NuGet Version](https://img.shields.io/nuget/v/Sidio.Text.Base32)](https://www.nuget.org/packages/Sidio.Text.Base32/)
[![Coverage Status](https://coveralls.io/repos/github/marthijn/Sidio.Text.Base32/badge.svg?branch=main)](https://coveralls.io/github/marthijn/Sidio.Text.Base32?branch=main)

# Usage
## Encode
```csharp
var myString = "foobar";
var bytes = Encoding.UTF8.GetBytes(myString);
var base32 = Base32.Encode(bytes);
```

## Decode
```csharp
var base32 = "MZXW6YTBOI======";
var bytes = Base32.Decode(base32);
var myString = Encoding.UTF8.GetString(bytes);
```

## Encode hex
```csharp
var myString = "foobar";
var bytes = Encoding.UTF8.GetBytes(myString);
var base32 = Base32.EncodeHex(bytes);
```

## Decode hex
```csharp
var base32 = "CPNMUOJ1E8======";
var bytes = Base32.DecodeHex(base32);
var myString = Encoding.UTF8.GetString(bytes);
```

# References
* [RFC 4648](https://datatracker.ietf.org/doc/html/rfc4648)
