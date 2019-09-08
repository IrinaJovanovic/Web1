using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemZaRezervacijeApartmana.Models
{
    public class Image
    {
        public string Name { get; set; }
        public string LocalPath { get; set; }

        public Image(string name,string localPath)
        {
            Name = name;
            LocalPath = localPath;
        }
    }
}