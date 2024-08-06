namespace TDL.Client.Audit
{
    public interface IAuditable
    {
        string AuditText { get; }
    }
}
