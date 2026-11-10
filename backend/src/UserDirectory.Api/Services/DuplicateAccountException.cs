namespace UserDirectory.Api.Services;

public sealed class DuplicateAccountException : Exception
{
    public DuplicateAccountException() : base("An account with this email already exists.")
    {
    }
}
