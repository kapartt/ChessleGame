using ChessleGame.UI.ViewModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChessleGame.UI.Views
{
    public partial class GameView : UserControl
    {
        public GameView()
        {
            InitializeComponent();
        }

        private void MouseUpOnChessboardCommand(object sender, MouseButtonEventArgs e)
        {
            var gameVm = (GameViewModel)DataContext;
            var pos = e.GetPosition(Chessboard);
            gameVm.ClickOnChessboardCommand(pos.X, pos.Y, false);
        }

        private void MouseDownOnChessboardCommand(object sender, MouseButtonEventArgs e)
        {
            var gameVm = (GameViewModel)DataContext;
            var pos = e.GetPosition(Chessboard);
            gameVm.ClickOnChessboardCommand(pos.X, pos.Y, true);
        }

        private void ScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;

            if (e.VerticalChange == 0)
            {
                scrollViewer?.ScrollToBottom();
            }
        }
    }
}
