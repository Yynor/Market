using System;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Avalonia.Interactivity; 
using System.Linq;

namespace Market
{
    public partial class MainWindow : Window
    {
         public List<Product> Products { get; set; } = new List<Product>();
        public List<Product> FilteredProducts { get; set; } = new List<Product>();
        public int Role;
        public int Idd;
       public MainWindow(int id,int role)
       {
           InitializeComponent();
           Idd = id;
           Role = role;
           Products = ListCreate();
           FilteredProducts = new List<Product>(Products); // Изначально все продукты видимы
           DataContext = this; // Устанавливаем контекст данных
           fff.ItemsSource = FilteredProducts; // Устанавливаем источник данных для ListBox
        SetButtonText();
            
            CBoxMaker.Items.Add("По умолчанию");           
            CBoxMaker.Items.Add("Webber");
            CBoxMaker.Items.Add("Luminarc");
            CBoxMaker.Items.Add("Нева");
            CBoxMaker.Items.Add("Эмаль");
            CBoxMaker.Items.Add("Solaris");
            CBoxMaker.Items.Add("Galaxy");
            CBoxMaker.Items.Add("По умолчанию");


            CBoxCategory.Items.Add("По умолчанию");
            CBoxCategory.Items.Add("Посуда");
            CBoxCategory.Items.Add("Сковорода");
            CBoxCategory.Items.Add("Сервиз");
            CBoxCategory.Items.Add("Кострюля");

            CBoxPrice.Items.Add("По умолчанию");
            CBoxPrice.Items.Add("По возрастанию");
            CBoxPrice.Items.Add("По убыванию");
            



            
           
        }
        public void SetButtonText(){
            if(Role == 1) BtnMainFormItem.Content = "Редактировать";
            if(Role == 2) BtnMainFormItem.Content = "Просмотреть";
            if(Role == 3) BtnMainFormItem.Content = "Добавить в карзину";
        }

        public List<Product> ListCreate()
        {
            string connectionString = "Server=localhost;Database=shopDB;User Id=root;Password=;";
            List<Product> list = new List<Product>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand("SELECT * FROM product", connection);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string productArticleNumber = reader.GetString(0);
                            string productName = reader.GetString(1);
                            string productDescription = reader.GetString(2);
                            string productCategory = reader.GetString(3);
                            Bitmap productPhoto = new Bitmap("Assets/logo.png"); // Замените на реальный путь к изображению
                            string productManufacturer = reader.GetString(5);
                            float productCost = reader.GetFloat(6); // Используйте GetFloat для получения значения типа float
                            int productDiscountAmount = reader.GetInt32(7);
                            int productQuantityInStock = reader.GetInt32(8);
                            string productStatus = reader.GetString(9);

                            Product product = new Product(productArticleNumber,
                                productName, productDescription, productCategory, productPhoto,
                                productManufacturer, productCost, productDiscountAmount, productQuantityInStock, productStatus);
                            list.Add(product);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            return list;
        }
 private void ShowArticleButton_Click(object sender, RoutedEventArgs e)
{
    if(Role == 3){
        if (sender is Button button && button.CommandParameter is Product product)
        {
            Console.WriteLine($"Артикул: {product.ProductArticleNumber}");
        }
    }
    if(Role==2){

    }
    if(Role==1){
        if (sender is Button button && button.CommandParameter is Product product)
        {
           
        this.Hide();
        new ChangeItemWindow(product.ProductArticleNumber, Idd,Role).Show();
        this.Close();
        }
    }
}

        public Bitmap GetImage(int photoId)
        {
            return new Bitmap("Assets/logo.png");
        }
    

        // Обработчик для изменения текста в TextBox
        private void SearchTextBox_TextChanged(object sender, Avalonia.Controls.TextChangedEventArgs e)
        {
            FilterAndSortProducts();
        }

        // Обработчик для изменения выбора в ComboBox для производителя
        private void CBoxMaker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterAndSortProducts();
        }

        // Обработчик для изменения выбора в ComboBox для категории
        private void CBoxCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterAndSortProducts();
        }

        // Обработчик для изменения выбора в ComboBox для сортировки
        private void CBoxPrice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterAndSortProducts();
        }

            // Метод для фильтрации и сортировки продуктов
        private void FilterAndSortProducts()
        {
            var selectedManufacturer = CBoxMaker.SelectedItem as string;
            var selectedCategory = CBoxCategory.SelectedItem as string;
            var sortOption = CBoxPrice.SelectedItem as string;
            var searchText = SearchTextBox.Text;

            // Фильтрация
            var filtered = Products.AsQueryable();

            // Фильтрация по производителю
            // Фильтрация по производителю
            if (selectedManufacturer != "По умолчанию" && !string.IsNullOrEmpty(selectedManufacturer))
            {
                filtered = filtered.Where(p => p.ProductManufacturer == selectedManufacturer);
            }
            // Фильтрация по категории
            if (selectedCategory != "По умолчанию" && !string.IsNullOrEmpty(selectedCategory))
            {
                filtered = filtered.Where(p => p.ProductCategory == selectedCategory);
            }
            // Фильтрация по названию
            if (!string.IsNullOrEmpty(searchText))
            {
                filtered = filtered.Where(p => p.ProductName.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }

            // Сортировка
            if (sortOption == "По возрастанию")
            {
                filtered = filtered.OrderBy(p => p.ProductCost);
            }
            else if (sortOption == "По убыванию")
            {
                filtered = filtered.OrderByDescending(p => p.ProductCost);
            }

            // Обновление списка
            FilteredProducts = filtered.ToList();
            fff.ItemsSource = FilteredProducts; // Обновляем источник данных для ListBox
        }
    }
}