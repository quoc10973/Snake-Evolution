
namespace Snake
{
    public class GameState
    {
        public int Rows { get; }
        public int Columns { get; }

        public GridValue[,] Grid { get; } //mảng 2 chiều chứa giá trị của các ô
        public Direction Direction { get; private set; } //hướng di chuyển của rắn
        public int Score { get; private set; } //điểm số
        public bool GameOver { get; private set; } //trạng thái game kết thúc

        private readonly LinkedList<Position> SnakePosition = new LinkedList<Position>(); //danh sách liên kết những position - vị trí của rắn (đầu rắn là head, đuôi rắn là tail)

        private Random random = new Random(); //tạo số ngẫu nhiên cho vị trí của thức ăn

        public GameState(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            Grid = new GridValue[Rows, Columns]; //khởi tạo mảng 2 chiều chứa giá trị của các ô, mỗi ô có giá trị ban đầu là Empty (first Enum)
            Direction = Direction.Right; //hướng di chuyển ban đầu của rắn là Right
            AddSnake();
            AddFood();
        }

        //khởi tạo game thêm rắn vào Grid
        private void AddSnake()
        {
            int r = Rows / 2; //vị trí của rắn xuất phát ở trung tâm của Grid

            for (int c = 1; c <= 3; c++)
            {
                Grid[r, c] = GridValue.Snake; //thêm rắn vào Grid (rắn dài 3 ô)
                SnakePosition.AddLast(new Position(r, c)); //thêm vị trí của rắn vào danh sách liên kết
            }
        }

        private IEnumerable<Position> EmptyPositions()
        {
            //duyệt qua tất cả các ô trong Grid trống không chứa rắn, return vị trí của các ô đó bằng cách sử dụng yield return -> Trả về một giá trị cho caller và tạm dừng thực thi của phương thức cho đến khi giá trị tiếp theo được yêu cầu.
           for (int r = 0; r < Rows; r++)
           {
               for(int c = 0; c < Columns; c++)
               {
                   if(Grid[r, c] == GridValue.Empty)
                   {
                       yield return new Position(r, c);
                   }
               }
           }
        }

        private void AddFood()
        {
           List<Position> empty = EmptyPositions().ToList(); //lấy danh sách vị trí của các ô trống
           if(empty.Count == 0) //nếu không còn ô trống nào
           {
               return;
           }
           Position position = empty[random.Next(empty.Count)]; //chọn ngẫu nhiên một vị trí trong danh sách vị trí trống
           Grid[position.Row, position.Column] = GridValue.Food; //thêm thức ăn vào vị trí đã chọn
        }

    }
}
