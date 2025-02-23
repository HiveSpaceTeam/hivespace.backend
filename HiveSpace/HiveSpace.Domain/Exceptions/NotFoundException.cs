namespace HiveSpace.Domain.Exceptions
{
    public class NotFoundException : Exception
    {
        public string MessageCode { get; set; }
        public object Data { get; set; }

        public NotFoundException(string message)
        {
            MessageCode = message;
        }
    }
}
