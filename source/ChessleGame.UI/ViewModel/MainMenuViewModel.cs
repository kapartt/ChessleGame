using ChessleGame.UI.Enums;
using ChessleGame.UI.Utils;
using Egor92.MvvmNavigation;
using Egor92.MvvmNavigation.Abstractions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace ChessleGame.UI.ViewModel
{
    public class MainMenuViewModel : ViewModelBase, INavigatedToAware
    {
        private readonly INavigationManager _navigationManager;
        private GameTypeVm _gameType;

        public MainMenuViewModel(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
            _gameType = GameTypeVm.SinglePlayer;
        }

        public GameTypeVm GameType
        {
            get => _gameType;
            set
            {
                if (Equals(_gameType, value)) return;
                _gameType = value;
                RaisePropertyChanged(nameof(GameType));
            }
        }

        private RelayCommand _launchGameCommand;

        public ICommand LaunchGameCommand => _launchGameCommand ??= new RelayCommand(LaunchGame);

        public void LaunchGame()
        {
            var parameters = new object[2];
            const bool isNewGame = true;

            parameters[0] = GameType;
            parameters[1] = isNewGame;

            _navigationManager.Navigate(UserControlKeys.Game, parameters);
        }

        public void OnNavigatedTo(object arg)
        {
            //_navigationManager.Navigate("Game");
        }
    }
}
