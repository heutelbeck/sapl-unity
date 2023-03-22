

public interface IBreakingTheGlas
{
    bool PermitExecution { get; set; }

    /* Regard: This method needs to be copied into the Class implementing this interface
     * if you want to call this methods via the unity editor
     * (editor is not able to find interface methods).
    */
    public sealed void SecuredExecute()
    {
        if (PermitExecution)
        {
            UserExecute();
        }
    }

    protected void UserExecute();

    public sealed void ExecuteWithPermission()
    {
        PermitExecution = true;
        SecuredExecute();
    }

    public sealed void ResetState()
    {
        PermitExecution = true;
    }
}
