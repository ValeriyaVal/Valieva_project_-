using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Valieva_project
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        //private List<Agent> currentAgents;
        //public List<Agent> TableList;
        public Agent currentAgents = new Agent();
        private List<Product> Items;
        private ProductSale currentProductSale = new ProductSale();

        public AddEditPage(Agent SelectedAgents)
        {
            InitializeComponent();

            if (SelectedAgents != null)
            {
                this.currentAgents = SelectedAgents;

                ComboType.SelectedIndex = currentAgents.AgentTypeID - 1;
            }


            var allProducts = ВалиеваГлазкиSaveEntities.GetContext().Product.ToList();

            var currentProductSales = ВалиеваГлазкиSaveEntities.GetContext().ProductSale.ToList();
            currentProductSales = currentProductSales.Where(p => p.AgentID == currentAgents.ID).ToList();

            History.ItemsSource = currentProductSales;


            DataContext = currentProductSale;

            //DataContext = currentProductSales;
            //DataContext = allProductSales;


            DataContext = currentAgents;
            LoadItems();

            //   var currentAgents = ВалиеваГлазкиSaveEntities.GetContext().Agent.ToList();

        }

        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            if (myOpenFileDialog.ShowDialog() == true)
            {
                currentAgents.Logo = myOpenFileDialog.FileName;
                LogoImage.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName));
                
                
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            currentAgents.AgentTypeID = ComboType.SelectedIndex + 1;
            if (string.IsNullOrWhiteSpace(currentAgents.Title))
                errors.AppendLine("Укажите наименование агента");
            if (string.IsNullOrWhiteSpace(currentAgents.Address))
                errors.AppendLine("Укажите адрес агента");
            if (string.IsNullOrWhiteSpace(currentAgents.DirectorName))
                errors.AppendLine("Укажите ФИО директора");
            if (ComboType.SelectedItem == null)
                errors.AppendLine("Укажите тип агента");
            if (string.IsNullOrWhiteSpace(currentAgents.Priority.ToString()))
                errors.AppendLine("Укажите приоритет агента");
            if (currentAgents.Priority <= 0)
                errors.AppendLine("Укажите положительный приоритет агента");
            if (string.IsNullOrWhiteSpace(currentAgents.INN))
                errors.AppendLine("Укажите ИИН агента");
            if (string.IsNullOrWhiteSpace(currentAgents.KPP))
                errors.AppendLine("Укажите КПП агента");
            if (string.IsNullOrWhiteSpace(currentAgents.Phone))
                errors.AppendLine("Укажите телефон агента");
            else
            {
                string ph = currentAgents.Phone.Replace("(", "").Replace("-", "").Replace("+", "");
                if (((ph[1] == '9' || ph[1] == '4' || ph[1] == '8') && ph.Length != 11)
                    || (ph[1] == '3' && ph.Length != 12))
                    errors.AppendLine("Укажите правильно телефон агента");

                string kpp = currentAgents.KPP;
                if ((kpp.Length != 9))
                    errors.AppendLine("Укажите корректный КПП агента. В КПП должно быть 9 цифр.");

                string inn = currentAgents.INN;
                if ((inn.Length != 10))
                    errors.AppendLine("Укажите корректный ИНН агента. В ИНН должно быть 10 цифр");

            }
            if (string.IsNullOrWhiteSpace(currentAgents.Email))
                errors.AppendLine("Укажите почту агента");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            if (currentAgents.ID == 0)
                ВалиеваГлазкиSaveEntities.GetContext().Agent.Add(currentAgents);
            try
            {
                ВалиеваГлазкиSaveEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void LoadItems()
        {
            // Здесь вы загружаете элементы из базы данных
            Items = ВалиеваГлазкиSaveEntities.GetContext().Product.ToList();
            ComboProduct.ItemsSource = Items; // Устанавливаем источник данных для ComboBox
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var currentAgent = (sender as Button).DataContext as Agent;
            var currentAgent1 = ВалиеваГлазкиSaveEntities.GetContext().ProductSale.ToList();
            currentAgent1 = currentAgent1.Where(p => p.AgentID == currentAgent.ID).ToList();
            if (currentAgent1.Count != 0)
                MessageBox.Show("Невозможно выполнить удаление, т.к. существуют записи на эту услугу");
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {                  
                    ВалиеваГлазкиSaveEntities.GetContext().Agent.Remove(currentAgent);

                    try
                    {
                        ВалиеваГлазкиSaveEntities.GetContext().SaveChanges();
                        MessageBox.Show("Информация сохранена");
                        Manager.MainFrame.GoBack();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }

        private void SaveBtn_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void AddHistory_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ComboProduct.SelectedItem as Product;

            // Получаем количество из TextBox (например, для количества продукта)
            int productCount;
            if (!int.TryParse(CountProductTB.Text, out productCount) || productCount <= 0)
            {
                MessageBox.Show("Пожалуйста, введите корректное количество.");
                return;
            }

            if (selectedProduct != null)
            {
                // Создаем новый объект ProductSale
                var newSale = new ProductSale
                {
                    AgentID = currentAgents.ID, // Указываем ID агента
                    ProductID = selectedProduct.ID, // Указываем ID выбранного продукта
                    SaleDate = StartDate.SelectedDate ?? DateTime.Now, // Указываем дату продажи (если выбрана)
                    ProductCount = productCount // Указываем количество, введенное пользователем
                };

                // Добавляем новый объект в контекст и сохраняем изменения
                ВалиеваГлазкиSaveEntities.GetContext().ProductSale.Add(newSale);
                ВалиеваГлазкиSaveEntities.GetContext().SaveChanges();

                MessageBox.Show("информация сохранена");

                // Обновляем список продаж
                LoadProductSalesForCurrentAgent();

                // Очистка полей ввода (по желанию)
                ComboProduct.SelectedItem = null;
                CountProductTB.Clear();
                StartDate.SelectedDate = null;
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите продукт для добавления.");
            }
        }

        private void DelHistory_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = History.SelectedItems.Cast<ProductSale>().ToList();

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите хотя бы один элемент для удаления.");
                return;
            }
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление? ", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        var context = ВалиеваГлазкиSaveEntities.GetContext();

                        // Удаляем каждый выбранный элемент из контекста
                        foreach (var item in selectedItems)
                        {
                            context.ProductSale.Remove(item);
                        }

                        // Сохраняем изменения в базе данных
                        context.SaveChanges();

                        LoadProductSalesForCurrentAgent();

                        MessageBox.Show("Элементы успешно удалены.");
                    }


                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }
    
        /*
        private void ComboTitle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboProduct.SelectedItem is Product selectedProduct)
            {
                // Подставляем значение Title в TextBox
                SearchBoxForComboBox.Text = selectedProduct.Title; // Убедитесь, что у вас есть TextBox с именем SearchBoxForComboBox
            }
        }

        private void SearchBoxForComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBoxForComboBox.Text.ToLower();
            var filteredItems = Items.Where(p => p.Title.ToLower().Contains(searchText)).ToList();
            ComboTitle.ItemsSource = filteredItems;

            // Если ничего не найдено, показываем все элементы
            if (string.IsNullOrEmpty(searchText))
            {
                ComboTitle.ItemsSource = Items;
            }
        }*/
        private void LoadProductSalesForCurrentAgent()
        {
            var currentProductSales = ВалиеваГлазкиSaveEntities.GetContext().ProductSale
                .Where(ps => ps.AgentID == currentAgents.ID) // Предполагается, что у ProductSale есть поле AgentID
                .ToList();

            // Устанавливаем источник данных для списка продаж
            History.ItemsSource = currentProductSales;
        }

        //private void ComboProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    string searchText = SearchBoxForComboBox.Text.ToLower();
        //    var filteredItems = Items.Where(p => p.Title.ToLower().Contains(searchText)).ToList();
        //    ComboTitle.ItemsSource = filteredItems;

        //    // Если ничего не найдено, показываем все элементы
        //    if (string.IsNullOrEmpty(searchText))
        //    {
        //        ComboTitle.ItemsSource = Items;
        //    }

        //}
    }
}
