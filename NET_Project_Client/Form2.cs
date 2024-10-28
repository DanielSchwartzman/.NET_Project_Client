﻿using NET_Project_Client.Model;
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
using System.Net.Http;
using Newtonsoft.Json; // For JSON deserialization, install via NuGet if not present

namespace NET_Project_Client
{
    public partial class Form2 : Form
    {
        int selectedGid;
        int selectedPid;
        int userPid;
        string selectedName;
        private string connectionstr = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\GitRep\\NET_Project_Client\\NET_Project_Client\\Database1.mdf;Integrated Security=True";
        private static readonly HttpClient client = new HttpClient();
        private bool button1Lock = false;
        public Form2()
        {
            InitializeComponent();

            this.Load += new EventHandler(Form2_Load);
            
        }

        private async void Form2_Load(object sender, System.EventArgs e)
        {
            List<Client> clients = await GetClientsAsync();
            var clientDisplays = clients.Select(client => new
            {
                DisplayName = $"{client.Name} (ID: {client.ID})",
                ID = client.ID,
                Name = client.Name
            }).ToList();

            if (clients != null && clients.Any())
            {
                button1Lock = false;
                // Bind the data to the ComboBox
                comboBox3.DataSource = clientDisplays;
                comboBox3.DisplayMember = "DisplayName";  // Display client names
                comboBox3.ValueMember = "ID";      // Use client ID as the value
                userPid = (int)clients.ElementAt(0).ID;
            }
            else
            {
                button1Lock = true;
                MessageBox.Show("No clients found or failed to retrieve data.");
            }
            addComboboxUserNames();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!button1Lock)
            {
                Form1 form1 = new Form1(userPid);
                form1.Show();
            }
            
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

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            int.TryParse(comboBox3.SelectedValue.ToString(), out userPid);
            
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        internal async Task<List<Client>> GetClientsAsync()
        {
            try
            {
                // Replace with your API's base URL
                string apiUrl = "http://localhost:5177/api/Clients";

                // Send the GET request to the API
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                response.EnsureSuccessStatusCode(); // Throws if not a success code (e.g., 404, 500)

                // Get the response content as a string
                string responseData = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON data into a list of clients
                List<Client> clients = JsonConvert.DeserializeObject<List<Client>>(responseData);

                return clients;
            }
            catch (HttpRequestException e)
            {
                //MessageBox.Show($"Request error: {e.Message}");
                return new List<Client>();
            }
        }
    }
}
