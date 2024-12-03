
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
                for (int c = 0; c < Columns; c++)
                {
                    if (Grid[r, c] == GridValue.Empty)
                    {
                        yield return new Position(r, c);
                    }
                }
            }
        }

        private void AddFood()
        {
            List<Position> empty = EmptyPositions().ToList(); //lấy danh sách vị trí của các ô trống
            if (empty.Count == 0) //nếu không còn ô trống nào
            {
                return;
            }
            Position position = empty[random.Next(empty.Count)]; //chọn ngẫu nhiên một vị trí trong danh sách vị trí trống
            Grid[position.Row, position.Column] = GridValue.Food; //thêm thức ăn vào vị trí đã chọn
        }

        //chua commit va push
        public Position Head => SnakePosition.First.Value; //lấy vị trí của đầu rắn

        public Position Tail => SnakePosition.Last.Value; //lấy vị trí của đuôi rắn

        public IEnumerable<Position> Snake => SnakePosition; //lấy danh sách vị trí của rắn

        private void AddHead(Position position)
        {
            SnakePosition.AddFirst(position); //thêm vị trí của đầu rắn vào đầu danh sách liên kết
            Grid[position.Row, position.Column] = GridValue.Snake; //thêm đầu rắn vào Grid
        }

        private void RemoveTail()
        {
            Position tail = SnakePosition.Last.Value; //lấy vị trí của đuôi rắn
            SnakePosition.RemoveLast(); //xóa vị trí của đuôi rắn khỏi danh sách liên kết
            Grid[tail.Row, tail.Column] = GridValue.Empty; //xóa đuôi rắn khỏi Grid
        }

        public void ChangeDirection(Direction newDirection)
        {
            Direction = newDirection; //thay đổi hướng di chuyển của rắn
        }

        public bool OutSideGrid(Position position)
        {
            //kiểm tra xem vị trí có nằm ngoài Grid không
            return position.Row < 0 || position.Row >= Rows || position.Column < 0 || position.Column >= Columns;
        }

        public GridValue WillHit(Position newHeadPos)
        {
            if (OutSideGrid(newHeadPos)) //nếu vị trí tiếp theo nằm ngoài Grid
            {
                return GridValue.Wall; //rắn sẽ đụng vào tường
            }
            if (newHeadPos == Tail)
            {
                return GridValue.Empty; //rắn sẽ không đụng vào đuôi
            }
            return Grid[newHeadPos.Row, newHeadPos.Column]; //trả về giá trị của ô tiếp theo
        }

        public void Move()
        {
            Position newHeadPos = Head.Translate(Direction); //tính vị trí của đầu rắn sau khi di chuyển
            GridValue hit = WillHit(newHeadPos); //kiểm tra xem rắn có đụng vào tường hoặc đuôi không

            if (hit == GridValue.Wall || hit == GridValue.Snake) //nếu rắn đụng vào tường hoặc đuôi
            {
                GameOver = true; //kết thúc game
            }
            else if (hit == GridValue.Empty)
            {
                RemoveTail(); //xóa đuôi rắn
                AddHead(newHeadPos); //thêm đầu rắn
            }
            else if (hit == GridValue.Food)
            {
                AddHead(newHeadPos); //thêm đầu rắn
                Score++; //tăng điểm
                AddFood(); //thêm thức ăn mới
            }
            
        }


    }
}
