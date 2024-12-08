using System;
using System.Collections.Generic;
using System.Windows;
using LiteDB;
using Microsoft.Win32;
using System.Threading.Tasks;
using ReceiptSorter.Objects;
using ReceiptSorter.Services;
using BitMiracle.LibTiff.Classic;

namespace ReceiptSorter.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeDatabase();
            UpdateProgressText(); // 初期化時にプログレスバーのテキストを更新
        }

        private void InitializeDatabase()
        {
            string dbPath = "stores.db"; // データベースのパスを設定

            using (var db = new LiteDatabase(dbPath)) // DBへ接続　usingを使うことで処理後自動で閉じてくれる
            {
                var stores = db.GetCollection<Stores>("stores");
            }
        }

        #region ボタン関連

        //スキャンボタンのメソッド

        private void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>　// タスク実行
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "画像ファイル|*.jpg;*.jpeg;*.png;*.bmp", // ファイルフィルタを設定
                    Multiselect = true // 複数ファイルの選択を許可
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    int fileCount = openFileDialog.FileNames.Length;　//指定されたファイル分だけループ
                    for (int i = 0; i < fileCount; i++)
                    {
                        string imagePath = openFileDialog.FileNames[i];
                        string receiptText = OCRProcessor.ReadReceiptText(imagePath);
                        if (receiptText == "読み取りに失敗しました")　//OCRで画像の読み取りに失敗している場合
                        {
                            Dispatcher.Invoke(() =>
                            {
                                MessageBox.Show(receiptText, "エラー", MessageBoxButton.OK, MessageBoxImage.Error); // メッセージウィンドウにエラーメッセージを表示
                            });
                            continue; // 次のファイルに進む
                        }

                        string destFolder = FolderSorter.MoveReceiptToFolder(receiptText, imagePath); //ファイル移動メソッドを使うとともに移動先フォルダパスを取得

                        double progress = ((i + 1) / (double)fileCount) * 100;
                        UpdateProgressBar(progress);

                        Dispatcher.Invoke(() =>
                        {
                            StatusTextBox.AppendText("処理完了: 画像は、" + destFolder + "へ移動されました。" + Environment.NewLine);
                        });
                    }
                }
            });
        }

        // 確認ボタンのメソッド

        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            string dbPath = "stores.db";
            var storeWindow = new StoreWindow();
            using (var db = new LiteDatabase(dbPath))
            {
                var stores = db.GetCollection<Stores>("stores");
                storeWindow.StoreDataGrid.ItemsSource = stores.FindAll();
            }
            storeWindow.ShowDialog();
        }

        #endregion

        #region プログレスバー関連

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateProgressText(); // プログレスバーのテキストを更新
        }

        private void UpdateProgressBar(double value)
        {
            Dispatcher.Invoke(() =>
            {
                ProgressBar.Value = Math.Floor(value);
                UpdateProgressText(); // プログレスバーのテキストを更新
            });
        }

        private void UpdateProgressText()
        {
            Progress_TextBlock.Text = ProgressBar.Value.ToString() + "%"; // プログレスバーの値をテキストに表示
        }

        #endregion

    }
}
