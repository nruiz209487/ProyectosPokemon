

using AdivnaElPokemon.models.MainPage;

namespace AdivnaElPokemon.Pages;

public partial class GamePage : ContentPage
{
    MainPageVM _viewModel;
    public GamePage()
    {
        InitializeComponent();
        _viewModel = new MainPageVM();
        BindingContext = _viewModel;
    }
}