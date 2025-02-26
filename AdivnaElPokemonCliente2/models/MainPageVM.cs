
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
        public Contador Contador { get; set; } = new Contador();
        private HubConnection _connection;
        private Random random = new Random();
        public int NumColumnas { get; set; }

        public ObservableCollection<Pokemon> _ListadoDePokemons = new ObservableCollection<Pokemon>();
        public ObservableCollection<Pokemon> ListadoDePokemons
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
        public int _numeroDeAciertosEnemigo = 0;
        public int NumeroDeAciertosEnemigo { get { return _numeroDeAciertosEnemigo; } set { _numeroDeAciertosEnemigo = value; OnPropertyChanged(nameof(NumeroDeAciertosEnemigo)); } }


        public Pokemon? _pokemonSeleccionado;
        public Pokemon? PokemonSeleccionado
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
        public Pokemon? _pokemonRespuesta;
        public Pokemon? PokemonRespuesta
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
                .WithUrl("https://localhost:7211/gameHub")
                .WithAutomaticReconnect()
                .Build();

            conexionServidor();
            _connection.On<int>("ReceiveAcierto", (numAciertos) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    NumeroDeAciertosEnemigo = numAciertos;
                });
            });
            pedirPokemon();

        }

        private async void conexionServidor()
        {
            await _connection.StartAsync();
        }

        private async void pedirPokemon(int numeroDePokemon = 15)
        {
            NumColumnas = numeroDePokemon / 3;
            List<Pokemon> list = await DTO.ServiceAdivinaElPokemon.ObtenerListadoDePokemonsDTO(numeroDePokemon);
            int idAleatorio = random.Next(0, numeroDePokemon);
            ListadoDePokemons = new ObservableCollection<Pokemon>(list);
            PokemonRespuesta = ListadoDePokemons[idAleatorio];
        }
        private async void enviarContadorDeAciertos()
        {
              await _connection.InvokeAsync("SendAcierto", NumeroDeAciertos);
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
                enviarContadorDeAciertos();
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
