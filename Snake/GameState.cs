
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

        private readonly LinkedList<Position> snakePosition = new LinkedList<Position>(); //danh sách liên kết những position - vị trí của rắn (đầu rắn là head, đuôi rắn là tail)

        private readonly LinkedList<Direction> dirChanges = new LinkedList<Direction>(); //danh sách liên kết những hướng di chuyển của rắn

        private static readonly Random random = new Random(); //tạo số ngẫu nhiên cho vị trí của thức ăn

        public GameState(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            Grid = new GridValue[rows, columns]; //khởi tạo mảng 2 chiều chứa giá trị của các ô, mỗi ô có giá trị ban đầu là Empty (first Enum)
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
                snakePosition.AddFirst(new Position(r, c)); //thêm vị trí của rắn vào danh sách liên kết
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
            List<Position> empty = new List<Position>(EmptyPositions()); //lấy danh sách vị trí của các ô trống
            if (empty.Count == 0) //nếu không còn ô trống nào
            {
                return;
            }
            Position position = empty[random.Next(empty.Count)]; //chọn ngẫu nhiên một vị trí trong danh sách vị trí trống
            Grid[position.Row, position.Column] = GridValue.Food; //thêm thức ăn vào vị trí đã chọn
        }

        public Position HeadPosition => snakePosition.First.Value; //lấy vị trí của đầu rắn

        public Position TailPosition => snakePosition.Last.Value; //lấy vị trí của đuôi rắn

        public IEnumerable<Position> SnakePosition => snakePosition; //lấy danh sách vị trí của rắn

        private void AddHead(Position position)
        {
            snakePosition.AddFirst(position); //thêm vị trí của đầu rắn vào đầu danh sách liên kết
            Grid[position.Row, position.Column] = GridValue.Snake; //thêm đầu rắn vào Grid
        }

        private void RemoveTail()
        {
            Position tail = snakePosition.Last.Value; //lấy vị trí của đuôi rắn
            Grid[tail.Row, tail.Column] = GridValue.Empty; //xóa đuôi rắn khỏi Grid
            snakePosition.RemoveLast(); //xóa vị trí của đuôi rắn khỏi danh sách liên kết
        }

        private Direction GetLastDirection()
        {
            if (dirChanges.Count == 0) //nếu không có hướng di chuyển mới
            {
                return Direction; //trả về hướng di chuyển hiện tại
            }
            return dirChanges.Last.Value; //trả về hướng di chuyển cuối cùng mà người chơi bấm
        }

        private bool CanChangeDirection(Direction newDirection)
        {
            if(dirChanges.Count == 2) 
            {
                return false; //nếu danh sách liên kết chứa 2 hướng di chuyển thì không thể chuyển hướng
            }
            Direction lastDirection = GetLastDirection(); //lấy hướng di chuyển cuối cùng
            return newDirection != lastDirection && newDirection != lastDirection.Opposite(); //nếu hướng di chuyển mới khác hướng di chuyển cuối cùng và hướng di chuyển mới khác hướng di chuyển đối diện với hướng di chuyển cuối cùng
        }

        public void ChangeDirection(Direction newDirection)
        {
            if (CanChangeDirection(newDirection))
            {
                dirChanges.AddLast(newDirection); //thêm hướng di chuyển mới vào danh sách liên kết
            }
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
            if (newHeadPos == TailPosition)
            {
                return GridValue.Empty; //rắn sẽ không đụng vào đuôi
            }
            return Grid[newHeadPos.Row, newHeadPos.Column]; //trả về giá trị của ô tiếp theo
        }

        public void Move()
        {
            if(dirChanges.Count > 0)
            {
                Direction = dirChanges.First.Value; //lấy hướng di chuyển đầu tiên trong danh sách liên kết
                dirChanges.RemoveFirst(); //xóa hướng di chuyển đầu tiên khỏi danh sách liên kết
            }

            Position newHeadPos = HeadPosition.Translate(Direction); //tính vị trí của đầu rắn sau khi di chuyển
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
