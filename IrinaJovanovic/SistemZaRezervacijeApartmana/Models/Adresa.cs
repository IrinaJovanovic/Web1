using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemZaRezervacijeApartmana.Models
{
    public class Adresa
    {
        public string Ulica { get; set; }
        public string Broj { get; set; }
        public string PostanskiBroj { get; set; }
        public string Grad { get; set; }

        public Adresa()
        {

        }

        public Adresa(string broj,string ulica,string postBroj,string grad)
        {
            Broj = broj;
            Ulica = ulica;
            PostanskiBroj = postBroj;
            Grad = grad;
        }
   

    }
}