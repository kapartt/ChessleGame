using ChessleGame.UI.Utils;
using Egor92.MvvmNavigation;
using Egor92.MvvmNavigation.Abstractions;
using GalaSoft.MvvmLight;

namespace ChessleGame.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(NavigationManager navigationManager)
        {
            navigationManager.Navigate(UserControlKeys.MainMenu);
        }
    }
}
