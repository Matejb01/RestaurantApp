using RestaurantApp.Interfaces;

namespace RestaurantApp.Presenters
{
    public abstract class BasePresenter<TView> where TView : IView
    {
        protected TView View { get; }

        protected BasePresenter(TView view)
        {
            View = view;
        }
    }
}