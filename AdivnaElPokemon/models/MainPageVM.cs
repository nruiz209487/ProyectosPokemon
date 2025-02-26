
using Microsoft.AspNetCore.SignalR.Client;
using MODELS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdivnaElPokemon.models
{
    public class MainPageVM : INotifyPropertyChanged
    {
        private HubConnection _connection;
        private Random random = new Random();
        public int NumColumnas { get; set; }

        public ObservableCollection<PokemonCompleto> _ListadoDePokemons = new ObservableCollection<PokemonCompleto>();
        public ObservableCollection<PokemonCompleto> ListadoDePokemons
        {
            get
            {
                return _ListadoDePokemons;
            }
            set
            {
                if (value != null)
                {
                    _ListadoDePokemons = value;

                }
                OnPropertyChanged(nameof(ListadoDePokemons));
            }
        }

        public int _numeroDeAciertos = 0;
        public int NumeroDeAciertos { get { return _numeroDeAciertos; } set { _numeroDeAciertos = value; OnPropertyChanged(nameof(NumeroDeAciertos)); } }


        public PokemonCompleto? _pokemonSeleccionado;
        public PokemonCompleto? PokemonSeleccionado
        {
            get
            {
                return _pokemonSeleccionado;
            }
            set
            {
                if (value != null)
                {
                    _pokemonSeleccionado = value;
                    comprobarRespuesta();
                }
            }
        }


        public PokemonCompleto? _pokemonRespuesta;
        public PokemonCompleto? PokemonRespuesta
        {
            get
            {
                return _pokemonRespuesta;
            }
            set
            {
                if (value != null)
                {
                    _pokemonRespuesta = value;
                    _pokemonRespuesta.name = _pokemonRespuesta.name.ToUpper();
                    OnPropertyChanged(nameof(PokemonRespuesta));
                }
            }
        }



        public MainPageVM()
        {

            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7278/gameHub")
                .WithAutomaticReconnect() // Habilita reconexión automática
                .Build();

            ConnectToServer();
            _connection.On<int>("ReceiveAcierto", (acierto) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    int num = acierto;
                });
            });

            pedirPokemon();

        }

        private async void ConnectToServer()
        {
           
                await _connection.StartAsync();
            
   
        }
        private async void pedirPokemon(int numeroDePokemon = 15)
        {
            NumColumnas = numeroDePokemon / 3;
            int idAleatorio = random.Next(0, numeroDePokemon);
            List<PokemonCompleto> list = await DTO.ServiceAdivinaElPokemon.ObtenerListadoDePokemonsDTO(numeroDePokemon);
            ListadoDePokemons = new ObservableCollection<PokemonCompleto>(list);
            PokemonRespuesta = ListadoDePokemons[idAleatorio];
        }

        private void comprobarRespuesta()
        {
            if (_pokemonSeleccionado != null && PokemonRespuesta != null)
            {
                if (PokemonRespuesta.id == _pokemonSeleccionado.id)
                {
                    NumeroDeAciertos++;
                }
                else { NumeroDeAciertos = 0; }

                pedirPokemon();
                _pokemonSeleccionado = null;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
