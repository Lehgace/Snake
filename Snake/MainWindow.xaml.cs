using Snake;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Snake
{
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValueToImage = new()
        {
            { GridValue.Empty, Images.Empty },
            { GridValue.Snake, Images.Body },
            { GridValue.Food, Images.Food },
        };

        // for manipulating snake eyes direction
        private readonly Dictionary<Direction, int> directionToRotation = new()
        {
            
            { Direction.Up, 0 },
            { Direction.Right, 90 },
            { Direction.Down, 180 },
            { Direction.Left, 270 }
        };

        
        private readonly int rows = 15, columns = 15; // can be changed, ideally equal
        private readonly Image[,] gridImages;
        private GameState gameState;
        private bool gameRunning;
        private int[] SeededSnakeBody { get; set; }

        private int GetGridSize()
        {
            return rows * columns;
        }

        private int[] SeedRandomSnakeBody()
        {
            int gridSize = GetGridSize();
            int[] seedSnakeBody = new int[gridSize];
            Random random = new Random();
            int pickSuit;

            for (int i = 0; i < gridSize - 1; i++)
            {
                pickSuit = random.Next(1, 5);
                seedSnakeBody[i] = pickSuit;
            }
            return seedSnakeBody;
        }


        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            gameState = new GameState(rows, columns);
        }

        private async Task RunGame()
        {
            SeededSnakeBody = SeedRandomSnakeBody(); // generate seed for an individual game's snake body
            Draw();
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            await ShowGameOver();
            gameState = new GameState(rows, columns);
        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }

            if(!gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    gameState.ChangeDirection(Direction.Left);
                    break;
                case Key.Right:
                    gameState.ChangeDirection(Direction.Right);
                    break;
                case Key.Up:
                    gameState.ChangeDirection(Direction.Up);
                    break;
                case Key.Down:
                    gameState.ChangeDirection(Direction.Down);
                    break;
                case Key.A:
                    gameState.ChangeDirection(Direction.Left);
                    break;
                case Key.D:
                    gameState.ChangeDirection(Direction.Right);
                    break;
                case Key.W:
                    gameState.ChangeDirection(Direction.Up);
                    break;
                case Key.S:
                    gameState.ChangeDirection(Direction.Down);
                    break;
                case Key.Space:
                    gameState.GamePaused = !gameState.GamePaused;
                    break;
            }
        }

        // TODO: Options for Game Speed
        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                if (gameState.GamePaused)
                {
                    await ShowGamePaused();
                }
                else
                {
                    OverlayPause.Visibility = Visibility.Hidden;
                    await Task.Delay(100);
                    //await Task.Delay(200);
                    gameState.Move();
                    Draw();
                }
            }
        }

        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, columns];
            GameGrid.Rows = rows;
            GameGrid.Columns = columns;
            GameGrid.Width = GameGrid.Height * (columns / (double)rows); // to handle rectangular grid specifications

            for (int r = 0; r < rows; r++) 
            {
                for (int c = 0; c < columns; c++)
                {
                    Image image = new Image
                    {
                        Source = Images.Empty,
                        RenderTransformOrigin = new Point(0.5,0.5)
                    };

                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }
            return images;
        }

        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            DrawRandomSnakeBody();
            ScoreText.Text = $"Bet ${ gameState.Score },000";
        }

        // Looks a grid array in gamestate, update grid images to reflect changes
        private void DrawGrid()
        {
            for (int r = 0;r < rows; r++)
            {
                for (int c = 0;c < columns; c++)
                {
                    GridValue gridVal = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValueToImage[gridVal];
                    gridImages[r, c].RenderTransform = Transform.Identity;
                }
            }
        }

        private void DrawSnakeHead()
        {
            Position headPosition = gameState.HeadPosition();
            Image image = gridImages[headPosition.Row, headPosition.Column];
            image.Source = Images.Head;

            int rotation = directionToRotation[gameState.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }

        private void DrawRandomSnakeBody()
        {
            List<Position> positions = new List<Position>(gameState.SnakePositions());
            int snakeLength = positions.Count;

            for (int i = 1; i < snakeLength; i++)
            {
                int pickedSuit = SeededSnakeBody[i];
                Position bodypos = positions[i];
                
                ImageSource source;
                switch(pickedSuit)
                {
                    case 2:
                        source = Images.Body2;
                        break;
                    case 3:
                        source = Images.Body3;
                        break;
                    case 4:
                        source = Images.Body4;
                        break;
                    default:
                        source = Images.Body;
                        break;
                };
                gridImages[bodypos.Row, bodypos.Column].Source = source;
            }
        }

        private async Task DrawDeadSnake()
        {
            List<Position> positions = new List<Position>(gameState.SnakePositions());
            int snakeLength = positions.Count;

            for (int i = 0; i < snakeLength; i++)
            {
                Position pos = positions[i];
                ImageSource source = (i == 0) ? Images.DeadHead : Images.DeadBody;
                gridImages[pos.Row, pos.Column].Source = source;
                
                // Speed up decay of snake as it grows longer; snakelength includes the starting snake size of 3
                if (snakeLength > 40)
                {
                    await Task.Delay(10);
                }
                else if (snakeLength > 20)
                {
                    await Task.Delay(25);
                }
                else
                {
                    await Task.Delay(50);
                }    
            }
        }

        private async Task ShowCountDown()
        {
            for (int countdown = 3; countdown >= 1; countdown--)
            {
                OverlayText.Text = countdown.ToString();
                await Task.Delay(500);
            }
        }

        private async Task ShowGameOver()
        {
            await DrawDeadSnake();
            await Task.Delay(1000);
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "\tBusted!\nAny Key To Gamble Again";
        }

        private async Task ShowGamePaused()
        {
            await Task.Delay(1);
            OverlayPause.Visibility = Visibility.Visible;
            OverlayPauseText.Text = "Game Paused\nSpace to Continue";
        }
    }
}