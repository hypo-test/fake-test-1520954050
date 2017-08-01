using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace fb_groups_intersector
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: fb_groups_intersector sourceGroupId targetGroupId accessToken");
                Console.WriteLine("E.g.: fb_groups_intersector 200429360160731 647709162072194 EAAC...UZD");

                Environment.Exit(1);
            }

            var sourceGroupId = args[0];
            var targetGroupId = args[1];
            var accessToken = args[2];

            Console.WriteLine($"Starting intersecting group id = {sourceGroupId} against group id = {targetGroupId}");

            var sourceMembers = new List<GroupMember>();
            LoadMembers(sourceGroupId, accessToken, sourceMembers);

            var targetGroupMembers = new List<GroupMember>();
            LoadMembers(targetGroupId, accessToken, targetGroupMembers);

            List<GroupMember> intersectedMemebers =
                sourceMembers.Join(targetGroupMembers, i => i.id, i => i.id, (a, b) => b)
                    .ToList();

            Console.WriteLine(JsonConvert.SerializeObject(intersectedMemebers, Formatting.Indented));
            Console.WriteLine($"Intersected Count: {intersectedMemebers.Count:N0}");

            var resultFile = $"GroupId_{sourceGroupId}_vs_Group_{targetGroupId}";
            System.IO.File.WriteAllLines(resultFile, new[]
            {
                JsonConvert.SerializeObject(intersectedMemebers, Formatting.Indented)
            });

            Console.WriteLine($"results saved as {resultFile}");

            Console.ReadLine();
        }

        private static void LoadMembers(string sourceGroupId, string accessToken, List<GroupMember> members)
        {
            var firstPageUrl = $"https://graph.facebook.com/v2.10/{sourceGroupId}/members?access_token={accessToken}&limit=1000";
            using (var httpClient = new HttpClient())
            {
                LoadMemebrsByUrl(members, httpClient, firstPageUrl);
            }
        }

        private static void LoadMemebrsByUrl(List<GroupMember> members, HttpClient httpClient, string url)
        {
            string sourceMembersRequest = "{}";
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    sourceMembersRequest = httpClient.GetStringAsync(url).Result;
                }
                catch (Exception)
                {
                    Console.WriteLine($"Retry {i}");
                    Task.Delay(TimeSpan.FromSeconds(10));
                    if (i == 2)
                    {
                        throw;
                    }
                }
            }

            var groupMembersResponse = JsonConvert.DeserializeObject<GroupMembersResponse>(sourceMembersRequest);
            Console.WriteLine(url);
            members.AddRange(groupMembersResponse.data);
            Console.WriteLine($"loaded: {groupMembersResponse.data.Length:N0}, total loaded by now: {members.Count:N0} ");

            if (!string.IsNullOrWhiteSpace(groupMembersResponse.paging?.next))
            {
                LoadMemebrsByUrl(members, httpClient, groupMembersResponse.paging.next);
            }
        }
    }
}
