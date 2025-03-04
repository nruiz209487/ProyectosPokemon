
using MODELS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class ServiceAdivinaElPokemon
    {
        private static Random rand = new Random();
        public static async Task<List<Pokemon>> ObtenerListadoDePokemonsDTO(int cantidad)
        {
            List<Pokemon> listado = new List<Pokemon>();

            List<int> lista = new List<int>();
            for (int i = 0; i < cantidad; i++)
            {
                int inidcePokemon = rand.Next(1, 1050);
                if (!lista.Contains(inidcePokemon)) { lista.Add(inidcePokemon); }
                else
                {
                    while (lista.Contains(inidcePokemon))
                    {
                        inidcePokemon = rand.Next(1, 1025);
                    }

                }
                Pokemon? apiResponse;
                Uri miUri = new Uri($"https://pokeapi.co/api/v2/pokemon/{inidcePokemon}");
                HttpClient mihttpClient = new HttpClient();

                try
                {
                    HttpResponseMessage miCodigoRespuesta = await mihttpClient.GetAsync(miUri);

                    if (miCodigoRespuesta.IsSuccessStatusCode)
                    {
                        string textoJsonRespuesta = await miCodigoRespuesta.Content.ReadAsStringAsync();
                        apiResponse = JsonConvert.DeserializeObject<Pokemon>(textoJsonRespuesta);
                        if (apiResponse != null)
                        {
                            listado.Add(apiResponse);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    mihttpClient.Dispose();
                }

            }
            return listado;
        }


    }
}
