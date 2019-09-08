using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemZaRezervacijeApartmana.Models
{
    public class SadrzajApartmana
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public bool IsDeleted { get; set; }

        public SadrzajApartmana()
        {
            IsDeleted = false;
        }

        public SadrzajApartmana(int id,string naziv)
        {
            Id = id;
            Naziv = naziv;
            IsDeleted = false;
        }
   
    }
}