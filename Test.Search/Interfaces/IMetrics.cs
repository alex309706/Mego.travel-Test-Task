namespace Test.Search.Interfaces
{
    public interface IMetrics
    {
        IRequestable RequestableSystem { get; set; }
        public string NameOfSearchingSystem { get; set; }
        string Result { get; set; }
        long TimeSpentToRequest { get; set; }
    }
}
