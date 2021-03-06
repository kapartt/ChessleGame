using ChessleGame.Algo;
using ChessleGame.Algo.Entities;
using ChessleGame.UI.Enums;
using ChessleGame.UI.Model;
using ChessleGame.UI.Utils;
using Egor92.MvvmNavigation;
using Egor92.MvvmNavigation.Abstractions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ChessleGame.UI.ViewModel
{
    public class GameViewModel : ViewModelBase, INavigatedToAware
    {
        private const double BoardTotalSize = 560;
        private const double BoardBoundSize = 20;
        private readonly double _squareSize;
        private readonly INavigationManager _navigationManager;

        private List<string> _clearedSquareBrushes;
        private BitmapImage _boardImage;
        private bool _isPreviouslyChoosedSquare;
        private int _previouslyChoosedSquareId;
        private int _clickedHorizontal, _clickedVertical;
        private BitmapImage[] _piecesImages;
        private string[] _squareBrushes;
        private bool[] _isContainsPiece;
        private PositionsHistory _history;
        private PgnVariant _rightAnswer;

        private ObservableCollection<ChessleSubmissionVm> _submissionsList;

        public GameViewModel(NavigationManager navigationManager)
        {
            InitClearBrushes();
            InitBoardAndHistory();
            ClearSquareBrushes();

            _navigationManager = navigationManager;
            _squareSize = (BoardTotalSize - 2 * BoardBoundSize) / ConstantsHelper.SquaresInLineCount;
            _isPreviouslyChoosedSquare = false;

            _submissionsList = new ObservableCollection<ChessleSubmissionVm>();
            _submissionsList.Add(new ChessleSubmissionVm());
            _rightAnswer = new ChessleGenerator(@"database.pgn").GetRandomVariant();
        }

        public void OnNavigatedTo(object arg)
        {
            //_navigationManager.Navigate("MainMenu");
        }

        #region Properties

        public BitmapImage BoardImage
        {
            get => _boardImage;
            set
            {
                if (Equals(_boardImage, value)) return;
                _boardImage = value;
                RaisePropertyChanged(nameof(BoardImage));
            }
        }

        public BitmapImage[] PiecesImages
        {
            get => _piecesImages;
            set
            {
                if (Equals(_piecesImages, value)) return;
                _piecesImages = value;
                RaisePropertyChanged(nameof(PiecesImages));
            }
        }

        public string[] SquareBrushes
        {
            get => _squareBrushes;
            set
            {
                if (Equals(_squareBrushes, value)) return;
                _squareBrushes = value;
                RaisePropertyChanged(nameof(SquareBrushes));
            }
        }

        public ObservableCollection<ChessleSubmissionVm> SubmissionsList
        {
            get => _submissionsList;
            set
            {
                if (Equals(_submissionsList, value)) return;
                _submissionsList = value;
                RaisePropertyChanged(nameof(SubmissionsList));
            }
        }


        #endregion

        #region Commands

        private RelayCommand _toMainMenuCommand;

        public ICommand ToMainMenuCommand => _toMainMenuCommand ??= new RelayCommand(ToMainMenu);

        public void ToMainMenu()
        {
            _navigationManager.Navigate(UserControlKeys.MainMenu);
        }

        private RelayCommand _playAgainCommand;

        public ICommand PlayAgainCommand => _playAgainCommand ??= new RelayCommand(PlayAgain);

        public void PlayAgain()
        {
            _navigationManager.Navigate(UserControlKeys.Game);
        }

        private RelayCommand _sendSubmissionCommand;

        public ICommand SendSubmissionCommand => _sendSubmissionCommand ??= new RelayCommand(SendSubmission);

        public void SendSubmission()
        {
            var lastSubmisison = SubmissionsList.LastOrDefault();

            if (lastSubmisison == null) return;

            if (lastSubmisison.CurrentMove == ChessleSubmissionVm.MovesCount)
            {
                var bullsCows = BullsAndCowsCounter.GetBullsAndCows(lastSubmisison, _rightAnswer);

                lastSubmisison.FillColorsCheckSubmission(bullsCows);
                SubmissionsList = new ObservableCollection<ChessleSubmissionVm>(SubmissionsList.ToList()) { new ChessleSubmissionVm() };
                InitBoardAndHistory();
                return;
            }

            for (int i = lastSubmisison.CurrentMove; i < ChessleSubmissionVm.MovesCount; i++)
            {
                lastSubmisison.MoveColors[i] = ChessleColors.RedMove;
            }

            SubmissionsList = new ObservableCollection<ChessleSubmissionVm>(SubmissionsList.ToList());
        }

        private RelayCommand _undoMoveCommand;

        public ICommand UndoMoveCommand => _undoMoveCommand ??= new RelayCommand(UndoMove);

        public void UndoMove()
        {
            if (_history.PositionsList.Count <= 1) return;

            _history.PositionsList.RemoveAt(_history.PositionsList.Count - 1);

            SetPiecesFromPosition(_history.PositionsList.LastOrDefault());
            UpdateSubmissionOnUndoMove();
            ClearSquareBrushes();
            _isPreviouslyChoosedSquare = false;
        }

        public void ClickOnChessboardCommand(double x, double y, bool isDown)
        {
            while (true)
            {
                if (SubmissionsList.LastOrDefault()?.CurrentMove == ChessleSubmissionVm.MovesCount)
                {
                    return;
                }

                if (x > BoardTotalSize - BoardBoundSize || x < BoardBoundSize || y > BoardTotalSize - BoardBoundSize || y < BoardBoundSize)
                {
                    return;
                }

                var horizontal = (int)((x - BoardBoundSize) / _squareSize);
                var vertical = (int)((y - BoardBoundSize) / _squareSize);

                if (isDown)
                {
                    _clickedHorizontal = horizontal;
                    _clickedVertical = vertical;
                }
                else if (horizontal == _clickedHorizontal && vertical == _clickedVertical)
                {
                    var choosedSquareId = GetIdFromClickedHorizontalVertical(horizontal, vertical);
                    var position = _history.PositionsList.Last();
                    var possibleMoves = position.GetPossibleMovesEndSquareIds(choosedSquareId);

                    switch (_isPreviouslyChoosedSquare)
                    {
                        case false when _isContainsPiece[choosedSquareId]:
                            {
                                if (possibleMoves.Count == 0) return;

                                _previouslyChoosedSquareId = choosedSquareId;
                                _isPreviouslyChoosedSquare = true;
                                BrushNewSelectedSquare(choosedSquareId);
                                break;
                            }
                        case true when _isContainsPiece[_previouslyChoosedSquareId]:
                            {
                                ClearSquareBrushes();
                                _isPreviouslyChoosedSquare = false;

                                if (_previouslyChoosedSquareId != choosedSquareId)
                                    if (position.IsLegalMove(_previouslyChoosedSquareId, choosedSquareId))
                                    {
                                        UpdateHistoryAfterNextMove(_previouslyChoosedSquareId, choosedSquareId, out var moveNotation);
                                        SetPiecesFromPosition(_history.PositionsList.Last());
                                        _isContainsPiece[choosedSquareId] = true;
                                        _isContainsPiece[_previouslyChoosedSquareId] = false;
                                        UpdateSubmissionOnNewMove(moveNotation);
                                    }
                                    else if (_isContainsPiece[choosedSquareId])
                                    {
                                        continue;
                                    }
                                break;
                            }
                    }
                }
                break;
            }
        }

        #endregion

        #region Private methods

        private void InitClearBrushes()
        {
            _clearedSquareBrushes = new List<string>();

            for (int i = 0; i < ConstantsHelper.TotalSquaresCount; i++)
            {
                _clearedSquareBrushes.Add(ChessleColors.Transparent);
            }
        }

        private void InitBoardAndHistory()
        {
            BoardImage = GetBitmapImage(PicturesDirectories.Chessboard);

            var startPos = PositionVm.GetStartPosition();

            _history = new PositionsHistory();
            _history.PositionsList.Add(startPos);
            SetPiecesFromPosition(startPos);
        }

        private void SetPiecesFromPosition(PositionVm position)
        {
            if (position == null) return;

            var piecesImagesList = new List<BitmapImage>();
            _isContainsPiece = new bool[ConstantsHelper.TotalSquaresCount];

            for (int i = 0; i < ConstantsHelper.TotalSquaresCount; i++)
            {
                var bitmapImage = GetPieceBitmapImage(position.PiecesOnBoard[i]);
                piecesImagesList.Add(bitmapImage);
                _isContainsPiece[i] = position.PiecesOnBoard[i] != PieceTypeVm.Empty;
            }

            PiecesImages = piecesImagesList.ToArray();
        }

        private BitmapImage GetPieceBitmapImage(PieceTypeVm pieceType)
        {
            if (pieceType == PieceTypeVm.Empty) return new BitmapImage();

            var relativeDir = PicturesDirectories.GetDirectoryFromPieceName(pieceType);
            return GetBitmapImage(relativeDir);
        }

        private BitmapImage GetBitmapImage(string relativeDir)
        {
            var uri = new Uri(Environment.CurrentDirectory + relativeDir);
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = uri;
            bitmapImage.EndInit();

            return bitmapImage;
        }

        private int GetIdFromClickedHorizontalVertical(int horizontal, int vertical)
        {
            return ConstantsHelper.SquaresInLineCount * vertical + horizontal;
        }

        private void UpdateHistoryAfterNextMove(int beginSquareId, int endSquareId, out string moveNotation)
        {
            moveNotation = string.Empty;
            if (_history.PositionsList.Count == 0) return;

            var positionAfterMove = _history.PositionsList.Last().GetNewPositionAfterMove(beginSquareId, endSquareId, out moveNotation);
            _history.PositionsList.Add(positionAfterMove);
        }

        private void BrushNewSelectedSquare(int selectedSquareId)
        {
            var newBrushesList = new List<string>();

            for (int i = 0; i < SquareBrushes.Length; i++)
            {
                newBrushesList.Add(i == selectedSquareId ? GetSelectedSquare(selectedSquareId) : ChessleColors.Transparent);
            }

            SquareBrushes = newBrushesList.ToArray();
        }

        private string GetSelectedSquare(int squareId)
        {
            var horizontal = squareId / ConstantsHelper.SquaresInLineCount;
            var vertical = squareId % ConstantsHelper.SquaresInLineCount;
            return (horizontal + vertical) % 2 == 0 ? ChessleColors.LightSelectedSquare : ChessleColors.DarkSelectedSquare;
        }

        private void ClearSquareBrushes()
        {
            SquareBrushes = _clearedSquareBrushes.ToArray();
        }

        private void UpdateSubmissionOnNewMove(string moveNotation)
        {
            var lastSubmission = SubmissionsList.LastOrDefault();
            if (lastSubmission == null) return;

            lastSubmission.MovesNotation[lastSubmission.CurrentMove] = moveNotation;
            lastSubmission.FontSize[lastSubmission.CurrentMove] =
                moveNotation.Length <= ChessleSubmissionVm.MaxNotationLengthForDefaultFontSize
                    ? ChessleSubmissionVm.DefaultFontSize
                    : ChessleSubmissionVm.SmallerFontSize;
            lastSubmission.CurrentMove++;
            lastSubmission.FillTransparent();

            SubmissionsList = new ObservableCollection<ChessleSubmissionVm>(SubmissionsList.ToList());
        }

        private void UpdateSubmissionOnUndoMove()
        {
            var lastSubmission = SubmissionsList.LastOrDefault();
            if (lastSubmission == null || lastSubmission.CurrentMove == 0) return;

            lastSubmission.MovesNotation[--lastSubmission.CurrentMove] = string.Empty;
            lastSubmission.FillTransparent();

            SubmissionsList = new ObservableCollection<ChessleSubmissionVm>(SubmissionsList.ToList());
        }

        #endregion
    }
}
