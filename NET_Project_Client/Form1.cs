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
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using Newtonsoft.Json;
using System.Net.Http;
using System.Runtime.InteropServices;

namespace NET_Project_Client
{
    public partial class Form1 : Form
    {
        private string connectionstr = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\GitRep\\NET_Project_Client\\NET_Project_Client\\Database1.mdf;Integrated Security=True";
        int gameID;
        int playerID;
        Client client;
        Form1 formInstance;

        ChessBoard chessBoard;
        int squareWidth;
        int squareHeight;
        int click;
        bool turn;
        public Dictionary<int, int> promotions = new Dictionary<int, int>();
        string PieceType = "";

        private static System.Timers.Timer aTimer;
        private static double timerInterval = 20000;
        private double timerInterval2 = 20000;
        private static double elapsedTime = 0;

        bool toDraw = false;
        bool animationLock =false;
        bool ServerReplyLock = false;

        int clickrow;
        int clickcol;

        private Bitmap bitmap;
        private const int SIZE = 2;
        private int x = -SIZE;
        private int y = -SIZE;

        private int promoMove= 0;

        // Animation related variables
        private System.Windows.Forms.Timer animationTimer;
        private Piece movingPiece;
        private Point sourceLocation;
        private Point targetLocation;
        private float animationProgress;
        private const int animationDuration = 500; // 500 ms
        private DateTime animationStartTime;

        //custom messagebox for promotion variables
        private Panel messageBoxPanel;
        private Label messageLabel;
        private Button buttonRook;
        private Button buttonBishop;
        private Button buttonKnight;

        public Form1(int PlayerID, int timerValue)
        {
            InitializeComponent();
            formInstance = this;
            this.Load += new EventHandler(Form1_Load);
            this.Paint += new PaintEventHandler(Form1_Paint);
            this.MouseClick += new MouseEventHandler(Form1_MouseClick);
            this.MouseMove += new MouseEventHandler(Form1_MouseMove);
            this.playerID =PlayerID;
            this.FormClosed += Form1_FormClosed;
            this.FormClosing += Form1_FormClosing;
            // Initialize animation timer
            animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = 16; // ~60 FPS
            animationTimer.Tick += new EventHandler(AnimateMove);

            timerInterval2 = timerValue * 1000;

            this.DoubleBuffered = true;
            label1.Text = "Current Move: White";

            InsertNewPlayerToDB(PlayerID);

            ReadLastID();
            
        }

        private async void Form1_Load(object sender, System.EventArgs e)
        {
            chessBoard = new ChessBoard(this, true);
            click = 0;
            turn = false;
            bitmap = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height);
            client = await GetClientAsync(playerID);
            UpdateClientLabels(client.Name, client.ID.ToString(), client.Phone, client.Country);
            aTimer = new System.Timers.Timer();
            aTimer.Elapsed += OnTimedEvent;
            ResetTimer();
            
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            elapsedTime += aTimer.Interval;

            if (elapsedTime >= timerInterval2)
            {
                aTimer.Stop();
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate {
                        ShowTimeoutMessage();
                    }));
                }
                else
                {
                    ShowTimeoutMessage();
                }
            }

            if (!label2.IsDisposed && !this.IsDisposed)
            {
                if (label2.InvokeRequired)
                {
                    label2.Invoke(new MethodInvoker(delegate {
                        // Check again within the delegate
                        if (!label2.IsDisposed && !this.IsDisposed)
                        {
                            try
                            {
                                label2.Text = "Time Left: " + (int)(GetTimeLeft(this) / 1000);
                            }
                            catch (ObjectDisposedException)
                            {
                            }
                        }
                    }));
                }
                else
                {
                    // UI thread - update directly
                    try
                    {
                        label2.Text = "Time Left: " + (int)(GetTimeLeft(this) / 1000);
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            label2.Dispose();
            aTimer.Stop();
            aTimer.Dispose();
            
        }

        private void ShowTimeoutMessage()
        {
            this.Enabled = false;
            DialogResult result = MessageBox.Show("Times up", "Timeout", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Enabled = true;

            if (result == DialogResult.OK)
            {

                this.Close(); // Close the current form
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            UpdatePromoInDB(promotions);
        }

        public double GetTimeLeft(Form form1)
        {
            double  timeLeft = timerInterval2 - elapsedTime;

            if (timeLeft > 0)
            {
                return timeLeft;
            }
            else
            {
                //aTimer.Stop();
                return 0;
            }
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

        private async void OnSquareClick(int row, int col)
        {
            if (!animationLock || ServerReplyLock)
            {
                if (row >= 0 && row <= 7 && col <= 3 && col >= 0)
                {
                    if (chessBoard.chessBoard[row, col] != null && click == 0)
                    {
                        if (chessBoard.chessBoard[row, col].getColor() == turn)
                        {
                            //MessageBox.Show(chessBoard.chessBoard[row, col].AvailableMovesToString());
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
                        if (p is Pawn)
                        {
                            int lengthP = ((Pawn)p).Threatening.Count();
                            for (int i = 0; i < lengthP; i++)
                            {
                                if (((Pawn)p).Threatening[i].Equals(new Coordinate(row, col)))
                                {
                                    ok = true;
                                }
                            }
                        }
                        for (int i = 0; i < length; i++)
                        {
                            if (p.AvailableMoves[i].Equals(new Coordinate(row, col)))
                            {
                                ok = true;
                            }
                        }
                        if (ok)
                        {
                            PieceType = await ShowCustomMessageBoxAsync(clickrow, clickcol, row, col);
                            // Initiate animation
                            StartMoveAnimation(new Coordinate(clickrow, clickcol), new Coordinate(row, col));
                            label3.Invoke(new MethodInvoker(delegate {
                                label3.Text = "The move: [" + clickrow + " , " + clickcol + "]  ->  " + "[" + row + " , " + col + "]";
                            }));
                            click = 0;
                            ok = false;
                        }
                        else
                        {
                            //MessageBox.Show("Invalid move", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            click = 0;
                        }
                    }
                }
            }
        }
        private bool IsPawnPromoting(Coordinate start, Coordinate end)
        {
            if (chessBoard.chessBoard[start.y,start.x] is Pawn)
            {
                if(end.y == 0 || end.y == 7)
                    return true;
            }
            return false;
        }

        private static void ResetTimer()
        {
            aTimer.Stop(); // Stop the timer
            elapsedTime = 0; // Reset the elapsed time

            aTimer.Start(); // Restart the timer
        }

        public async void switchTurn()
        {
            if (label1.InvokeRequired)
            {
                //background thread - Invoke to call this on the UI thread
                label1.Invoke(new MethodInvoker(delegate {
                    label1.Text = "Current Move: " + (turn ? "White" : "Server");
                }));
            }
            else
            {
                //UI thread - update directly
                label1.Text = "Current Move: " + (turn ? "White" : "Server");
            }

            if (!turn) //Server's Turn - API call to server
            {
                ServerReplyLock = true;
                turn = !turn;
                ChessClient chessClient = new ChessClient();
                ResetTimer();
                if (!animationLock)
                {
                    List<List<int>> availableMovesForServer = chessBoard.CalculateMovesForServer();
                    try
                    {
                        var result = await chessClient.GetRandomMove(availableMovesForServer);

                        if (result != null)
                        {
                            //MessageBox.Show($"Random move selected: Piece {result.pieceIndex} -> Move {result.moveIndex}: {result.move}");
                            // Apply the move to the chessboard
                            int moveparsed = result.move;
                            ApplyMoveFromServer(moveparsed);
                        }
                        else
                        {
                            Console.WriteLine("Failed to retrieve a random move.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
                else
                {
                    // Set up a mechanism to check when the animation is done and call OnSquareClick then
                    await Task.Run(async () =>
                    {
                        // Continuously check if the animationLock is released
                        while (animationLock)
                        {
                            await Task.Delay(100); // Small delay to prevent busy-waiting
                        }
                        
                    });

                    try
                    {
                        List<List<int>> availableMovesForServer = chessBoard.CalculateMovesForServer();
                        var result = await chessClient.GetRandomMove(availableMovesForServer);

                        if (result != null)
                        {
                            //MessageBox.Show($"Random move selected: Piece {result.pieceIndex} -> Move {result.moveIndex}: {result.move}");
                            // Apply the move to the chessboard
                            int moveparsed = result.move;
                            ApplyMoveFromServer(moveparsed);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
            
        }

        private void ApplyMoveFromServer(int move)
        {
            
            // Assuming the move is a 4-digit number
            int fromRow; // First digit
            fromRow = (move >= 1000)? move / 1000:0;
            int fromCol = (move / 100) % 10; // Second digit
            int toRow = (move / 10) % 10; // Third digit
            int toCol = move % 10; // Fourth digit
            label3.Invoke(new MethodInvoker(delegate {
                label3.Text = "The move: [" + fromRow + " , " + fromCol + "]  ->  " + "[" + toRow + " , " + toCol + "]";
            }));
            //MessageBox.Show("Animation lock is: " +animationLock);
            OnSquareClick(fromRow, fromCol);
            OnSquareClick(toRow, toCol);
            turn = !turn;
            ServerReplyLock = false;
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
            animationLock = true;
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
                animationLock = false;
                if (chessBoard.MakeMove(new Coordinate(clickrow, clickcol), new Coordinate(targetLocation.Y / squareHeight, targetLocation.X / squareWidth), PieceType))
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

        public void InsertNewPlayerToDB(int Pid)
        {
            string queryString = "INSERT INTO [dbo].[GamesPlayed] (Pid) VALUES("+Pid+");";
            using (SqlConnection connection = new SqlConnection(
                       connectionstr))
            {
                SqlCommand command = new SqlCommand(
                    queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                bindingSource1.DataSource = reader;
                reader.Close();
                connection.Close();
            }
        }

        public void ReadLastID()
        {
            string queryString = "SELECT MAX(Gid) AS HighestGid FROM [dbo].[GamesPlayed];";
            using (SqlConnection connection = new SqlConnection(
                       connectionstr))
            {
                SqlCommand command = new SqlCommand(
                    queryString, connection);
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    gameID = Convert.ToInt32(result); // Retrieve the Gid from SCOPE_IDENTITY()
                }
                connection.Close();
            }
        }

        public void InsertMoveIntoDB(int moveID, int fromRow, int fromCol, int toRow, int toCol,int isPromo,int promoteTo)
        {
            int rep = (((((fromRow * 10) + fromCol) * 10) + toRow) * 10) + toCol;
            string queryString = "INSERT INTO [dbo].[GameOrder] (Gid, MoveID, TheMove, IsPromoting, PromoteTo) VALUES(" + gameID + ", " + moveID + ", " + rep + ", " + isPromo + ", " + promoteTo + ");";
            using (SqlConnection connection = new SqlConnection(
                       connectionstr))
            {
                SqlCommand command = new SqlCommand(
                    queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                bindingSource1.DataSource = reader;
                reader.Close();
                connection.Close();
            }
        }

        public void UpdatePromoInDB(Dictionary<int,int> promotions)
        {
            if (promotions.Count()>0)
            {
                foreach (KeyValuePair<int, int> promo in promotions)
                {
                    string queryString = "UPDATE [dbo].[GameOrder] SET IsPromoting = 1, PromoteTo = @promoteTo WHERE Gid = @gameID AND MoveID = @moveID;";
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(connectionstr))
                        {
                            SqlCommand command = new SqlCommand(queryString, connection);
                            command.Parameters.AddWithValue("@promoteTo", promo.Value);
                            command.Parameters.AddWithValue("@gameID", gameID);
                            command.Parameters.AddWithValue("@moveID", promo.Key);
                            connection.Open();
                            int rowsAffected = command.ExecuteNonQuery();
                            connection.Close();
                        }
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine("SQL Error: " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }

                }
            } 
        }
        // Get Client from ID
        internal async Task<Client> GetClientAsync(int PlayerID)
        {
            HttpClient client = new HttpClient();
            try
            {
                // Replace with your API's base URL
                string apiUrl = "http://localhost:5177/api/Clients/" +PlayerID;

                // Send the GET request to the API
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                response.EnsureSuccessStatusCode(); // Throws if not a success code (e.g., 404, 500)

                // Get the response content as a string
                string responseData = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON data into a list of clients
                Client nclient = JsonConvert.DeserializeObject<Client>(responseData);

                return nclient;
            }
            catch (HttpRequestException e)
            {
                //MessageBox.Show($"Request error: {e.Message}");
                return new Client();
            }
        }
        //Upadte Client Label data
        private void UpdateClientLabels(string name,string id,string phone,string country)
        {
            label5.Invoke(new MethodInvoker(delegate {
                label5.Text = name;
            }));

            label7.Invoke(new MethodInvoker(delegate {
                label7.Text = id;
            }));

            label9.Invoke(new MethodInvoker(delegate {
                label9.Text = phone;
            }));

            label11.Invoke(new MethodInvoker(delegate {
                label11.Text = country;
            }));
        }

        // TaskCompletionSource to signal when a choice is made
        private TaskCompletionSource<string> _choiceCompletionSource;

        public async Task<string> ShowCustomMessageBoxAsync(int startRow, int startCol, int endRow, int endCol)
        { 
             // Initialize the TaskCompletionSource
             _choiceCompletionSource = new TaskCompletionSource<string>();

            if (IsPawnPromoting(new Coordinate(startRow, startCol), new Coordinate(endRow, endCol)))
            {

                // Create the panel for the message box
                messageBoxPanel = new Panel
                {
                    Size = new Size(300, 150),
                    Location = new Point((this.ClientSize.Width - 300) / 2, (this.ClientSize.Height - 150) / 2),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White
                };

                // Create the message label
                messageLabel = new Label
                {
                    Text = "Choose a promotion",
                    Size = new Size(280, 60),
                    Location = new Point(10, 10),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                // Create buttons
                buttonBishop = new Button
                {
                    Text = "Bishop",
                    Size = new Size(75, 30),
                    Location = new Point(50, 80)
                };
                buttonBishop.Click += (s, e) => { HandleChoice("Bishop"); };

                buttonKnight = new Button
                {
                    Text = "Knight",
                    Size = new Size(75, 30),
                    Location = new Point(125, 80)
                };
                buttonKnight.Click += (s, e) => { HandleChoice("Knight"); };

                buttonRook = new Button
                {
                    Text = "Rook",
                    Size = new Size(75, 30),
                    Location = new Point(200, 80)
                };
                buttonRook.Click += (s, e) => { HandleChoice("Rook"); };

                // Add controls to the panel
                messageBoxPanel.Controls.Add(messageLabel);
                messageBoxPanel.Controls.Add(buttonBishop);
                messageBoxPanel.Controls.Add(buttonKnight);
                messageBoxPanel.Controls.Add(buttonRook);

                // Add the panel to the form
                this.Controls.Add(messageBoxPanel);
                messageBoxPanel.BringToFront();

                return await _choiceCompletionSource.Task;
            }
            return "";
        }

        private void HandleChoice(string choice)
        {
            // Remove the message box panel
            this.Controls.Remove(messageBoxPanel);

            // Complete the TaskCompletionSource, allowing the awaiting code to continue
            _choiceCompletionSource.SetResult(choice);
        }
    }
}
