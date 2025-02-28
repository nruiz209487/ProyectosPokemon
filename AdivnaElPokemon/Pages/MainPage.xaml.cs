using AdivnaElPokemon.models.MainPage;


namespace AdivnaElPokemon
{
    public partial class MainPage : ContentPage
    {
        MainPageVM _viewModel;

        public MainPage()
        {
            InitializeComponent();
            _viewModel = new MainPageVM();
            BindingContext = _viewModel;

        }


    }
}
