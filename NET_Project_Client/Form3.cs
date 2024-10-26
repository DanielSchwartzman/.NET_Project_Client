using NET_Project_Client.Model.ChessPieces;
using NET_Project_Client.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Threading;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Threading;

namespace NET_Project_Client
{
    public partial class Form3 : Form
    {
        private string connectionstr = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\GitRep\\NET_Project_Client\\NET_Project_Client\\Database1.mdf;Integrated Security=True";
        int gameID;

        ChessBoard chessBoard;
        int squareWidth;
        int squareHeight;
        int click;
        bool turn;

        List<int> allMoves = new List<int>();

        int clicked = 0;

        int clickrow;
        int clickcol;

        private Bitmap bitmap;
        private const int SIZE = 2;
        private int x = -SIZE;
        private int y = -SIZE;

        // Animation related variables
        private Thread animationThread;
        private System.Timers.Timer replayTimer;
        private Piece movingPiece;
        private Point sourceLocation;
        private Point targetLocation;
        private float animationProgress;
        private const int animationDuration = 500; // 500 ms
        private DateTime animationStartTime;
        private int currentMoveIndex = 0;

        public Form3(int Gid)
        {
            InitializeComponent();
            gameID = Gid;

            this.Load += new EventHandler(Form1_Load);
            this.Paint += new PaintEventHandler(Form1_Paint);

            replayTimer = new System.Timers.Timer();
            replayTimer.Interval = 1000; // Set interval for move replay (1 second)
            replayTimer.Elapsed += OnReplayTimerTick; // Attach event handler
            replayTimer.AutoReset = true;

            this.DoubleBuffered = true;
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            chessBoard = new ChessBoard(this, false);
            click = 0;
            turn = false;
            loadDataFromDB();
            StartGameReplay();
        }

        public void victory()
        {
            if (!turn)
                MessageBox.Show("Victory for Blacks");
            else
                MessageBox.Show("Victory for Whites");
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
        }


        private void StartGameReplay()
        {
            currentMoveIndex = 0; // Start from the first move

            // Start the replay timer
            replayTimer.Start();
        }
        private void OnReplayTimerTick(object sender, ElapsedEventArgs e)
        {
            // Ensure that UI updates are done on the main thread
            this.Invoke((MethodInvoker)delegate
            {
                if (currentMoveIndex < allMoves.Count())
                {
                    // Get the next move
                    int fromRow, fromCol, toRow, toCol;

                    toCol = allMoves[currentMoveIndex] % 10;
                    allMoves[currentMoveIndex] /= 10;

                    toRow = allMoves[currentMoveIndex] % 10;
                    allMoves[currentMoveIndex] /= 10;

                    fromCol = allMoves[currentMoveIndex] % 10;
                    allMoves[currentMoveIndex] /= 10;

                    fromRow = allMoves[currentMoveIndex] > 0 ? allMoves[currentMoveIndex] : 0;

                    // Start the move animation
                    StartMoveAnimation(new Coordinate(fromRow, fromCol), new Coordinate(toRow, toCol));

                    // Increment to the next move
                    currentMoveIndex++;
                }
                else
                {
                    // Stop the timer once all moves are played
                    replayTimer.Stop();
                    MessageBox.Show("Replay over");
                }
            });
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
                        bool res = chessBoard.MakeMove(new Coordinate(clickrow, clickcol), new Coordinate(row, col));
                        this.Invalidate();
                        click = 0;
                        switchTurn();
                        if (res)
                        {
                            victory();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid move", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        click = 0;
                    }
                }
            }
        }

        private void switchTurn()
        {
            turn = !turn;
        }

        // Start animation when moving a piece
        private void StartMoveAnimation(Coordinate startloc, Coordinate endloc)
        {
            movingPiece = chessBoard.GetPieceAt(startloc.y, startloc.x);
            sourceLocation = new Point(startloc.x * squareWidth, startloc.y * squareHeight);
            targetLocation = new Point(endloc.x * squareWidth, endloc.y * squareHeight);
            animationProgress = 0;
            animationStartTime = DateTime.Now;

            // Start the animation timer
            animationThread = new Thread(() => AnimateMove(startloc, endloc));
            animationThread.IsBackground = true; // Set as background thread
            animationThread.Start();
        }

        // Animation tick method
        private void AnimateMove(Coordinate start, Coordinate end)
        {
            while (animationProgress < 1.0f)
            {
                // Calculate animation progress based on time
                TimeSpan elapsed = DateTime.Now - animationStartTime;
                animationProgress = (float)elapsed.TotalMilliseconds / animationDuration;

                // Ensure progress is capped at 1.0
                if (animationProgress >= 1.0f)
                {
                    animationProgress = 1.0f;
                }

                // Trigger form repaint to reflect the animation progress
                this.Invoke((MethodInvoker)delegate
                {
                    this.Invalidate();
                });

                // Sleep briefly to control the animation frame rate (~60 FPS)
                Thread.Sleep(16);
            }

            // Once animation is complete, perform the move
            this.Invoke((MethodInvoker)delegate
            {
                OnSquareClick(start.y, start.x);
                OnSquareClick(end.y, end.x);

                movingPiece = null;
                this.Invalidate();
            });
        }

        private void loadDataFromDB()
        {
            string queryString = "SELECT * FROM [dbo].[GameOrder] WHERE Gid = " + gameID + ";";

            using (SqlConnection connection = new SqlConnection(connectionstr))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Access each column by index or column name
                        int gid = reader.GetInt32(0); // Assuming Gid is the first column
                        int moveID = reader.GetInt32(1); // Pid column
                        int theMove = reader.GetInt32(2); // Name column

                        allMoves.Add(theMove);
                    }
                }

                connection.Close();
            }
        }
    }
}
