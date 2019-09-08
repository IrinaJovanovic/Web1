using SistemZaRezervacijeApartmana.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SistemZaRezervacijeApartmana.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegistrujSe(string ime,string prezime,string korIme,string lozinka,string pol)
        {
            Pol p;

            if (pol == "MUSKI")
                p = Pol.MUSKI;
            else
                p = Pol.ZENSKI;
               
            if(!Database.registrovaniKorisnici.ContainsKey(korIme))
            {
                Korisnik k = new Korisnik(ime, prezime, korIme, lozinka,p);
                Database.registrovaniKorisnici.Add(korIme, k);
                Database.UpisiRegistrovaneKorisnike();

                ViewBag.Message = "Uspesno ste se registrovali!";

                return View("RegistrationResult");

            }

            ViewBag.ErrorMessage = "Registracija nije uspela, jer vec postoji korisnik sa unetim korisnickim imenom!";

            return View("RegistracijaGreska");
        }

        [HttpPost]
        public ActionResult Login(string username,string pass)
        {
            Korisnik korisnik = (Korisnik)Session["korisnik"];

            if (korisnik == null)
            {
                korisnik = new Korisnik();
                Session["korisnik"] = korisnik;
            }
         
            if(Database.registrovaniKorisnici.ContainsKey(username))
            {
                if(Database.registrovaniKorisnici[username].Lozinka == pass)
                {
                    if (Database.registrovaniKorisnici[username].Uloga == Uloga.ADMINISTRATOR)
                    {
                        Session["korisnik"] = Database.registrovaniKorisnici[username];
                        return View("Admin",Database.registrovaniKorisnici[username]);
                    }

                    else if (Database.registrovaniKorisnici[username].Uloga == Uloga.DOMACIN)
                    {
                        Domacin domacin = Database.domacini[username];
                        Session["korisnik"] = Database.registrovaniKorisnici[username];
                        return View("Domacin", domacin);
                    }

                    else
                    {
                        Korisnik gost = new Gost();
                        gost = Database.registrovaniKorisnici[username];
                        Session["korisnik"] = Database.registrovaniKorisnici[username];

                        return View("Gost", gost);

                    }
                }

                else
                {
                    ViewBag.ErrorMessage = "Pogresna lozinka!";
                    return View("LoginError");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Niste u mogucnosti da se prijavite,jer niste registrovani!";
                return View("LoginError");
            }           
        }

        [HttpPost]
        public ActionResult PretragaNeregKorisnik(string datumOd,string datumDo,string grad,string cenaOd,string cenaDo,string brSobaOd,string brSobaDo,string brOsoba)
        {
            List<Apartman> apartmani = new List<Apartman>();

            string[] splitted;

            if (datumOd != ""  &&   datumDo!="")
            {
                splitted = datumOd.Split('-','.','/');

                DateTime DatumOd = new DateTime(Int32.Parse(splitted[2]), Int32.Parse(splitted[1]), Int32.Parse(splitted[0]));

                splitted = datumDo.Split('-', '.', '/');

                DateTime DatumDo = new DateTime(Int32.Parse(splitted[2]), Int32.Parse(splitted[1]), Int32.Parse(splitted[0]));

                foreach(Apartman apart in Database.sviApartmani.Values)
                {                   
                    foreach(var dostupan in apart.DostupnostPoDatumima)
                    {
                        if (apart.Status == StatusApartmana.AKTIVAN && !apart.IsDeleted)
                        {
                            if (dostupan.Date > DatumOd && dostupan.Date < DatumDo)
                                apartmani.Add(apart);
                        }                           
                    }
                }
           }

            else if(datumOd != "")
            {
                splitted = datumOd.Split('-', '.', '/');
                DateTime DatumOd = new DateTime(Int32.Parse(splitted[2]), Int32.Parse(splitted[1]), Int32.Parse(splitted[0]));

                foreach (Apartman apart in Database.sviApartmani.Values)
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

            else if(datumDo != "")
            {
                splitted = datumDo.Split('-', '.', '/');
                DateTime DatumDo = new DateTime(Int32.Parse(splitted[2]), Int32.Parse(splitted[1]), Int32.Parse(splitted[0]));

                foreach (Apartman apart in Database.sviApartmani.Values)
                {
                    foreach (var dostupan in apart.DostupnostPoDatumima)
                    {
                        if(apart.Status == StatusApartmana.AKTIVAN && !apart.IsDeleted)
                        {
                            if (dostupan.Date < DatumDo)
                            {
                                apartmani.Add(apart);
                            }
                        }
                       
                    }
                }
            }

            if(grad != "")
            {
                foreach(Apartman ap in Database.sviApartmani.Values)
                {
                    if(ap.Status == StatusApartmana.AKTIVAN && !ap.IsDeleted) 
                    {
                        if (ap.Lokacija.Adresa.Grad == grad)
                             apartmani.Add(ap);                                                 
                    }                
                }
            }

            int cenaMin;
            int cenaMax;

            if (cenaOd != "" && cenaDo != "")
            {
               cenaMin = Int32.Parse(cenaOd);
               cenaMax= Int32.Parse(cenaDo);

                foreach (Apartman ap in Database.sviApartmani.Values)
                {
                    if(ap.Status == StatusApartmana.AKTIVAN && !ap.IsDeleted)
                    {
                        if (ap.CenaPoNoci >= cenaMin && ap.CenaPoNoci <= cenaMax)
                            apartmani.Add(ap);
                    }                
                }
            }

            else if(cenaOd != "")
            {
                cenaMin = Int32.Parse(cenaOd);

                foreach (Apartman ap in Database.sviApartmani.Values)
                {
                    if (ap.Status == StatusApartmana.AKTIVAN && !ap.IsDeleted)
                    {                  
                        if (ap.CenaPoNoci >= cenaMin)                        
                              apartmani.Add(ap);                         
                    }                                              
                }                       
           }
            

            else if(cenaDo != "")
            {
                cenaMax = Int32.Parse(cenaDo);

                foreach (Apartman ap in Database.sviApartmani.Values)
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

                foreach (Apartman ap in Database.sviApartmani.Values)
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

                foreach (Apartman ap in Database.sviApartmani.Values)
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

                foreach (Apartman ap in Database.sviApartmani.Values)
                {
                    if (ap.Status == StatusApartmana.AKTIVAN && !ap.IsDeleted)
                    {
                        if (ap.BrojSoba <= brSobaMax)
                            apartmani.Add(ap);
                    }                       
                }
            }

            
            if(brOsoba != "")
            {
                int brojOsoba = Int32.Parse(brOsoba);

                foreach (Apartman ap in Database.sviApartmani.Values)
                {
                    if (ap.Status == StatusApartmana.AKTIVAN && !ap.IsDeleted)
                    {
                        if (ap.BrojGostiju == brojOsoba)
                            apartmani.Add(ap);
                    }                      
                }
            }

            return View("RezultatPretrage",apartmani);
        }

        [HttpPost]
        public ActionResult SortiranjeNeregKorisnik(string sortirajPoCeni)
        {
            List<Apartman> apartmani = new List<Apartman>();
            List<Apartman> sortiraniApartmani = new List<Apartman>();

            foreach (Apartman ap in Database.sviApartmani.Values)
            {
                if(ap.Status == StatusApartmana.AKTIVAN && !ap.IsDeleted)
                     apartmani.Add(ap);
            }

            if (sortirajPoCeni == "rastucoj")
                sortiraniApartmani = apartmani.OrderBy(s => s.CenaPoNoci).ToList();

            else
                sortiraniApartmani = apartmani.OrderByDescending(s => s.CenaPoNoci).ToList();

            return View("RezultatSortiranja",sortiraniApartmani);
        }

        [HttpPost]
        public ActionResult FiltrirajApartmane(string sadrzaj, string tip)
        {
                   List<Apartman> filtriraniApartmani = new List<Apartman>();

                    TipApartmana t;

                    if (tip.ToUpper() == "CEO_APARTMAN")
                        t = TipApartmana.CEO_APARTMAN;
                    else
                        t = TipApartmana.SOBA;

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
                        foreach (Apartman ap in Database.sviApartmani.Values)
                        {
                            sadr = ap.SadrzajiApartmana.Find(x => x.Naziv == sadrz.Naziv);
                            if (sadr != null && ap.Status == StatusApartmana.AKTIVAN)
                            {
                                filtriraniApartmani.Add(ap);
                            }
                        }
                    }

                    if (tip != "")
                    {
                        foreach (Apartman ap in Database.sviApartmani.Values)
                        {
                            if (ap.TipApartman == t && ap.Status == StatusApartmana.AKTIVAN)
                                filtriraniApartmani.Add(ap);
                        }
                    }
                
            return View("RezultatFiltriranja", filtriraniApartmani);
        }

        [HttpPost]
        public ActionResult Preusmeri()
        {
            Korisnik kor = (Korisnik)Session["korisnik"];

            if(kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            foreach(Korisnik k in Database.registrovaniKorisnici.Values)
            {
                if(k.KorisnickoIme == kor.KorisnickoIme &&  k.Uloga == Uloga.GOST)
                {
                    return View("Gost", kor);
                }

                else if(k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.DOMACIN)
                {
                    return View("Domacin", Database.domacini[kor.KorisnickoIme]);
                }
                 
                else if(k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.ADMINISTRATOR)
                {
                    return View("Admin",Database.registrovaniKorisnici[k.KorisnickoIme]);
                }
            }

            ViewBag.ErrorMessage = "Dogodila se neka greska.";
            return View("Greska");

        }

        public ActionResult LogOut()
        {
            Session.Abandon();
            Korisnik k = new Korisnik();
            Session["korisnik"] = k;
            return View("Index");
        }

      
       
    }
}