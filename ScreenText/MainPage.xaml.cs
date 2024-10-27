using ScreenText.ViewModels;

namespace ScreenText
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel(); // Привязка ViewModel
        }
    }
}
