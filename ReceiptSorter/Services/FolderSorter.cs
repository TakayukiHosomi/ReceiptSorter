using System;
using System.IO;
using LiteDB;
using ReceiptSorter.Objects;

namespace ReceiptSorter.Services
{
    public class FolderSorter
    {
        // 画像から読み取ったテキストを元に、適切なフォルダにレシートを移動する
        public static string MoveReceiptToFolder(string text, string imagePath)
        {
            string dbPath = "stores.db";
            string destinationFolder = "";

            using (var db = new LiteDatabase(dbPath))
            {
                var stores = db.GetCollection<Stores>("stores");//storesテーブルを取得

                // 全てのストアを取得し、textにPhoneNumが含まれているかを確認
                foreach (var store in stores.FindAll())
                {
                    if (text.Contains(store.PhoneNum))
                    {
                        destinationFolder = Path.Combine(@"C:\Receipts\", store.StoreName);
                        break; // 一致するものが見つかったらループを抜ける
                    }
                    else //一致するものが見つからなかった時
                    {
                        destinationFolder = Path.Combine(@"C:\Receipts\", "該当店舗なし");
                    }
                }
            }

            if (!string.IsNullOrEmpty(destinationFolder))　//移動先がないパターンはないはずだが
            {
                string destPath = Path.Combine(destinationFolder, Path.GetFileName(imagePath));
                File.Move(imagePath, destPath);
            }

            return destinationFolder; //呼び出し下に移動先フォルダを返し表示できるようにする
        
        }
    }
}
