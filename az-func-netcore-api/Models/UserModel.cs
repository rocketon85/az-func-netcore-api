using Microsoft.WindowsAzure.Storage.Table;

namespace az_func_netcore_api.Models
{
    public class User : TableEntity
    {
        public int UserId { get; set; }
        public string Detail { get; set; }
    }
}
