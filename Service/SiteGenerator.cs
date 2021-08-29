using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;

namespace Service
{
    public static class SiteGenerator
    {
        public static string GenerateWebsite()
        {
            try
            {
                return Resources.PHPTemplate;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex}");
                throw;
            }
        }

        public static string GenerateNavbar()
        {
            throw new NotImplementedException();
        }

        public static string GenerateExternalCSS()
        {
            throw new NotImplementedException();
        }
    }
}
