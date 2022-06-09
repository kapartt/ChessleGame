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
        private const string DatabaseDir = @"database.csv";
        private const int MaxEngineIterations = 100;
        private const string SendSubmissionText = "Отправить решение";
        private const string NewMoveText = "Начать ход №{0}";
        private const string ShowEngineMoves = "Ответы компьютера";
        private const string ShowYourMoves = "Свои ответы";

        private readonly double _squareSize;
        private readonly INavigationManager _navigationManager;
        private PgnVariant _rightAnswer;
        private Dictionary<int, PositionVm> _guessedPositions;

        private List<string> _clearedSquareBrushes;
        private BitmapImage _boardImage;
        private bool _isPreviouslyChoosedSquare;
        private int _previouslyChoosedSquareId;
        private int _clickedHorizontal, _clickedVertical;
        private BitmapImage[] _piecesImages;
        private string[] _squareBrushes;
        private bool[] _isContainsPiece;
        private PositionsHistory _history;
        private bool _isSolved;
        private string _gameInfo;
        private ObservableCollection<ChessleSubmissionVm> _submissionsList;
        private GameTypeVm _gameTypeVm;
        private GameState _gameState;
        private string _sendSubmissionButtonText;

        public GameViewModel(NavigationManager navigationManager)
        {
            _isContainsPiece = new bool[ConstantsHelper.TotalSquaresCount];
            InitClearBrushes();
            InitBoardAndHistory();
            ClearSquareBrushes();

            _navigationManager = navigationManager;
            _squareSize = (BoardTotalSize - 2 * BoardBoundSize) / ConstantsHelper.SquaresInLineCount;
            _isPreviouslyChoosedSquare = false;

            _submissionsList = new ObservableCollection<ChessleSubmissionVm> {new ChessleSubmissionVm()};
            _rightAnswer = new ChessleGenerator(DatabaseDir).GetRandomVariant();
            _isSolved = false;
            _gameInfo = string.Empty;
            _guessedPositions = new Dictionary<int, PositionVm>();
            _gameState = new GameState();
            _sendSubmissionButtonText = SendSubmissionText;
        }

        public void OnNavigatedTo(object arg)
        {
            var args = arg as object[];
            _gameTypeVm = (GameTypeVm)args[0];
            var isNewGame = (bool)args[1];

            if (_gameTypeVm == GameTypeVm.SinglePlayer) return;

            if (isNewGame)
            {
                _gameState.EngineSubmissions = GetEngineSubmissions();
                _gameState.RightAnswer = _rightAnswer;
                _gameState.GameInfo = _rightAnswer.GameInfo;
                return;
            }

            _gameState = args[2] as GameState;
            if (_gameState == null) return;

            _rightAnswer = _gameState.RightAnswer;
            _guessedPositions = _gameState.GuessedPositions;

            if (_gameState.PlayerSubmissions.Count >= 1)
            {
                _isSolved = _gameState.PlayerSubmissions.Last().IsSolved;
            }

            if (_isSolved)
            {
                GameInfo = _rightAnswer.GameInfo;
                SetPiecesFromPosition(_gameState.EndingPositionVm);
            }

            if (_gameState.IsUserMove)
            {
                SendSubmissionButtonText = _isSolved ? ShowEngineMoves : SendSubmissionText;
                SubmissionsList = new ObservableCollection<ChessleSubmissionVm>(_gameState.PlayerSubmissions);
            }
            else
            {
                SendSubmissionButtonText = _isSolved ? ShowYourMoves : string.Format(NewMoveText, _gameState.CurrentMove + 1);
                if (_isSolved)
                {
                    SubmissionsList = new ObservableCollection<ChessleSubmissionVm>(_gameState.EngineSubmissions);
                    return;
                }

                SubmissionsList = new ObservableCollection<ChessleSubmissionVm>();

                for (int i = 0; i < Math.Min(_gameState.CurrentMove, _gameState.EngineSubmissions.Count); i++)
                {
                    var submission = new ChessleSubmissionVm(_gameState.EngineSubmissions[i]);
                    if (!_isSolved) submission.ClearText();
                    SubmissionsList.Add(submission);
                }
            }
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

        public string GameInfo
        {
            get => _gameInfo;
            set
            {
                if (Equals(_gameInfo, value)) return;
                _gameInfo = value;
                RaisePropertyChanged(nameof(GameInfo));
            }
        }

        public string SendSubmissionButtonText
        {
            get => _sendSubmissionButtonText;
            set
            {
                if (Equals(_sendSubmissionButtonText, value)) return;
                _sendSubmissionButtonText = value;
                RaisePropertyChanged(nameof(SendSubmissionButtonText));
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
            var args = new object[2];
            const bool isNewGame = true;

            args[0] = _gameTypeVm;
            args[1] = isNewGame;

            _navigationManager.Navigate(UserControlKeys.Game, args);
        }

        private RelayCommand _sendSubmissionCommand;

        public ICommand SendSubmissionCommand => _sendSubmissionCommand ??= new RelayCommand(SendSubmission);

        public void SendSubmission()
        {
            object[] args;

            if (_gameTypeVm != GameTypeVm.SinglePlayer)
            {
                args = new object[3];
                args[0] = _gameTypeVm;
                args[1] = false;
                args[2] = _gameState;

                if (!_gameState.IsUserMove)
                {
                    _gameState.IsUserMove = true;
                    if (!_isSolved) _gameState.CurrentMove++;
                    _navigationManager.Navigate(UserControlKeys.Game, args);
                    return;
                }
            }
            else if (_isSolved) return;

            var lastSubmission = SubmissionsList.LastOrDefault();

            if (lastSubmission == null) return;

            if (lastSubmission.CurrentMove == ChessleSubmissionVm.MovesCount)
            {
                var bullsCows = BullsAndCowsCounter.GetBullsAndCows(lastSubmission, _rightAnswer);

                lastSubmission.FillColorsCheckSubmission(bullsCows);
                _isSolved = lastSubmission.IsSolved;

                SubmissionsList = new ObservableCollection<ChessleSubmissionVm>(SubmissionsList.ToList());


                if (!_isSolved)
                {
                    foreach (var success in lastSubmission.GetSuccessfulMoveIndexes())
                    {
                        _guessedPositions[success] = _history.PositionsList[success + 1];

                        if (success == ChessleSubmissionVm.MovesCount - 1)
                        {
                            _gameState.EndingPositionVm = new PositionVm(_guessedPositions[success]);
                        }
                    }

                    SubmissionsList.Add(new ChessleSubmissionVm());
                    InitBoardAndHistory();
                }
                else
                {
                    GameInfo = _rightAnswer.GameInfo;
                }

                if (_gameTypeVm != GameTypeVm.SinglePlayer)
                {
                    if (_gameState.IsUserMove)
                    {
                        _gameState.IsUserMove = false;
                        _gameState.GuessedPositions = _guessedPositions;
                        _gameState.PlayerSubmissions = SubmissionsList.ToList();
                    }

                    args = new object[3];
                    args[0] = _gameTypeVm;
                    args[1] = false;
                    args[2] = _gameState;
                    _navigationManager.Navigate(UserControlKeys.Game, args);
                }
            }

            for (int i = lastSubmission.CurrentMove; i < ChessleSubmissionVm.MovesCount; i++)
            {
                lastSubmission.MoveColors[i] = ChessleColors.RedMove;
            }

            SubmissionsList = new ObservableCollection<ChessleSubmissionVm>(SubmissionsList.ToList());
        }

        private RelayCommand _undoMoveCommand;

        public ICommand UndoMoveCommand => _undoMoveCommand ??= new RelayCommand(UndoMove);

        public void UndoMove()
        {
            if (_isSolved) return;

            if (_history.PositionsList.Count <= 1) return;

            _history.PositionsList.RemoveAt(_history.PositionsList.Count - 1);

            SetPiecesFromPosition(_history.PositionsList.LastOrDefault());
            UpdateSubmissionOnUndoMove();
            ClearSquareBrushes();
            _isPreviouslyChoosedSquare = false;
        }

        private RelayCommand _playGuessedMovesCommand;

        public ICommand PlayGuessedMovesCommand => _playGuessedMovesCommand ??= new RelayCommand(PlayGuessedMoves);

        public void PlayGuessedMoves()
        {
            if (_isSolved) return;

            var firstGuessedMovesCount = GetFirstGuessedMovesCount();

            if (firstGuessedMovesCount == 0) return;

            SubmissionsList.Remove(SubmissionsList.Last());
            SubmissionsList.Add(new ChessleSubmissionVm());

            InitBoardAndHistory();
            ClearSquareBrushes();
            _isPreviouslyChoosedSquare = false;

            for (int i = 0; i < firstGuessedMovesCount; i++)
            {
                var position = _guessedPositions[i];
                _history.PositionsList.Add(position);
                UpdateSubmissionOnNewMove(position.LastMoveMotation);

                if (i == firstGuessedMovesCount - 1)
                {
                    SetPiecesFromPosition(position);
                }
            }
        }

        public void ClickOnChessboardCommand(double x, double y, bool isDown)
        {
            if (_isSolved) return;

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
                                        UpdateHistoryAfterNextMove(_previouslyChoosedSquareId, choosedSquareId);
                                        SetPiecesFromPosition(_history.PositionsList.Last());
                                        UpdateSubmissionOnNewMove(_history.PositionsList.Last().LastMoveMotation);
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

        private void UpdateHistoryAfterNextMove(int beginSquareId, int endSquareId)
        {
            if (_history.PositionsList.Count == 0) return;

            var positionAfterMove = _history.PositionsList.Last().GetNewPositionAfterMove(beginSquareId, endSquareId);
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

        private int GetFirstGuessedMovesCount()
        {
            for (int i = 0; i < ChessleSubmissionVm.MovesCount; i++)
            {
                if (!_guessedPositions.TryGetValue(i, out _)) return i;
            }

            return ChessleSubmissionVm.MovesCount;
        }

        private List<ChessleSubmissionVm> GetEngineSubmissions()
        {
            var solver = new ChessleSolver();
            solver.InitSolver(DatabaseDir);

            var submissions = new List<ChessleSubmissionVm>();

            var isSolved = false;
            var it = 0;

            while (it < MaxEngineIterations && !isSolved)
            {
                var solverSubmission = solver.GetSubmission();
                var submissionVm = new ChessleSubmissionVm { MovesNotation = solverSubmission };
                var bullsCows = BullsAndCowsCounter.GetBullsAndCows(submissionVm, _rightAnswer);
                submissionVm.FillColorsCheckSubmission(bullsCows);
                solver.UpdateSolver(solverSubmission, bullsCows);

                submissions.Add(submissionVm);
                isSolved = submissionVm.IsSolved;
                it++;
            }

            return submissions;
        }

        #endregion
    }
}
