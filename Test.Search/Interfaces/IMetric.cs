namespace Test.Search.Interfaces
{
    public interface IMetric
    {
        int MetricID { get; set; }
        public string NameOfSearchingSystem { get; set; }
        string Result { get; set; }
        int? TimeSpentToRequest { get; set; }
    }
}
