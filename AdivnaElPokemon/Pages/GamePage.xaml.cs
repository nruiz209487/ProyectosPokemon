using AdivnaElPokemon.models.MainPage;

namespace AdivnaElPokemon.Pages;

public partial class GamePage : ContentPage
{
    MainPageVM _viewModel;
    public GamePage()
    {
        this.Disappearing += (s, e) => _viewModel.salirLobby(); //es para que si un jugador se sale del programa salga de la cola
        InitializeComponent();
        _viewModel = new MainPageVM();
        BindingContext = _viewModel;
    }



}