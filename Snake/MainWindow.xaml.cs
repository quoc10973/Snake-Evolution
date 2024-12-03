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
        private readonly GameState gameState;

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
                }
            }
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Draw();
            await GameLoop();
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

      
    }
}