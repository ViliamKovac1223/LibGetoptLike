namespace LibGetoptLike;

using System.Text.RegularExpressions;

public class GetoptLike
{
    internal const char FLAG_SYMBOL = '-';
    private const string SHORT_OPS_ARG_SYMBOL = ":";
    private const string SHORT_OPS_REGEX_PATTERN =
        $"'^\\(\\([a-zA-Z]\\)\\"
        + $"|\\([a-zA-Z]{SHORT_OPS_ARG_SYMBOL}\\)\\"
        + $"|\\([a-zA-Z]{SHORT_OPS_ARG_SYMBOL}{SHORT_OPS_ARG_SYMBOL}\\)\\)*$'";

    /// <summary>
    /// shortOps, short option
    /// format: "xy:z::"
    /// Where x is a flag without argument;
    /// y is a flag with mandatory argument;
    /// z is a flag with optional argument;
    /// </summary>
    private string? shortOps;
    private string[] args;
    public Dictionary<string, GetoptArg> argsDictionary { get; private set; }
    public List<GetoptArg> gArgs { get; private set; }

    public GetoptLike(string[] args)
    {
        this.args = args;
        this.argsDictionary = new Dictionary<string, GetoptArg>();
        this.gArgs = new List<GetoptArg>();
    }

    public GetoptLike(string[] args, string shortOps)
        : this(args)
    {
        this.shortOps = shortOps;

        processShortOps();
    }

    private void processShortOps()
    {
        if (string.IsNullOrEmpty(shortOps)) return;

        // Fill argsDictionary with all flags from shortOps
        for (int i = 0; i < shortOps.Length; i++)
        {
            if (shortOps.ElementAt(i) == SHORT_OPS_ARG_SYMBOL.ElementAt(0)) continue;
            GetoptArg gArg = getNewGetopArg(i);
            argsDictionary.Add(gArg.shortFlag, gArg);
        }

        // Fill argsDictionary with all flags from LongOps TODO:

        GetoptLikeStateMachine machine = new GetoptLikeStateMachine(args, argsDictionary);
        gArgs = machine.argList;
    }

    /// <summary>
    /// Takes index for shortOps and process it into a new GetoptArg
    /// </summary>
    /// <param name="indexForShortOps">
    /// Indicates where to start processing new GetoptArg from shortOps
    /// </param name="indexForShortOps">
    /// <returns>
    /// returns a new GetoptArg
    /// </returns>
    private GetoptArg getNewGetopArg(int indexForShortOps)
    {
        if (string.IsNullOrEmpty(shortOps)
                || !(indexForShortOps < shortOps.Length)
                || shortOps.ElementAt(indexForShortOps) == SHORT_OPS_ARG_SYMBOL.ElementAt(0))
        {
            // TODO: throw exception
        }

        string flagKey = shortOps.ElementAt(indexForShortOps).ToString();
        FlagType flagType = FlagType.NoArgument;

        // Check if flag has an optional argument (x::)
        if (indexForShortOps + 2 < shortOps.Length
                && shortOps.ElementAt(indexForShortOps + 1) == SHORT_OPS_ARG_SYMBOL.ElementAt(0)
                && shortOps.ElementAt(indexForShortOps + 2) == SHORT_OPS_ARG_SYMBOL.ElementAt(0))
        {
            flagType = FlagType.ArgumentOptional;
        }
        // Check if flag has an required argument (x:)
        else if (indexForShortOps + 1 < shortOps.Length
                && shortOps.ElementAt(indexForShortOps + 1) == SHORT_OPS_ARG_SYMBOL.ElementAt(0))
        {
            flagType = FlagType.ArgumentRequired;
        }

        GetoptArg gArg = new GetoptArg(flagKey, "", flagType);
        return gArg;
    }

    /// <summary>
    /// Checks if shortOps are valid
    /// </summary>
    /// <returns>
    /// Returns true is shortOps are valid, returns false otherwise
    /// </returns>
    private bool checkShortOps()
    {
        if (string.IsNullOrEmpty(this.shortOps)) return false;

        bool matchRegexPattern = Regex.IsMatch(this.shortOps, SHORT_OPS_REGEX_PATTERN);
        return matchRegexPattern && !repeatingLettersInShortOps();

    }

    /// <summary>
    /// Checks if shortOps has any repeating letters (flags) in it
    /// </summary>
    /// <returns>
    /// return true if has no repeating letters, false otherwise
    /// </returns>
    private bool repeatingLettersInShortOps()
    {
        if (string.IsNullOrEmpty(this.shortOps)) return false;

        HashSet<char> uniqLetters = new HashSet<char>();

        // Go through all letters in shortOps and add them to the HashSet,
        // if there is any repeating letter in the set return false
        foreach (char letter in this.shortOps)
        {
            if (letter == SHORT_OPS_ARG_SYMBOL.ElementAt(0)) continue;

            if (!uniqLetters.Contains(letter))
                uniqLetters.Add(letter);
            else
                return true;
        }

        return false;
    }

}
