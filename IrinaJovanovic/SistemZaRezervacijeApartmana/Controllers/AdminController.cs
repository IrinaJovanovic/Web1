using SistemZaRezervacijeApartmana.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SistemZaRezervacijeApartmana.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult IzmenaPodataka(string username,string pass,string ime, string prezime,string pol)
        {
            Korisnik k = (Korisnik)Session["korisnik"];

            if (k == null)
            {
                k = new Korisnik();
                Session["korisnik"] = k;
            }

            foreach(Korisnik kor in Database.registrovaniKorisnici.Values)
            {
                if(kor.KorisnickoIme == username && kor.Uloga == Uloga.ADMINISTRATOR)
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

                    Database.AzurirajPodatkeAdmina();


                    ViewBag.Message = "Podaci su uspesno izmenjeni.";
                    return View("IzmenaPodatakaRez", Database.registrovaniKorisnici[username]);
                }

            }

            ViewBag.Message = "Podaci nisu izmenjeni, desila se neka greska!";
            return View("IzmenaPodatakaRez");


        }

        [HttpPost]
        public ActionResult KreirajDomacina(string ime,string prezime,string korIme,string lozinka,string pol)
        {
            Korisnik kor = (Korisnik)Session["korisnik"];

            if(kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            if(ime == "" || prezime == "" || korIme=="" || lozinka=="")
            {
                ViewBag.ErrorMessage = "Morate popuniti sva polja da biste kreirali domacina.";
                return View("Greska");
            }

            foreach(Korisnik k in Database.registrovaniKorisnici.Values)
            {
                if(k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.ADMINISTRATOR)
                {

                    Pol p;

                    if (pol == "MUSKI")
                        p = Pol.MUSKI;
                    else
                        p = Pol.ZENSKI;

                    Domacin domacin = new Domacin(ime, prezime, korIme, lozinka, p);

                   foreach(Korisnik d in Database.registrovaniKorisnici.Values)
                    {
                        if(d.KorisnickoIme == domacin.KorisnickoIme)
                        {
                            ViewBag.Message = "Korisnik sa unetim korisnickim imenom vec postoji,unesite novo korisnicko ime!";
                            return View("KreirajDomacinaRezultat");
                        }
                    }

                    Database.registrovaniKorisnici.Add(domacin.KorisnickoIme, domacin);
                    Database.domacini.Add(domacin.KorisnickoIme, domacin);

                    Database.UpisiRegistrovaneKorisnike();
                    Database.UpisiDomacine();

                    ViewBag.Message = "Domacin je uspesno kreiran.";

                    return View("KreirajDomacinaRezultat");          
                }
            }

            ViewBag.Message = "Domacin nije kreiran desila se neka greska!";

            return View("KreirajDomacinaRezultat");
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
                if (k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.ADMINISTRATOR)
                {
                    Apartman apartman = new Apartman();
                    foreach (int id in Database.sviApartmani.Keys)
                    {
                        if (id == Int32.Parse(idApartmana))
                        {
                            Database.sviApartmani.TryGetValue(id,out apartman);
                            
                        }
                    }

                    return View("IzmeniApartman", apartman);
                }
            }
            ViewBag.Message = "Apartman nije promenjen, desila se greska!";
            return View("IzmenaPodatakaRez");
        }

        [HttpPost]
        public ActionResult IzmenaApartmana(string tipApartmana,string brSoba,string brGostiju,string ulica,string broj,string grad,string postBroj,string datumiIzdavanje,string cena,string statusApartmana,string vremePrijava,string vremeOdjava, List<string> sadrzajiApartmana, string idApartmana)
        {
            Korisnik kor = (Korisnik)Session["korisnik"];

            if (kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            if (brSoba == "" || brGostiju == "" || ulica == "" || broj == "" || grad == "" || postBroj == "" || cena == "" || vremePrijava == "" || vremeOdjava == ""  || datumiIzdavanje == "" || sadrzajiApartmana.Count == 0)
            {
                ViewBag.ErrorMessage = "Ne smete nijedno  polje ostaviti prazno, da biste uspesno mogli da izmenite apartman!";
                return View("Greska");
            }

            foreach (Korisnik k in Database.registrovaniKorisnici.Values)
            {
                if(k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.ADMINISTRATOR)
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
                    apartman.DatumiZaIzdavanje = noviDatumi;
                    apartman.DostupnostPoDatumima = noviDatumi;
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

                    Database.domacini[apartman.Domacin.KorisnickoIme].ApartmaniZaIzdavanje.Add(apartman);

                    Database.UpisiSveApartmane();

                    ViewBag.Message = "Apartman je uspesno izmenjen!";
                    return View("IzmenaPodatakaRez");
        
                }
            }

            ViewBag.Message = "Apartman nije izmenjen dogodila se greska.";
            return View("IzmenaPodatakaRez");
        }

        [HttpPost]
        public ActionResult ObrisiApartman(string idApartmana)
        {
            Korisnik kor = (Korisnik)Session["korisnik"];

            if (kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            foreach (Korisnik k in Database.registrovaniKorisnici.Values)
            {
                if (k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.ADMINISTRATOR)
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
        public ActionResult DodajSadrzajApartmanaView(/*string idApartmana*/)
        {
            return View("KreirajSadrzajApartmana"/*, idApartmana*/);
        }

        
        [HttpPost]
        public ActionResult KreirajSadrzajApartmana(string nazivSadrzaja)
        {
            Korisnik kor = (Korisnik)Session["korisnik"];

            if(kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            if( nazivSadrzaja == "")
            {
                ViewBag.ErrorMessage = "Morate popuniti polje za naziv sadrzaja apartmana!";
                return View("Greska");
            }

            foreach(Korisnik k in Database.registrovaniKorisnici.Values)
            {
                if(k.KorisnickoIme == kor.KorisnickoIme && k.Uloga== Uloga.ADMINISTRATOR)
                {
                    Random id = new Random();
                    SadrzajApartmana sadrzaj = new SadrzajApartmana(id.Next(0,100),nazivSadrzaja);

                    foreach (SadrzajApartmana s in Database.sadrzajiApartmana.Values)
                    {
                        if (s.Id == sadrzaj.Id ||  s.Naziv == sadrzaj.Naziv)
                        {
                            ViewBag.ErrorMessage = "Sadrzaj apartmana vec postoji!";
                            return View("Greska");
                           
                        }
                    }

                    Database.sadrzajiApartmana.Add(sadrzaj.Id,sadrzaj);

                    Database.UpisiSadrzajeApartmana();

                    ViewBag.Message = "Uspesno ste dodali sadrzaj apartmana.";
                    return View("RezultatKreiranjaSadrzaja");
                }
            }

            ViewBag.Message = "Sadrzaj apartmana nije dodat,desila se neka greska!";
            return View("RezultatKreiranjaSadrzaja");
        }

        [HttpPost]
        public ActionResult IzmeniSadrzajApartView(string idSadrzaja)
        {
            Korisnik kor = (Korisnik)Session["korisnik"];

            if(kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            foreach(Korisnik k in Database.registrovaniKorisnici.Values)
            {
                if(k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.ADMINISTRATOR)
                {
                    SadrzajApartmana sadrzaj = new SadrzajApartmana();

                    foreach(int id in Database.sadrzajiApartmana.Keys)
                    {
                        if (id == Int32.Parse(idSadrzaja))
                            Database.sadrzajiApartmana.TryGetValue(id, out sadrzaj);
                    }


                    return View("IzmeniSadrzajApartmana", sadrzaj);
                }
            }

            ViewBag.Message = "Sadrzaj apartmana nije promenjen, desila se greska!";
            return View("IzmenaPodatakaRez");
        }

        [HttpPost]
        public ActionResult IzmeniSadrzajApartmana(string id,string naziv)
        {
            Korisnik kor = (Korisnik)Session["korisnik"];

            if(kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            if(naziv == "")
            {
                ViewBag.ErrorMessage = "Ne smete nijedno polje ostaviti prazno, da biste uspesno mogli da izmenite sadrzaj apartmana!";
                return View("Greska");
            }

            foreach(Korisnik k in Database.registrovaniKorisnici.Values)
            {
                if(k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.ADMINISTRATOR)
                {
                    SadrzajApartmana sadrzaj = new SadrzajApartmana();

                    foreach(int ID in Database.sadrzajiApartmana.Keys)
                    {
                        if (ID == Int32.Parse(id))
                            Database.sadrzajiApartmana.TryGetValue(ID, out sadrzaj);
                    }
                  
                    SadrzajApartmana sadrzajNovi = new SadrzajApartmana(Int32.Parse(id), naziv);

                    foreach (SadrzajApartmana s in Database.sadrzajiApartmana.Values)
                    {                   
                        if (s.Naziv == sadrzajNovi.Naziv)
                        {
                            ViewBag.ErrorMessage = "Sadrzaj apartmana vec postoji!";
                            return View("Greska");

                        }
                    }
                    Database.sadrzajiApartmana.Remove(sadrzaj.Id);

                    Database.sadrzajiApartmana.Add(sadrzajNovi.Id, sadrzajNovi);

                    Database.UpisiSadrzajeApartmana();

                    ViewBag.Message = "Uspesno ste izmenili sadrzaj apartmana.";
                    return View("IzmenaPodatakaRez");
                }
            }

            ViewBag.Message = "Sadrzaj apartmana nije izmenjen, desila se neka greska!";
            return View("IzmenaPodatakaRez");
        }

        [HttpPost]
        public ActionResult ObrisiSadrzajApartmana(string idSadrzaja)
        {
            Korisnik kor = (Korisnik)Session["korisnik"];

            if(kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            foreach(Korisnik k in Database.registrovaniKorisnici.Values)
            {
                if(k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.ADMINISTRATOR)
                {
                    SadrzajApartmana sadrzaj = new SadrzajApartmana();

                    foreach(int id in Database.sadrzajiApartmana.Keys)
                    {
                        if (id == Int32.Parse(idSadrzaja))
                        {
                            Database.sadrzajiApartmana.TryGetValue(id, out sadrzaj);
                            sadrzaj.IsDeleted = true;
                        }
                          
                    }

                    ViewBag.Message = "Sadrzaj apartmana je uspesno obrisan.";
                    return View("RezultatBrisanja");
                }
            }

            ViewBag.Message = "Sadrzaj apartmana nije obrisan, desila se neka greska!";
            return View("RezultatBrisanja");
        }

        [HttpPost]
        public ActionResult OdobriApartman(string apartman)
        {
            Korisnik kor = (Korisnik)Session["korisnik"];

            if(kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            if (apartman == null)
            {
                ViewBag.ErrorMessage = "Ne postoji nijedan apartman!";
                return View("Greska");
            }

            Apartman apart = new Apartman();
            foreach(Korisnik k in Database.registrovaniKorisnici.Values)
            {
                if(k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.ADMINISTRATOR)
                {
                    apart = Database.sviApartmani[Int32.Parse(apartman)];
                   
                    Database.sviApartmani.Remove(apart.Id);
                    Database.domacini[apart.Domacin.KorisnickoIme].ApartmaniZaIzdavanje.Remove(apart);

                    apart.Status = StatusApartmana.AKTIVAN;

                    Database.sviApartmani.Add(apart.Id,apart);

                    Database.domacini[apart.Domacin.KorisnickoIme].ApartmaniZaIzdavanje.Add(apart);

                    Database.UpisiSveApartmane();

                    ViewBag.Message = "Apartman je odobren.";
                    return View("OdobriApartmanRez");                   
                }
            }

            ViewBag.Message = "Apartman nije odobren!";
            return View("OdobriApartmanRez");
        }

        [HttpPost]
        public ActionResult PretragaKorisnika(string uloga,string pol,string korIme)
        {
            Korisnik kor = (Korisnik)Session["korisnik"];

            if(kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            List<Korisnik> korisnici = new List<Korisnik>();

            foreach(Korisnik k in Database.registrovaniKorisnici.Values)
            {
                if(k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.ADMINISTRATOR)
                {
                    Uloga u;

                    if (uloga.ToUpper() == "ADMINISTRATOR")
                        u = Uloga.ADMINISTRATOR;
                    else if (uloga.ToUpper() == "DOMACIN")
                        u = Uloga.DOMACIN;
                    else
                        u = Uloga.GOST;

                    Pol p;

                    if (pol.ToUpper() == "MUSKI")
                        p = Pol.MUSKI;
                    else
                        p = Pol.ZENSKI;

                    if(uloga != "")
                    {
                        foreach(Korisnik ko in Database.registrovaniKorisnici.Values)
                        {
                            if (ko.Uloga == u)
                                korisnici.Add(ko);
                        }
                    }


                    if(pol != "")
                    {
                        foreach (Korisnik ko in Database.registrovaniKorisnici.Values)
                        {
                            if (ko.Pol == p)
                                korisnici.Add(ko);
                        }
                    }

                    if(korIme != "")
                    {
                        foreach (Korisnik ko in Database.registrovaniKorisnici.Values)
                        {
                            if (ko.KorisnickoIme == korIme)
                                korisnici.Add(ko);
                        }
                    }                    
                }
            }

            return View("RezultatPretrageKorisnika", korisnici);
        }

        [HttpPost]
        public ActionResult FiltrirajApartmane(string sadrzaj,string tip,string status)
        {
            List<Apartman> filtriraniApartmani = new List<Apartman>();

            Korisnik kor = (Korisnik)Session["korisnik"];

            if(kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            foreach(Korisnik k in Database.registrovaniKorisnici.Values)
            {
                if(k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.ADMINISTRATOR)
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

                    SadrzajApartmana sadrz= new SadrzajApartmana();

                    foreach(SadrzajApartmana sad in Database.sadrzajiApartmana.Values)
                    {
                        if(sad.Naziv == sadrzaj)
                        {
                            sadrz.Naziv = sad.Naziv;
                            sadrz.Id = sad.Id;
                        }
                    }

                    SadrzajApartmana sadr;
                    if (sadrzaj != "")
                    {
                       foreach(Apartman ap in Database.sviApartmani.Values)
                       {
                           sadr = ap.SadrzajiApartmana.Find(x => x.Naziv == sadrz.Naziv);
                           if(sadr != null)
                            {
                                filtriraniApartmani.Add(ap);
                            }
                       }
                    }

                    if(tip != "")
                    {
                        foreach(Apartman ap in Database.sviApartmani.Values)
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

            return View("RezultatFiltriranja",filtriraniApartmani);
        }


    }
}