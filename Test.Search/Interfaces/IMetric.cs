namespace Test.Search.Interfaces
{
    public interface IMetric
    {
        int MetricID { get; set; }
        public string NameOfSearchingSystem { get; set; }
        string Result { get; set; }
        long TimeSpentToRequest { get; set; }
    }
}
