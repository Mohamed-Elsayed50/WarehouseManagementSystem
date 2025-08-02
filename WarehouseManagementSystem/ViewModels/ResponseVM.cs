using WarehouseManagementSystem.Enums;

namespace WarehouseManagementSystem.ViewModels
{
    public class ResponseVM<T>
    {
        public ResponseStatus Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public ResponseVM() { }

        public ResponseVM(ResponseStatus status, string message, T data)
        {
            Status = status;
            Message = message;
            Data = data;
        }
    }
}
