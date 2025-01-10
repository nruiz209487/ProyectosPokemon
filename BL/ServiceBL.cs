using ENT;
using ProyectosPokemon;
using System.Collections.Generic;

namespace BL
{
    public class ServiceBL
    {
        public static int ObtenerIndicePokemonBL()
        {
            return Service.ObtenerIndicePokemonDAL();
        }

        public static async Task<List<Pokemon>> ObtenerListadoDePokemonsBL(int avance)
        {
            List<Pokemon> list = await Service.ObtenerListadoDePokemonsDAL(avance);
            return list;
        }


    }
}
