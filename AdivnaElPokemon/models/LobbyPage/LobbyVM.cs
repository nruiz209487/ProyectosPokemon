using AdivnaElPokemon;
using AdivnaElPokemon.models.utils;
using AdivnaElPokemon.Pages;
using Microsoft.AspNetCore.SignalR.Client;
using System.ComponentModel;
using System.Windows.Input;

public class LobbyVM : ClsINotify
{
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
    public DelegateCommand buscarPartidaCommand { get; }
    public DelegateCommand abandonarColaCommand { get; }
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
                BotonBusquedaPulsado = true;
                App.Current.MainPage.Navigation.PushAsync(new GamePage());
            });
        });


        conexionServidor();
        abandonarColaCommand = new DelegateCommand(abandonarCola, abandonarColaCommandActivo);
        buscarPartidaCommand = new DelegateCommand(buscarPartida, buscarPartidaCommandActivo);
    }



    private async void conexionServidor()
    {
        try { await _connection.StartAsync(); } catch (Exception ex) { App.Current.MainPage.Navigation.PushAsync(new LobbyPage()); }

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
        catch (Exception ex) { App.Current.MainPage.Navigation.PushAsync(new LobbyPage()); }
    }
}
