using ENT;
using ProyectosPokemon;

namespace BL
{
    public class ServiceBL
    {
        public static Task<List<Pokemon>> ObtenerListadoDePokemonsBL()
        {
            return Service.ObtenerListadoDePokemonsDAL();
        }
    }
}
