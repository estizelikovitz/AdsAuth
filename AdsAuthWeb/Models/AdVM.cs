using AdsAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdsAuthWeb.Models
{
    public class AdVM
    {
        public List<Ad> Ads;
        public User CurrentUser;
        public bool IsAuthenticated;
    }
}
