using LibGetoptLike;

public class Program
{
    public static void Main(string[] args)
    {

        // Create a new GetoptLike object with flags in short format
        GetoptLike getopt = new GetoptLike(args,
                new GetoptArg[] {
                    //            shortFlag longFlag        FlagType
                    new GetoptArg("s",      "standalone",   FlagType.NoArgument),
                    new GetoptArg("r",      "required",     FlagType.ArgumentRequired),
                    new GetoptArg("",       "optional",     FlagType.ArgumentOptional)
                }
                );

        // Get a list of processed arguments
        List<GetoptArg> gArgs = getopt.gArgs;
        // Get a list of other arguments that couldn't be processed
        List<string> otherArgs = getopt.otherArgs;

        // Go through all given flags
        Console.WriteLine("Flags:");
        foreach (GetoptArg gArg in gArgs)
        {
            switch (gArg.longFlag)
            {
                case "standalone":
                    Console.WriteLine("Standalone flag (--standalone) has been used.");
                    break;
                case "required":
                    Console.WriteLine($"Flag with required argument (--required) has been used with argument: \"{gArg.argument}\"");
                    break;
                case "optional":
                    if (!string.IsNullOrEmpty(gArg.argument))
                        Console.WriteLine($"Flag with optional argument (--optional) has been used with argument: \"{gArg.argument}\"");
                    else
                        Console.WriteLine($"Flag with optional argument (--optional) has been used with no argument");
                    break;
            }
        }

        // Go through all arguments that are left unprocessed
        Console.WriteLine("Other arguments:");
        foreach (string arg in otherArgs)
            Console.WriteLine(arg);

    }
}
