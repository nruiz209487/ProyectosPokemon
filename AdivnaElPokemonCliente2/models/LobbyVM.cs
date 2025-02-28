using AdivnaElPokemon;
using Microsoft.AspNetCore.SignalR.Client;
using System.ComponentModel;
using System.Windows.Input;

public class LobbyVM : INotifyPropertyChanged
{
    private HubConnection _connection;
    private bool _botonPulsado = true;
    public bool BotonPulsado
    {
        get { return _botonPulsado; }
        set
        {
            _botonPulsado = value;
            OnPropertyChanged(nameof(BotonPulsado));
        }
    }
    public ICommand buscarPartidaCommand { get; }

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
                App.Current.MainPage.Navigation.PushAsync(new MainPage());
            });
        });


        conexionServidor();
        buscarPartidaCommand = new Command(buscarPartida);
    }

    private async void conexionServidor()
    {
        await _connection.StartAsync();
    }

    private async void buscarPartida()
    {
        BotonPulsado = false;
        await _connection.InvokeAsync("SendEntarLobby");
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
