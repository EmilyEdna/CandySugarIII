using CandySugar.Com.Library;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Service
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
            {
                await DbContext.Lite.InsertAsync(model);
                "加入收藏成功".Info();
            }
        }

        public async Task Delete(Guid Id)
        {
            await DbContext.Lite.DeleteAsync<CollectModel>(Id);
        }

        public async Task<Tuple<int, List<CollectModel>>> Get(int Category, int PageIndex)
        {
            var query = DbContext.Lite.Table<CollectModel>().Where(t => t.Category == Category)
                  .OrderByDescending(t => t.Span);

            var Count = await query.CountAsync();
            var Data = await query.Skip((PageIndex - 1) * 15).Take(15).ToListAsync();
            return Tuple.Create(Count, Data);
        }

        public async Task Remove(int Category)
        {
            await DbContext.Lite.Table<CollectModel>().DeleteAsync(t => t.Category == Category);
        }

        public async Task Update(string key, string input)
        {
            var model = await DbContext.Lite.Table<CollectModel>()
                 .Where(t => t.Category == 6)
                 .Where(t => string.IsNullOrEmpty(t.Commom))
                 .Where(t => t.Hash == key).FirstAsync();
            if (model != null)
            {
                model.Commom = input;
                await DbContext.Lite.UpdateAsync(model);
            }
        }
    }
}
