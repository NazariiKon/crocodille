using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CrocodileClient
{
    /// <summary>
    /// Логика взаимодействия для RegWindow.xaml
    /// </summary>
    public partial class RegWindow : Window
    {
        private readonly string connectionStr = "Data Source=HOME-PC;Initial Catalog=Crocodile;Integrated Security=True";
        private string clientIp;
        private bool isClientReg = false;
        private string clientNickname;
        public RegWindow()
        {
            InitializeComponent();
            clientIp = GetIPAddress3().ToString();

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionStr))
                {
                    sqlConnection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT * FROM Users", sqlConnection);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows) // якщо є данні
                    {
                        while (reader.Read()) // построчно считываем данные
                        {
                            if (reader.GetValue(2).ToString().Contains(clientIp))
                            {
                                clientNickname = reader.GetValue(1).ToString();
                                nicknameTextBox.Text = clientNickname; // нік сам записується, якщо ip користувача вже э в базі
                                isClientReg = true;
                                return;
                            }
                        }

                    }
                }
            }
            catch
            {
                MessageBox.Show("ERROR ON EXIT");
            }
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nicknameTextBox.Text)) return;

            if (isClientReg)
            {
                if (!(clientNickname == nicknameTextBox.Text))
                {
                    try
                    {
                        using (SqlConnection sqlConnection = new SqlConnection(connectionStr))
                        {
                            sqlConnection.Open();
                            SqlCommand command = new SqlCommand(@"UPDATE Users SET Nickname = @name", sqlConnection);
                            SqlParameter nameParam = new SqlParameter("@name", nicknameTextBox.Text);
                            command.Parameters.Add(nameParam);
                            command.ExecuteNonQuery();
                        }
                    }
                    catch
                    {
                        MessageBox.Show("NO CONNECT TO DB", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                try
                {
                    using (SqlConnection sqlConnection = new SqlConnection(connectionStr))
                    {
                        sqlConnection.Open();
                        SqlCommand command = new SqlCommand(@"INSERT INTO Users(Nickname, Ip) VALUES(@name, @ip)", sqlConnection);
                        SqlParameter nameParam = new SqlParameter("@name", nicknameTextBox.Text);
                        command.Parameters.Add(nameParam);
                        SqlParameter ipParam = new SqlParameter("@ip", clientIp);
                        command.Parameters.Add(ipParam);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("NO CONNECT TO DB", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            MainWindow mainWindow = new MainWindow(new User() {Nickname = nicknameTextBox.Text, ip = null});
            mainWindow.Show();
            this.Close();
        }

        // повертає ip клієнта
        public static IPAddress GetIPAddress3()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint.Address;
            }
        }
    }
}
