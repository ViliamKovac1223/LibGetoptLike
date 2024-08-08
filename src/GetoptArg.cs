namespace LibGetoptLike;

public enum FlagType
{
    NoArgument,
    ArgumentRequired,
    ArgumentOptional
}

public class GetoptArg
{
    public string shortFlag { get; }
    public string longFlag { get; }
    public FlagType flagType { get; }
    public string? argument { get; }

    public GetoptArg(string shortFlag, FlagType flagType) {
        this.shortFlag = shortFlag;
        this.flagType = flagType;
        this.longFlag = "";
    }

    public GetoptArg(string shortFlag, string longFlag, FlagType flagType)
        : this(shortFlag, flagType)
    {
        this.longFlag = longFlag;
    }

    public GetoptArg(string shortFlag, string longFlag, string argument, FlagType flagType)
       : this(shortFlag, longFlag, flagType)
    {
        this.argument = argument;
    }
}
