using System;
using System.Timers; // Importar para usar System.Timers.Timer
using AdivnaElPokemon.models;
using Microsoft.Maui.Dispatching;

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
