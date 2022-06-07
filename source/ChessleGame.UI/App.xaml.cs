using ChessleGame.UI.Utils;
using ChessleGame.UI.ViewModel;
using ChessleGame.UI.Views;
using Egor92.MvvmNavigation;
using System.Windows;

namespace ChessleGame.UI
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var window = new MainWindow();

            var navigationManager = new NavigationManager(window);
            navigationManager.Register<GameView>(UserControlKeys.Game, () => new GameViewModel(navigationManager));
            navigationManager.Register<MainMenuView>(UserControlKeys.MainMenu, () => new MainMenuViewModel(navigationManager));

            var mainViewModel = new MainViewModel(navigationManager);
            window.DataContext = mainViewModel;
            window.Show();
        }
    }
}
