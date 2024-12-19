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

namespace Valieva_project
{
    /// <summary>
    /// Логика взаимодействия для ServicePage.xaml
    /// </summary>
    public partial class AgentPage : Page
    {
        int CountRecords;
        int CountPage;
        int CurrentPage = 0;
        private Agent CurrentAgent = new Agent();

        public List<Agent> CurrentPageList = new List<Agent>();
        public List<Agent> TableList;

        public AgentPage()
        {
            InitializeComponent();
            var currentServices = ВалиеваГлазкиSaveEntities.GetContext().Agent.ToList();

            ServiceListView.ItemsSource = currentServices;
            ServiceListView.SelectedIndex = 0;
            UpdateAgents();
        }

        private void ChangePage(int direction, int? selectedPage)
        {
            CurrentPageList.Clear();
            CountRecords = TableList.Count;
            if (CountRecords % 10 > 0)
            {
                CountPage = CountRecords / 10 + 1;
            }
            else
            {
                CountPage = CountRecords / 10;
            }

            Boolean Ifupdate = true;

            int min;
            if (selectedPage.HasValue)
            {
                if (selectedPage >= 0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                    for (int i = CurrentPage * 10; i < min; i++)
                    {
                        CurrentPageList.Add(TableList[i]);
                    }
                }
            }
            else
            {
                switch (direction)
                {
                    case 1:
                        if (CurrentPage > 0)
                        {
                            CurrentPage--;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords; 
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }

                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;

                    case 2:
                        if (CurrentPage < CountPage - 1)
                        {
                            CurrentPage++;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;
                }

            }
            if (Ifupdate)
            {
                PageListBox.Items.Clear();

                for (int i = 1; i <= CountPage; i++)
                {
                    PageListBox.Items.Add(i);
                }
                PageListBox.SelectedIndex = CurrentPage;

                min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                TBCount.Text = min.ToString();
                TBAllRecords.Text = " из " + CountRecords.ToString();

                ServiceListView.ItemsSource = CurrentPageList;
                ServiceListView.Items.Refresh();

            }
        }

        private void UpdateAgents()
        {
            var currentAgents =ВалиеваГлазкиSaveEntities.GetContext().Agent.ToList();

            currentAgents = currentAgents.Where(p => p.Title.ToLower().Contains(TBSearch.Text.ToLower()) || p.Phone.Replace("+7", "8").Replace("(", "").Replace(") ", "").Replace(" ", "").Replace("-", "").Contains(TBSearch.Text.Replace("+7", "8").Replace("(", "").Replace(") ", "").Replace(" ", "").Replace("-", ""))
            || p.Email.ToLower().Contains(TBSearch.Text.ToLower())).ToList();


            switch (SortCombo.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    currentAgents = currentAgents.OrderBy(p => p.Title).ToList();
                    break;
                case 2:
                    currentAgents = currentAgents.OrderByDescending(p => p.Title).ToList();
                    break;
                case 3:
                    currentAgents = currentAgents.OrderBy(p => p.Discount).ToList();
                    break;

                case 4:
                    currentAgents = currentAgents.OrderByDescending(p => p.Discount).ToList();
                    break;
                case 5:
                    currentAgents = currentAgents.OrderBy(p => p.Priority).ToList();
                    break;
                case 6:
                    currentAgents = currentAgents.OrderByDescending(p => p.Priority).ToList();
                    break;
            }
            

            //if (SortCombo.SelectedIndex == 0)
            //    currentAgents = currentAgents.OrderBy(p => p.Title).ToList();
            //if (SortCombo.SelectedIndex == 1)
            //    currentAgents = currentAgents.OrderByDescending(p => p.Title).ToList();
            ////if (SortCombo.SelectedIndex == 2)
            ////    currentAgents = currentAgents.OrderBy(p => p.SaleProduct).ToList();
            ////if (SortCombo.SelectedIndex == 3)
            ////    currentAgents = currentAgents.OrderByDescending(p => p.SaleProduct).ToList();
            //if (SortCombo.SelectedIndex == 4)
            //    currentAgents = currentAgents.OrderBy(p => p.Priority).ToList();
            //if (SortCombo.SelectedIndex == 5)
            //    currentAgents = currentAgents.OrderByDescending(p => p.Priority).ToList();


            if (FilterCombo.SelectedIndex == 0)
                currentAgents = currentAgents.Where(p => (p.AgentTypeID >= 1 && p.AgentTypeID <= 6)).ToList();
            if (FilterCombo.SelectedIndex == 1)
                currentAgents = currentAgents.Where(p => (p.AgentTypeID == 1)).ToList();
            if (FilterCombo.SelectedIndex == 2)
                currentAgents = currentAgents.Where(p => (p.AgentTypeID == 2)).ToList();
            if (FilterCombo.SelectedIndex == 3)
                currentAgents = currentAgents.Where(p => (p.AgentTypeID == 3)).ToList();
            if (FilterCombo.SelectedIndex == 4)
                currentAgents = currentAgents.Where(p => (p.AgentTypeID == 4)).ToList();
            if (FilterCombo.SelectedIndex == 5)
                currentAgents = currentAgents.Where(p => (p.AgentTypeID == 5)).ToList();
            if (FilterCombo.SelectedIndex == 6)
                currentAgents = currentAgents.Where(p => (p.AgentTypeID == 6)).ToList();

            ServiceListView.ItemsSource = currentAgents;
            ServiceListView.Items.Refresh();

            TableList = currentAgents;
            ChangePage(0, 0);

        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
          //  Manager.MainFrame.Navigate(new AddEditPage());
        }

        private void TBSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAgents();
            
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void ComboSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void TBSearch_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void AgentListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();

        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }

        private void TBSearch_TextChanged_2(object sender, TextChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void SortCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();

        }

        private void FilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateAgents();
        }

        private void ChangePriority_Click(object sender, RoutedEventArgs e)
        {
            UpdateAgents();
        }

        private void ServiceListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();
            if (ServiceListView.SelectedItems.Count > 1)
            {
                ChangePriorityButton.Visibility = Visibility.Visible;
            }
            else
            {
                ChangePriorityButton.Visibility = Visibility.Hidden;
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Agent));
            UpdateAgents();
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            UpdateAgents();
        }

        private void PageListBox_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }

        private void AddButton_Click_1(object sender, RoutedEventArgs e)
        {
            //Manager.MainFrame.Navigate(new AddEditPage(null));
            Manager.MainFrame.Navigate(new AddEditPage(null));
            UpdateAgents();

        }

        private void ChangePriorityButton_Click(object sender, RoutedEventArgs e)
        {
            int maxPriority = 0;
            foreach (Agent agent in ServiceListView.SelectedItems)
            {
                if (agent.Priority > maxPriority)
                    maxPriority = agent.Priority;
            }

            SetWindow myWindow = new SetWindow(maxPriority);
            myWindow.ShowDialog();

            if (string.IsNullOrEmpty(myWindow.TBPriority.Text))
            {
                MessageBox.Show("Изменение не произошло");
            }
            else
            {
                // Проверяем, является ли введенное значение числом
                if (int.TryParse(myWindow.TBPriority.Text, out int newPriority))
                {
                    // Проверяем, является ли новое значение приоритета отрицательным
                    if (newPriority < 0)
                    {
                        MessageBox.Show("Приоритет не может быть отрицательным.");
                        return; // Выход из метода, если приоритет отрицательный
                    }

                    foreach (Agent agent in ServiceListView.SelectedItems)
                    {
                        agent.Priority = newPriority;
                    }

                    try
                    {
                        ВалиеваГлазкиSaveEntities.GetContext().SaveChanges();
                        MessageBox.Show("Информация сохранена");
                        UpdateAgents();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
                else
                {
                    MessageBox.Show("Введите корректное числовое значение для приоритета.");
                }
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));

        }

        private void History_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new SalePage(CurrentAgent));

        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                ВалиеваГлазкиSaveEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                ServiceListView.ItemsSource = ВалиеваГлазкиSaveEntities.GetContext().Agent.ToList();
            }

        }
    }
}
