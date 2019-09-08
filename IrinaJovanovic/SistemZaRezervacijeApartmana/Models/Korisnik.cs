using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemZaRezervacijeApartmana.Models
{
    public class Korisnik
    {
        public string KorisnickoIme { get; set; }
        public string Lozinka { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public Pol Pol { get; set; }
        public Uloga Uloga { get; set; }


        public Korisnik()
        {
            Uloga = Uloga.GOST;
        }

        public Korisnik(string ime,string prezime, string korIme, string lozinka, Pol pol)
        {
            KorisnickoIme = korIme;
            Lozinka = lozinka;
            Ime = ime;
            Prezime = prezime;
            Pol = pol;
            Uloga = Uloga.GOST;
        }
 
    }
}