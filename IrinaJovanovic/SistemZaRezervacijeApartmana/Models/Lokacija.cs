using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemZaRezervacijeApartmana.Models
{
    public class Lokacija
    {
        public double GeografskaSirina { get; set; }
        public double GeografskaDuzina { get; set; }
        public Adresa Adresa { get; set; }

        public Lokacija(double geoSirina,double geoDuzina,Adresa adresa)
        {
            GeografskaSirina = geoSirina;
            GeografskaDuzina = geoDuzina;
            Adresa = adresa;
        }

    }
}