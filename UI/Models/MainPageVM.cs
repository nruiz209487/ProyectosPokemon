using ENT;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UI.Models
{
    internal class MainPageVM : INotifyPropertyChanged
    {
        private int _indicePokemon = 0;
        public int IndicePokemon
        {
            get
            {
                return _indicePokemon;
            }
            set
            {
                _indicePokemon = value;
                OnPropertyChanged(nameof(IndicePokemon));
            }

        }

        private List<Pokemon> _listadoDePokemons = new List<Pokemon>();

        public List<Pokemon> ListadoDePokemons
        {
            get => _listadoDePokemons;
            private set
            {
                _listadoDePokemons = value;
                IndicePokemon = BL.ServiceBL.ObtenerIndicePokemonBL();
                OnPropertyChanged(nameof(ListadoDePokemons));
            }
        }

        public async Task ActualizarListadoAsync(int avance)
        {
            ListadoDePokemons = await BL.ServiceBL.ObtenerListadoDePokemonsBL(avance);
        }

        public ICommand LlamarActualizarListado { get; }
        public ICommand ListadoAnterior { get; }

        public MainPageVM()
        {
            ActualizarListadoAsync(0);
            LlamarActualizarListado = new Command(async () => await ActualizarListadoAsync(1));
            ListadoAnterior = new Command(async () => await ActualizarListadoAsync(-1));
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
