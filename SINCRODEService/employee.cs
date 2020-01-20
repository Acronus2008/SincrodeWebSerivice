namespace SINCRODEService
{
    class CustomField
    {
        public string EM_NUMPERSO { get; set; }
        public string EM_REDUCCION { get; set; }
        public string EM_TIPOCONTRATO { get; set; }
        public string EM_IDORACLE { get; set; }
    }

    class Employee
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string CodeArea { get; set; }
        public string CodeDepartment { get; set; }
        public string CodeCompany { get; set; }
        public string CodeSection { get; set; }
        public string CodeSchedule { get; set; }
        public CustomField CustomFields { get; set; }
}
}
