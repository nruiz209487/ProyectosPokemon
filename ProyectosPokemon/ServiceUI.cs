using DAL;
using ENT;
using Newtonsoft.Json;

namespace ProyectosPokemon
{
    public class ServiceUI
    {
        private const string URL = "https://pokeapi.co/api/v2/pokemon";
        private static int IndicePokemon { get; set; } = 0;

        public static int ObtenerIndicePokemonDAL()
        {
            return IndicePokemon;
        }

        public static async Task<List<Pokemon>> ObtenerListadoDePokemonsDAL(int avance)
        {
            AsignarIndiceBusqueda(avance);

            Uri miUri = new Uri($"{URL}?limit=20&offset={IndicePokemon}");

            List<Pokemon>? listadoDePokemons = new List<Pokemon>();
            HttpClient mihttpClient = new HttpClient();

            try
            {
                HttpResponseMessage miCodigoRespuesta = await mihttpClient.GetAsync(miUri);

                if (miCodigoRespuesta.IsSuccessStatusCode)
                {
                    string textoJsonRespuesta = await miCodigoRespuesta.Content.ReadAsStringAsync();
                    PokemonApiResponse? apiResponse = JsonConvert.DeserializeObject<PokemonApiResponse>(textoJsonRespuesta);
                    if (apiResponse != null)
                    {
                        listadoDePokemons = apiResponse.Results;
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

            return listadoDePokemons;
        }

        private static void AsignarIndiceBusqueda(int avance)
        {
            switch (avance)
            {
                case 1:
                    if (IndicePokemon + 20 <= 1300)
                    {
                        IndicePokemon += 20;
                    }
                    break;
                case -1:
                    if (IndicePokemon - 20 >= 0)
                    {
                        IndicePokemon = IndicePokemon - 20;
                    }
                    break;
            }
        }
    }
}

