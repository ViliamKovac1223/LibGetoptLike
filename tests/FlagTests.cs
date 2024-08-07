namespace LibGetoptLikeTests;
using Xunit;
using LibGetoptLike;

public class FlagTests
{
    public struct GetoptLikeTestAnswer {
        public GetoptLikeTestAnswer(string flag, string answer){
            this.flag = flag;
            this.answer = answer;
        }

        public string flag;
        public string answer;
    }

    public static IEnumerable<object[]> DataForShortFlagTests =>
        new List<object[]>
        {
            new object[] {
                "ab:c::",
                new string[] { "-a", "-barg" },
                    new GetoptLikeTestAnswer[] {
                        new GetoptLikeTestAnswer("a", ""),
                        new GetoptLikeTestAnswer("b", "arg")
                    }
            },
            new object[] {
                "ab:c::",
                new string[] { "-a", "-b", "arg" },
                    new GetoptLikeTestAnswer[] {
                        new GetoptLikeTestAnswer("a", ""),
                        new GetoptLikeTestAnswer("b", "arg")
                    }
            },
            new object[] {
                "ab:c::",
                new string[] { "-carg" },
                    new GetoptLikeTestAnswer[] {
                        new GetoptLikeTestAnswer("c", "arg")
                    }
            },
            new object[] {
                "ab:c::",
                new string[] { "-acopt", "-abreq", "-ab", "req1" },
                    new GetoptLikeTestAnswer[] {
                        new GetoptLikeTestAnswer("a", ""),
                        new GetoptLikeTestAnswer("c", "opt"),
                        new GetoptLikeTestAnswer("a", ""),
                        new GetoptLikeTestAnswer("b", "req"),
                        new GetoptLikeTestAnswer("a", ""),
                        new GetoptLikeTestAnswer("b", "req1"),
                    }
            },
        };

    [Theory]
    [MemberData(nameof(DataForShortFlagTests))]
    public void ShortFlagTests(string shortFlags, string[] args, GetoptLikeTestAnswer[] answers)
    {
        GetoptLike getopt = new GetoptLike(args, shortFlags);

        if (answers.Count() != getopt.gArgs.Count())
            Assert.Fail();

        for (int i = 0; i < getopt.gArgs.Count(); i++)
        {
            string? rightAnswer = answers.ElementAt(i).answer;
            string? testedAnswer = getopt.gArgs.ElementAt(i).argument;
            string? rightShortFlag = answers.ElementAt(i).flag;
            string? testedShortFlag = getopt.gArgs.ElementAt(i).shortFlag;

            // If the different flag is caught, fail the test
            if (!rightShortFlag.Equals(testedShortFlag))
            {
                Assert.Fail();
                return ;
            }

            // If one of rightAnswer and testedAnswer,
            // is null and the other isn't, fail the test
            if (string.IsNullOrEmpty(rightAnswer) != string.IsNullOrEmpty(testedAnswer))
            {
                Assert.Fail();
                return ;
            }

            if (string.IsNullOrEmpty(rightAnswer)) continue;

            // If the different answer is caught, fail the test
            if (!rightAnswer.Equals(testedAnswer))
            {
                Assert.Fail();
                return ;
            }
        }

        // If everything passed, then the test is successful
        Assert.True(true);
    }
}
