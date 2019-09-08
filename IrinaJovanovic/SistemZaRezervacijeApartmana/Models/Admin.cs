using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemZaRezervacijeApartmana.Models
{
    public class Admin : Korisnik
    {
      
        public Admin(string ime,string prezime, string korIme, string lozinka, Pol pol): base()
        {
            KorisnickoIme = korIme;
            Lozinka = lozinka;
            Ime = ime;
            Prezime = prezime;
            Pol = pol;
            Uloga = Uloga.ADMINISTRATOR;
        }

       
    }
}