namespace LibGetoptLikeTests;
using Xunit;
using LibGetoptLike;

public class FlagTests
{
    public struct GetoptLikeTestAnswer
    {
        public GetoptLikeTestAnswer(string shortFlag, string longFlag, string answer)
        {
            this.shortFlag = shortFlag;
            this.longFlag = longFlag;
            this.answer = answer;
        }

        public string shortFlag;
        public string longFlag;
        public string answer;
    }

    public static IEnumerable<object[]> dataForShortFlagTests =>
        new List<object[]>
        {
            // Short flag tests
            new object[] {
                "ab:c::", // shortFlags
                new GetoptArg[] {}, // getoptArgs
                new string[] { "-a", "-barg" }, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("a", "", ""),
                    new GetoptLikeTestAnswer("b", "", "arg")
                },
                new string[] {}, // Other strings
            },
            new object[] {
                "ab:c::", // shortFlags
                new GetoptArg[] {}, // getoptArgs
                new string[] { "-a", "-b", "arg" }, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("a", "", ""),
                    new GetoptLikeTestAnswer("b", "", "arg")
                },
                new string[] {}, // Other strings
            },
            new object[] {
                "ab:c::", // shortFlags
                new GetoptArg[] {}, // getoptArgs
                new string[] { "-carg" }, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("c", "", "arg")
                },
                new string[] {}, // Other strings
            },
            new object[] {
                "ab:c::", // shortFlags
                new GetoptArg[] {}, // getoptArgs
                new string[] { "-acopt", "-abreq", "-ab", "req1" }, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("a", "", ""),
                    new GetoptLikeTestAnswer("c", "", "opt"),
                    new GetoptLikeTestAnswer("a", "", ""),
                    new GetoptLikeTestAnswer("b", "", "req"),
                    new GetoptLikeTestAnswer("a", "", ""),
                    new GetoptLikeTestAnswer("b", "", "req1"),
                },
                new string[] {}, // Other strings
            },

            // Long flag tests
            new object[] {
                "", // shortFlags
                new GetoptArg[] { // getoptArgs
                    new GetoptArg("", "hello", "", FlagType.NoArgument)
                },
                new string[] { "--hello" }, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("", "hello", ""),
                },
                new string[] {}, // Other strings
            },
            new object[] {
                "", // shortFlags
                new GetoptArg[] { // getoptArgs
                    new GetoptArg("", "hello", "", FlagType.ArgumentRequired)
                },
                new string[] { "--hello=abc" }, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("", "hello", "abc"),
                },
                new string[] {}, // Other strings
            },
            new object[] {
                "", // shortFlags
                new GetoptArg[] { // getoptArgs
                    new GetoptArg("", "hello", "", FlagType.ArgumentOptional)
                },
                new string[] { "--hello=abc" }, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("", "hello", "abc"),
                },
                new string[] {}, // Other strings
            },
            new object[] {
                "", // shortFlags
                new GetoptArg[] { // getoptArgs
                    new GetoptArg("", "hello", "", FlagType.ArgumentRequired)
                },
                new string[] { "--hello", "abc" }, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("", "hello", "abc"),
                },
                new string[] {}, // Other strings
            },
            new object[] {
                "", // shortFlags
                new GetoptArg[] { // getoptArgs
                    new GetoptArg("h", "hello", "", FlagType.NoArgument)
                },
                new string[] { "--hello" }, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("", "hello", ""),
                },
                new string[] {}, // Other strings
            },
            new object[] {
                "", // shortFlags
                new GetoptArg[] { // getoptArgs
                    new GetoptArg("h", "hello", "", FlagType.ArgumentRequired)
                },
                new string[] { "--hello", "abc" }, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("", "hello", "abc"),
                },
                new string[] {}, // Other strings
            },
            new object[] {
                "", // shortFlags
                new GetoptArg[] { // getoptArgs
                    new GetoptArg("h", "hello", "", FlagType.ArgumentRequired)
                },
                new string[] { "-h", "abc"}, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("h", "", "abc"),
                },
                new string[] {}, // Other strings
            },
            new object[] {
                "", // shortFlags
                new GetoptArg[] { // getoptArgs
                    new GetoptArg("h", "hello", "", FlagType.ArgumentRequired)
                },
                new string[] { "-h=abc"}, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("h", "", "=abc"),
                },
                new string[] {}, // Other strings
            },
            new object[] {
                "", // shortFlags
                new GetoptArg[] { // getoptArgs
                    new GetoptArg("h", "hello", "", FlagType.ArgumentRequired),
                    new GetoptArg("a", "", "", FlagType.NoArgument)
                },
                new string[] {"-a", "-h=abc", "-a", "--hello", "abc"}, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("a", "", ""),
                    new GetoptLikeTestAnswer("h", "", "=abc"),
                    new GetoptLikeTestAnswer("a", "", ""),
                    new GetoptLikeTestAnswer("",  "hello", "abc"),
                },
                new string[] {}, // Other strings
            },
            new object[] {
                "", // shortFlags
                new GetoptArg[] { // getoptArgs
                    new GetoptArg("h", "hello", "", FlagType.ArgumentRequired),
                    new GetoptArg("a", "", "", FlagType.NoArgument),
                    new GetoptArg("", "opt", "", FlagType.ArgumentOptional)
                },
                new string[] {"-a", "-h=abc", "-a", "--hello", "abc", "--opt=abc"}, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("a", "", ""),
                    new GetoptLikeTestAnswer("h", "", "=abc"),
                    new GetoptLikeTestAnswer("a", "", ""),
                    new GetoptLikeTestAnswer("",  "hello", "abc"),
                    new GetoptLikeTestAnswer("",  "opt", "abc"),
                },
                new string[] {}, // Other strings
            },
            new object[] {
                "", // shortFlags
                new GetoptArg[] { // getoptArgs
                    new GetoptArg("h", "hello", "", FlagType.ArgumentRequired),
                    new GetoptArg("a", "aa", "", FlagType.NoArgument),
                    new GetoptArg("", "opt", "", FlagType.ArgumentOptional)
                },
                new string[] {"-a"}, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("a",  "aa", ""),
                },
                new string[] {}, // Other strings
            },

            // Other string tests
            new object[] {
                "", // shortFlags
                new GetoptArg[] { // getoptArgs
                    new GetoptArg("h", "hello", "", FlagType.NoArgument)
                },
                new string[] { "-h", "cow" }, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("", "hello", ""),
                },
                new string[] {"cow"}, // Other strings
            },
            new object[] {
                "", // shortFlags
                new GetoptArg[] { // getoptArgs
                    new GetoptArg("h", "hello", "", FlagType.NoArgument)
                },
                new string[] { "hello", "-h", "cow" }, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("", "hello", ""),
                },
                new string[] {"hello" ,"cow"}, // Other strings
            },
            new object[] {
                "", // shortFlags
                new GetoptArg[] { // getoptArgs
                    new GetoptArg("h", "hello", "", FlagType.NoArgument)
                },
                new string[] { "-" }, // args
                new GetoptLikeTestAnswer[] { // answers
                },
                new string[] { "-" }, // Other strings
            },
            new object[] {
                "", // shortFlags
                new GetoptArg[] { // getoptArgs
                    new GetoptArg("h", "hello", "", FlagType.NoArgument)
                },
                new string[] { "--" }, // args
                new GetoptLikeTestAnswer[] { // answers
                },
                new string[] { "--" }, // Other strings
            },
            new object[] {
                "ab:c::", // shortFlags
                new GetoptArg[] { // getoptArgs
                },
                new string[] { "-a", "hello", "cow"}, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("a", "", ""),
                },
                new string[] { "hello", "cow" }, // Other strings
            },
        };

    [Theory]
    [MemberData(nameof(dataForShortFlagTests))]
    public void LongFlagTests(string shortFlags, GetoptArg[] getoptArgs, string[] args,
            GetoptLikeTestAnswer[] answers, string[] otherStrings)
    {
        GetoptLike getopt;
        if (!string.IsNullOrEmpty(shortFlags))
        {
            getopt = new GetoptLike(args, shortFlags);
        }
        else if (getoptArgs.Count() != 0)
        {
            getopt = new GetoptLike(args, getoptArgs);
        }
        else
        {
            Assert.Fail();
            return;
        }

        if (answers.Count() != getopt.gArgs.Count())
            Assert.Fail();

        for (int i = 0; i < getopt.gArgs.Count(); i++)
        {
            string? rightAnswer = answers.ElementAt(i).answer;
            string? testedAnswer = getopt.gArgs.ElementAt(i).argument;
            string? rightShortFlag = answers.ElementAt(i).shortFlag;
            string? testedShortFlag = getopt.gArgs.ElementAt(i).shortFlag;
            string? rightLongFlag = answers.ElementAt(i).longFlag;
            string? testedLongFlag = getopt.gArgs.ElementAt(i).longFlag;

            // If the different flag is caught, fail the test
            if (!rightShortFlag.Equals(testedShortFlag)
                    && rightLongFlag.Equals(rightShortFlag))
            {
                Assert.Fail();
                return;
            }

            // If one of rightAnswer and testedAnswer,
            // is null and the other isn't, fail the test
            if (string.IsNullOrEmpty(rightAnswer) != string.IsNullOrEmpty(testedAnswer))
            {
                Assert.Fail();
                return;
            }

            if (string.IsNullOrEmpty(rightAnswer)) continue;

            // If the different answer is caught, fail the test
            if (!rightAnswer.Equals(testedAnswer))
            {
                Assert.Fail();
                return;
            }
        }

        // If the otherString is different than expected fail the test
        if (getopt.otherArgs.Count() != otherStrings.Count())
            Assert.Fail();

        for (int i = 0; i < otherStrings.Count(); i++)
        {
            if (!otherStrings.ElementAt(i).Equals(getopt.otherArgs.ElementAt(i)))
                Assert.Fail();
        }

        // If everything passed, then the test is successful
        Assert.True(true);
    }
}
