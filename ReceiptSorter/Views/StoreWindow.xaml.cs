using LiteDB;
using ReceiptSorter.Objects;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ReceiptSorter.Views
{
    public partial class StoreWindow : Window
    {
        public StoreWindow()
        {
            InitializeComponent();
            LoadDataGrid();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var storeName = StoreNameTextBox.Text;
            var phoneNum = PhoneNumTextBox.Text;

            if (storeName == "追加する店舗名を入力してください" || phoneNum == "追加する電話番号を入力してください" || string.IsNullOrWhiteSpace(storeName) || string.IsNullOrWhiteSpace(phoneNum))
            {
                MessageBox.Show("店舗名と電話番号を入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newStore = new Stores
            {
                StoreName = storeName,
                PhoneNum = phoneNum
            };

            using (var db = new LiteDatabase(@"Filename=stores.db;Connection=shared"))
            {
                var stores = db.GetCollection<Stores>("stores");
                stores.Insert(newStore);
            }

            LoadDataGrid();

            // 入力フィールドをクリア
            PhoneNumTextBox.Text = "電話番号を入力してください";
            PhoneNumTextBox.Foreground = System.Windows.Media.Brushes.Gray;
            StoreNameTextBox.Text = "店舗名を入力してください";
            StoreNameTextBox.Foreground = System.Windows.Media.Brushes.Gray;
        }

        private void LoadDataGrid()
        {
            using (var db = new LiteDatabase(@"Filename=stores.db;Connection=shared"))
            {
                var stores = db.GetCollection<Stores>("stores");
                StoreDataGrid.ItemsSource = stores.FindAll().ToList();
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (textBox.Text == "店舗名を入力してください" || textBox.Text == "電話番号を入力してください")
            {
                textBox.Text = "";
                textBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (textBox.Name == "StoreNameTextBox")
                {
                    textBox.Text = "店舗名を入力してください";
                }
                else if (textBox.Name == "PhoneNumTextBox")
                {
                    textBox.Text = "電話番号を入力してください";
                }
                textBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // 選択された店舗を取得
            var selectedStore = (Stores)StoreDataGrid.SelectedItem;
            if (selectedStore == null)
            {
                MessageBox.Show("編集する店舗を選択してください。", "選択エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var storeName = StoreNameTextBox.Text;
            var phoneNum = PhoneNumTextBox.Text;

            if (storeName == "店舗名を入力してください" || phoneNum == "電話番号を入力してください" || string.IsNullOrWhiteSpace(storeName) || string.IsNullOrWhiteSpace(phoneNum))
            {
                MessageBox.Show("店舗名と電話番号を入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var db = new LiteDatabase(@"Filename=stores.db;Connection=shared"))
            {
                var stores = db.GetCollection<Stores>("stores");
                var storeToEdit = stores.FindById(selectedStore.Id);

                if (storeToEdit != null)
                {
                    storeToEdit.StoreName = storeName;
                    storeToEdit.PhoneNum = phoneNum;

                    stores.Update(storeToEdit);
                }
            }

            LoadDataGrid();

            // 入力フィールドをクリア
            StoreNameTextBox.Text = "店舗名を入力してください";
            StoreNameTextBox.Foreground = System.Windows.Media.Brushes.Gray;
            PhoneNumTextBox.Text = "電話番号を入力してください";
            PhoneNumTextBox.Foreground = System.Windows.Media.Brushes.Gray;
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // 選択された店舗を取得
            var selectedStore = (Stores)StoreDataGrid.SelectedItem;
            if (selectedStore == null)
            {
                MessageBox.Show("削除する店舗を選択してください。", "選択エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show("本当に削除しますか？", "削除確認", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                using (var db = new LiteDatabase(@"Filename=stores.db;Connection=shared"))
                {
                    var stores = db.GetCollection<Stores>("stores");
                    stores.Delete(selectedStore.Id);
                }

                LoadDataGrid();
            }
        }

    }
}
