using System;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Avalonia.Interactivity; 
using System.IO;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;


namespace Market
{
    public partial class Orderr : Window
    {
        public int COUNT=1;
        public List<UserItemStorage> Products { get; set; }
        public int Role { get; set; }
        public int Idd { get; set; }
        public List<string> lll = new List<string>();

        public Orderr(int id, int role, List<string> listik)
        {
            InitializeComponent();

            lll = listik;
            Idd = id;
            Role = role;

            Products = FillFromListic();
            // Устанавливаем источник данных для ListBox
            fff.ItemsSource = Products;
            
        }
        private void Button_Click(object sender, RoutedEventArgs e){
            this.Hide();
            new MainWindow(Idd,Role).Show();
            this.Close();
        }
        private void Pressed(object sender, PointerPressedEventArgs e)
        {
            OpenChoosePickupPoint();
        }
        private void OpenChoosePickupPoint()
       {
           var choosePickupPointWindow = new ChoosePickupPoint();
           choosePickupPointWindow.AddressSelected += OnAddressSelected; // Подписываемся на событие
           choosePickupPointWindow.ShowDialog(this); // Открываем окно как модальное
       }
       private void OnAddressSelected(int id, string address)
       {
           // Обработка выбранного адреса
           Console.WriteLine($"Выбранный адрес: {address}, ID: {id}");
           PickUpp.Text = address;
           // Здесь вы можете выполнить дополнительные действия, например, сохранить адрес или обновить интерфейс
       }
      private void BntMakeZakaz_Click(object? sender, RoutedEventArgs e){

      }

        private List<UserItemStorage> FillFromListic()
        {
            List<UserItemStorage> list = new List<UserItemStorage>();
            string connectionString = "Server=localhost;Database=shopDB;User Id=root;Password=;";

            foreach (string datab in lll)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();
                        MySqlCommand command = new MySqlCommand("SELECT ProductName, ProductDescription, ProductCost, ProductPhoto, ProductQuantityInStock FROM product WHERE ProductArticleNumber = @art", connection);
                        command.Parameters.AddWithValue("@art", datab);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string name = reader.GetString(0);
                                string discr = reader.GetString(1);
                                int price = reader.GetInt32(2);
                                Bitmap image = GetImage(reader); // Получаем изображение
                                int count = reader.GetInt32(4);

                                UserItemStorage userItem = new UserItemStorage(datab, name, discr, price, 1, image);
                                list.Add(userItem);
                            }
                        }
                    }
                }
                                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
            return list;
        }

       public Bitmap GetImage(MySqlDataReader reader)
{
    // Предполагаем, что изображение хранится в столбце с индексом 3
    if (!reader.IsDBNull(3)) // Проверяем, есть ли данные в столбце
    {
        byte[] imageData = (byte[])reader[3]; // Получаем байты изображения
        if (imageData != null && imageData.Length > 0) // Проверяем, что данные не пустые
        {
            try
            {
                using (var stream = new MemoryStream(imageData))
                {
                    return new Bitmap(stream); // Создаем Bitmap из потока
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке изображения: {ex.Message}");
            }
        }
    }
    return new Bitmap("Assets/logo.png"); // Возвращаем изображение по умолчанию, если данных нет
}

        private void PluseButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is UserItemStorage userItemStorage)
            {
                // Логика для увеличения количества товара
                userItemStorage.ItemCount++;
                fff.ItemsSource = Products;
                Console.WriteLine($"Увеличено количество: {userItemStorage.ItemArticul}, новое количество: {userItemStorage.ItemCount}");
            }
        }

        private void MinuseButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is UserItemStorage userItemStorage)
            {
                // Логика для уменьшения количества товара
                if (userItemStorage.ItemCount > 0)
                {
                    userItemStorage.ItemCount--;
                    fff.ItemsSource = Products;
                    Console.WriteLine($"Уменьшено количество: {userItemStorage.ItemArticul}, новое количество: {userItemStorage.ItemCount}");
                }
            }
        }
    }
}