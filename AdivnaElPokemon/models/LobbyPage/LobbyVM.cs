using AdivnaElPokemon;
using AdivnaElPokemon.models.utils;
using AdivnaElPokemon.Pages;
using Microsoft.AspNetCore.SignalR.Client;
using System.ComponentModel;
using System.Windows.Input;

public class LobbyVM : ClsINotify
{
    #region Propiedades
    private HubConnection _connection;
    /// <summary>
    /// controla si los botones que interactuan con el hub estan activos o no
    /// </summary>
    private bool _botonPulsado = true;
    public bool BotonBusquedaPulsado
    {
        get { return _botonPulsado; }
        set
        {
            _botonPulsado = value;
            buscarPartidaCommand.RaiseCanExecuteChanged();
            abandonarColaCommand.RaiseCanExecuteChanged();
            OnPropertyChanged(nameof(BotonSalirColaPulsado));
        }
    }
    /// <summary>
    /// quiero que si el salir  cola  este activo si el entrar cola no lo este 
    /// </summary>
    public bool BotonSalirColaPulsado
    {
        get { return !_botonPulsado; }

    }
    /// <summary>
    /// Si el lobby esta lleno hace visible el labbel
    /// </summary>
    private bool _lobbyLleno = false;
    public bool LobbyLleno
    {
        get { return _lobbyLleno; }
        set
        {
            _lobbyLleno = value;
            OnPropertyChanged(nameof(LobbyLleno));
        }
    }

    /// <summary>
    /// cuenta el numero de jugadores en la cola
    /// </summary>
    private int jugadoresCola = 0;
    public int JugadoresCola
    {
        get { return jugadoresCola; }
        set
        {
            jugadoresCola = value;
            OnPropertyChanged(nameof(JugadoresCola));
        }
    }

    private bool errorConexion = false;
    /// <summary>
    /// Command que controla el boton buscar partida
    /// </summary>
    public DelegateCommand buscarPartidaCommand { get; }
    /// <summary>
    /// Command que controla el boton abandonar cola
    /// </summary>
    public DelegateCommand abandonarColaCommand { get; }
    #endregion
    #region constructor y funciones
    public LobbyVM()
    {
        _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7211/gameHub")
                .WithAutomaticReconnect()
                .Build();

        _connection.On("ReceiveIniciarJuego", async () =>
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (_botonPulsado == false) // solo entra en partida si esta buscando partida
                {
                    BotonBusquedaPulsado = true;
                    App.Current.MainPage.Navigation.PushAsync(new GamePage());
                }

            });
        });

        _connection.On<int>("ReceiveLobby", (numJugadores) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (numJugadores > 2) // si hay mas de dos jugadores lanza la advertencia
                {
                    LobbyLleno = true;
                }
                JugadoresCola = numJugadores;
            });
        });

        conexionServidor();
        abandonarColaCommand = new DelegateCommand(abandonarCola, abandonarColaCommandActivo);
        buscarPartidaCommand = new DelegateCommand(buscarPartida, buscarPartidaCommandActivo);
    }
    private async void conexionServidor()
    {
        try { await _connection.StartAsync(); } catch (Exception ex) { if (!errorConexion) { errorConexion = true; await App.Current.MainPage.DisplayAlert("¡Ocurrio un error inesperado!", "No se pudo acceder al servidor, intentalo mas tarde", "OK"); App.Current.MainPage.Navigation.PushAsync(new LobbyPage()); } }

    }
    /// <summary>
    /// Controla el command  buscarPartidaCommand y si esta activo
    /// </summary>
    /// <returns> boton activo</returns>
    private bool buscarPartidaCommandActivo()
    {
        return BotonBusquedaPulsado;
    }
    /// <summary>
    /// Controla el command  abandonarColaCommand y si esta activo
    /// </summary>
    /// <returns> boton activo</returns>
    private bool abandonarColaCommandActivo()
    {
        return BotonSalirColaPulsado;
    }
    /// <summary>
    /// Abandona la cola en el hub
    /// </summary>
    private async void abandonarCola()
    {
        BotonBusquedaPulsado = true;
        try
        {
            await _connection.InvokeAsync("SalirLobby");
        }
        catch (Exception ex) { App.Current.MainPage.Navigation.PushAsync(new LobbyPage()); }
    }
    /// <summary>
    /// entra en cola en el hub
    /// </summary>
    private async void buscarPartida()
    {
        BotonBusquedaPulsado = false;
        try
        {
            await _connection.InvokeAsync("SendEntarLobby");
        }
        catch (Exception ex)
        {
            if (!errorConexion) { errorConexion = true; await App.Current.MainPage.DisplayAlert("¡Ocurrio un error inesperado al buscar partida!", "No se pudo acceder al servidor, intentalo mas tarde", "OK"); App.Current.MainPage.Navigation.PushAsync(new LobbyPage()); }
        }
    }
    #endregion
}
