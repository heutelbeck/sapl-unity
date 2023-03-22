using UnityEngine;

public abstract class BreakingTheGlasBehavior: MonoBehaviour
{
	private void Awake()
	{
        ResetState();
    }

	public bool PermitExecution { get; set; }

    public void SecuredExecute()
    {
        if (PermitExecution)
        {
            ExecuteBreakingTheGlas();
        }
    }

    protected abstract void ExecuteBreakingTheGlas();

    public void ExecuteWithPermission()
    {
        PermitExecution = true;
        SecuredExecute();
    }

    public void ResetState()
    {
        PermitExecution = true;
    }
}
