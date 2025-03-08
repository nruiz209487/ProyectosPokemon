using MODELS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DTO
{
    public class ServiceAdivinaElPokemon
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly Random _rand = new Random();
        private const int MaxPokemonId = 1025;

        public static async Task<List<Pokemon>> ObtenerListadoDePokemonsDTO(int cantidad)
        {
            var indices = GenerateRandomIndices(cantidad);
            return await FetchPokemonInParallel(indices);
        }


        private static List<int> GenerateRandomIndices(int cantidad)
        {
            var indices = Enumerable.Range(1, MaxPokemonId).ToList();
            Shuffle(indices);
            return indices.Take(cantidad).ToList();
        }

        private static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _rand.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }

        private static async Task<List<Pokemon>> FetchPokemonInParallel(List<int> indices)
        {
            var semaphore = new SemaphoreSlim(10); // Limit concurrent requests
            var tasks = indices.Select(async index =>
            {
                await semaphore.WaitAsync();
                try
                {
                    return await FetchPokemon(index);
                }
                finally
                {
                    semaphore.Release();
                }
            });

            var results = await Task.WhenAll(tasks);
            return results.Where(p => p != null).ToList();
        }

        private static async Task<Pokemon> FetchPokemon(int index)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{index}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Pokemon>(json);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error fetching Pokémon {index}: {ex.Message}");
                return null;
            }
        }
    }
}