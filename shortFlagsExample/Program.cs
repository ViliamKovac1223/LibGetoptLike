using LibGetoptLike;

public class Program
{
    public static void Main(string[] args)
    {
        // Create a new GetoptLike object with flags in short format
        GetoptLike getopt = new GetoptLike(args, "sr:o::");

        // Get a list of processed arguments
        List<GetoptArg> gArgs = getopt.gArgs;
        // Get a list of other arguments that couldn't be processed
        List<string> otherArgs = getopt.otherArgs;

        // Go through all given flags
        Console.WriteLine("Flags:");
        foreach (GetoptArg gArg in gArgs)
        {
            switch (gArg.shortFlag)
            {
                case "s":
                    Console.WriteLine("Standalone flag (-s) has been used.");
                    break;
                case "r":
                    Console.WriteLine($"Flag with required argument (-r) has been used with argument: \"{gArg.argument}\"");
                    break;
                case "o":
                    if (!string.IsNullOrEmpty(gArg.argument))
                        Console.WriteLine($"Flag with optional argument (-o) has been used with argument: \"{gArg.argument}\"");
                    else
                        Console.WriteLine($"Flag with optional argument (-o) has been used with no argument");
                    break;
            }
        }

        // Go through all arguments that are left unprocessed
        Console.WriteLine("Other arguments:");
        foreach (string arg in otherArgs)
            Console.WriteLine(arg);

    }
}
