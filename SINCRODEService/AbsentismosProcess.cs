using System;
using static SINCRODEService.Program;

namespace SINCRODEService
{
    public static class AbsentismosProcess
    {
        public static bool ExecuteAbsentismosProcess(DateTime fechaini, DateTime fechafin, bool AutoPro = true)
        {
            Log("Execute absentismos process");

            try
            {
                SincroDEService.ProcesaAusencias(fechaini, fechafin, AutoPro);
                return true;
            }
            catch (Exception ex)
            {
                Log(string.Format("Error executing absentismos process Message:{0} \nTrace:{1}", ex.Message, ex.StackTrace));
                return false;
            }
        }
    }
}
