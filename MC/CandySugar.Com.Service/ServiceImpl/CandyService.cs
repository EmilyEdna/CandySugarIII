using CandySugar.Com.Service.IServiceImpl;
using CandySugar.Com.Service.Model;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Service.ServiceImpl
{
    public class CandyService : ICandyService
    {
        public async Task Add(CollectModel model)
        {
            model.InitProperty();
            model.Hash = $"{model.Route}_{model.Cover}_{model.Name}".ToMd5();

            var Db = DbContext.Lite;
            var check = await Db.Table<CollectModel>().FirstOrDefaultAsync(t => t.Hash == model.Hash);
            if (check == null)
                await DbContext.Lite.InsertAsync(model);
        }

        public async Task Delete(Guid Id)
        {
            await DbContext.Lite.DeleteAsync<CollectModel>(Id);
        }

        public async Task<List<CollectModel>> Get(int Category, int PageIndex)
        {
            return await DbContext.Lite.Table<CollectModel>().Where(t => t.Category == Category)
                  .OrderByDescending(t => t.Span)
                  .Skip((PageIndex - 1) * 15)
                  .Take(15).ToListAsync();
        }

        public async Task Remove(int Category)
        {
            await DbContext.Lite.Table<CollectModel>().DeleteAsync(t => t.Category == Category);
        }
    }
}
