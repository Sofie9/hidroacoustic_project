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
            if (generator.GenerateAndSave())
            {
                SoundPlayer player = new SoundPlayer(WaveGenerator.filePath);
                player.Play();
            }

            ViewBag.FileName = WaveGenerator.filePath;
            return new FilePathResult(WaveGenerator.filePath, "audio/dat");
        }

        private CoreHelper _initCore(FormCollection form)
        {
            int.TryParse(form["xSource"], out int xSource);
            int.TryParse(form["ySource"], out int ySource);
            int.TryParse(form["zSource"], out int zSource);

            int.TryParse(form["xSystem"], out int xSystem);
            int.TryParse(form["ySystem"], out int ySystem);
            int.TryParse(form["zSystem"], out int zSystem);


            double.TryParse(form["frequency"], out double frequency);
            double.TryParse(form["pressureAmplitude"], out double pressureAmplitude);
            double.TryParse(form["deep"], out double deep);

            int.TryParse(form["time"], out int time);


            var ship = new CoreHelper.SourceShip(xSource, ySource, zSource);
            var gac = new CoreHelper.HydroacousticSystem(xSystem, ySystem, zSystem);

            return new CoreHelper(gac, ship, frequency, pressureAmplitude, deep, time);

        }
    }
}