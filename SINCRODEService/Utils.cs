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

        public static DateTime StrToDateTime(string fecha, bool hora00)
        {
            try
            {

            }
            catch (Exception ex)
            {
                return new DateTime();
            }
            return new DateTime(
                Convert.ToInt32(fecha.Substring(0, 4)),
                Convert.ToInt32(fecha.Substring(4, 2)),
                Convert.ToInt32(fecha.Substring(6, 2)),
                hora00 == true? 0 : 23,
                hora00 == true ? 0 : 59,
                hora00 == true ? 0 : 59);
        }
        public static bool Between(int numero, int lower, int upper, bool inclusive = true)
        {
            return inclusive
                ? lower <= numero && numero <= upper
                : lower < numero && numero < upper;

        }
    }
}
