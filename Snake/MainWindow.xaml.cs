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
        private readonly int rows = 15, cols = 15;
        private readonly Image[,] gridImages;

        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
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
    }
}