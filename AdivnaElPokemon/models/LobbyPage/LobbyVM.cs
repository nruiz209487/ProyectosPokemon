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
    public bool BotonSalirColaPulsado
    {
        get { return !_botonPulsado; }

    }
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
    private bool errorConexion;
    public DelegateCommand buscarPartidaCommand { get; }
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
                if (_botonPulsado == false)
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
                LobbyLleno = true;
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
    private bool buscarPartidaCommandActivo()
    {

        return BotonBusquedaPulsado;
    }
    private bool abandonarColaCommandActivo()
    {

        return BotonSalirColaPulsado;
    }
    private async void abandonarCola()
    {
        BotonBusquedaPulsado = true;
        try
        {
            await _connection.InvokeAsync("SalirLobby");
        }
        catch (Exception ex) { App.Current.MainPage.Navigation.PushAsync(new LobbyPage()); }
    }
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
