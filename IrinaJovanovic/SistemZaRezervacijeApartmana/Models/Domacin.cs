using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemZaRezervacijeApartmana.Models
{
    public class Domacin : Korisnik
    {
        public List<Apartman> ApartmaniZaIzdavanje;

        public Domacin()
        {
            Uloga = Uloga.DOMACIN;
            ApartmaniZaIzdavanje = new List<Apartman>();
        }

        public Domacin(string ime,string prezime, string korIme, string lozinka, Pol pol):base()
        {
            KorisnickoIme = korIme;
            Lozinka = lozinka;
            Ime = ime;
            Prezime = prezime;
            Pol = pol;
            Uloga = Uloga.DOMACIN;
            ApartmaniZaIzdavanje = new List<Apartman>();
        }
    }
}