using AdivnaElPokemon;
using AdivnaElPokemon.Pages;
using Microsoft.AspNetCore.SignalR.Client;
using System.ComponentModel;
using System.Windows.Input;

public class LobbyVM : INotifyPropertyChanged
{
    private HubConnection _connection;
    private bool _botonPulsado = true;
    public bool BotonBusquedaPulsado
    {
        get { return _botonPulsado; }
        set
        {
            _botonPulsado = value;
            OnPropertyChanged(nameof(BotonBusquedaPulsado));
            OnPropertyChanged(nameof(BotonColaPulsado));
        }
    }

    public bool BotonColaPulsado
    {
        get { return !_botonPulsado; }

    }
    public ICommand buscarPartidaCommand { get; }
    public ICommand abandonarColaCommand { get; }
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
        abandonarColaCommand = new Command(abandonarCola);
        buscarPartidaCommand = new Command(buscarPartida);
    }



    private async void conexionServidor()
    {
        await _connection.StartAsync();
    }
    private async void abandonarCola()
    {
        BotonBusquedaPulsado = true;
        await _connection.InvokeAsync("SalirLobby");
    }
    private async void buscarPartida()
    {
        BotonBusquedaPulsado = false;
        await _connection.InvokeAsync("SendEntarLobby");
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
