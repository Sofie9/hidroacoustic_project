using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Web;
using System.Web.Mvc;
using HidroacousticSygnals.Core;

namespace HidroacousticSygnals.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GenerateSound(FormCollection form)
        {
            var core = this._initCore(form);
            var generator = new WaveGenerator(core);
            //if (generator.GenerateAndSave())
            //{
            //    SoundPlayer player = new SoundPlayer(WaveGenerator.filePath);
            //    player.Play();
            //}

            ViewBag.FileName = WaveGenerator.filePath;
            return new FilePathResult(WaveGenerator.filePath, "audio/dat");
        }

        private CoreHelper _initCore(FormCollection form)
        {
            int.TryParse(form["zSource"], out int zSource);

            int.TryParse(form["xSystem"], out int xSystem);
            int.TryParse(form["ySystem"], out int ySystem);
            int.TryParse(form["zSystem"], out int zSystem);


            double.TryParse(form["frequency"], out double frequency);
            double.TryParse(form["deep"], out double deep);
            double.TryParse(form["angle"], out double angle);


            int.TryParse(form["time"], out int time);


            var ship = new CoreHelper.SourceShip(zSource);
            var gac = new CoreHelper.HydroacousticSystem(xSystem, ySystem, zSystem);


            return new CoreHelper(gac, ship, frequency, 16000, deep, time, angle);

        }
    }
}