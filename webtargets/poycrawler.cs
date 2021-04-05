using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PantryDataCollector.webtargets
{
    public class poycrawler
    {
        public async Task<bool> Init()
        {
            var totalPages = Enumerable.Range(2, 90).ToList();
            foreach (var i in totalPages)
            {
                Console.WriteLine($"Starting Crawl on page {i}.");
            }
        }
    }
}
