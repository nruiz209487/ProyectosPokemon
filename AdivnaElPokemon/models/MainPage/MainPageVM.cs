using AdivnaElPokemon.models.utils;
using AdivnaElPokemon.Pages;
using Microsoft.AspNetCore.SignalR.Client;
using Ent;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;

namespace AdivnaElPokemon.models.MainPage
{
    public class MainPageVM : ClsINotify
    {
        #region contador
        private int _segundos = 120;
        public int Segundos { get { return _segundos; } set { _segundos = value; OnPropertyChanged(nameof(SegundosVista)); } }
        private System.Timers.Timer _timer;
        public string SegundosVista
        {
            get
            {
                int minutes = Segundos / 60;
                int seconds = Segundos % 60;
                return $"{minutes:D2}:{seconds:D2}";
            }
        }

        private async void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            Segundos--;
            if (_segundos == 0)
            {
                _timer.Stop();

                navLobby();
            }
        }
        #endregion

        #region Hub
        private HubConnection _connection;

        private async void conexionServidor()
        {
            try
            {
                await _connection.StartAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.GoToAsync("//LobbyPage");
            }
        }

        private async void enviarContadorDeAciertos()
        {
            try
            {
                await _connection.InvokeAsync("SendAcierto", NumeroDeAciertos);
            }
            catch (Exception ex)
            {
                await Shell.Current.GoToAsync("//LobbyPage");
            }
        }
        #endregion

        #region JuegoPokemon
        private int _comodinesRestantes = 5;
        public int ComodinesRestantes { get { return _comodinesRestantes; } set { _comodinesRestantes = value; usarComodinCommand.RaiseCanExecuteChanged(); OnPropertyChanged(nameof(ComodinesRestantes)); } }
        private string ResultadoPartida
        {
            get
            {
                return NumeroDeAciertos > NumeroDeAciertosEnemigo
                        ? "¡Has Ganado!"
                        : (NumeroDeAciertos < NumeroDeAciertosEnemigo
                            ? "¡Has Perdido!"
                            : "¡Empate!");
            }
        }
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
                    SelecionarPokemonDisponible = false;
                    comprobarRespuesta();
                }
            }
        }
        public bool _selecionarPokemonDisponible;
        public bool SelecionarPokemonDisponible
        {
            get
            {
                return _selecionarPokemonDisponible;
            }
            set
            {

                _selecionarPokemonDisponible = value;
                OnPropertyChanged(nameof(SelecionarPokemonDisponible));
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

        public async void pedirPokemon()
        {
            int numeroDePokemon = 15;
            NumColumnas = numeroDePokemon / 3;
            List<Pokemon> list = new List<Pokemon>();
            try { list = await Service.ServiceAdivinaElPokemon.ObtenerListadoDePokemonsDTO(numeroDePokemon); }
            catch (Exception ex) { await Shell.Current.GoToAsync("//LobbyPage"); }

            int idAleatorio = random.Next(0, numeroDePokemon - 1);
            ListadoDePokemons = new ObservableCollection<Pokemon>(list);
            PokemonRespuesta = ListadoDePokemons[idAleatorio];
            SelecionarPokemonDisponible = true;
        }
        public async void pedirPokemonComodin()
        {
            SelecionarPokemonDisponible = false; //lo tengo que poner por que como no tiene que hacerla comprobacion salta directamente al metodo y si la api va lenta puede selecionar muchos al mismo tiempo
            int numeroDePokemon = 15;
            NumColumnas = numeroDePokemon / 3;
            List<Pokemon> list = new List<Pokemon>();
            try { list = await Service.ServiceAdivinaElPokemon.ObtenerListadoDePokemonsDTO(numeroDePokemon); }
            catch (Exception ex) { await Shell.Current.GoToAsync("//LobbyPage"); }


            int idAleatorio = random.Next(0, numeroDePokemon - 1);
            ListadoDePokemons = new ObservableCollection<Pokemon>(list);
            PokemonRespuesta = ListadoDePokemons[idAleatorio];
            SelecionarPokemonDisponible = true;
            ComodinesRestantes--;
        }

        private bool usarComodinCommandActivo()
        {
            bool res = true;

            if (_comodinesRestantes <= 0) { res = false; }
            return res;
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
        public DelegateCommand usarComodinCommand { get; }
        #endregion
        #region implementacion de la clase 
        public MainPageVM()
        {
            //hub
            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7211/gameHub")
                .WithAutomaticReconnect()
                .Build();

            conexionServidor();
            _connection.On<int>("ReceiveAcierto", (numAciertos) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (_segundos != 0)
                    {
                        NumeroDeAciertosEnemigo = numAciertos;
                    }
                });
            });

            //juego
            pedirPokemon();
            usarComodinCommand = new DelegateCommand(pedirPokemonComodin, usarComodinCommandActivo);
            //contador 
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += OnTimerTick;
            _timer.AutoReset = true;
            _timer.Start();


        }

        private async void navLobby()
        {

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    salirLobby();
                    await App.Current.MainPage.DisplayAlert("¡Se acabó el tiempo!", ResultadoPartida, "OK");
                    await Shell.Current.GoToAsync("//LobbyPage");
                }
                catch (Exception ex) { await Shell.Current.GoToAsync("//LobbyPage"); }

            });
        }
        public async void salirLobby()
        {
            await _connection.InvokeAsync("SalirLobby");
        }
        #endregion
    }
}
