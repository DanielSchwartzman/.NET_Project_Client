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
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace NET_Project_Client
{
    public partial class Form2 : Form
    {
        int selectedGid;
        int selectedPid;
        string selectedName;
        private string connectionstr = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\GitRep\\NET_Project_Client\\NET_Project_Client\\Database1.mdf;Integrated Security=True";
        public Form2()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form2_Load);
        }

        private void Form2_Load(object sender, System.EventArgs e)
        {
            addComboboxUserNames();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1(selectedPid);
            form1.Show();  
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3(selectedGid);
            form3.Show();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string cb = comboBox2.SelectedItem.ToString();
            string numberPart = cb.Substring(cb.IndexOf(": ") + 2);
            selectedGid = int.Parse(numberPart);
        }

        private void addComboboxItems(int PID)
        {
            string queryString = "SELECT * FROM [dbo].[GamesPlayed] WHERE Pid="+PID+";";

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
                        int pid = reader.GetInt32(1); // Pid column // Name column

                        comboBox2.Items.Add("Game ID: "+gid);
                    }
                }
                connection.Close();
            }
        }

        private void addComboboxUserNames()
        {
            comboBox1.Items.Add("Shmulik ID: " + 1);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string cb = comboBox1.SelectedItem.ToString();
            string numberPart = cb.Substring(cb.IndexOf(": ") + 2);
            selectedPid = int.Parse(numberPart);
            addComboboxItems(selectedPid);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
