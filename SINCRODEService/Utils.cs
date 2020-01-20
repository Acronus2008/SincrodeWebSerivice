using System;

namespace SINCRODEService
{
    public static class Utils
    {
        public static DateTime StrToDateTime(string fecha)
        {
            return new DateTime(
                Convert.ToInt32(fecha.Substring(0, 4)),
                Convert.ToInt32(fecha.Substring(4, 2)),
                Convert.ToInt32(fecha.Substring(6, 2)),
                Convert.ToInt32(fecha.Substring(8, 2)),
                Convert.ToInt32(fecha.Substring(10, 2)),
                Convert.ToInt32(fecha.Substring(12, 2)));
        }

        public static DateTime StrToDateTime(string fecha, string hora, bool hora00)
        {
            try
            {
                return new DateTime(
                    Convert.ToInt32(fecha.Substring(0, 4)),
                    Convert.ToInt32(fecha.Substring(4, 2)),
                    Convert.ToInt32(fecha.Substring(6, 2)),
                    hora00 == true ? 0 : Convert.ToInt32(hora.Substring(0, 2)),
                    hora00 == true ? 0 : Convert.ToInt32(hora.Substring(2, 2)),
                    hora00 == true ? 0 : Convert.ToInt32(hora.Substring(4, 2)));
            }
            catch (Exception)
            {
                return new DateTime();
            }

        }
        public static bool Between(int numero, int lower, int upper, bool inclusive = true)
        {
            return inclusive
                ? lower <= numero && numero <= upper
                : lower < numero && numero < upper;

        }
    }
}
