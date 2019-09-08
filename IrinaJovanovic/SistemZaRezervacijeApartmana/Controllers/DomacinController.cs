using SistemZaRezervacijeApartmana.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace SistemZaRezervacijeApartmana.Controllers
{
    public class DomacinController : Controller
    {
        // GET: Domacin
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult IzmenaPodataka(string username, string pass, string ime, string prezime, string pol)
        {

            Korisnik k = (Korisnik)Session["korisnik"];

            if (k == null)
            {
                k = new Korisnik();
                Session["korisnik"] = k;
            }

            foreach (Korisnik kor in Database.domacini.Values)
            {
                if (kor.KorisnickoIme == username && kor.Uloga == Uloga.DOMACIN)
                {
                    Database.registrovaniKorisnici[username].Ime = ime;
                    Database.registrovaniKorisnici[username].Prezime = prezime;
                    Database.registrovaniKorisnici[username].Lozinka = pass;
                    Pol p;
                    if (pol == "MUSKI")
                        p = Pol.MUSKI;
                    else
                        p = Pol.ZENSKI;
                    Database.registrovaniKorisnici[username].Pol = p;

                    Database.AzurirajPodatkeDomacina();


                    ViewBag.Message = "Podaci su uspesno izmenjeni.";

                    return View("IzmenaPodatakaRez", Database.registrovaniKorisnici[username]);
                }

            }

            ViewBag.Message = "Podaci nisu izmenjeni, desila se neka greska!";

            return View("IzmenaPodatakaRez");
          
        }

        [HttpPost]
        public ActionResult KreirajApartman(string tipApartmana, string brSoba,string brGostiju,string ulica,string broj,string grad,string postBroj,string datumiIzdavanje,string cena,string statusApartmana,string vremePrijava,string vremeOdjava,List<string> sadrzajiApartmana)
        {
            Korisnik kor=(Korisnik)Session["korisnik"];

            if(kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }
          

            if (brSoba == "" || brGostiju == "" || ulica==""|| broj=="" || grad=="" || postBroj=="" || cena=="" || vremePrijava=="" || vremeOdjava=="" || datumiIzdavanje== "" || sadrzajiApartmana.Count==0)
            {
                ViewBag.ErrorMessage = "Morate popuniti sva polja da bi ste kreirali apartman!";
                return View("Greska");
            }

            foreach(Korisnik k in Database.registrovaniKorisnici.Values)
            {
                if(kor.KorisnickoIme == k.KorisnickoIme && k.Uloga == Uloga.DOMACIN)
                {
                    TipApartmana tip;

                    if (tipApartmana == "CEO_APARTMAN")
                        tip = TipApartmana.CEO_APARTMAN;
                    else
                        tip = TipApartmana.SOBA;

                    StatusApartmana status;

                    if (statusApartmana == "NEAKTIVAN")
                        status = StatusApartmana.NEAKTIVAN;
                    else
                        status = StatusApartmana.AKTIVAN;

                    Adresa adresa = new Adresa(broj, ulica, postBroj, grad);
                    Lokacija lokacija = new Lokacija(45.18,19.87,adresa);

                    Domacin domacin = new Domacin(kor.Ime, kor.Prezime, kor.KorisnickoIme, kor.Lozinka, kor.Pol);
                  

                    Random id = new Random();
                
                    Apartman apartman = new Apartman(id.Next(0, 700), tip,int.Parse(brSoba), int.Parse(brGostiju), lokacija, domacin, double.Parse(cena),status, vremePrijava, vremeOdjava);

                    string[] datumi = datumiIzdavanje.Split(' ', ',');
                    string[] split;
                    foreach (string dat in datumi)
                    {
                        split = dat.Split('-', '.', '/');
                        DateTime datum = new DateTime(Int32.Parse(split[2]), Int32.Parse(split[1]), Int32.Parse(split[0]));
                        apartman.DatumiZaIzdavanje.Add(datum);
                        apartman.DostupnostPoDatumima.Add(datum);
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
                                Image uf = new Image(fileName, path);
                                apartman.Slike.Add(uf);
                            }
                        }
                    }
                
                    SadrzajApartmana sadrzaj; //ovdeeeeeeeeeeeeeeeeeeee
                    foreach (string s in sadrzajiApartmana)
                    {
                        foreach (SadrzajApartmana sadrz in Database.sadrzajiApartmana.Values)
                        {
                            if (sadrz.Naziv == s)
                            {
                                sadrzaj = new SadrzajApartmana(sadrz.Id, sadrz.Naziv);
                                apartman.SadrzajiApartmana.Add(sadrzaj);
                            }
                        }
                    }

                    Database.sviApartmani.Add(apartman.Id, apartman);

                    Database.domacini[domacin.KorisnickoIme].ApartmaniZaIzdavanje.Add(apartman);

                    Database.UpisiSveApartmane();

                    ViewBag.Message = "Apartman je uspesno kreiran.";

                    return View("KreiranjeApartmanaRez");

                }
            }

            ViewBag.Message = "Apartman nije kreiran, desila neka greska.Molimo Vas, pokustajte ponovo!";

            return View("KreiranjeApartmanaRez");
        }

        [HttpPost]
        public ActionResult IzmenaApartmana(string tipApartmana, string brSoba, string brGostiju, string ulica, string broj, string grad, string postBroj, string datumiIzdavanje, string cena, string statusApartmana, string vremePrijava, string vremeOdjava, List<string> sadrzajiApartmana,string idApartmana)
        {
            Korisnik kor = (Korisnik)Session["korisnik"];

            if(kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            if (brSoba == "" || brGostiju == "" || ulica == "" || broj == "" || grad == "" || postBroj == "" || cena == "" || vremePrijava == "" || vremeOdjava == "" || datumiIzdavanje == "" || sadrzajiApartmana.Count == 0)
            {
                ViewBag.ErrorMessage = "Ne smete nijedno  polje ostaviti prazno, da biste uspesno mogli da izmenite apartman!";
                return View("Greska");
            }

            foreach (Korisnik k in Database.registrovaniKorisnici.Values)
            {
                if(k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.DOMACIN)
                {
                    TipApartmana tip;

                    if (tipApartmana == "CEO_APARTMAN")
                        tip = TipApartmana.CEO_APARTMAN;
                    else
                        tip = TipApartmana.SOBA;

                    StatusApartmana status;

                    if (statusApartmana == "AKTIVAN")
                        status = StatusApartmana.AKTIVAN;
                    else
                        status = StatusApartmana.NEAKTIVAN;

                    Adresa adresa = new Adresa(broj, ulica, postBroj, grad);
                    Lokacija lokacija = new Lokacija(45.245, 19.87, adresa);

                    Apartman apartman = new Apartman();
                    foreach (int id in Database.sviApartmani.Keys)
                    {
                        if (id == Int32.Parse(idApartmana))
                        {
                            Database.sviApartmani.TryGetValue(id, out apartman);
                        }
                    }

                    string[] datumi = datumiIzdavanje.Split(' ', ',');
                    string[] split;
                    List<DateTime> noviDatumi = new List<DateTime>();

                    foreach (string dat in datumi)
                    {
                        split = dat.Split('-', '.', '/');
                        DateTime datum = new DateTime(Int32.Parse(split[2]), Int32.Parse(split[1]), Int32.Parse(split[0]));
                        noviDatumi.Add(datum);                                          
                    }

                    SadrzajApartmana sadrzaj;
                    List<SadrzajApartmana> sadrzajiAp = new List<SadrzajApartmana>();

                    foreach (string s in sadrzajiApartmana)
                    {
                        foreach (SadrzajApartmana sadrz in Database.sadrzajiApartmana.Values)
                        {
                            if (sadrz.Naziv == s)
                            {
                                sadrzaj = new SadrzajApartmana(sadrz.Id, sadrz.Naziv);
                                sadrzajiAp.Add(sadrzaj);
                            }
                        }
                    }

                    Database.domacini[apartman.Domacin.KorisnickoIme].ApartmaniZaIzdavanje.Remove(apartman);
                    Database.sviApartmani.Remove(Int32.Parse(idApartmana));

                    apartman.TipApartman = tip;
                    apartman.BrojSoba = Int32.Parse(brSoba);
                    apartman.BrojGostiju = Int32.Parse(brGostiju);
                    apartman.CenaPoNoci = Double.Parse(cena);
                    apartman.DatumiZaIzdavanje=noviDatumi;
                    apartman.DostupnostPoDatumima=noviDatumi;
                    apartman.Lokacija = lokacija;
                    apartman.Status = status;
                    apartman.VremeZaPrijavu = vremePrijava;
                    apartman.VremeZaOdjavu = vremeOdjava;
                    apartman.SadrzajiApartmana = sadrzajiAp;

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
                                Image uf = new Image(fileName, path);
                                apartman.Slike.Add(uf);
                            }
                        }
                    }


                    Database.sviApartmani.Add(Int32.Parse(idApartmana), apartman);

                    Database.domacini[kor.KorisnickoIme].ApartmaniZaIzdavanje.Add(apartman);

                    Database.UpisiSveApartmane();

                    ViewBag.Message = "Apartman je uspesno izmenjen!";
                    return View("IzmenaPodatakaRez");
                }
            }

            ViewBag.Message = "Apartman nije izmenjen dogodila se greska.";
            return View("IzmenaPodatakaRez");
        }

        [HttpPost]
        public ActionResult PretragaDomacin(string datumOd, string datumDo, string grad, string cenaOd, string cenaDo, string brSobaOd, string brSobaDo, string brOsoba,string domacinKorIme)
        {
            List<Apartman> apartmani = new List<Apartman>();

            string[] splitted;

            if (datumOd != "" && datumDo != "")
            {
                splitted = datumOd.Split('-', '.', '/');

                DateTime DatumOd = new DateTime(Int32.Parse(splitted[2]), Int32.Parse(splitted[1]), Int32.Parse(splitted[0]));

                splitted = datumDo.Split('-', '.', '/');

                DateTime DatumDo = new DateTime(Int32.Parse(splitted[2]), Int32.Parse(splitted[1]), Int32.Parse(splitted[0]));

                foreach (Apartman apart in Database.domacini[domacinKorIme].ApartmaniZaIzdavanje)
                {
                    foreach (var dostupan in apart.DostupnostPoDatumima)
                    {
                        if (apart.Status == StatusApartmana.AKTIVAN && !apart.IsDeleted)
                        {
                            if (dostupan.Date > DatumOd && dostupan.Date < DatumDo)
                                apartmani.Add(apart);
                        }
                    }
                }
            }

            else if (datumOd != "")
            {
                splitted = datumOd.Split('-', '.', '/');
                DateTime DatumOd = new DateTime(Int32.Parse(splitted[2]), Int32.Parse(splitted[1]), Int32.Parse(splitted[0]));

                foreach (Apartman apart in Database.domacini[domacinKorIme].ApartmaniZaIzdavanje)
                {
                    foreach (var dostupan in apart.DostupnostPoDatumima)
                    {
                        if (apart.Status == StatusApartmana.AKTIVAN && !apart.IsDeleted)
                        {
                            if (dostupan.Date > DatumOd)
                                apartmani.Add(apart);
                        }

                    }
                }
            }

            else if (datumDo != "")
            {
                splitted = datumDo.Split('-', '.', '/');
                DateTime DatumDo = new DateTime(Int32.Parse(splitted[2]), Int32.Parse(splitted[1]), Int32.Parse(splitted[0]));

                foreach (Apartman apart in Database.domacini[domacinKorIme].ApartmaniZaIzdavanje)
                {
                    foreach (var dostupan in apart.DostupnostPoDatumima)
                    {
                        if (apart.Status == StatusApartmana.AKTIVAN && !apart.IsDeleted)
                        {
                            if (dostupan.Date < DatumDo)
                            {
                                apartmani.Add(apart);
                            }
                        }

                    }
                }
            }

            if (grad != "")
            {
                foreach (Apartman ap in Database.domacini[domacinKorIme].ApartmaniZaIzdavanje)
                {
                    if (ap.Status == StatusApartmana.AKTIVAN)
                    {
                        if (ap.Lokacija.Adresa.Grad == grad && !ap.IsDeleted)
                            apartmani.Add(ap);
                    }
                }
            }

            int cenaMin;
            int cenaMax;

            if (cenaOd != "" && cenaDo != "")
            {
                cenaMin = Int32.Parse(cenaOd);
                cenaMax = Int32.Parse(cenaDo);

                foreach (Apartman ap in Database.domacini[domacinKorIme].ApartmaniZaIzdavanje)
                {
                    if (ap.Status == StatusApartmana.AKTIVAN && !ap.IsDeleted)
                    {
                        if (ap.CenaPoNoci >= cenaMin && ap.CenaPoNoci <= cenaMax)
                            apartmani.Add(ap);
                    }
                }
            }

            else if (cenaOd != "")
            {
                cenaMin = Int32.Parse(cenaOd);

                foreach (Apartman ap in Database.domacini[domacinKorIme].ApartmaniZaIzdavanje)
                {
                    if (ap.Status == StatusApartmana.AKTIVAN && !ap.IsDeleted)
                    {
                        if (ap.CenaPoNoci >= cenaMin)
                            apartmani.Add(ap);
                    }
                }
            }

            else if (cenaDo != "")
            {
                cenaMax = Int32.Parse(cenaDo);

                foreach (Apartman ap in Database.domacini[domacinKorIme].ApartmaniZaIzdavanje)
                {
                    if (ap.Status == StatusApartmana.AKTIVAN && !ap.IsDeleted)
                    {
                        if (ap.CenaPoNoci <= cenaMax)
                            apartmani.Add(ap);
                    }
                }
            }

            int brSobaMin;
            int brSobaMax;

            if (brSobaOd != "" && brSobaDo != "")
            {
                brSobaMin = Int32.Parse(brSobaOd);
                brSobaMax = Int32.Parse(brSobaDo);

                foreach (Apartman ap in Database.domacini[domacinKorIme].ApartmaniZaIzdavanje)
                {
                    if (ap.Status == StatusApartmana.AKTIVAN && !ap.IsDeleted)
                    {
                        if (ap.BrojSoba >= brSobaMin && ap.BrojSoba <= brSobaMax)
                            apartmani.Add(ap);
                    }
                }
            }

            else if (brSobaOd != "")
            {
                brSobaMin = Int32.Parse(brSobaOd);

                foreach (Apartman ap in Database.domacini[domacinKorIme].ApartmaniZaIzdavanje)
                {
                    if (ap.Status == StatusApartmana.AKTIVAN && !ap.IsDeleted)
                    {
                        if (ap.BrojSoba >= brSobaMin)
                            apartmani.Add(ap);
                    }
                }
            }

            else if (brSobaDo != "")
            {
                brSobaMax = Int32.Parse(brSobaDo);

                foreach (Apartman ap in Database.domacini[domacinKorIme].ApartmaniZaIzdavanje)
                {
                    if (ap.Status == StatusApartmana.AKTIVAN && !ap.IsDeleted)
                    {
                        if (ap.BrojSoba <= brSobaMax)
                            apartmani.Add(ap);
                    }
                }
            }


            if (brOsoba != "")
            {
                int brojOsoba = Int32.Parse(brOsoba);

                foreach (Apartman ap in Database.domacini[domacinKorIme].ApartmaniZaIzdavanje)
                {
                    if (ap.Status == StatusApartmana.AKTIVAN && !ap.IsDeleted)
                    {                      
                          if (ap.BrojGostiju == brojOsoba)
                                apartmani.Add(ap);                                                       
                    }                       
               }
           }
       
            return View("RezultatPretrage", apartmani);
        }

        [HttpPost]
        public ActionResult SortiranjeDomacin(string sortirajPoCeni,string domacinKorIme)
        {
            List<Apartman> apartmani = new List<Apartman>();
            List<Apartman> sortiraniApartmani = new List<Apartman>();

            foreach (Apartman ap in Database.domacini[domacinKorIme].ApartmaniZaIzdavanje)
            {
                if (ap.Status == StatusApartmana.AKTIVAN && !ap.IsDeleted)
                    apartmani.Add(ap);
            }

            if (sortirajPoCeni == "rastucoj")
                sortiraniApartmani = apartmani.OrderBy(s => s.CenaPoNoci).ToList();

            else
                sortiraniApartmani = apartmani.OrderByDescending(s => s.CenaPoNoci).ToList();

            return View("RezultatSortiranja", sortiraniApartmani);
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
                        Image uf = new Image(fileName, path);
                        list.Add(uf);
                    }
                }
            }
            // ViewBag.uploadedFiles = list;
            ViewBag.Message = "Uspesno ste upload-vali slike.";
            return View("UploadResult");
        }

        [HttpPost]
        public ActionResult OtvoriKreirajApartView(string domacinKorIme)
        {           
            return View("KreirajApartman",domacinKorIme);
        }

        [HttpPost]
        public ActionResult OtvoriIzmeniApartView(string idApartmana)
        {
            Korisnik kor = (Korisnik)Session["korisnik"];
            
            if(kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            foreach (Korisnik k in Database.registrovaniKorisnici.Values)
            {
                if (k.KorisnickoIme == kor.KorisnickoIme  && k.Uloga == Uloga.DOMACIN)
                {
                    Apartman apartman = new Apartman();
                    foreach (int id in Database.sviApartmani.Keys)
                    {
                        if (id == Int32.Parse(idApartmana))
                        {
                            Database.sviApartmani.TryGetValue(id, out apartman);                           
                        }
                    }
                    
                    return View("IzmeniApartman", apartman);
                }
            }
            ViewBag.Message = "Apartman nije promenjen, desila se greska!";
            return View("IzmenaPodatakaRez");
        }

        [HttpPost]
        public ActionResult ObrisiApartman(string idApartmana)
        {
            Korisnik kor = (Korisnik)Session["korisnik"];

            if(kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            foreach(Korisnik k in Database.domacini.Values)
            {
                if(k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.DOMACIN)
                {
                    Apartman apartman = new Apartman();
                    foreach (int id in Database.sviApartmani.Keys)
                    {
                        if (id == Int32.Parse(idApartmana))
                        {
                            Database.sviApartmani.TryGetValue(id, out apartman);
                            apartman.IsDeleted = true;
                        }
                    }
                }

                ViewBag.Message = "Apartman je uspesno obrisan.";
                return View("RezultatBrisanja");
            }
            ViewBag.Message = "Apartman nije obrisan, desila se neka greska!";
            return View("RezultatBrisanja");
        }

        [HttpPost]
        public ActionResult FiltrirajApartmane(string sadrzaj,string tip,string status)
        {
            List<Apartman> filtriraniApartmani = new List<Apartman>();

            Korisnik kor = (Korisnik)Session["korisnik"];

            if (kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            foreach (Korisnik k in Database.registrovaniKorisnici.Values)
            {
                if (k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.DOMACIN)
                {
                    TipApartmana t;

                    if (tip.ToUpper() == "CEO_APARTMAN")
                        t = TipApartmana.CEO_APARTMAN;
                    else
                        t = TipApartmana.SOBA;

                    StatusApartmana s;

                    if (status.ToUpper() == "AKTIVAN")
                        s = StatusApartmana.AKTIVAN;
                    else
                        s = StatusApartmana.NEAKTIVAN;

                    SadrzajApartmana sadrz = new SadrzajApartmana();

                    foreach (SadrzajApartmana sad in Database.sadrzajiApartmana.Values)
                    {
                        if (sad.Naziv == sadrzaj)
                        {
                            sadrz.Naziv = sad.Naziv;
                            sadrz.Id = sad.Id;
                        }
                    }

                    SadrzajApartmana sadr;
                    if (sadrzaj != "")
                    {
                        foreach (Apartman ap in Database.domacini[kor.KorisnickoIme].ApartmaniZaIzdavanje)
                        {
                            sadr = ap.SadrzajiApartmana.Find(x => x.Naziv == sadrz.Naziv);
                            if (sadr != null)
                            {
                                filtriraniApartmani.Add(ap);
                            }
                        }
                    }

                    if (tip != "")
                    {
                        foreach (Apartman ap in Database.sviApartmani.Values)
                        {
                            if (ap.TipApartman == t)
                                filtriraniApartmani.Add(ap);
                        }
                    }

                    if (status != "")
                    {
                        foreach (Apartman ap in Database.sviApartmani.Values)
                        {
                            if (ap.Status == s)
                                filtriraniApartmani.Add(ap);
                        }
                    }
                }
            }

            return View("RezultatFiltriranja", filtriraniApartmani);
        }

        [HttpPost]
        public ActionResult PrikaziKomentApart(string komentar)
        {
            Korisnik kor = (Korisnik)Session["korisnik"];

            if(kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            if(komentar == null)
            {
                ViewBag.ErrorMessage = "Ne postoji nijedan komentar!";
                return View("Greska");
            }

            KomentarZaApartman koment = new KomentarZaApartman();
            foreach(Korisnik k in Database.registrovaniKorisnici.Values)
            {
                if(k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.DOMACIN)
                {
                    koment = Database.komentariZaApartmane[Int32.Parse(komentar)];

                    //foreach (Apartman ap in Database.sviApartmani.Values)
                    //{
                    //    foreach(KomentarZaApartman kom in ap.Komentari)
                    //    {
                    //        if(kom.Id == koment.Id)
                    //        {
                    //            koment.Apartman = ap;
                    //        }
                    //    }
                    //}

                   // Database.komentariZaApartmane.Remove(koment.Id);
                    koment.Odobren = true;
                    //Database.komentariZaApartmane.Add(koment.Id, koment);

                    ViewBag.Message = "Komentar je odobren.";
                    return View("OdobriKomentarRez");
                }
            }

            ViewBag.Message = "Komentar nije odobren,desila se neka greska!";
            return View("OdobriKomentarRez");
        }

                 
                   


                   

      
    }

    
}