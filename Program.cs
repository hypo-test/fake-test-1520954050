using System;
using System.Net.Http;
using Newtonsoft.Json;

namespace fb_groups_intersector
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            using (var httpClient = new HttpClient())
            {
                var accessToken = "EAACEdEose0cBAIsVHL3XPwGk0yOfeGsbKeIq2nD70L1DlS3wwKcFdKndSRsC9r7EROBS48K2YlLE1SJ7Gmkti9X89Ry3vPqxygKr7iugjH8eZBKwxq611V9YdmLVoQM2NNWNrtLWjraJBotSJ5uZCa5hZBLs1V73f9JpOlsEwS9sIKzYJ5zEzRD4JbzUZBsZD";

                var sourceGroupId = "246401099154708";
                var sourceMembersResp = httpClient
                    .GetStringAsync(
                        $"https://graph.facebook.com/v2.10/{sourceGroupId}/members?access_token={accessToken}");

                var souurceMembersResponse = JsonConvert.DeserializeObject<GroupMembersResponse>(sourceMembersResp.Result);
                Console.WriteLine(JsonConvert.SerializeObject(souurceMembersResponse, Formatting.Indented));
            }

            Console.ReadLine();
        }
    }
}
