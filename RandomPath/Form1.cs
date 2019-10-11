using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RandomPath
{
    public partial class Form1 : Form
    {
        Square[,] squares = new Square[7, 7];
        List<Square> lastClicked = new List<Square>(2);
        Timer mover = new Timer();

        public Form1()
        {
            InitializeComponent();

            mover.Interval = 500;
            mover.Tick += Mover_Tick;

            for (int x = 0; x <= squares.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= squares.GetUpperBound(1); y++)
                {
                    squares[x, y] = new Square
                    {
                        BackColor = SystemColors.ActiveCaption,
                        BorderStyle = BorderStyle.FixedSingle,
                        Location = new Point(x * 40, y * 40),
                        Name = "square",
                        Size = new Size(40, 40),
                        TabIndex = 2,
                        TabStop = false,
                        X = x,
                        Y = y,
                        Pathed = false
                    };
                    
                    squares[x, y].MouseClick += Form1_MouseClick;

                    this.Controls.Add(squares[x, y]);
                }
            }

            this.Size = new Size(56 + (40 * squares.GetUpperBound(0)), 79 + (40 * squares.GetUpperBound(1)));
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            //Square current = (Square)sender;
            
            //current.BackColor = Color.Red;
            //current.Pathed = true;
            //lastClicked.Add(current);
            //if (lastClicked.Count > 2)
            //    lastClicked.RemoveAt(0);

            //List<Square> movable = GetMoveableNeighbors(current);

            //if (movable.Count > 0)
            //{
            //    Random rnd = new Random();

            //    Square next = movable[rnd.Next(movable.Count)];
            //    //lastClicked.Add(next);
            //    next.BackColor = Color.Yellow;
            //}

            Square next = (Square)sender;
            lastClicked.Add(next);
            MoveToNext();
            mover.Start();
        }

        private void Mover_Tick(object sender, EventArgs e)
        {
            MoveToNext();
        }

        private void MoveToNext()
        {
            if (lastClicked.Count > 0)
            {
                Square last = lastClicked.Last();
                last.BackColor = Color.Red;
                last.Pathed = true;
                List<Square> movable = GetMoveableNeighbors(last);

                if (movable.Count > 0)
                {
                    Random rnd = new Random();

                    Square next = movable[rnd.Next(movable.Count)];
                    lastClicked.Add(next);
                    if (lastClicked.Count > 2)
                        lastClicked.RemoveAt(0);
                }
            }
            else
                mover.Stop();
        }

        private List<Square> GetNeighbors(Square s)
        {
            List<Square> neighbors = new List<Square>();

            int xLowerBound = s.X == 0 ? 0 : s.X - 1;
            int xUpperBound = s.X == squares.GetUpperBound(0) ? squares.GetUpperBound(0) : s.X + 1;

            int yLowerBound = s.Y == 0 ? 0 : s.Y - 1;
            int yUpperBound = s.Y == squares.GetUpperBound(1) ? squares.GetUpperBound(1) : s.Y + 1;

            for (int x = xLowerBound; x <= xUpperBound; x++)
                for (int y = yLowerBound; y <= yUpperBound; y++)
                    if (!(s.X == x && s.Y == y))
                        neighbors.Add(squares[x, y]);

            return neighbors;
        }

        private bool HasPathedNeighbors(Square square)
        {
            bool result = false;

            foreach (Square s in GetNeighbors(square))
            {
                if (!lastClicked.Contains(s))
                {
                    if (s.Pathed)
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        private List<Square> GetMoveableNeighbors(Square square)
        {
            List<Square> movableSquares = new List<Square>();

            foreach (Square s in squares)
                if (!lastClicked.Contains(s) && !s.Pathed)
                    if ((s.X == square.X && (s.Y >= square.Y - 1 && s.Y <= square.Y + 1)
                        || s.Y == square.Y && s.X >= square.X - 1 && s.X <= square.X + 1)
                        && s.X != 0 && s.X != squares.GetUpperBound(0)
                        && s.Y != 0 && s.Y != squares.GetUpperBound(1)
                        && !HasPathedNeighbors(s))
                            movableSquares.Add(s);

            return movableSquares;
        }
    }

    class Square : PictureBox
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool Pathed { get; set; }
    }
}
