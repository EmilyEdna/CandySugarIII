using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.Com.Service
{
    public interface ICandyService
    {
        Task Add(CollectModel model);
        Task Delete(Guid Id);
        Task Remove(int Category);
        Task<Tuple<int, List<CollectModel>>> Get(int Category, int PageIndex);
        Task Update(string key, string input);
    }
}
