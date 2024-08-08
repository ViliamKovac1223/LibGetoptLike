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

    public static IEnumerable<object[]> DataForShortFlagTests =>
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
                }
            },
            new object[] {
                "ab:c::", // shortFlags
                new GetoptArg[] {}, // getoptArgs
                new string[] { "-a", "-b", "arg" }, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("a", "", ""),
                    new GetoptLikeTestAnswer("b", "", "arg")
                }
            },
            new object[] {
                "ab:c::", // shortFlags
                new GetoptArg[] {}, // getoptArgs
                new string[] { "-carg" }, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("c", "", "arg")
                }
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
                }
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
                }
            },
            new object[] {
                "", // shortFlags
                new GetoptArg[] { // getoptArgs
                    new GetoptArg("", "hello", "", FlagType.ArgumentRequired)
                },
                new string[] { "--hello=abc" }, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("", "hello", "abc"),
                }
            },
            new object[] {
                "", // shortFlags
                new GetoptArg[] { // getoptArgs
                    new GetoptArg("", "hello", "", FlagType.ArgumentOptional)
                },
                new string[] { "--hello=abc" }, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("", "hello", "abc"),
                }
            },
            new object[] {
                "", // shortFlags
                new GetoptArg[] { // getoptArgs
                    new GetoptArg("", "hello", "", FlagType.ArgumentRequired)
                },
                new string[] { "--hello", "abc" }, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("", "hello", "abc"),
                }
            },
            new object[] {
                "", // shortFlags
                new GetoptArg[] { // getoptArgs
                    new GetoptArg("h", "hello", "", FlagType.NoArgument)
                },
                new string[] { "--hello" }, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("", "hello", ""),
                }
            },
            new object[] {
                "", // shortFlags
                new GetoptArg[] { // getoptArgs
                    new GetoptArg("h", "hello", "", FlagType.ArgumentRequired)
                },
                new string[] { "--hello", "abc" }, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("", "hello", "abc"),
                }
            },
            new object[] {
                "", // shortFlags
                new GetoptArg[] { // getoptArgs
                    new GetoptArg("h", "hello", "", FlagType.ArgumentRequired)
                },
                new string[] { "-h", "abc"}, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("h", "", "abc"),
                }
            },
            new object[] {
                "", // shortFlags
                new GetoptArg[] { // getoptArgs
                    new GetoptArg("h", "hello", "", FlagType.ArgumentRequired)
                },
                new string[] { "-h=abc"}, // args
                new GetoptLikeTestAnswer[] { // answers
                    new GetoptLikeTestAnswer("h", "", "=abc"),
                }
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
                }
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
                }
            },
        };

    [Theory]
    [MemberData(nameof(DataForShortFlagTests))]
    public void LongFlagTests(string shortFlags, GetoptArg[] getoptArgs, string[] args, GetoptLikeTestAnswer[] answers)
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

        // If everything passed, then the test is successful
        Assert.True(true);
    }
}
