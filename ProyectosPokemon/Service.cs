using DAL;
using ENT;

namespace ProyectosPokemon
{
    public class Service
    {
        public static async Task<List<Pokemon>> ObtenerListadoDePokemonsDAL()

        {

            //Pido la cadena de la Uri al método estático

            string miCadenaUrl = ApiUriBase.getUriBase();

            Uri miUri = new Uri($"{miCadenaUrl}pokemon?limit=20&offset=0");

            List<Pokemon> listadoDePokemons = new List<Pokemon>();

            HttpClient mihttpClient;

            HttpResponseMessage miCodigoRespuesta;

            string textoJsonRespuesta;

            //Instanciamos el cliente Http

            mihttpClient = new HttpClient();


            try

            {

                miCodigoRespuesta = await mihttpClient.GetAsync(miUri);

                if (miCodigoRespuesta.IsSuccessStatusCode)

                {

                    textoJsonRespuesta = await mihttpClient.GetStringAsync(miUri);

                    mihttpClient.Dispose();

                    //JsonConvert necesita using Newtonsoft.Json;

                    //Es el paquete Nuget de Newtonsoft

                    listadoDePokemons = JsonConvert.DeserializeObject<List<Pokemon>>(textoJsonRespuesta);

                }

            }

            catch (Exception ex)

            {

                throw ex;

            }

            return listadoDePokemons;

        }

    }
}

