using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
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

namespace Valieva_project
{
    /// <summary>
    /// Логика взаимодействия для SalePage.xaml
    /// </summary>
    public partial class SalePage : Page
    {
        private ProductSale currentProductSale = new ProductSale();
        private List<Product> currentProduct;
        private Agent currentAgent = new Agent();
        public SalePage(Agent agent)
        {
            InitializeComponent();

            currentAgent = agent;
            currentProduct = ВалиеваГлазкиSaveEntities.GetContext().Product.ToList();
            ComboProduct.ItemsSource = currentProduct;

            DataContext = currentProductSale;

        }

        private void ProductsComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        
        }

        private void SaveSaleButton_Click(object sender, RoutedEventArgs e)
        {
            

        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
        public class SaleHistoryItem
        {
            public string SaleDetail { get; set; }
        }

        private void AddSaleBtn_Click(object sender, RoutedEventArgs e)
        {
           

        }

        private void DelSaleBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
          
        }

        private void AddProdHistoryBtn_Click(object sender, RoutedEventArgs e)
        {

            if (currentProductSale.ID == 0)
            {
                ВалиеваГлазкиSaveEntities.GetContext().ProductSale.Add(currentProductSale);
            }
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(ProdCount.Text))
            {
                errors.AppendLine("Укажите количество");
            }
            else
            {
                int c = Convert.ToInt32(ProdCount.Text);
                if (c < 1)
                    errors.AppendLine("Укажите количество");
            }
            if (StartDate.Text == "")
                errors.AppendLine("Укажите дату");
            if (ComboProduct.SelectedItem == null)
                errors.AppendLine("Укажите наименование продукта");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            var currentProduc = ВалиеваГлазкиSaveEntities.GetContext().Product.ToList();

            currentProductSale.ProductID = currentProduc[ComboProduct.SelectedIndex].ID;
            currentProductSale.AgentID = currentAgent.ID;
            currentProductSale.ProductCount = Convert.ToInt32(ProdCount.Text);
            currentProductSale.SaleDate = Convert.ToDateTime(StartDate.Text);


            if (currentProductSale.ID == 0)
            {
                ВалиеваГлазкиSaveEntities.GetContext().ProductSale.Add(currentProductSale);
            }




            try
            {
                ВалиеваГлазкиSaveEntities.GetContext().SaveChanges();
                MessageBox.Show("информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show(ex.InnerException?.Message ?? ex.Message);
            }

        }
    }
}
