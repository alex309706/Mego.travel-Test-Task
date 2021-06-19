using System;
using System.Collections.Generic;
using System.Linq;
using Test.Search.Interfaces;

namespace Test.Search.Models
{
    public class MetricsStorage : IStorage<Metrics>
    {
        public List<Metrics> MetricsData { get; set; } = new List<Metrics>();
        public void Create(Metrics newInstantce)
        {
            MetricsData.Add(newInstantce);
        }

        public void Delete(int ID)
        {
            Metrics metricsToDelete = MetricsData.FirstOrDefault(metrics =>metrics.MetricsID == ID );
            MetricsData.Remove(metricsToDelete);
        }

        public IEnumerable<Metrics> Get()
        {
            return MetricsData.ToList();
        }

        public Metrics Get(int ID)
        {
            return MetricsData.FirstOrDefault(metrics=>metrics.MetricsID == ID);
        }

        public void Update(Metrics instatceToUpdate)
        {
            if (instatceToUpdate == null)
            {
                throw new Exception("Cant update Null object!");
            }
            if (!MetricsData.Any(metrics => metrics.MetricsID==instatceToUpdate.MetricsID))
            {
                throw new Exception("Cant update the instance because there is no such an object in storage!");
            }
            Metrics metricsFromStorage = MetricsData.FirstOrDefault(metrics=>metrics.MetricsID ==instatceToUpdate.MetricsID);
            metricsFromStorage = instatceToUpdate;
        }
    }
}
