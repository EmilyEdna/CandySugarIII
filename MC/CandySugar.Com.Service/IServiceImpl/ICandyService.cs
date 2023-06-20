using CandySugar.Com.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.Com.Service.IServiceImpl
{
    public interface ICandyService
    {
        Task Add(CollectModel model);
        Task Delete(Guid Id);
        Task Remove(int Category);
        Task<List<CollectModel>> Get(int Category, int PageIndex);
    }
}
