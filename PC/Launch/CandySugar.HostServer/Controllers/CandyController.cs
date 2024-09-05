using CandySugar.Com.Data;
using CandySugar.Com.Data.Entity.AnimeEntity;
using CandySugar.Com.Data.Entity.AxgleEntity;
using CandySugar.Com.Data.Entity.ComicEntity;
using CandySugar.Com.Data.Entity.CosplayEntity;
using CandySugar.Com.Data.Entity.HitomiEntity;
using CandySugar.Com.Data.Entity.MusicEntity;
using CandySugar.Com.Data.Entity.NHentaiEntity;
using CandySugar.Com.Data.Entity.RifanEntity;
using CandySugar.Com.Data.Entity.WallEntity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExten.Advance.IocFramework;
using XExten.Advance.LinqFramework;

namespace CandySugar.HostServer.Controllers
{
    /// <summary>
    /// 甜糖服务
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CandyController : Controller
    {
        private IService<AxgleModel> AxgleService;
        private IService<ComicModel> ComicService;
        private IService<CosplayModel> CosplayService;
        private IService<RifanModel> RifanService;
        private IService<WallModel> WallService;
        private IService<MusicModel> MusiceService;
        private IService<AnimeModel> AnimeService;
        private IService<NHentaiModel> NHentaiService;
        private IService<HitomiModel> HitomiService;
        /// <summary>
        /// 甜糖服务
        /// </summary>
        public CandyController()
        {
            AxgleService = IocDependency.Resolve<IService<AxgleModel>>();
            ComicService = IocDependency.Resolve<IService<ComicModel>>();
            CosplayService = IocDependency.Resolve<IService<CosplayModel>>();
            RifanService = IocDependency.Resolve<IService<RifanModel>>();
            WallService = IocDependency.Resolve<IService<WallModel>>();
            MusiceService = IocDependency.Resolve<IService<MusicModel>>();
            AnimeService =IocDependency.Resolve<IService<AnimeModel>>();
            NHentaiService = IocDependency.Resolve<IService<NHentaiModel>>();
            HitomiService = IocDependency.Resolve<IService<HitomiModel>>();
        }
        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Export(DataEnums type)
        {
            var bytes = type switch
            {
                DataEnums.Axgle => Encoding.UTF8.GetBytes(AxgleService.QueryAll().ToJson()),
                DataEnums.Comic => Encoding.UTF8.GetBytes(ComicService.QueryAll().ToJson()),
                DataEnums.Cosplay => Encoding.UTF8.GetBytes(CosplayService.QueryAll().ToJson()),
                DataEnums.Rifan => Encoding.UTF8.GetBytes(RifanService.QueryAll().ToJson()),
                DataEnums.Wallpaper => Encoding.UTF8.GetBytes(WallService.QueryAll().ToJson()),
                DataEnums.Music => Encoding.UTF8.GetBytes(MusiceService.QueryAll().ToJson()),
                DataEnums.Anime => Encoding.UTF8.GetBytes(AnimeService.QueryAll().ToJson()),
                DataEnums.NHentai => Encoding.UTF8.GetBytes(NHentaiService.QueryAll().ToJson()),
                DataEnums.Hitomi => Encoding.UTF8.GetBytes(HitomiService.QueryAll().ToJson()),
                _ => null
            };

            return new FileContentResult(bytes, "application/octet-stream")
            {
                FileDownloadName = $"{type}.json"
            };
           
        }
        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="type"></param>
        /// <param name="file"></param>
        [HttpPost]
        public void Import(DataEnums type, IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            file.OpenReadStream().CopyTo(memoryStream);
            memoryStream.Position = 0;
            using var streamReader = new StreamReader(memoryStream);
            var fileContent = streamReader.ReadToEnd();

            switch (type)
            {
                case DataEnums.Axgle:
                    fileContent.ToModel<List<AxgleModel>>().ForEach(item =>
                    {
                        AxgleService.Insert(item);
                    });
                    break;
                case DataEnums.Comic:
                    fileContent.ToModel<List<ComicModel>>().ForEach(item =>
                    {
                        ComicService.Insert(item);
                    });
                    break;
                case DataEnums.Cosplay:
                    fileContent.ToModel<List<CosplayModel>>().ForEach(item =>
                    {
                        CosplayService.Insert(item);
                    });
                    break;
                case DataEnums.Rifan:
                    fileContent.ToModel<List<RifanModel>>().ForEach(item =>
                    {
                        RifanService.Insert(item);
                    });
                    break;
                case DataEnums.Wallpaper:
                    fileContent.ToModel<List<WallModel>>().ForEach(item =>
                    {
                        WallService.Insert(item);
                    });
                    break;
                case DataEnums.Music:
                    fileContent.ToModel<List<MusicModel>>().ForEach(item =>
                    {
                        MusiceService.Insert(item);
                    });
                    break;
                case DataEnums.Anime:
                    fileContent.ToModel<List<AnimeModel>>().ForEach(item =>
                    {
                        AnimeService.Insert(item);
                    });
                    break;
                case DataEnums.NHentai:
                    fileContent.ToModel<List<NHentaiModel>>().ForEach(item =>
                    {
                        NHentaiService.Insert(item);
                    });
                    break;
                case DataEnums.Hitomi:
                    fileContent.ToModel<List<HitomiModel>>().ForEach(item =>
                    {
                        HitomiService.Insert(item);
                    });
                    break;
                default:
                    break;
            }
        }
    }
}
