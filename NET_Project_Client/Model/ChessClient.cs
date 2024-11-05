using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NET_Project_Client.Model
{
    internal class ChessClient
    {
        private readonly HttpClient _httpClient;

        public ChessClient()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5177/"); 
        }

        public async Task<dynamic> GetRandomMove(List<List<int>> availableMoves)
        {
            var json = JsonConvert.SerializeObject(availableMoves);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync("api/RandomMove/random-move", content);
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<dynamic>(result);
            }
            else
            {
                throw new Exception("Error retrieving random move from the server: " + response.ReasonPhrase);
            }
        }

        public async Task<dynamic> postGame(Game game)
        {
            var json = JsonConvert.SerializeObject(game); // Convert game to JSON
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Send POST request to api/Games
            HttpResponseMessage response = await _httpClient.PostAsync("api/Games", content);

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<dynamic>(result); // Deserialize server response
            }
            else
            {
                throw new Exception("Error posting game to server: " + response.ReasonPhrase);
            }
        }
    }
}
