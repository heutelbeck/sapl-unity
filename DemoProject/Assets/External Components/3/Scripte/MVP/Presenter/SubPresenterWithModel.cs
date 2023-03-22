using MVP.Model;

namespace MVP.Presenter
{
    public abstract class SubPresenterWithModel : SubPresenter
    {
        public abstract bool InitModel(IModel model);
    }

}