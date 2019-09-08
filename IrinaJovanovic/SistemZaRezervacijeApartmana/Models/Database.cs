using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace SistemZaRezervacijeApartmana.Models
{
    public class Database
    {
        public static Dictionary<string, Korisnik> registrovaniKorisnici = new Dictionary<string, Korisnik>();
        public static Dictionary<string, Domacin> domacini = new Dictionary<string, Domacin>();
        public static Dictionary<int, Apartman> sviApartmani = new Dictionary<int, Apartman>();
        public static Dictionary<int, SadrzajApartmana> sadrzajiApartmana = new Dictionary<int, SadrzajApartmana>(); //kljuc id, vrednost naziv sadrzaja
        public static Dictionary<int,KomentarZaApartman> komentariZaApartmane = new Dictionary<int,KomentarZaApartman>(); // kljuc je id komentara,vrednost komentar 
        public static Dictionary<string, Rezervacija> rezervacije = new Dictionary<string, Rezervacija>();

        public static void UpisiRegistrovaneKorisnike()
        {
            string upis = "";
            string path = "~/App_Data/RegistrovaniKorisnici.txt";
            path = HostingEnvironment.MapPath(path);
            if(File.Exists(path))
            {
                StreamWriter sw = new StreamWriter(path);

                foreach (Korisnik k in registrovaniKorisnici.Values)
                {
                    if (k.Uloga != Uloga.ADMINISTRATOR)
                    {
                        upis += string.Format(k.Ime + "_" + k.Prezime + "_" + k.KorisnickoIme + "_" + k.Lozinka + "_" + k.Pol.ToString() + "_" + k.Uloga.ToString() + "\n");
                    }
                }
                sw.WriteLine(upis);
                sw.Close();
            }
        }

        public static void UpisiDomacine()
        {
            string d = "";
            string path = "~/App_Data/Domacini.txt";
            path = HostingEnvironment.MapPath(path);

            if(File.Exists(path))
            {
                foreach(Domacin dom in domacini.Values)
                {
                    d += string.Format(dom.Ime + "_" +dom.Prezime+"_"+dom.KorisnickoIme+"_"+dom.Lozinka+"_"+dom.Pol.ToString()+"_"+dom.Uloga.ToString()+"\n");
                }

                StreamWriter sw = new StreamWriter(path);
                sw.WriteLine(d);
                sw.Close();
            }

        }

        public static void UpisiSveApartmane()
        {
            string apartmani = "";
            string path = "~/App_Data/SviApartmani.txt";
            path = HostingEnvironment.MapPath(path);

            if(File.Exists(path))
            {
                foreach(Apartman apartman in sviApartmani.Values)
                {
                    apartmani += string.Format(apartman.TipApartman.ToString() + "_" + apartman.BrojSoba + "_" + apartman.BrojGostiju + "_"
                        + apartman.CenaPoNoci + "|" + apartman.Lokacija.Adresa.Ulica + "|" + apartman.Lokacija.Adresa.Broj + "|" + apartman.Lokacija.Adresa.PostanskiBroj + "|" + apartman.Lokacija.Adresa.Grad
                        + "|" + apartman.Domacin.Ime + "_" + apartman.Domacin.Prezime + "_" + apartman.VremeZaPrijavu + "-" + apartman.VremeZaOdjavu
                        + "_" + apartman.Status.ToString()+"\n");
                }

                StreamWriter sw = new StreamWriter(path);
                sw.WriteLine(apartmani);
                sw.Close();
            }

            
        }

        public static void UpisiSadrzajeApartmana()
        {
            string sadrzaji = "";
            string path = "~/App_Data/SadrzajiApartmana.txt";
            path = HostingEnvironment.MapPath(path);

            if(File.Exists(path))
            {
                foreach(SadrzajApartmana sadrzaj in sadrzajiApartmana.Values)
                {
                    sadrzaji += string.Format(sadrzaj.Id.ToString()+"_"+ sadrzaj.Naziv + "\n");
                }

                StreamWriter sw = new StreamWriter(path);
                sw.WriteLine(sadrzaji);
                sw.Close();
            }
        }

        public static void UpisiKomentareZaApartmane()
        {
            string kometari = "";

            string path = "~/App_Data/KomentariZaApartmane.txt";
            path = HostingEnvironment.MapPath(path);
            
            if(File.Exists(path))
            {
                foreach(KomentarZaApartman komentar in komentariZaApartmane.Values)
                {
                    kometari += string.Format(komentar.Id + "_" + komentar.Gost.KorisnickoIme + "_" + komentar.Apartman.Id + "_" + komentar.Tekst + "+" + komentar.OcenaApartmana.ToString() + "\n");
                }

                StreamWriter sw = new StreamWriter(path);
                sw.WriteLine(kometari);
                sw.Close();
            }

        }

        public static void AzurirajPodatkeAdmina()
        {
            string izmena = "";

            string path = "~/App_Data/Admini.txt";
            path = HostingEnvironment.MapPath(path);

            if(File.Exists(path))
            {
                StreamWriter sw = new StreamWriter(path);

                foreach(Korisnik k in registrovaniKorisnici.Values)
                {
                   if(k.Uloga == Uloga.ADMINISTRATOR)
                      izmena += string.Format(k.Ime + "_" + k.Prezime + "_" + k.KorisnickoIme + "_" + k.Lozinka + "_" + k.Pol.ToString()+"\n");
                   
                }

                sw.WriteLine(izmena);
                sw.Close();
            }
        }


        public static void AzurirajPodatkeDomacina()
        {
            string izmena = "";

            string path = "~/App_Data/Domacini.txt";
      
            path = HostingEnvironment.MapPath(path);
         
            if (File.Exists(path))
            {
                StreamWriter sw = new StreamWriter(path);

                foreach (Korisnik d in registrovaniKorisnici.Values)
                {
                    if (d.Uloga == Uloga.DOMACIN)
                        izmena += string.Format(d.Ime + "_" + d.Prezime + "_" + d.KorisnickoIme + "_" + d.Lozinka + "_" + d.Pol.ToString() +"_"+ d.Uloga.ToString() + "\n");

                }

                sw.WriteLine(izmena);
                sw.Close();
            }

            izmena = "";

            string path1 = "~/App_Data/RegistrovaniKorisnici.txt";
            path1 = HostingEnvironment.MapPath(path1);

            if (File.Exists(path1))
            {
                StreamWriter sw = new StreamWriter(path1);

                foreach (Korisnik k in registrovaniKorisnici.Values)
                {
                    if (k.Uloga != Uloga.ADMINISTRATOR)
                        izmena += string.Format(k.Ime + "_" + k.Prezime + "_" + k.KorisnickoIme + "_" + k.Lozinka + "_" + k.Pol.ToString() + "_" + k.Uloga.ToString() + "\n");

                }

                sw.WriteLine(izmena);
                sw.Close();
            }
        }

        public static void AzurirajPodatkeGosta()
        {
            string izmena = "";

            string path = "~/App_Data/RegistrovaniKorisnici.txt";
            path = HostingEnvironment.MapPath(path);

            if (File.Exists(path))
            {
                StreamWriter sw = new StreamWriter(path);

                foreach (Korisnik k in registrovaniKorisnici.Values)
                {
                    if (k.Uloga != Uloga.ADMINISTRATOR)
                        izmena += string.Format(k.Ime + "_" + k.Prezime + "_" + k.KorisnickoIme + "_" + k.Lozinka + "_" + k.Pol.ToString() + "_"+ k.Uloga.ToString()+ "\n");

                }

                sw.WriteLine(izmena);
                sw.Close();
            }
        }
    }
}