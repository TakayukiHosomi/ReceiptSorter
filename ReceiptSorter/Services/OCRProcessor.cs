using System;
using Tesseract;

namespace ReceiptSorter.Services
{
    public class OCRProcessor
    {
        public static string ReadReceiptText(string imagePath)
        {
            try
            {
                using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))　//日本語版だと精度が良くない。電話番号に注視して検出できるように英語版を使用
                {
                    using (var img = Pix.LoadFromFile(imagePath))
                    {
                        using (var page = engine.Process(img))
                        {
                            var text = page.GetText();　// 読み取ったテキストを取得
                            return text;　// 読み取ったテキストを返す
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                return null;　// 読み取りに失敗した場合はnullを返す
            }
        }
    }
}
