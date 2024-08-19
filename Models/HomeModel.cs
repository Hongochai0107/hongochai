using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoNgocHai_2122110473_ASP.NET.Models
{
    public class HomeModel
    {
        public List<HangHoa> HangHoas { get; set; }
        public List<Loai> Loais { get; set; }
        public List<NhaCungCap> NhaCungCap { get; set; }
    }
}