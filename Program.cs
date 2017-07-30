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
            Console.WriteLine("Hello World!");
            var accessToken = "EAACEdEose0cBAMjynsM9ZCZBTtcAHZBGFtZAW66iWFY7ZCQRB6NifZAeVhWXnqBZAGAOMo2LAkotX5HkjBFhPGna6W6cAXSaQKJDc46kZAvP1yOSARhuPXQ43e0b4C1glsHajXF0tpRBKnSznsCaxUPBw7FuMzCL8hQhfWy2YOrSSD6QtMfbNNFa3uSDKfC1oPQZD";

            //"246401099154708" - // yazhe mat' 
            //"1428660687388397"; // Shritlits
            // "582593101850963" // MK
            // 108935396446747 test

            var sourceMembers = new List<GroupMember>();


            var sourceGroupId = "582593101850963";
            var targetGroupId = "246401099154708";

            LoadMembers(sourceGroupId, accessToken, sourceMembers);

            var targetGroupMembers = new List<GroupMember>();
            LoadMembers(targetGroupId, accessToken, targetGroupMembers);

            List<GroupMember> intersectedMemebers =
                sourceMembers.Join(targetGroupMembers, i => i.id, i => i.id, (a, b) => b)
                    .ToList();

            Console.WriteLine($"Intersected Count: {intersectedMemebers.Count:N0}");
            Console.WriteLine(JsonConvert.SerializeObject(intersectedMemebers, Formatting.Indented));
            

            System.IO.File.WriteAllLines($"GroupId_{sourceGroupId}_vs_Group_{targetGroupId}", new[]
            {
                JsonConvert.SerializeObject(intersectedMemebers, Formatting.Indented)
            });

            var intersectedIds = sourceMembers.Select(i => i.id)
                .Intersect(targetGroupMembers.Select(i => i.id))
                .ToList();

            Console.WriteLine($"IntersectedIds Count: {intersectedIds.Count:N0}");
            Console.WriteLine(JsonConvert.SerializeObject(intersectedIds, Formatting.Indented));

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
            for (int i = 3 - 1; i >= 0; i--)
            {
                try
                {
                    sourceMembersRequest = httpClient.GetStringAsync(url).Result;
                }
                catch (Exception)
                {
                    Console.WriteLine($"Retry {i}");
                    Task.Delay(TimeSpan.FromSeconds(10));
                    if (i == 0)
                    {
                        throw;
                    }
                }
            }

            var groupMembersResponse = JsonConvert.DeserializeObject<GroupMembersResponse>(sourceMembersRequest);
            Console.WriteLine(url);
            Console.WriteLine(members.Count);
            members.AddRange(groupMembersResponse.data);

            if (!string.IsNullOrWhiteSpace(groupMembersResponse.paging?.next))
            {
                LoadMemebrsByUrl(members, httpClient, groupMembersResponse.paging.next);
            }
        }
    }
}
