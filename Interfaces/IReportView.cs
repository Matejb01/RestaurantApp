namespace RestaurantApp.Interfaces
{
    public interface IReportView : IView
    {
        DateTime? DatumOd { get; set; }
        DateTime? DatumDo { get; set; }
    }
}