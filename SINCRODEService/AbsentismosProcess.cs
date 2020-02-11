using System;
using System.Collections.Generic;
using System.Text;

namespace SINCRODEService
{
    public static class AbsentismosProcess
    {
        public static bool ExecuteAbsentismosProcess(DateTime fechaini, DateTime fechafin, bool AutoPro = true)
        {
            try
            {
                SincroDEService.ProcesaAusencias(fechaini, fechafin, AutoPro);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
