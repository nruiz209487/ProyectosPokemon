using AdivnaElPokemon.Pages;

namespace AdivnaElPokemon
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new LobbyPage());
        }
    }
}
