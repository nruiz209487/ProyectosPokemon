using Ent;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Service
{
    public class ServiceAdivinaElPokemon
    {
        private static readonly HttpClient _httpClient = new HttpClient(); //  http 
        private static readonly Random _rand = new Random(); // objeto Random que generara numeros aleatorios 
        private const int MaxPokemonId = 1025; // Max id 

        public static async Task<List<Pokemon>> ObtenerListadoDePokemonsDTO(int cantidad)
        {
            var indices = indicesAleatorios(cantidad); // genera una lista de indices aleatorios
            return await pedirPokemonsEnParalelo(indices); // llama a la funcion que trae los pokemons en paralelo
        }

        /// <summary>
        /// crea una lista del 1 al max id de inidices aleatorios
        /// </summary>
        /// <param name="cantidad"></param>
        /// <returns>Listado de indices</returns>
        private static List<int> indicesAleatorios(int cantidad)
        {
            var indices = Enumerable.Range(1, MaxPokemonId).ToList(); 
            desordenarLista(indices); 
            return indices.Take(cantidad).ToList(); 
        }
        /// <summary>
        /// Desordena el listado (si lo he copiado de stackOverflow)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        private static void desordenarLista<T>(IList<T> list)
        {
            int n = list.Count; 
            while (n > 1) 
            {
                n--;
                int k = _rand.Next(n + 1); 
                (list[n], list[k]) = (list[k], list[n]); 
            }
        }

        /// <summary>
        /// Crea un semaforo que hace multiples peticiones a la api al mismo tiempo
        /// (si me lo ha hecho el chatgpt de la manera normal tardava entre 2 y 3 segundos y no era conveninente)
        /// </summary>
        /// <param name="indices"></param>
        /// <returns>Listado pokemins</returns>
        private static async Task<List<Pokemon>> pedirPokemonsEnParalelo(List<int> indices)
        {
            var semaphore = new SemaphoreSlim(10); // limita la cantidad de peticiones en paralelo a 10
            var tasks = indices.Select(async index => 
            {
                await semaphore.WaitAsync();
                try
                {
                    return await solicitudApi(index); 
                }
                finally
                {
                    semaphore.Release(); 
                }
            });

            return (await Task.WhenAll(tasks)).ToList(); 
        }
        /// <summary>
        /// hace la peticion a la pokeapi
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Listado de poekmons</returns>
        /// <exception cref="Exception"></exception>
        private static async Task<Pokemon> solicitudApi(int index)
        {
            var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{index}"); 
            response.EnsureSuccessStatusCode(); 

            var json = await response.Content.ReadAsStringAsync();
            var pokemon = JsonConvert.DeserializeObject<Pokemon>(json);

            if (pokemon == null)
            {
                throw new Exception($"No se pudo deserializar el Pokémon con ID {index}");
            }

            return pokemon;
        }
    }
}
