using ENT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{

    public class PokemonApiResponse
    {
        public int Count { get; set; } = 0;
        public string Next { get; set; } = "";
        public string Previous { get; set; } = "";
        public List<Pokemon>? Results { get; set; }
    }
}
