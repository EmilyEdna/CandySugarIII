namespace CandySugar.Com.Service
{
    public interface ICandyService
    {
        Task Add(CollectModel model);
        Task Alter(CollectModel mode);
        Task Delete(Guid Id);
        Task Remove(string Category);
        Task<Tuple<int, List<CollectModel>>> Get(string Category, int PageIndex);
        Task<List<CollectModel>> Export();
    }
}
