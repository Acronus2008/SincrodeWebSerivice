namespace SINCRODEService
{
    class CustomField
    {
        public string EM_NUMPERSO { get; set; }
        public string EM_REDUCCION { get; set; }
        public string EM_TIPOCONTRATO { get; set; }
        public string EM_IDORACLE { get; set; }
        public string EM_NIEJERARQUIA { get; set; }
    }

    class PatternCalendar
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool Replace { get; set; }
    }

    class Employee
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string CodeArea { get; set; }
        public string CodeDepartment { get; set; }
        public string CodeCompany { get; set; }
        public string CodeSection { get; set; }
        public string DateAdd { get; set; }
        public string CodeAccess { get; set; }
        public string CodeCorrection { get; set; }
        public string CodeSchedule { get; set; }
        public CustomField CustomFields { get; set; }
        public string Observations { get; set; }
        public string CodeWorkflow { get; set; }
        public string CodeKiosk { get; set; }
        public string Email { get; set; }
        public string CodePatternCalendar { get; set; }
        public PatternCalendar PatternCalendarData { get; set; }
    }
}
