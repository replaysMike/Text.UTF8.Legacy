# Text.UTF8.Legacy

[![nuget](https://img.shields.io/nuget/v/Text.UTF8.Legacy.svg)](https://www.nuget.org/packages/Text.UTF8.Legacy/)
[![nuget](https://img.shields.io/nuget/dt/Text.UTF8.Legacy.svg)](https://www.nuget.org/packages/Text.UTF8.Legacy/)
[![Build status](https://ci.appveyor.com/api/projects/status/gfwjabg1pta7em94?svg=true)](https://ci.appveyor.com/project/MichaelBrown/Text.UTF8.Legacy)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/8001bb10a20c4456a98ed4dde145350a)](https://app.codacy.com/app/replaysMike/Text.UTF8.Legacy?utm_source=github.com&utm_medium=referral&utm_content=replaysMike/Text.UTF8.Legacy&utm_campaign=Badge_Grade_Dashboard)
[![Codacy Badge](https://api.codacy.com/project/badge/Coverage/85f671af543f46a599cafd10dab36e5a)](https://www.codacy.com/app/replaysMike/Text.UTF8.Legacy?utm_source=github.com&utm_medium=referral&utm_content=replaysMike/Text.UTF8.Legacy&utm_campaign=Badge_Coverage)

A package replacement for .Net Standard's UTF8 string encoding/decoding implementation.

## Description

As of .Net Core 3.0+ a bug was fixed in Microsoft's UTF8 implementation that changes the behavior of UTF8 string encoding and decoding. This package offers the legacy behavior that is still present in .Net Framework today for maximum compatibility when using .Net Core 3.0+.

## Installation
Install Text.UTF8.Legacy from the Package Manager Console:
```
PM> Install-Package Text.UTF8.Legacy
```

## Usage

UTF8 String decoding
```csharp
using Text.UTF8.Legacy;

var bytes = // byte representation of a UTF8 string
var utf8String = Text.UTF8.Legacy.Encoding.UTF8.GetString(bytes);
```

UTF8 String encoding
```csharp
using Text.UTF8.Legacy;

var string = "A test string";
var bytes = Text.UTF8.Legacy.Encoding.UTF8.GetBytes(string);
```

## Compatibility

[See .NET Core 3.0 follows Unicode best practices when replacing ill-formed UTF-8 byte sequences](https://docs.microsoft.com/en-us/dotnet/core/compatibility/corefx#net-core-30-follows-unicode-best-practices-when-replacing-ill-formed-utf-8-byte-sequences)
[Issue Report](https://github.com/dotnet/standard/issues/1679)
