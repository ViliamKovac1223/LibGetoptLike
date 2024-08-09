namespace LibGetoptLike;

internal class GetoptLikeStateMachine
{
    // Possible states' keys
    private enum State
    {
        START_STATE,
        OTHER_STRING_STATE,
        FLAG_START_STATE,
        FLAG_STATE,
        LONG_FLAG_START_STATE,
        LONG_FLAG_SATE,
        BETWEEN_FLAG_AND_ARG_STATE,
        ARG_STATE,
        END_STATE
    }

    private string[] args;
    private string arg; // Always points to one of the args' strings
    private string flagsArg; // Keeps a string that represents last flag's argument

    private int argsIndex; // Index to loop through args
    private int index; // Index to loop through arg
    private bool isSpace; // This represents a state when we switched from arg to next arg in args

    private char lastShortFlag;
    private string lastLongFlag;
    private string otherString;
    private const char INVALID_SHORT_FLAG = ' ';
    private const char LONG_FLAG_OPTIONAL_ARG_START = '=';

    private Dictionary<string, GetoptArg> argsDictionary;
    private delegate void stateDelegate();
    private Dictionary<State, stateDelegate> states;
    private State currState;
    public List<string> otherArgs { get; }
    public List<GetoptArg> argList { get; }

    public GetoptLikeStateMachine(string[] args, Dictionary<string, GetoptArg> argsDictionary)
    {
        // Init basic variables
        this.argsIndex = 0;
        this.isSpace = false;
        this.index = -1; // It will be set to a 0 as soon as updateArg is called
        this.lastShortFlag = INVALID_SHORT_FLAG;
        this.lastLongFlag = "";
        this.otherString = "";
        // If state machine ends and this is set to true 
        // it indicates that error happened during processing

        this.args = args;
        this.arg = "";
        this.flagsArg = "";

        this.argList = new List<GetoptArg>();
        this.otherArgs = new List<string>();
        this.argsDictionary = argsDictionary;

        // Init states dictionary with all possible states
        this.states = new Dictionary<State, stateDelegate>();
        this.states.Add(State.START_STATE, start);
        this.states.Add(State.OTHER_STRING_STATE, otherStringState);
        this.states.Add(State.FLAG_START_STATE, flagStart);
        this.states.Add(State.FLAG_STATE, flag);
        this.states.Add(State.LONG_FLAG_START_STATE, longFlagStart);
        this.states.Add(State.LONG_FLAG_SATE, longFlag);
        this.states.Add(State.BETWEEN_FLAG_AND_ARG_STATE, betweenFlagAndArg);
        this.states.Add(State.ARG_STATE, argState);
        this.states.Add(State.END_STATE, end);

        currState = State.START_STATE;

        // If there are no arguments to be parsed don't start parsing
        if (this.args.Length == 0) return;

        this.arg = args.ElementAt(argsIndex);
        // Run the state machine
        while (currState != State.END_STATE)
        {
            updateArg(); // Update argument, which can set state to END_STATE, if we have no input left

            // Run current state, which takes us to the next state
            if (states.TryGetValue(currState, out stateDelegate? stateFunc))
            {
                stateFunc();
            }
        }
    }

    /// <summary>
    /// Start state of state machine.
    /// </summary>
    private void start()
    {
        // Reset last flags
        lastShortFlag = INVALID_SHORT_FLAG;
        lastLongFlag = "";

        // If we get a space in start state, we just stay in this state
        if (isSpace) { return; }

        // If we get a flag symbl ('-') we move the FLAG_START_STATE
        else if (this.arg[index] == GetoptLike.FLAG_SYMBOL)
        {
            currState = State.FLAG_START_STATE;
            updateOtherString();
            return;
        }

        // If we get a string in start state, 
        // we move into a state where we don't take a argument, neither we take a flag
        currState = State.OTHER_STRING_STATE;
        updateOtherString();
    }

    /// <summary>
    /// State that processes unwanted text,
    /// that cannot be flag, or argument
    /// </summary>
    private void otherStringState()
    {
        // Reset a lastShortFlag
        lastShortFlag = INVALID_SHORT_FLAG;

        // If we get a space, we move back into start state
        if (isSpace)
        {
            currState = State.START_STATE;
            saveOtherString();
            return;
        }
        // If we get a string, we stay in this state (no action required)
        updateOtherString();
    }

    /// <summary>
    /// State that processes start of the flag ('-')
    /// </summary>
    private void flagStart()
    {
        // Reset a lastShortFlag
        lastShortFlag = INVALID_SHORT_FLAG;

        if (isSpace)
        {
            currState = State.START_STATE;
            saveOtherString();
            return;
        }
        else if (this.arg[index] == GetoptLike.FLAG_SYMBOL)
        {
            updateOtherString();
            currState = State.LONG_FLAG_START_STATE;
            return;
        }
        // If we get a string, we go to flag state
        updateLastShortFlag();
        resetOtherString();
        currState = State.FLAG_STATE;
    }

    /// <summary>
    /// State that processes the flag input
    /// </summary>
    private void flag()
    {
        // If we get a space, we move to argState or start state,
        // depending if the last flag had required argument
        if (isSpace)
        {
            // If flag has a required argument, move the "between flag and arg state"
            GetoptArg? arg = null;
            if (argsDictionary.TryGetValue(lastShortFlag.ToString(), out arg)
                    && arg.flagType == FlagType.ArgumentRequired)
            {
                currState = State.BETWEEN_FLAG_AND_ARG_STATE;
                return;
            }
            // If flag doesn't have a required argument move back to the start state
            currState = State.START_STATE;
            return;
        }

        // If we get a string, we either stay in the same state
        // or go to the argState
        // depending on the last flag, if it has a required or optional flag
        GetoptArg? arg1 = null;
        if (argsDictionary.TryGetValue(lastShortFlag.ToString(), out arg1)
                && (arg1.flagType == FlagType.ArgumentRequired
                    || arg1.flagType == FlagType.ArgumentOptional))
        {
            this.flagsArg = "";
            // Add current processed character into the flag's argument
            updateFlagArg();

            currState = State.ARG_STATE;
            return;
        }

        updateLastShortFlag();

        // Other wise no action required as we're staying the same state
    }

    /// <summary>
    /// State that processes start of the long flag input
    /// </summary>
    private void longFlagStart()
    {
        if (isSpace)
        {
            currState = State.START_STATE;
            lastLongFlag = "";
            saveOtherString();
            return;
        }

        // If we get any string other than space, we go to the long flag state
        currState = State.LONG_FLAG_SATE;
        updateLastLongFlag();
        resetOtherString();
    }

    /// <summary>
    /// State that processes long flag input
    /// </summary>
    private void longFlag()
    {
        GetoptArg? gArg;

        if (isSpace)
        {
            // if last flag was with required argument
            // we will go into the argument state,
            // if not we will go the start state
            if (this.argsDictionary.TryGetValue(this.lastLongFlag, out gArg)
                    && gArg.flagType == FlagType.ArgumentRequired)
            {
                currState = State.ARG_STATE;
                return;
            }

            currState = State.START_STATE;
            saveLastLongFlag();
            return;
        }
        // If we got character that indicates start of the optional argument,
        // and the long flag has required/optional argument
        // we go the argument state
        else if (this.arg[index] == LONG_FLAG_OPTIONAL_ARG_START
                && argsDictionary.TryGetValue(this.lastLongFlag, out gArg)
                && (gArg.flagType == FlagType.ArgumentRequired
                    || gArg.flagType == FlagType.ArgumentOptional))
        {
            currState = State.ARG_STATE;
            return;
        }

        // If we got any other string we stay in the long flag state
        updateLastLongFlag();
    }

    /// <summary>
    /// State that processes input between flag/long_flag and argument state
    /// </summary>
    private void betweenFlagAndArg()
    {
        if (isSpace) { return; }

        // If last flag had required argument every string, including '-', 
        // will be counted as an argument
        GetoptArg? arg1 = null;
        if (argsDictionary.TryGetValue(lastShortFlag.ToString(), out arg1)
                && arg1.flagType == FlagType.ArgumentRequired)
        {
            // Reset flag's argument before saving first character there
            this.flagsArg = "";
            // Add current processed character into the flag's argument
            updateFlagArg();

            currState = State.ARG_STATE;
            return;
        }

        // If we get flag symbol ('-'),
        // and last flag didn't have required argument we move to flag start state
        if (this.arg[index] == GetoptLike.FLAG_SYMBOL)
        {
            currState = State.FLAG_START_STATE;
            return;
        }

        // If we get string, and last flag didn't have required argument
        currState = State.OTHER_STRING_STATE;
    }

    /// <summary>
    /// State that processes argument's input
    /// </summary>
    private void argState()
    {
        if (isSpace)
        {
            currState = State.START_STATE;
            saveArg();
            return;
        }

        // If we get any string, including flag symbol ('-'), will be counted as an argument
        // Add current processed character into the flag's argument
        updateFlagArg();
    }

    /// <summary>
    /// End state, this state is activated when there is no input left
    /// </summary>
    private void end()
    {
        if (!this.flagsArg.Equals(""))
        {
            saveArg();
        }
        else if (!this.lastLongFlag.Equals(""))
        {
            saveLastLongFlag();
        }
        saveOtherString();
    }

    /// <summary>
    /// Updates lastShortFlag, adds lastShortFlag to argList if appropriate
    /// </summary>
    private void updateLastShortFlag()
    {
        lastShortFlag = arg[index];
        // Reset the arg
        this.flagsArg = "";

        GetoptArg? gDicArg;
        if (argsDictionary.TryGetValue(lastShortFlag.ToString(), out gDicArg)
                &&
                !(gDicArg.flagType == FlagType.ArgumentRequired
                 || (gDicArg.flagType == FlagType.ArgumentOptional && index != arg.Length - 1)))
        {
            GetoptArg gArg = new GetoptArg(gDicArg.shortFlag, "", "", gDicArg.flagType);
            argList.Add(gArg);
        }
    }

    /// <summary>
    /// Updates lastLongFlag
    /// </summary>
    private void updateLastLongFlag()
    {
        this.lastLongFlag += arg[index];
        // Reset short flag if we're updating long flag
        this.lastShortFlag = INVALID_SHORT_FLAG;
    }

    /// <summary>
    /// Updates flagsArg
    /// </summary>
    private void updateFlagArg()
    {
        this.flagsArg += this.arg[index];
    }

    /// <summary>
    /// Saves argument into argList, if we have appropriate lastShortFlag/lastLongFlag
    /// </summary>
    private void saveArg()
    {
        GetoptArg? gDicArg;
        // Check for short/long flag, and if it's valid add this flag with argument to the list
        if (argsDictionary.TryGetValue(lastShortFlag.ToString(), out gDicArg)
                || argsDictionary.TryGetValue(lastLongFlag, out gDicArg))
        {
            GetoptArg gArg = new GetoptArg(gDicArg.shortFlag, gDicArg.longFlag, this.flagsArg, gDicArg.flagType);
            argList.Add(gArg);
        }

        // Reset the arg
        this.flagsArg = "";
    }

    /// <summary>
    /// Saves lastLongFlag into argList, if appropriate
    /// </summary>
    private void saveLastLongFlag()
    {
        GetoptArg? gArg;
        // Save flag if it is a valid flag
        if (this.argsDictionary.TryGetValue(this.lastLongFlag, out gArg))
        {
            GetoptArg newArg = new GetoptArg(gArg.shortFlag, gArg.longFlag, "", gArg.flagType);
            argList.Add(newArg);
        }
    }

    /// <summary>
    /// Update arg that points to args variable,
    /// can trigger isSpace to go to the true value
    /// </summary>
    private void updateArg()
    {
        this.isSpace = false;
        index++;
        // Go to next arg
        if (index >= arg.Length)
        {
            this.arg = nextArg();
            this.isSpace = true;
            // This can be -1 because isSpace is true, 
            // so states will ignore current position in string and will assume it is ' '.
            // And in next updateArg this will be set to the 0
            this.index = -1;

            if (currState == State.END_STATE) return;
        }
    }

    /// <summary>
    /// Return the next arg from args,
    /// and increase the argsIndex
    /// </summary>
    /// <returns>
    /// Return the next arg from args
    /// </returns>
    private string nextArg()
    {
        argsIndex++;
        if (argsIndex >= args.Length)
        {
            currState = State.END_STATE;
            return "";
        }

        return args.ElementAt(argsIndex);
    }

    /// <summary>
    /// Update otherString with currently processed character
    /// </summary>
    private void updateOtherString()
    {
        this.otherString += arg[index];
    }

    /// <summary>
    /// Reset otherString, sets to empty string
    /// </summary>
    private void resetOtherString()
    {
        this.otherString = "";
    }

    /// <summary>
    /// Save otherString into otherArgs
    /// </summary>
    private void saveOtherString()
    {
        if (!string.IsNullOrEmpty(this.otherString))
            this.otherArgs.Add(this.otherString);

        resetOtherString();
    }
}
