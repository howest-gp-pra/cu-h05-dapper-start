using System;
using System.Windows;
using System.Windows.Controls;
using Pra.Books.Core.Entities;
using Pra.Books.Core.Services;
using Pra.Books.Core.Interfaces;

namespace Pra.Books.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IBookService bibService = new BookServiceMem();
        private bool isNew;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateBooks();
            PopulateAuthors();
            PopulatePublishers();
            ActivateLeft();
        }

        private void PopulateBooks(Book bookToSelect = null)
        {
            ClearControls();

            lstBooks.SelectedValuePath = "Id";
            
            Author author = (Author)cmbFilterAuthor.SelectedItem;
            Publisher publisher = (Publisher)cmbFilterPublisher.SelectedItem;
            lstBooks.ItemsSource = bibService.GetBooks(author, publisher);

            if(bookToSelect != null)
            {
                // eerst deselecteren om refresh te forceren indien zelfde boek geselecteerd blijft
                lstBooks.UnselectAll();
                lstBooks.SelectedValue = bookToSelect.Id;
            }
        }

        private void RefreshSelectedBook()
        {
            Book book = (Book)lstBooks.SelectedItem;
            if(book != null)
            {
                PopulateBooks(book);
            }
        }

        private void PopulateAuthors()
        {
            Author filterAuthor = (Author)cmbFilterAuthor.SelectedItem;

            cmbFilterAuthor.SelectedValuePath = "Id";
            cmbAuthor.SelectedValuePath = "Id";

            cmbFilterAuthor.ItemsSource = null;
            cmbAuthor.ItemsSource = null;

            cmbFilterAuthor.ItemsSource = bibService.Authors;
            cmbAuthor.ItemsSource = bibService.Authors;

            // herstel filter op auteur indien aanvankelijk reeds geselecteerd
            if(filterAuthor != null)
            {
                cmbFilterAuthor.SelectedValue = filterAuthor.Id;
            }
        }

        private void PopulatePublishers()
        {
            Publisher filterPublisher = (Publisher)cmbFilterPublisher.SelectedItem;

            cmbFilterPublisher.SelectedValuePath = "Id";
            cmbPublisher.SelectedValuePath = "Id";

            cmbFilterPublisher.ItemsSource = null;
            cmbPublisher.ItemsSource = null;

            cmbFilterPublisher.ItemsSource = bibService.Publishers;
            cmbPublisher.ItemsSource = bibService.Publishers;

            // herstel filter op uitgeverij indien aanvankelijk reeds geselecteerd
            if (filterPublisher != null)
            {
                cmbFilterPublisher.SelectedValue = filterPublisher.Id;
            }
        }

        private void ClearControls()
        {
            txtTitle.Text = "";
            txtYear.Text = "";
            cmbAuthor.SelectedIndex = -1;
            cmbPublisher.SelectedIndex = -1;
        }

        private void ActivateLeft()
        {
            grpLeft.IsEnabled = true;
            grpRight.IsEnabled = false;
            grpToOtherWindows.IsEnabled = true;
            btnSave.Visibility = Visibility.Hidden;
            btnCancel.Visibility = Visibility.Hidden;
        }

        private void ActivateRight()
        {
            grpLeft.IsEnabled = false;
            grpRight.IsEnabled = true;
            grpToOtherWindows.IsEnabled = false;
            btnSave.Visibility = Visibility.Visible;
            btnCancel.Visibility = Visibility.Visible;
        }

        private void CmbFilterAuthor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateBooks();
        }

        private void CmbFilterPublisher_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateBooks();
        }

        private void BtnClearFilterAuthor_Click(object sender, RoutedEventArgs e)
        {
            // door selectie aan te passen wordt selection changed handler van combobox afgevuurd
            // en zo uiteindelijk ook de boekenlijst automatisch vernieuwd
            cmbFilterAuthor.SelectedIndex = -1;
        }

        private void BtnClearFilterPublisher_Click(object sender, RoutedEventArgs e)
        {
            // door selectie aan te passen wordt selection changed handler van combobox afgevuurd
            // en zo uiteindelijk ook de boekenlijst automatisch vernieuwd
            cmbFilterPublisher.SelectedIndex = -1;
        }

        private void LstBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstBooks.SelectedItem != null)
            {
                Book book = (Book)lstBooks.SelectedItem;
                txtTitle.Text = book.Title;
                txtYear.Text = book.Year.ToString();
                cmbAuthor.SelectedValue = book.Author.Id;
                cmbPublisher.SelectedValue = book.Publisher.Id;
            }
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            isNew = true;
            ActivateRight();
            ClearControls();
            txtTitle.Focus();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (lstBooks.SelectedItem != null)
            {
                isNew = false;
                ActivateRight();
                txtTitle.Focus();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ActivateLeft();
            RefreshSelectedBook();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string title = txtTitle.Text.Trim();
            if (title.Length == 0)
            {
                ShowError("Je dient een titel op te geven!", "Fout", txtTitle);
                return;
            }

            Author author = (Author)cmbAuthor.SelectedItem;
            if (author == null)
            {
                ShowError("Je dient een auteur te selecteren!", "Fout", cmbAuthor);
                return;
            }
            
            Publisher publisher = (Publisher)cmbPublisher.SelectedItem;
            if (publisher == null)
            {
                ShowError("Je dient een uitgeverij te selecteren!", "Fout", cmbPublisher);
                return;
            }
            
            bool yearOk = int.TryParse(txtYear.Text, out int year);
            if (!yearOk)
            {
                ShowError("Je dient als jaar een getal in te voeren!", "Fout", txtYear);
                return;
            }

            if (isNew)
            {
                AddBook(title, author, publisher, year);
            }
            else
            {
                Book book = (Book)lstBooks.SelectedItem;
                UpdateBook(book, title, author, publisher, year);
            }
        }

        private void AddBook(string title, Author author, Publisher publisher, int year)
        {
            try
            {
                Book book = new Book(title, author, publisher, year);
                if (!bibService.AddBook(book))
                {
                    throw new Exception("Nieuw boek kon niet bewaard worden");
                }
                SelectBookAfterUpdate(book);
            }
            catch (Exception ex)
            {
                // exceptie kan vanuit twee plaatsen optreden:
                // 1: vanuit constructor Book indien een van de gegevens niet geldig is (dus vanuit class lib)
                // 2: de exceptie die hierboven opgegooid wordt indien het opslaan niet lukt
                ShowError(ex.Message, "Fout bij aanmaken boek");
            }
        }

        private void UpdateBook(Book book, string title, Author author, Publisher publisher, int year)
        {
            try
            {
                book.Title = title;
                book.Author = author;
                book.Publisher = publisher;
                book.Year = year;
                if (!bibService.UpdateBook(book))
                {
                    throw new Exception("Wijziging boek kon niet bewaard worden");
                }
                SelectBookAfterUpdate(book);
            }
            catch (Exception ex)
            {
                // exceptie kan vanuit twee plaatsen optreden:
                // 1: vanuit de setter van een van de properties indien de waarde niet geldig is (dus vanuit class lib)
                // 2: de exceptie die hierboven opgegooid wordt indien het opslaan niet lukt
                ShowError(ex.Message, "Fout bij wijzigen boek");
            }
        }

        private void SelectBookAfterUpdate(Book updatedBook)
        {
            PopulateBooks(updatedBook);
            ActivateLeft();
        }

        private void ShowError(string message, string title, Control controlToFocus = null)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            if(controlToFocus != null)
            {
                controlToFocus.Focus();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lstBooks.SelectedItem != null)
            {
                if (MessageBox.Show("Ben je zeker?", "Boek wissen", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Book book = (Book)lstBooks.SelectedItem;
                    if (!bibService.DeleteBook(book))
                    {
                        MessageBox.Show("We konden het boek niet verwijderen!", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    ClearControls();
                    PopulateBooks();
                }
            }
        }
        
        private void RdbInMemory_Checked(object sender, RoutedEventArgs e)
        {
            bibService = new BookServiceMem();
            Window_Loaded(null, null);
        }

        private void RdbFromDatabase_Checked(object sender, RoutedEventArgs e)
        {
            //bibService = new BookServiceDB();
            //Window_Loaded(null, null);
        }

        private void BtnAuthors_Click(object sender, RoutedEventArgs e)
        {
            WinAuthors winAuthors = new WinAuthors(bibService);
            winAuthors.ShowDialog();
            // code gaat hier verder van zodra winAuthors gesloten wordt
            if(winAuthors.IsUpdated)
            {
                PopulateAuthors();
                RefreshSelectedBook();
            }
        }

        private void BtnPublishers_Click(object sender, RoutedEventArgs e)
        {
            WinPublishers winPublishers = new WinPublishers(bibService);
            winPublishers.ShowDialog();
            // code gaat hier verder van zodra winPublishers gesloten wordt
            if (winPublishers.IsUpdated)
            {
                PopulatePublishers();
                RefreshSelectedBook();
            }
        }
    }
}
