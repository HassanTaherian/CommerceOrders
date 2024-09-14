namespace Domain.Exceptions;
public class ExternalModuleException : BadRequestException
{
    public ExternalModuleException(string moduleName)
        : base($"Module {moduleName} Is Not Responding ...")
    {
    }
}