using SistemZaRezervacijeApartmana.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SistemZaRezervacijeApartmana
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

           
            string tmp = "";
            string[] line;
            Pol pol;

            string path = "~/App_Data/Admini.txt";
            path = HostingEnvironment.MapPath(path);

            if (File.Exists(path))
            {
                StreamReader sr = new StreamReader(path);
                while ((tmp = sr.ReadLine()) != ""&& !sr.EndOfStream)
                {
                    line = tmp.Split('_');
                    if (line[4] == "MUSKI")
                        pol = Pol.MUSKI;
                    else
                        pol = Pol.ZENSKI;

                    Admin admin = new Admin(line[0], line[1], line[2], line[3], pol);
                    Database.registrovaniKorisnici.Add(line[2], admin);
                }

                sr.Close();   
            }

            

            UcitajBazu();
        }

        public void UcitajBazu()
        {
            string temp = "";
            string[] read;

            string path = "~/App_Data/RegistrovaniKorisnici.txt";
            path = HostingEnvironment.MapPath(path);

            if (File.Exists(path))
            {
                StreamReader srUser = new StreamReader(path);
                //  while ((temp = srUser.ReadLine()) != "")
                while ((temp = srUser.ReadLine().Trim(' ')) != "" && !srUser.EndOfStream)
                {
                    read = temp.Split('_');
                    Pol pol;

                    if (read[4] == "MUSKI")
                        pol = Pol.MUSKI;
                    else
                        pol = Pol.ZENSKI;

                    Uloga uloga;

                    if (read[5] == "GOST")
                        uloga = Uloga.GOST;
                    else if (read[5] == "DOMACIN")
                        uloga = Uloga.DOMACIN;
                    else
                        uloga = Uloga.ADMINISTRATOR;

                    Korisnik k = new Korisnik(read[0], read[1], read[2], read[3], pol);
                    k.Uloga = uloga;

                    if (!Database.registrovaniKorisnici.ContainsKey(read[2]))
                    {
                        Database.registrovaniKorisnici.Add(k.KorisnickoIme, k);
                    }
                }

                srUser.Close();
            }


            string path1 = "~/App_Data/Domacini.txt";
            path1 = HostingEnvironment.MapPath(path1);


            if(File.Exists(path1))
            {
                StreamReader sr = new StreamReader(path1);

                //while((temp=sr.ReadLine())!="")
                while ((temp = sr.ReadLine().Trim(' ')) != "" && !sr.EndOfStream)
                {
                    read = temp.Split('_');

                    Pol p;
                    if (read[4] == "MUSKI")
                        p = Pol.MUSKI;

                    else
                        p = Pol.ZENSKI;

                    Domacin d = new Domacin(read[0],read[1],read[2],read[3],p);

                    if(!Database.domacini.ContainsKey(read[0]))
                    {
                        Database.domacini.Add(read[2], d);
                    }

                }

                sr.Close();
            }


            string path2 = "~/App_Data/KomentariZaApartmane.txt";
            path2 = HostingEnvironment.MapPath(path2);
            Random id = new Random();
           // Random idAp = new Random();
            int idKom;

            if (File.Exists(path2))
            {
                StreamReader sr = new StreamReader(path2);

                while ((temp = sr.ReadLine()) != null)
                {
                    read = temp.Split('|');


                    idKom = id.Next();

                    Gost gost = new Gost();
                    gost.KorisnickoIme = read[0];

                    Adresa adresa = new Adresa(read[1].Split('_', ',')[1], read[1].Split('_', ',')[0], "21000", read[1].Split('_', ',')[2]);
                    Lokacija lokacija = new Lokacija(45.19, 19.87, adresa);
                    Apartman apartman = new Apartman();

                    apartman.Lokacija = lokacija;
                    apartman.Status = StatusApartmana.AKTIVAN;
                    apartman.IsDeleted = false;
                    //apartman.Id = idAp.Next(0, 700);

                    OcenaApartmana ocena;

                    if (read[3].ToUpper() == "PET")
                        ocena = OcenaApartmana.PET;
                    else if (read[3].ToUpper() == "CETIRI")
                        ocena = OcenaApartmana.ČETIRI;
                    else if (read[3].ToUpper() == "TRI")
                        ocena = OcenaApartmana.TRI;
                    else if (read[3].ToUpper() == "DVA")
                        ocena = OcenaApartmana.DVA;
                    else if (read[3].ToUpper() == "JEDAN")
                        ocena = OcenaApartmana.JEDAN;
                    else
                        ocena = OcenaApartmana.NULA;

                    KomentarZaApartman komentar = new KomentarZaApartman(idKom, gost, apartman, read[2], ocena);

                    if (!Database.komentariZaApartmane.ContainsKey(idKom))
                    {
                        Database.komentariZaApartmane.Add(idKom, komentar);
                    }                
                }

                sr.Close();
            }


        }

       
    }
}

