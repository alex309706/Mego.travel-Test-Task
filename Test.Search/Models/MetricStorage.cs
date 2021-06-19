using System;
using System.Collections.Generic;
using System.Linq;
using Test.Search.Interfaces;

namespace Test.Search.Models
{
    public class MetricStorage : IStorage<Metric>
    {
        public List<Metric> MetricData { get; set; } = new List<Metric>();
        public void Create(Metric newInstantce)
        {
            MetricData.Add(newInstantce);
        }

        public void Delete(int ID)
        {
            Metric MetricToDelete = MetricData.FirstOrDefault(Metric =>Metric.MetricID == ID );
            MetricData.Remove(MetricToDelete);
        }

        public IEnumerable<Metric> Get()
        {
            return MetricData.ToList();
        }

        public Metric Get(int ID)
        {
            return MetricData.FirstOrDefault(Metric=>Metric.MetricID == ID);
        }

        public void Update(Metric instatceToUpdate)
        {
            if (instatceToUpdate == null)
            {
                throw new Exception("Cant update Null object!");
            }
            if (!MetricData.Any(Metric => Metric.MetricID==instatceToUpdate.MetricID))
            {
                throw new Exception("Cant update the instance because there is no such an object in storage!");
            }
            Metric MetricFromStorage = MetricData.FirstOrDefault(Metric=>Metric.MetricID ==instatceToUpdate.MetricID);
            MetricFromStorage = instatceToUpdate;
        }
    }
}
