using CandySugar.Com.Library.Enums;
using RestSharp;
using System;
using System.Threading.Tasks;
using XExten.Advance.CacheFramework;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Library.DownQueue
{
    public class DownloadRequest
    {
        /// <summary>
        /// 队列委托下载
        /// </summary>
        public static void DownByQueue()
        {
            DownloadQueue.ResultFunc = new(Reuqest);
        }

        private static async Task<byte[]> Reuqest(string route, Enum @enum)
        {
            if (@enum is EDownload EType)
            {
                switch (EType)
                {
                    case EDownload.Light:
                        return await Light(route);
                    case EDownload.Wallhav:
                        return await WallHav(route);
                    case EDownload.Chan:
                        return await WallChan(route);
                    case EDownload.Rifan:
                        return await Rifan(route);
                    default:
                        break;
                }
            }
            return null;
        }
        private static async Task<byte[]> WallChan(string route)
        {
            var key = route.ToMd5();
            var cache = await Caches.RunTimeCacheGetAsync<byte[]>(key);
            if (cache != null)
                return cache;
            RestClient client = new RestClient(new RestClientOptions { MaxTimeout = 50000 },
                opt => opt.Add("Host", "konachan.com"));
            var res = await client.DownloadDataAsync(new RestRequest(route));
            await Caches.RunTimeCacheSetAsync(key, res);
            return res;
        }
        private static async Task<byte[]> WallHav(string route)
        {
            var key = route.ToMd5();
            var cache = await Caches.RunTimeCacheGetAsync<byte[]>(key);
            if (cache != null)
                return cache;
            RestClient client = new RestClient(new RestClientOptions { MaxTimeout = 50000 });
            var res = await client.DownloadDataAsync(new RestRequest(route));
            await Caches.RunTimeCacheSetAsync(key, res);
            return res;
        }
        private static async Task<byte[]> Rifan(string route)
        {
            var key = route.ToMd5();
            var cache = await Caches.RunTimeCacheGetAsync<byte[]>(key);
            if (cache != null)
                return cache;
            RestClient client = new RestClient(new RestClientOptions { MaxTimeout = 50000 });
            var res = await client.DownloadDataAsync(new RestRequest(route));
            await Caches.RunTimeCacheSetAsync(key, res);
            return res;
        }
        private static async Task<byte[]> Light(string route)
        {
            var key = route.ToMd5();
            var cache = await Caches.RunTimeCacheGetAsync<byte[]>(key);
            if (cache != null)
                return cache;
            RestClient client = new RestClient(new RestClientOptions { MaxTimeout = 50000 });
            var res = await client.DownloadDataAsync(new RestRequest(route));
            await Caches.RunTimeCacheSetAsync(key, res);
            return res;
        }
    }
}
