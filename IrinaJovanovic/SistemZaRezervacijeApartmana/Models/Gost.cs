using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemZaRezervacijeApartmana.Models
{
    public class Gost : Korisnik
    {
        public List<Apartman> IznajmljeniApartmani { get; set; }
        public List<Rezervacija> Rezervacije { get; set; }

        public Gost()
        {
            Uloga = Uloga.GOST;
            IznajmljeniApartmani = new List<Apartman>();
            Rezervacije = new List<Rezervacija>();
        }

        public Gost(string ime,string prezime, string korIme, string lozinka, Pol pol): base()
        {
            KorisnickoIme = korIme;
            Lozinka = lozinka;
            Ime = ime;
            Prezime = prezime;
            Pol = pol;
            Uloga = Uloga.GOST;
            IznajmljeniApartmani = new List<Apartman>();
            Rezervacije = new List<Rezervacija>();
        }

    }
}