namespace RestaurantApp.Interfaces
{
    public interface IView
    {
        string ErrorMessage { get; set; }
        string SuccessMessage { get; set; }
    }
}