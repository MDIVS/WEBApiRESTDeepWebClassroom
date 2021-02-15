using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuizWebAPIREST.Services {
    public class Gravatar {
        HttpClient cliente = new HttpClient();

        public Gravatar(string hash) {
            cliente.BaseAddress = new Uri("https://en.gravatar.com/");
            cliente.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );
            cliente.DefaultRequestHeaders.Add("User-Agent", "C# App");
        }

        public async Task<dynamic> GetGravatarAsync(string hash) {
            HttpResponseMessage response = await cliente.GetAsync(hash+".json");
            if (response.IsSuccessStatusCode) {
                var dados = await response.Content.ReadAsStringAsync();
                var json = JsonSerializer.Deserialize<dynamic>(dados);
                return json;
            }
            Console.WriteLine("algo deu errado");
            return new {message = "usuário não existe no Gravatar :("};
        }
    }
}