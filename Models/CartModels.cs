using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoNgocHai_2122110473_ASP.NET.Models
{
    public class CartModels
    {
        public HangHoa HangHoa { get; set; }
        public order order { get; set; }
        public int Quantity { get; set; }
    }
}