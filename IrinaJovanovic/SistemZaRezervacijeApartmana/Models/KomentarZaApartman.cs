using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemZaRezervacijeApartmana.Models
{
    public class KomentarZaApartman
    {
        public int Id { get; set; }
        public Gost Gost { get; set; }
        public Apartman Apartman { get; set; }
        public string Tekst { get; set; }
        public OcenaApartmana OcenaApartmana { get; set; }
        public bool Odobren { get; set; }

        public KomentarZaApartman()
        {
            OcenaApartmana = OcenaApartmana.NULA;
            Odobren = false;
        }

        public KomentarZaApartman(int id,Gost gost,Apartman apartman,string tekst,OcenaApartmana ocena)
        {
            Id = id;
            Gost = gost;
            Apartman = apartman;
            Tekst = tekst;
            OcenaApartmana = ocena;
            Odobren = false;
        }
    }
}