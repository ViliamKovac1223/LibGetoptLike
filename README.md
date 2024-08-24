# Getopt Like Library
This is C# library that aims to bring Getopt like functionality to your C#
project.

# Installation
To install this library first clone this repo and from your project add
reference to this project. We don't recommend to clone this repo straight to
you source code directory, but rather to its parent folder.

```bash
dotnet add reference ../LibGetoptLike/src/LibGetoptLike.csproj
```

# Usage
To start you will need to create ``GetoptLike`` instance and fill it with
arguments and information about supported flags. Then parsed data can be read
from ``gArgs`` and ``otherArgs``.

```C
// Create a new GetoptLike object with flags in short format
GetoptLike getopt = new GetoptLike(args, "sr:o::");
// s - standalone flag (no arguments); example input "-s"
// r - flag with required argument; example input "-r arg"
// o - flag with optional argument; example input "-ooptional"

// Get a list of processed arguments
List<GetoptArg> gArgs = getopt.gArgs;
// Get a list of other arguments that couldn't be processed
List<string> otherArgs = getopt.otherArgs;

// Go through all processed arguments in for loop
foreach (GetoptArg gArg in gArgs)
{
    switch (gArg.shortFlag)
    {
    ...
    }
}
```

To run your project you can either run binary directly (after building the project) or use dotnet command.

```bash
./bin/Debug/net8.0/shortFlagsExample -r required
```
or
```bash
dotnet run -- -r required
```

For more complex examples please check out ``longFlagsExample`` and
``shortFlagsExample`` projects included in this repo.


# Fake internet money
If you like my work and want to support me by some fake internet money, here is my monero address.

8AW9BM1E5d67kaX3SiAT6B91Xvn4urhBeGL3FUWezSkRarSmxrAfvUK5XD5VcasXStHT6aYXwjVMrhm4YCNXTqGpRUekQ6i
