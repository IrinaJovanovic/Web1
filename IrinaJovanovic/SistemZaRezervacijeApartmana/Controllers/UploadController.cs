using SistemZaRezervacijeApartmana.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SistemZaRezervacijeApartmana.Controllers
{
    public class UploadController : Controller
    {
        // GET: Upload
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload()
        {
            List<Image> list = (List<Image>)Session["UploadedFiles"];
            if (list == null)
            {
                list = new List<Image>();
                Session["UploadedFiles"] = list;
            }
            if (Request.Files.Count > 0)
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Images/"), fileName);

                        file.SaveAs(path);
                        //Image image = new Image(fileName, path);
                        
                    }
                }
            }
          
            return View("Index");
        }
    }
}