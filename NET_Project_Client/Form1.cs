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

namespace NET_Project_Client
{
    public partial class Form1 : Form
    {
        ChessBoard chessBoard;
        public Form1()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form1_Load);
            this.Paint += new PaintEventHandler(Form1_Paint);
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            chessBoard = new ChessBoard();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // Set chessboard size (8x4 grid)
            int rows = 8;
            int columns = 4;
            int squareWidth = this.ClientSize.Width / columns / 3;
            int squareHeight = this.ClientSize.Height / rows;

            // Graphics object for drawing
            Graphics g = e.Graphics;

            // Draw each square on the chessboard
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    // Determine square color based on row and column indices
                    bool isWhiteSquare = (row + col) % 2 == 0;
                    Brush brush = isWhiteSquare ? Brushes.AliceBlue : Brushes.Wheat;

                    // Draw square
                    g.FillRectangle(brush, col * squareWidth, row * squareHeight, squareWidth, squareHeight);

                    // Draw chess piece if one is located here
                    Piece piece = chessBoard.GetPieceAt(row, col); // Assume you have a method to get the piece at a given row and column
                    if (piece != null && piece.img != null)
                    {
                        // Draw the piece image
                        g.DrawImage(piece.img, col * squareWidth, row * squareHeight, squareWidth, squareHeight);
                    }
                }
            }
        }
    }
}
