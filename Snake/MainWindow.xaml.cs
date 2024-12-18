﻿using System.Text;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Dictionary(key, value) chứa các hình ảnh tương ứng với giá trị của ô trên grid
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            { GridValue.Empty, Images.Empty },
            { GridValue.Snake, Images.Body },
            { GridValue.Food, Images.Food },
        };
        private readonly int rows = 15, cols = 15;
        private readonly Image[,] gridImages;
        private GameState gameState;
        private bool gameRunning;
        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            { Direction.Up, 0 },
            { Direction.Right, 90 },
            { Direction.Down, 180 },
            { Direction.Left, 270 },
        };

        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            gameState = new GameState(rows, cols);
        }

        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            //GameGrid 15x15 là uniform grid
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                    Image image = new Image // tạo hình ảnh từ Image trong class System.Windows.Controls
                    { 
                        Source = Images.Empty, // source hình ảnh ban đầu của các ô là Empty
                        RenderTransformOrigin = new Point(0.5, 0.5), // điểm xoay hình ảnh là tâm của hình ảnh (phép xoay sẽ lấy tâm của đối tượng làm điểm gốc)
                    };
                    images[r, c] = image; // lưu trữ hình ảnh vào mảng 2 chiều
                    GameGrid.Children.Add(image); // thêm hình ảnh vào GameGrid
                }
            }
            return images;
        }

        private void Draw()
        {
           DrawGrid();
           DrawSnakeHead();
           ScoreText.Text = $"Score: {gameState.Score}";
        }
        private void DrawGrid()
        {
            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                   GridValue gridValue = gameState.Grid[r,c];
                    gridImages[r, c].Source = gridValToImage[gridValue]; // cập nhật hình ảnh của ô trên grid tương ứng với giá trị của ô
                    gridImages[r, c].RenderTransform = Transform.Identity; // xoay hình ảnh về trạng thái ban đầu (không bị xoay, dịch chuyển, hoặc co giãn).
                }
            }
        }

        private void DrawSnakeHead()
        {
            Position head = gameState.HeadPosition;
            Image headImage = gridImages[head.Row, head.Column];
            headImage.Source = Images.Head;

            int rotation = dirToRotation[gameState.Direction];
            headImage.RenderTransform = new RotateTransform(rotation);
        }

        private async Task DrawDeadSnake()
        {
            List<Position> positions = new List<Position>(gameState.SnakePosition);

            for(int i = 0; i < positions.Count ; i++)
            {
               Position pos = positions[i];
               ImageSource source = (i == 0) ? Images.DeadHead : Images.DeadBody;
               gridImages[pos.Row, pos.Column].Source = source;
               await Task.Delay(50);
            }
        }
        private async Task RunGame()
        {
            Draw();
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            await ShowGameOver();
            gameState = new GameState(rows, cols);
        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true; // chặn game chạy và đợi await RunGame() 
            }
            if (!gameRunning)
            {
                gameRunning = true;
                await RunGame(); // chạy game overlay ẩn đi và e.Handled = true sẽ không xử lý sự kiện chặn game chạy nữa
                gameRunning = false;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
           if(gameState.GameOver)
           {
               return;
           }
           switch(e.Key)
           {
               case Key.Up:
                   gameState.ChangeDirection(Direction.Up);
                   break;
               case Key.Down:
                   gameState.ChangeDirection(Direction.Down);
                   break;
               case Key.Left:
                   gameState.ChangeDirection(Direction.Left);
                   break;
               case Key.Right:
                   gameState.ChangeDirection(Direction.Right);
                   break;
           }
        }

        private async Task GameLoop()
        {
            while(!gameState.GameOver)
            {
                await Task.Delay(100);
                gameState.Move();
                Draw();
            }
        }

        private async Task ShowCountDown()
        {
            for (int i = 3; i >= 1; i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }

        private async Task ShowGameOver()
        {
            await DrawDeadSnake();
            await Task.Delay(1000);
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "Press any key to restart";
        }
    }
}