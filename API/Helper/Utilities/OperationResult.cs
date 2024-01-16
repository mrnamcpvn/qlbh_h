namespace Glossary_API.Helpers.Utilities
{
    public class OperationResult
    {
        public string Caption { set; get; }
        public string Message { set; get; }
        public bool Success { set; get; }
        public object Data { set; get; }

        public OperationResult()
        {

        }

        public OperationResult(string message)
        {
            this.Message = message;
        }

        public OperationResult(bool success)
        {
            this.Success = success;
        }

        public OperationResult(bool success, string message)
        {
            this.Message = message;
            this.Success = success;
        }

        public OperationResult(bool success, string message, string caption)
        {
            this.Caption = caption;
            this.Message = message;
            this.Success = success;
        }

        public OperationResult(bool success, string message, string caption, object data)
        {
            this.Caption = caption;
            this.Message = message;
            this.Success = success;
            this.Data = data;
        }
    }
}