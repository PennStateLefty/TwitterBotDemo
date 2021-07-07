using System;
using System.Threading.Tasks;
using TwitterBot.Framework.BusinessLogic;
using TwitterBot.Framework.Mappings;
using TwitterBot.Framework.Types;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            var tweetOps = new TweetOperations("Dafj6W6zArpa0CGaGki4Sc5uS", "0JxlLQ2b9Fqd4yzeiHRXEBqzdnCzDmBA2JQo1Di9KNF9aOAAMx", @"AAAAAAAAAAAAAAAAAAAAAPOQRQEAAAAAzqLSsSfaHH8a2hnaQSboMTbEiZA%3DwgYNEjqFTFYFIGxoOgrpMMTYLuWu1TOe2ZzMNrYVxYypxIMto7");
            var result = await tweetOps.GetPopularTweetsByHashtagAsync(new Hashtag { Text = "#justsaying" });

            if (result != null)
            {
                System.Console.WriteLine(result.FullText);
            }

            Console.Read();
        }
    }
}
