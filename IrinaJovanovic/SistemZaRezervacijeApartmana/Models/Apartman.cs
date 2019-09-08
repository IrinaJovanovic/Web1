using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemZaRezervacijeApartmana.Models
{
    public class Apartman
    {
        public int Id { get; set; }
        public TipApartmana TipApartman { get; set; }
        public int BrojSoba { get; set; }
        public int BrojGostiju { get; set; }
        public Lokacija Lokacija { get; set; }
        public List<DateTime> DatumiZaIzdavanje { get; set; }
        public List<DateTime> DostupnostPoDatumima { get; set; }
        public Domacin Domacin { get; set; }
        public List<KomentarZaApartman> Komentari { get; set; }
        public List<Image> Slike { get; set; }
        public double CenaPoNoci { get; set; }
        public string VremeZaPrijavu { get; set; }
        public string VremeZaOdjavu { get; set; }
        public StatusApartmana Status { get; set; }
        public List<SadrzajApartmana> SadrzajiApartmana { get; set; }
        public List<Rezervacija> Rezervacije { get; set; }
        public bool IsDeleted { get; set; }

        public Apartman()
        {
            Status = StatusApartmana.NEAKTIVAN;
            VremeZaPrijavu = "2:00";
            VremeZaOdjavu="10:00";
            DatumiZaIzdavanje = new List<DateTime>();
            DostupnostPoDatumima = new List<DateTime>();
            Komentari = new List<KomentarZaApartman>();
            Slike = new List<Image>();
            SadrzajiApartmana = new List<SadrzajApartmana>();
            Rezervacije = new List<Rezervacija>();
            IsDeleted = false;
        }

        public Apartman(int id,TipApartmana tip,int brojSoba,int brojGostiju,Lokacija lok,Domacin domacin,double cenaPoNoci,StatusApartmana status,string vremePrijava,string vremeOdjava)
        {
            Id = id;
            TipApartman = tip;
            BrojSoba = brojSoba;
            BrojGostiju = brojGostiju;
            Lokacija = lok;
            Domacin = domacin;
            CenaPoNoci = cenaPoNoci;
            Status = status;
            VremeZaPrijavu = vremePrijava;
            VremeZaOdjavu = vremeOdjava;
            DatumiZaIzdavanje = new List<DateTime>();
            DostupnostPoDatumima = new List<DateTime>();
            Komentari = new List<KomentarZaApartman>();
            Slike = new List<Image>();
            SadrzajiApartmana = new List<SadrzajApartmana>();
            Rezervacije = new List<Rezervacija>();
            IsDeleted = false;
        }

    }
}