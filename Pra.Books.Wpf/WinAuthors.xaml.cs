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
using System.Xml.Linq;


namespace Pra.Books.Wpf
{
    /// <summary>
    /// Interaction logic for WinAuthors.xaml
    /// </summary>
    public partial class WinAuthors : Window
    {
        private readonly IBookService bibService;
        private bool isNew;

        public bool IsUpdated { get; private set; }

        public WinAuthors(IBookService bibService)
        {
            this.bibService = bibService;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateAuthors();
            ActivateLeft();
        }

        private void PopulateAuthors(Author authorToSelect = null)
        {
            lstAuthors.SelectedValuePath = "Id";
            lstAuthors.ItemsSource = bibService.Authors;

            if(authorToSelect != null)
            {
                // eerst deselecteren om refresh te forceren indien zelfde auteur geselecteerd blijft
                lstAuthors.UnselectAll();
                lstAuthors.SelectedValue = authorToSelect.Id;
            }
        }

        private void RefreshSelectedAuthor()
        {
            Author author = (Author)lstAuthors.SelectedItem;
            if(author != null)
            {
                PopulateAuthors(author);
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

        private void LstAuthors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearControls();
            if(lstAuthors.SelectedItem != null)
            {
                Author author = (Author)lstAuthors.SelectedItem;
                txtName.Text = author.Name;
                lstBooks.ItemsSource = bibService.GetBooks(author);
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
            if (lstAuthors.SelectedItem != null)
            {
                isNew = false;
                ActivateRight();
                txtName.Focus();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ActivateLeft();
            RefreshSelectedAuthor();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text.Trim();

            if (isNew)
            {
                AddAuthor(name);
            }
            else
            {
                Author author = (Author)lstAuthors.SelectedItem;
                UpdateAuthor(author, name);
            }
        }

        private void AddAuthor(string name)
        {
            try
            {
                Author author = new Author(name);
                if (!bibService.AddAuthor(author))
                {
                    throw new Exception("Nieuwe auteur kon niet bewaard worden");
                }
                SelectAuthorAfterUpdate(author);
            }
            catch (Exception ex)
            {
                // exceptie kan vanuit twee plaatsen optreden:
                // 1: vanuit constructor Author indien naam niet geldig is (dus vanuit class lib)
                // 2: de exceptie die hierboven opgegooid wordt indien het opslaan niet lukt
                ShowError(ex.Message, "Fout bij aanmaken auteur");
            }
        }

        private void UpdateAuthor(Author author, string newName)
        {
            try
            {
                author.Name = newName;
                if (!bibService.UpdateAuthor(author))
                {
                    throw new Exception("Wijziging aan auteur kon niet bewaard worden");
                }
                SelectAuthorAfterUpdate(author);
            }
            catch (Exception ex)
            {
                // exceptie kan vanuit twee plaatsen optreden:
                // 1: vanuit setter property Name indien naam niet geldig is (dus vanuit class lib)
                // 2: de exceptie die hierboven opgegooid wordt indien het opslaan niet lukt
                ShowError(ex.Message, "Fout bij aanpassen auteur");
            }
        }

        private void SelectAuthorAfterUpdate(Author updatedAuthor)
        {
            IsUpdated = true;
            PopulateAuthors(updatedAuthor);
            ActivateLeft();
        }

        private void ShowError(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            txtName.Focus();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lstAuthors.SelectedItem != null)
            {
                Author author = (Author)lstAuthors.SelectedItem;
                if (bibService.IsAuthorInUse(author))
                {
                    MessageBox.Show("Deze auteur is nog in gebruik en kan niet verwijderd worden!", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (MessageBox.Show("Ben je zeker?", "Auteur wissen", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (!bibService.DeleteAuthor(author))
                    {
                        MessageBox.Show("We konden deze auteur niet verwijderen!", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    IsUpdated = true;
                    ClearControls();
                    PopulateAuthors();
                }
            }
        }
    }
}
