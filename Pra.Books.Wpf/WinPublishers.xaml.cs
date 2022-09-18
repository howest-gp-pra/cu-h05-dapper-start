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
using System.Windows.Shapes;
using Pra.Books.Core.Entities;
using Pra.Books.Core.Services;
using Pra.Books.Core.Interfaces;

namespace Pra.Books.Wpf
{
    /// <summary>
    /// Interaction logic for WinPublishers.xaml
    /// </summary>
    public partial class WinPublishers : Window
    {
        private readonly IBookService bibService;
        private bool isNew;

        public bool IsUpdated { get; internal set; }

        public WinPublishers(IBookService bibService)
        {
            this.bibService = bibService;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PopulatePublishers();
            ActivateLeft();
        }

        private void PopulatePublishers(Publisher publisherToSelect = null)
        {
            lstPublishers.SelectedValuePath = "Id";
            lstPublishers.ItemsSource = bibService.Publishers;

            if(publisherToSelect != null)
            {
                // eerst deselecteren om refresh te forceren indien zelfde uitgever geselecteerd blijft
                lstPublishers.UnselectAll();
                lstPublishers.SelectedValue = publisherToSelect.Id;
            }
        }

        private void RefreshSelectedPublisher()
        {
            Publisher publisher = (Publisher)lstPublishers.SelectedItem;
            if(publisher != null)
            {
                PopulatePublishers(publisher);
            }
        }

        private void ClearControls()
        {
            txtName.Text = "";
            lstBooks.ItemsSource = null;
            lstBooks.Items.Refresh();
        }

        private void ActivateLeft()
        {
            grpLeft.IsEnabled = true;
            grpRight.IsEnabled = false;
            btnSave.Visibility = Visibility.Hidden;
            btnCancel.Visibility = Visibility.Hidden;
        }

        private void ActivateRight()
        {
            grpLeft.IsEnabled = false;
            grpRight.IsEnabled = true;
            btnSave.Visibility = Visibility.Visible;
            btnCancel.Visibility = Visibility.Visible;
        }

        private void LstPublishers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearControls();
            if (lstPublishers.SelectedItem != null)
            {
                Publisher publisher = (Publisher)lstPublishers.SelectedItem;
                txtName.Text = publisher.Name;
                lstBooks.ItemsSource = bibService.GetBooks(null, publisher);
            }
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            isNew = true;
            ActivateRight();
            ClearControls();
            txtName.Focus();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (lstPublishers.SelectedItem != null)
            {
                isNew = false;
                ActivateRight();
                txtName.Focus();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ActivateLeft();
            RefreshSelectedPublisher();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text.Trim();
            
            if (isNew)
            {
                AddPublisher(name);
            }
            else
            {
                Publisher publisher = (Publisher)lstPublishers.SelectedItem;
                UpdatePublisher(publisher, name);
            }
        }

        private void AddPublisher(string name)
        {
            try
            {
                Publisher publisher = new Publisher(name);
                if (!bibService.AddPublisher(publisher))
                {
                    throw new Exception("Nieuwe uitgeverij kon niet bewaard worden");
                }
                SelectPublisherAfterUpdate(publisher);
            }
            catch (Exception ex)
            {
                // exceptie kan vanuit twee plaatsen optreden:
                // 1: vanuit constructor Publisher indien naam niet geldig is (dus vanuit class lib)
                // 2: de exceptie die hierboven opgegooid wordt indien het opslaan niet lukt
                ShowError(ex.Message, "Fout bij aanmaken uitgeverij");
            }
        }

        private void UpdatePublisher(Publisher publisher, string newName)
        {
            try
            {
                publisher.Name = newName;
                if (!bibService.UpdatePublisher(publisher))
                {
                    throw new Exception("Wijziging aan uitgeverij kon niet bewaard worden");
                }
                SelectPublisherAfterUpdate(publisher);
            }
            catch (Exception ex)
            {
                // exceptie kan vanuit twee plaatsen optreden:
                // 1: vanuit setter property Name indien naam niet geldig is (dus vanuit class lib)
                // 2: de exceptie die hierboven opgegooid wordt indien het opslaan niet lukt
                ShowError(ex.Message, "Fout bij aanpassen uitgeverij");
            }
        }

        private void SelectPublisherAfterUpdate(Publisher updatedPublisher)
        {
            IsUpdated = true;
            PopulatePublishers(updatedPublisher);
            ActivateLeft();
        }

        private void ShowError(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            txtName.Focus();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lstPublishers.SelectedItem != null)
            {
                Publisher publisher = (Publisher)lstPublishers.SelectedItem;
                if (bibService.IsPublisherInUse(publisher))
                {
                    MessageBox.Show("Deze uitgever is nog in gebruik en kan niet verwijderd worden!", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (MessageBox.Show("Ben je zeker?", "Uitgever wissen", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (!bibService.DeletePublisher(publisher))
                    {
                        MessageBox.Show("We konden deze uitgever niet verwijderen!", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    IsUpdated = true;
                    ClearControls();
                    PopulatePublishers();
                }
            }
        }
    }
}
