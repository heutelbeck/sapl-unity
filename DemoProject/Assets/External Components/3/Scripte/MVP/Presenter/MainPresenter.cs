using MVP.Model;

namespace MVP.Presenter
{
    /// <summary>
    /// abstract super class for mainpresenters
    /// declares InitPresenter method to set the model
    /// </summary>
    public abstract class MainPresenter : PresenterBase
    {
        /// <summary>
        /// initialize presenter, sets the model
        /// </summary>
        /// <param name="model">MachineModel</param>
        public abstract bool InitPresenter(IModel model);
    }

}