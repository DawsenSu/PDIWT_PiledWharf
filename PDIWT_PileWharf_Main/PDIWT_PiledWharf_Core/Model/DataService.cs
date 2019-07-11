using System;

namespace PDIWT_PiledWharf_Core.Model
{
    public class DataService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to connect to the actual data service

            var item = new DataItem("Welcome to MVVM Light[Run]");
            callback(item, null);
        }
    }
}