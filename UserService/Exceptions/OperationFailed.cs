public class OperationFailed : BusinessException
{
    public OperationFailed(string message) : base(message)
    {
    }

    public OperationFailed(string message, Exception ex) : base(message, ex)
    {
    }
}