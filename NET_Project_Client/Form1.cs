using NET_Project_Client.Model;
using NET_Project_Client.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

namespace NET_Project_Client
{
    public partial class Form1 : Form
    {
        ChessBoard chessBoard;
        int squareWidth;
        int squareHeight;
        int click;
        bool turn;

        private static System.Timers.Timer aTimer;
        private static double timerInterval = 20000;
        private static double elapsedTime = 0;

        bool toDraw = false;

        int clickrow;
        int clickcol;

        private Bitmap bitmap;
        private const int SIZE = 2;
        private int x = -SIZE;
        private int y = -SIZE;

        // Animation related variables
        private System.Windows.Forms.Timer animationTimer;
        private Piece movingPiece;
        private Point sourceLocation;
        private Point targetLocation;
        private float animationProgress;
        private const int animationDuration = 500; // 500 ms
        private DateTime animationStartTime;

        public Form1()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form1_Load);
            this.Paint += new PaintEventHandler(Form1_Paint);
            this.MouseClick += new MouseEventHandler(Form1_MouseClick);
            this.MouseMove += new MouseEventHandler(Form1_MouseMove);

            // Initialize animation timer
            animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = 16; // ~60 FPS
            animationTimer.Tick += new EventHandler(AnimateMove);

            this.DoubleBuffered = true;
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            chessBoard = new ChessBoard();
            click = 0;
            turn = false;
            bitmap = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height);

            aTimer = new System.Timers.Timer();
            aTimer.Elapsed += OnTimedEvent;
            aTimer.Start();
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            elapsedTime += aTimer.Interval;

            if (elapsedTime >= timerInterval)
            {
                aTimer.Stop();
                Console.WriteLine("Timer elapsed!");
            }

            label2.Text = "Time Left: " + (int)(GetTimeLeft()/1000);
        }

        static double GetTimeLeft()
        {
            double timeLeft = timerInterval - elapsedTime;

            if (timeLeft > 0)
            {
                return timeLeft;
            }
            else
            {
                MessageBox.Show("Times up");
                aTimer.Stop();
                return 0;
            }
        }

        public void victory()
        {
            this.Close();
        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            int rows = 8;
            int columns = 4;
            squareWidth = this.ClientSize.Width / columns / 3;
            squareHeight = this.ClientSize.Height / rows;

            Graphics g = e.Graphics;

            // Draw chessboard and pieces
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    bool isWhiteSquare = (row + col) % 2 == 0;
                    Brush brush = isWhiteSquare ? Brushes.AliceBlue : Brushes.Wheat;

                    g.FillRectangle(brush, col * squareWidth, row * squareHeight, squareWidth, squareHeight);

                    Piece piece = chessBoard.GetPieceAt(row, col);
                    if (piece != null && piece.img != null && piece != movingPiece)
                    {
                        // Draw pieces that are not being moved
                        g.DrawImage(piece.img, col * squareWidth, row * squareHeight, squareWidth, squareHeight);
                    }
                }
            }

            // Draw moving piece during animation
            if (movingPiece != null)
            {
                Point currentLocation = new Point(
                    (int)(sourceLocation.X + (targetLocation.X - sourceLocation.X) * animationProgress),
                    (int)(sourceLocation.Y + (targetLocation.Y - sourceLocation.Y) * animationProgress)
                );

                g.DrawImage(movingPiece.img, currentLocation.X, currentLocation.Y, squareWidth, squareHeight);
            }

            if (toDraw)
            {
                Graphics gr = Graphics.FromImage(bitmap);
                gr.FillEllipse(Brushes.Red, x - SIZE / 2, y - SIZE / 2, SIZE, SIZE);
                e.Graphics.DrawImage(bitmap, 0, 0);
                gr.Dispose();
            }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (!toDraw)
            {
                int clickedColumn = e.X / squareWidth;
                int clickedRow = e.Y / squareHeight;

                OnSquareClick(clickedRow, clickedColumn);
            }
        }

        private void OnSquareClick(int row, int col)
        {
            if (row >= 0 && row <= 7 && col <= 3 && col >= 0)
            {
                if (chessBoard.chessBoard[row, col] != null && click == 0)
                {
                    if (chessBoard.chessBoard[row, col].getColor() == turn)
                    {
                        // MessageBox.Show(chessBoard.chessBoard[row, col].AvailableMovesToString());
                        clickrow = row;
                        clickcol = col;
                        click = 1;
                    }
                }
                else if (click == 1)
                {
                    bool ok = false;
                    Piece p = chessBoard.chessBoard[clickrow, clickcol];
                    int length = p.AvailableMoves.Count();
                    for (int i = 0; i < length; i++)
                    {
                        if (p.AvailableMoves[i].Equals(new Coordinate(row, col)))
                        {
                            ok = true;
                        }
                    }
                    if (ok)
                    {
                        // Initiate animation
                        StartMoveAnimation(new Coordinate(clickrow, clickcol), new Coordinate(row, col));
                        click = 0;
                        switchTurn();
                        ResetTimer();
                    }
                    else
                    {
                        MessageBox.Show("Invalid move", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        click = 0;
                    }
                }
            }
        }

        private static void ResetTimer()
        {
            aTimer.Stop(); // Stop the timer
            elapsedTime = 0; // Reset the elapsed time

            aTimer.Start(); // Restart the timer
        }

        private void switchTurn()
        {
            turn = !turn;
        }

        // Start animation when moving a piece
        private void StartMoveAnimation(Coordinate start, Coordinate end)
        {
            movingPiece = chessBoard.GetPieceAt(start.y, start.x);
            sourceLocation = new Point(start.x * squareWidth, start.y * squareHeight);
            targetLocation = new Point(end.x * squareWidth, end.y * squareHeight);
            animationProgress = 0;
            animationStartTime = DateTime.Now;

            // Start the animation timer
            animationTimer.Start();
        }

        // Animation tick method
        private void AnimateMove(object sender, EventArgs e)
        {
            // Calculate animation progress based on time
            TimeSpan elapsed = DateTime.Now - animationStartTime;
            animationProgress = (float)elapsed.TotalMilliseconds / animationDuration;

            if (animationProgress >= 1.0f)
            {
                // Animation is complete
                animationProgress = 1.0f;
                animationTimer.Stop();
                if (chessBoard.MakeMove(new Coordinate(clickrow, clickcol), new Coordinate(targetLocation.Y / squareHeight, targetLocation.X / squareWidth)))
                    victory();
                if (chessBoard.check)
                    MessageBox.Show(chessBoard.coordinatecoordinateListString());
                // Reset moving piece
                movingPiece = null;
            }

            // Redraw the form to show animation
            this.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (bitmap != null)
            {
                bitmap.Dispose();
                bitmap = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height);
                this.Invalidate();
            }
            toDraw = !toDraw;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {

            if (toDraw)
            {
                if (e.Button == MouseButtons.Left)
                {
                    x = e.X;
                    y = e.Y;
                    this.Invalidate();
                    this.Update();
                }
            }
        }
    }
}
