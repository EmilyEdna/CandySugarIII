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
        public IActionResult GetAxgleBase() => new JsonResult(AxgleService.QueryAll());
        [HttpGet]
        public IActionResult GetComicBase() => new JsonResult(ComicService.QueryAll());
        [HttpGet]
        public IActionResult GetCosplayBase() => new JsonResult(CosplayService.QueryAll());
        [HttpGet]
        public IActionResult GetRifanBase() => new JsonResult(RifanService.QueryAll());
        [HttpGet]
        public IActionResult GetWallBase() => new JsonResult(WallService.QueryAll());
        [HttpGet]
        public IActionResult GetMusicBase() => new JsonResult(MusiceService.QueryAll());
        [HttpPost]
        public void Upload(int type, IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            file.OpenReadStream().CopyTo(memoryStream);
            memoryStream.Position = 0;
            using var streamReader = new StreamReader(memoryStream);
            var fileContent = streamReader.ReadToEnd();
            if (type == 1)
            {
                fileContent.ToModel<List<AxgleModel>>().ForEach(item =>
                {
                    AxgleService.Insert(item);
                });
            }
            if (type == 2)
            {
                fileContent.ToModel<List<WallModel>>().ForEach(item =>
                {
                    WallService.Insert(item);
                });
            }
        }
    }
}
