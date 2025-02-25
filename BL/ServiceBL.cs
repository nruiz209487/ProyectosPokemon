using ENT;
using ProyectosPokemon;
using System.Collections.Generic;

namespace BL
{
    public class ServiceBL
    {
        public static int ObtenerIndicePokemonBL()
        {
            return ServiceUI.ObtenerIndicePokemonDAL();
        }

        public static async Task<List<Pokemon>> ObtenerListadoDePokemonsBL(int avance)
        {
            List<Pokemon> list = await ServiceUI.ObtenerListadoDePokemonsDAL(avance);
            return list;
        }


    }
}
