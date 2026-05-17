using CandySugar.Com.Data;
using CandySugar.Com.Data.Entity.AxgleEntity;
using CandySugar.Com.Data.Entity.ComicEntity;
using CandySugar.Com.Data.Entity.CosplayEntity;
using CandySugar.Com.Data.Entity.MusicEntity;
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
        public CandyController()
        {
            AxgleService = IocDependency.Resolve<IService<AxgleModel>>();
            ComicService = IocDependency.Resolve<IService<ComicModel>>();
            CosplayService = IocDependency.Resolve<IService<CosplayModel>>();
            RifanService = IocDependency.Resolve<IService<RifanModel>>();
            WallService = IocDependency.Resolve<IService<WallModel>>();
            MusiceService = IocDependency.Resolve<IService<MusicModel>>();
        }
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
                _ => null
            };

            return new FileContentResult(bytes, "application/octet-stream")
            {
                FileDownloadName = $"{type}.json"
            };
           
        }
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
                default:
                    break;
            }
        }
    }
}
