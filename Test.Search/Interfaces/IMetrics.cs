namespace Test.Search.Interfaces
{
    public interface IMetrics
    {
        int MetricsID { get; set; }
        IRequestable RequestableSystem { get; set; }
        public string NameOfSearchingSystem { get; set; }
        string Result { get; set; }
        long TimeSpentToRequest { get; set; }
    }
}
