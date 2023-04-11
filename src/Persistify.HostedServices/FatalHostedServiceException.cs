namespace Persistify.HostedServices;

public class FatalHostedServiceException : Exception
{
    public FatalHostedServiceException(string message) : base(message)
    {
    }
}