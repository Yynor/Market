using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using MySql.Data.MySqlClient;

namespace Market;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        LoginButton.Click += LoginButton_Click;
    }

   private void LoginButton_Click(object? sender, RoutedEventArgs e)
{
    // Получаем текст из TextBox и убираем пробелы в начале и в конце
    string loginText = LoginTextBox.Text.Trim();
    string passwordText = PasswordTextBox.Text.Trim();

    string connectionString = "Server=localhost;Database=shopDB;User Id=root;Password=;";
    using (MySqlConnection connection = new MySqlConnection(connectionString))
    {
        connection.Open();
        
        // Используем параметризованный запрос
        MySqlCommand command = new MySqlCommand("SELECT UserID,UserRole, UserPassword FROM user WHERE UserLogin = @login", connection);
        command.Parameters.AddWithValue("@login", loginText);
        
        using (MySqlDataReader reader = command.ExecuteReader())
        {
            if (reader.Read())
            {
                // Получаем пароль из базы данных
                string storedPassword = reader.GetString(2);

                // Проверяем введенный пароль
                if (passwordText == storedPassword)
                {
                    Hide();
                    new MainWindow(reader.GetInt32(0),reader.GetInt32(1)).Show();
                    Close();
                }
                else
                {
                    Console.WriteLine("Неверный логин либо пароль");
                }
            }
            else
            {
                Console.WriteLine("Пользователь не найден");
            }
        }
    }
}

    private async void MainWindowRegestrationTextBlockPressed(object sender, PointerPressedEventArgs e)
        {
          var RegWindow = new RegWindow();
          await RegWindow.ShowDialog(this);
          
       }
}