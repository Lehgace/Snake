using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public class GameState
    {
        public int Rows { get; }
        public int Columns { get; }
        public GridValue[,] Grid { get; }
        public Direction Dir { get; private set; }
        public int Score { get; private set; }
        public bool GameOver { get; private set; }

        private readonly LinkedList<Position> snakePositions = new LinkedList<Position>();
        private readonly Random random = new Random(); // random generator for food generation

        // Construct the game
        public GameState(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            Grid = new GridValue[Rows, Columns]; // Game will start with all empty grid
            Dir = Direction.Right; // start with snake moving right

            AddSnake();
            AddFood();
        }

        private void AddSnake()
        {
            int r = Rows / 2;

            for (int c = 1; c <= 3; c++)
            {
                Grid[r, c] = GridValue.Snake;
                snakePositions.AddFirst(new Position(r, c));
            }
        }

        private IEnumerable<Position> EmptyPositions()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c <= Columns; c++)
                {
                    if (Grid[r,c] == GridValue.Empty)
                    {
                        yield return new Position(r, c);
                    }
                }
            }
        }

        private void AddFood()
        {
            List<Position> empty = new List<Position>(EmptyPositions());

            if (empty.Count == 0)
            {
                return;
            }

            Position pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Column] = GridValue.Food;
        }

        // Find position of the snake's head
        public Position HeadPosition()
        {
            return snakePositions.First.Value;
        }

        // Find position of the snake's tail
        public Position TailPosition()
        {
            return snakePositions.Last.Value;
        }

        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions;
        }

        // Methods for modifying snake
        private void AddHead(Position pos)
        {
            snakePositions.AddFirst(pos);
            Grid[pos.Row, pos.Column] = GridValue.Snake;
        }

        private void RemoveTail()
        {
            Position tail = snakePositions.Last.Value;
            Grid[tail.Row, tail.Column] = GridValue.Empty;
            snakePositions.RemoveLast();
        }

        // Snake Directions
        public void ChangeDirection(Direction direction)
        {
            Dir = direction;
        }

        // Check if snake is out-of-bounds
        private bool OutofBounds(Position position)
        {
            return position.Row < 0 || position.Row >= Rows || position.Column < 0 || position.Column >= Columns;
        }

        // Check if snake will hit itself
        private GridValue WillHit(Position newHeadPosition)
        {
            // If snake is out-of-bounds
            if (OutofBounds(newHeadPosition))
            {
                return GridValue.OutofBounds;
            }

            // Allow for snake head to move through snake tail simultaneously
            if (newHeadPosition == TailPosition())
            {
                return GridValue.Empty;
            }


            return Grid[newHeadPosition.Row, newHeadPosition.Column];
        }

        // Move snake one head in the current direction
        public void Move()
        {
            Position newHeadPos = HeadPosition().Translate(Dir);
            GridValue hit = WillHit(newHeadPos);

            // If snake moves out of bounds or hits itself = game over
            if (hit == GridValue.OutofBounds || hit == GridValue.Snake)
            {
                GameOver = true;
            }
            // If next grid tile is empty, "move" snake
            else if (hit == GridValue.Empty)
            {
                RemoveTail();
                AddHead(newHeadPos);
            }
            // If next grid is food, add to snake and score
            else if (hit == GridValue.Food)
            {
                AddHead(newHeadPos);
                Score++;
                AddFood();
            }
        }
    }
}
