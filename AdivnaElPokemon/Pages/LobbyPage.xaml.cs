namespace AdivnaElPokemon.Pages;

public partial class LobbyPage : ContentPage
{
    LobbyVM _viewModel;
    public LobbyPage()
    {
        _viewModel = new LobbyVM();
        BindingContext = _viewModel;
        InitializeComponent();
    }

}