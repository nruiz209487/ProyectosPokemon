using UI.Models;
using UI.Pages;

namespace UI
{
    public partial class MainPage : ContentPage
    {
        MainPageVM? objVm;

        public MainPage()
        {
            try
            {
                objVm = new MainPageVM();
                BindingContext = objVm;
                InitializeComponent();
            }
            catch (Exception)
            {
                Navigation.PushAsync(new ErrorPage());
            }
        }

    }
}
