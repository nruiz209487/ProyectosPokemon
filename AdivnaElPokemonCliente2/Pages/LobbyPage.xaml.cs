namespace AdivnaElPokemon.Pages;

public partial class LobbyPage : ContentPage
{
    LobbyVM lobbyVM;
    public LobbyPage()
    {
        lobbyVM = new LobbyVM();
        BindingContext = lobbyVM;
        InitializeComponent();
    }

}