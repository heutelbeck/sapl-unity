namespace MVP.Model
{
    /// <summary>
    /// Interface for machine models
    /// </summary>
    public interface IModel
    {
        ModelType ModelType { get; }
        void ChangeState(Condition condition);
        void Destroy();
    }

    public enum ModelType
    {
        NONE, PROCESSING_STATION, BUILD_UNIT, PRINT_JOB
    }

}
