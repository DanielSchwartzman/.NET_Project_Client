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
using System.Net.Http;
using Newtonsoft.Json;

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
            List<string> customNames = clients.Select(client => $"{client.Name} (ID: {client.ID})").ToList();
            var clientDisplays = clients.Select(client => new
            {
                DisplayName = $"{client.Name} (ID: {client.ID})",
                ID = client.ID,
                Name = client.Name
            }).ToList();//for combobox 3 

            if (clients != null && clients.Any())
            {
                button1Lock = true;
                // Bind the data to ComboBox3
                comboBox3.DataSource = clientDisplays;
                comboBox3.DisplayMember = "DisplayName";  // Display client names
                comboBox3.ValueMember = "ID";      // Use client ID as the value
                userPid = (int)clients.ElementAt(0).ID;

                // Bind the data to ComboBox1
                comboBox1.DataSource = customNames;
                selectedPid = (int)clients.ElementAt(0).ID;
                addComboboxItems(selectedPid);
            }
            else
            {
                button1Lock = false;
                MessageBox.Show("No clients found or failed to retrieve data.");
            }
            //addComboboxUserNames();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1Lock)
            {
                Form1 form1 = new Form1(userPid);
                form1.Show();
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!comboBox2.SelectedItem.ToString().Equals("No recorded games"))
            {
                Form3 form3 = new Form3(selectedGid);
                form3.Show();
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!comboBox2.SelectedItem.ToString().Equals("No recorded games"))
            {
                string cb = comboBox2.SelectedItem.ToString();
                string numberPart = cb.Substring(cb.IndexOf(": ") + 2);
                selectedGid = int.Parse(numberPart);
            }
        }

        private void addComboboxItems(int PID)
        {
            string queryString = "SELECT * FROM [dbo].[GamesPlayed] WHERE Pid="+PID+";";
            List<string> customGid= new List<string>();
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

                        customGid.Add("Game ID: " + gid);
                    }
                    if (customGid.Count() > 0)
                        comboBox2.DataSource = customGid;
                    else
                        comboBox2.DataSource = new List<string> {"No recorded games"};

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
            if (comboBox1.SelectedValue != null)
            {
                string cb = comboBox1.SelectedItem.ToString();
                int startIndex = cb.IndexOf("ID: ") + 4;
                int endIndex = cb.IndexOf(")", startIndex);
                string idString = cb.Substring(startIndex, endIndex - startIndex).Trim();
                selectedPid = int.Parse(idString);
                addComboboxItems(selectedPid);
            }
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
                return new List<Client>();
            }
        }
    }
}
