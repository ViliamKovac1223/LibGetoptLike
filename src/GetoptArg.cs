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
    public string? longFlag { get; }
    public FlagType flagType { get; }
    public string? argument { get; }

    public GetoptArg(string flag)
    {
        this.shortFlag = flag;
        this.longFlag = null;
        this.argument = null;
        this.flagType = FlagType.NoArgument;
    }

    public GetoptArg(string shortFlag, string argument)
        : this(shortFlag)
    {
        this.argument = argument;
    }

    public GetoptArg(string shortFlag, string argument, FlagType flagType)
        : this(shortFlag, argument)
    {
        this.flagType = flagType;
    }

    public GetoptArg(string shortFlag, string longFlag, string argument, FlagType flagType)
        : this(shortFlag, argument, flagType)
    {
        this.longFlag = longFlag;
    }
}
