namespace TDL.Client.Audit
{
    public interface IAuditStream
    {
        void WriteLine(string value);
    }
}
