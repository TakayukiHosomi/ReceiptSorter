using System;
using LiteDB;

namespace ReceiptSorter.Objects
{
    public class Stores
    {
        private int id;
        private string phoneNum;
        private string storeName;

        [BsonId]//このアノテーションでIDをインクリメント
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string StoreName
        {
            get { return storeName; }
            set { this.storeName = value; }
        }

        public string PhoneNum
        {
            get { return phoneNum; }
            set { this.phoneNum = value; }
        }
    }
}
