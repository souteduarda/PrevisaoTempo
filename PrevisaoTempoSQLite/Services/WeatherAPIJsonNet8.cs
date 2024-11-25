using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using PrevisaoTempoSQLite.Models;

namespace PrevisaoTempoSQLite.Services
{
    public class WeatherAPIJsonNet8
    {
        public static async Task<APImodel?> GetPrevisaoDoTempo(string cidade)
        {
            // https://home.openweathermap.org/api_keys
            string appId = "6135072afe7f6cec1537d5cb08a5a1a2";

            string url = $"http://api.openweathermap.org/data/2.5/weather?q={cidade}&units=metric&appid={appId}";

            APImodel? tempo = null;

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = response.Content.ReadAsStringAsync().Result;

                    Debug.WriteLine("--------------------------------------------------------------------");
                    Debug.WriteLine(json);
                    Debug.WriteLine("--------------------------------------------------------------------");

                    tempo = JsonSerializer.Deserialize<APImodel>(json);
                }
            }

            return tempo;
        }
    }
}
