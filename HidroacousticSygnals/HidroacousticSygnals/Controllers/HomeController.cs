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
            
            int.TryParse(form["xSource"], out int xSource);
            int.TryParse(form["ySource"], out int ySource);
            int.TryParse(form["zSource"], out int zSource);

            int.TryParse(form["xSystem"], out int xSystem);
            int.TryParse(form["ySystem"], out int ySystem);
            int.TryParse(form["zSystem"], out int zSystem);


            double.TryParse(form["frequency"], out double frequency);
            double.TryParse(form["pressureAmplitude"], out double pressureAmplitude);
            double.TryParse(form["deep"], out double deep);


            var ship = new CoreHelper.SourceShip(xSource, ySource);
            var gac = new CoreHelper.HydroacousticSystem(xSystem, ySystem);

            var core = new CoreHelper(gac,ship,frequency,pressureAmplitude,deep);


            string filePath = @"C:\Users\sofy9\Desktop\test2.dat";
            WaveGenerator wave = null;
            if (xSource == 0)
            {
                wave = new WaveGenerator();
                wave.GenerateSimpleData();
            }
            else
            {
                wave = new WaveGenerator(core);
            }

            wave.Save(filePath);
            SoundPlayer player = new SoundPlayer(filePath);
            player.Play();
            //SoundPlayer player = new SoundPlayer(filePath);
            var bytes = new byte[0];
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var br = new BinaryReader(fs);
                long numBytes = new FileInfo(filePath).Length;
                bytes = br.ReadBytes((int)numBytes);

            }
            return File(bytes, "audio/dat", "callrecording.dat");
            //return RedirectToAction("Index");
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}