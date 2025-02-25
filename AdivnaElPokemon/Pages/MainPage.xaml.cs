

using AdivnaElPokemon.models;

namespace AdivnaElPokemon
{
    public partial class MainPage : ContentPage
    {

        MainPageVM _viewModel;
        public MainPage()
        {
            _viewModel = new MainPageVM();
            BindingContext = _viewModel;
            InitializeComponent();

        }
    }

}
