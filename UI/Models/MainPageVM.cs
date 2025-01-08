using ENT;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Models
{
    internal class MainPageVM
    {
        public Task<List<Pokemon>> listadoDePokemons
        {
            get { return BL.ServiceBL.ObtenerListadoDePokemonsBL(); }
        }
    }
}
