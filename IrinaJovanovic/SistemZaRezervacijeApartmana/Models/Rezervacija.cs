using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemZaRezervacijeApartmana.Models
{
    public class Rezervacija
    {
        public Apartman Apartman { get; set; }
        public DateTime PocetniDatumRezervacije { get; set; }
        public int BrojNocenja { get; set; }
        public double UkupnaCena { get; set; }
        public Gost Gost { get; set; }
        public StatusRezervacije Status { get; set; }
    }
}