using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiPerfTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Running Init Test Set");
            var testSets = GetTestSets();
            await RunTestSet(testSets);
            var totalElapsed = TimeSpan.Zero;
            for (var i = 0; i < 10; i++)
            {
                var sw = Stopwatch.StartNew();
                await RunTestSet(testSets);
                sw.Stop();
                Console.WriteLine(sw.ElapsedMilliseconds);
                totalElapsed = totalElapsed.Add(sw.Elapsed);
            }

            Console.WriteLine("-----------");
            Console.WriteLine(totalElapsed.TotalMilliseconds / 10);
        }

        private static async Task<List<(string Name, TimeSpan Elapsed)>> RunTestSet(List<(string Name, string Url)> testSet)
        {
            var results = new List<(string Name, TimeSpan Elapsed)>();
            foreach (var test in testSet)
            {
                var sw = Stopwatch.StartNew();
                await RunTest(test.Url);
                sw.Stop();
                results.Add((test.Name, sw.Elapsed));
            }

            return results;
        }

        private static async Task RunTest(string url)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(5);
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
            }
        }

        private static List<(string Name, string Url)> GetTestSets()
        {
            return new List<(string Name, string Url)>
            {
                //("Allocation_SiteUuid", "https://wadeapiperf.azurewebsites.net/api/v1/SiteAllocationAmounts?SiteUUID=UT_01-1080&code=xdrUbG2/F/fd5vhOEh0pO0LEwNqWJeAZzUQiMuKSitzLmsJRYQax8Q=="),
                ("Allocation_SiteType", "https://wadeapiperf.azurewebsites.net/api/v1/SiteAllocationAmounts?SiteTypeCV=Surface&code=xdrUbG2/F/fd5vhOEh0pO0LEwNqWJeAZzUQiMuKSitzLmsJRYQax8Q==")
            };
        }
    }
}
