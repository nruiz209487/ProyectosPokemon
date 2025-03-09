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
using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Collections;

namespace AdivnaElPokemon.models.MainPage
{
    public class MainPageVM : ClsINotify
    {
        #region contador
        /// <summary>
        /// Variable que controlar el numero de segundos y notifica a la vista
        /// </summary>
        private int _segundos = 120;
        public int Segundos { get { return _segundos; } set { _segundos = value; OnPropertyChanged(nameof(SegundosVista)); } }
        /// <summary>
        /// Timer
        /// </summary>
        private System.Timers.Timer _timer;
        /// <summary>
        /// propiedad que pone el tiempo restante de manera estetica en la vista
        /// </summary>
        public string SegundosVista
        {
            get
            {
                int minutes = Segundos / 60;
                int seconds = Segundos % 60;
                return $"{minutes:D2}:{seconds:D2}";
            }
        }
        /// <summary>
        /// timer implementacion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            Segundos--; //resta un segundo
            if (_segundos == 0)
            {
                _timer.Stop();
                juegoTerminado(); //cuando el contador llega a zero termina el juego
            }
        }
        #endregion
        #region Hub
        private HubConnection _connection;
        /// <summary>
        /// Conexion servidor
        /// </summary>
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

        /// <summary>
        /// envia el numero de aciertos al resto
        /// </summary>
        private async void enviarContadorDeAciertos()
        {
            try
            {
                await _connection.InvokeAsync("SendAcierto", NumeroDeAciertos); // envia el numero de aciertos al resto
            }
            catch (Exception ex)
            {
                await Shell.Current.GoToAsync("//LobbyPage");
            }
        }
        #endregion
        #region JuegoPokemon
        private Random random = new Random();
        /// <summary>
        /// variable que almacena  los comodines restantes y notifica a la vista
        /// </summary>
        private int _comodinesRestantes = 5;
        public int ComodinesRestantes { get { return _comodinesRestantes; } set { _comodinesRestantes = value; usarComodinCommand.RaiseCanExecuteChanged(); OnPropertyChanged(nameof(ComodinesRestantes)); } }
       /// <summary>
       /// Mensaje de resultado partida
       /// </summary>
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
        /// <summary>
        /// variable que sirve para actualizar el numero de columnas de manera dinamica
        /// </summary>
        public int NumColumnas { get; set; }
        /// <summary>
        /// variable que almacena  el lisatdo de pokemons y notifica a la vista
        /// </summary>
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
        /// <summary>
        /// variable que almacena el numero de aciertos local y notifica a la vista
        /// </summary>
        public int _numeroDeAciertos = 0;
        public int NumeroDeAciertos { get { return _numeroDeAciertos; } set { _numeroDeAciertos = value; OnPropertyChanged(nameof(NumeroDeAciertos)); }}
        /// <summary>
        /// variable que almacena el numero de aciertos del enemigo y notifica a la vista
        /// </summary>
        public int _numeroDeAciertosEnemigo = 0;
        public int NumeroDeAciertosEnemigo { get { return _numeroDeAciertosEnemigo; } set { _numeroDeAciertosEnemigo = value; OnPropertyChanged(nameof(NumeroDeAciertosEnemigo)); } }
        /// <summary>
        /// Pokemon selecionado bindeado en el obserbable collection no hace falta ponerlo a null por que la lisat se vuelve a crear cada vez que se llama a la api
        /// al setearlo llama a   comprobarRespuesta(); y desabilita el obserable collection mientras se hace la busqueda
        /// </summary>
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
                    SelecionarPokemonDisponible = false;// lisata bloqueada
                    comprobarRespuesta();// comprueba la respuesta
                }
            }
        }
        /// <summary>
        /// Habilita o desabilita el obserbable collection (necesario si la api tarda en responder)
        /// </summary>
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
        /// <summary>
        /// Pokemon respuesta pasa el nombre a upper por motivos de estetica
        /// </summary>
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
                    _pokemonRespuesta.name = _pokemonRespuesta.name.ToUpper(); // nombre a mayusculas
                    OnPropertyChanged(nameof(PokemonRespuesta));
                }
            }
        }
        /// <summary>
        /// Constante para inidcar el numero de pokemons que se piden a la api
        /// </summary>
        private const int NUMERO_DE_POKEMONS = 15;
        /// <summary>
        /// Pide pokemons esta funcion se accede al comparar dos pokemons
        /// </summary>
        public async void pedirPokemon()
        {
            NumColumnas = NUMERO_DE_POKEMONS / 3;
            List<Pokemon> list = new List<Pokemon>();
            try { list = await Service.ServiceAdivinaElPokemon.ObtenerListadoDePokemonsDTO(NUMERO_DE_POKEMONS); }
            catch (Exception ex) { await Shell.Current.GoToAsync("//LobbyPage"); }
            int idAleatorio = random.Next(0, NUMERO_DE_POKEMONS - 1); //coje un id aleatorio
            ListadoDePokemons = new ObservableCollection<Pokemon>(list);
            PokemonRespuesta = ListadoDePokemons[idAleatorio]; // seleciona un poekmon aleatorio como respuesta
            SelecionarPokemonDisponible = true; // habilita el obserbable collection
        }
        /// <summary>
        /// tengo que poner otro metodo por que como no tiene que hacerla comprobacion entre pokemons
        /// y  hay cualquier delay al selecionar pokemon selecionar muchos al mismo tiempo lo que causa problemas 
        /// tampoco lo puedo poner en pedirPokemon por que en la comapracion da tiempo ha selecionar varios
        /// </summary>
        public async void pedirPokemonComodin()
        {
            SelecionarPokemonDisponible = false; // desabilita el obserbable mientras espera a la api
            pedirPokemon();
            ComodinesRestantes--; // resta los comodines
        }


        /// <summary>
        /// Compara el pokemon selecionado con el pokemon respuesta
        /// </summary>
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
        /// <summary>
        /// Si el numero de comodines es mayor a _comodinesRestantes el boton sigue activo
        /// </summary>
        /// <returns>bool indicando si el boton esta activo</returns>
        private bool usarComodinCommandActivo()
        {
            bool res = true;

            if (_comodinesRestantes <= 0) { res = false; }
            return res;
        }
        /// <summary>
        /// Command comodin acutualiza la lista sin perder puntos
        /// </summary>
        public DelegateCommand usarComodinCommand { get; }
        #endregion
        #region implementacion de la clase 
        /// <summary>
        /// Constructor inicializa el hub el juego y el contador
        /// </summary>
        public MainPageVM()
        {
            //hub
            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7211/gameHub")
                .WithAutomaticReconnect()
                .Build();

            conexionServidor();
            //recivir contador de aciertos
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
        /// <summary>
        /// Nav a lobby
        /// </summary>
        private async void juegoTerminado()
        {

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    salirLobby();//sale del lobby automaticamente
                    await App.Current.MainPage.DisplayAlert("¡Se acabó el tiempo!", ResultadoPartida, "OK"); // mensaje salida
                    await Shell.Current.GoToAsync("//LobbyPage");
                }
                catch (Exception ex) { await Shell.Current.GoToAsync("//LobbyPage"); }

            });
        }
        /// <summary>
        /// Tengo que ponerlo por separado para llamarlo desde el code behind en caso de que el juagdor cierre el programa
        /// </summary>
        public async void salirLobby()
        {
            await _connection.InvokeAsync("SalirLobby");
        }
        #endregion
    }
}
