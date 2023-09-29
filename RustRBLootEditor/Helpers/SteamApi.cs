using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RustRBLootEditor.Helpers
{
    public class SteamApi
    {
        private static readonly HttpClient client = new HttpClient();
        public static async Task<SteamPublishedFileResponse> GetPublishedFileDetailsAsync(List<ulong> skinList)
        {
            try
            {
                if (skinList.Contains(0))
                    skinList.Remove(0);

                if (skinList.Count == 0)
                    return null;

                var values = new Dictionary<string, string>
                {
                    { "itemcount", skinList.Count()+"" }
                };

                for (int i = 0; i < skinList.Count; i++)
                {
                    values.Add($"publishedfileids[{i}]", skinList[i].ToString());
                }

                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync("https://api.steampowered.com/ISteamRemoteStorage/GetPublishedFileDetails/v1/", content);

                var responseString = await response.Content.ReadAsStringAsync();

                SteamPublishedFileResponse result = Common.DeserializeJSONString<SteamPublishedFileResponse>(responseString);

                return result;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }
}
