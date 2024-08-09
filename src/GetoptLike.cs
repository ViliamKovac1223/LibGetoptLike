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
    private GetoptArg[] longOps;
    private string[] args;

    public Dictionary<string, GetoptArg> argsDictionary { get; private set; }
    public List<GetoptArg> gArgs { get; private set; }
    public List<string> otherArgs { get; private set; }

    private GetoptLike(string[] args)
    {
        this.args = args;
        this.argsDictionary = new Dictionary<string, GetoptArg>();
        this.longOps = new GetoptArg[1];
        this.gArgs = new List<GetoptArg>();
        this.otherArgs = new List<string>();
    }

    public GetoptLike(string[] args, string shortOps)
        : this(args)
    {
        this.shortOps = shortOps;
        processShortOps();
    }

    public GetoptLike(string[] args, GetoptArg[] longOps)
        : this(args)
    {
        this.longOps = longOps;
        processLongOps();
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

        GetoptLikeStateMachine machine = new GetoptLikeStateMachine(args, argsDictionary);
        gArgs = machine.argList;
        otherArgs = machine.otherArgs;
    }

    private void processLongOps()
    {
        // Fill argsDictionary with all flags from shortOps
        foreach (GetoptArg gArg in longOps)
        {
            // Add shortFlag into dictionaryArg
            if (!string.IsNullOrEmpty(gArg.shortFlag))
            {
                GetoptArg dictionaryArg = new GetoptArg(gArg.shortFlag, "", "", gArg.flagType);
                argsDictionary.Add(gArg.shortFlag, gArg);
            }

            // Add longFlag into dictionaryArg
            if (!string.IsNullOrEmpty(gArg.longFlag))
            {
                GetoptArg dictionaryArg = new GetoptArg("", gArg.longFlag, "", gArg.flagType);
                argsDictionary.Add(gArg.longFlag, gArg);
            }
        }

        GetoptLikeStateMachine machine = new GetoptLikeStateMachine(args, argsDictionary);
        gArgs = machine.argList;
        otherArgs = machine.otherArgs;
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
            throw new GetoptException("Couldn't get new getopt argument because either shortOps is null or its format is wrong");
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

        GetoptArg gArg = new GetoptArg(flagKey, "", "", flagType);
        return gArg;
    }

    /// <summary>
    /// Check if longOps are valid
    /// </summary>
    /// <returns>
    /// Returns true if longOps are valid, otherwise returns false
    /// </returns>
    private bool checkLongOps()
    {
        bool shortFlagsOkay = checkForShortFlagsInLongsOps();
        bool longFlagsOkay = checkForRepeatingLongOps();

        // Check if every arg has at least one flag (short or long)
        foreach (GetoptArg gArg in longOps)
        {
            if (string.IsNullOrEmpty(gArg.shortFlag)
                    && string.IsNullOrEmpty(gArg.longFlag))
                return false;
        }

        return shortFlagsOkay && longFlagsOkay;
    }

    /// <summary>
    /// Check if there are any repeated long flags in longOps array
    /// </summary>
    /// <returns>
    /// Returns true if long flags are valid, otherwise returns false
    /// </returns>
    private bool checkForRepeatingLongOps()
    {
        HashSet<string> uniqFlags = new HashSet<string>();

        // Go through all flags in longFlagsOkay and add them to the HashSet,
        // if there is any repeating long-flag in the set, we return false
        foreach (GetoptArg gArg in longOps)
        {
            if (string.IsNullOrEmpty(gArg.longFlag)) continue;

            if (!uniqFlags.Contains(gArg.longFlag))
                uniqFlags.Add(gArg.longFlag);
            else
                return false;
        }

        return true;
    }

    /// <summary>
    /// Check if there are any repeated short flags in longOps array
    /// </summary>
    /// <returns>
    /// Returns true if short flags in longOps are valid, otherwise returns false
    /// </returns>
    private bool checkForShortFlagsInLongsOps()
    {
        // Make a string with all shortFlags
        string shortOpsToCheck = "";
        foreach (GetoptArg gArg in longOps)
        {
            if (!string.IsNullOrEmpty(gArg.shortFlag))
                shortOpsToCheck += gArg.shortFlag;

            if (gArg.shortFlag.Length != 1)
            {
                throw new GetoptException("Wrong format of longOps, shortFlag defined in longOps must be one character long");
            }
        }

        return !repeatingLettersInShortOps(shortOpsToCheck);
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
        return matchRegexPattern && !repeatingLettersInShortOps(shortOps);
    }

    /// <summary>
    /// Checks if shortOps has any repeating letters (flags) in it
    /// </summary>
    /// <param name="shortOpsToCheck">
    /// String that contains shortOps that will be checked
    /// </param>
    /// <returns>
    /// return true if has repeating letters, false otherwise
    /// </returns>
    private bool repeatingLettersInShortOps(string shortOpsToCheck)
    {
        if (string.IsNullOrEmpty(shortOpsToCheck)) return false;

        HashSet<char> uniqLetters = new HashSet<char>();

        // Go through all letters in shortOpsToCheck and add them to the HashSet,
        // if there is any repeating letter in the set, we return true
        foreach (char letter in shortOpsToCheck)
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
