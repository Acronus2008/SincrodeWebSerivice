using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SINCRODEService.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SINCRODEWebApp.DataBaseService
{
    public class DataBaseService
    {
        private DbContextOptionsBuilder<SINCRODEDBContext> _dbContextOptions;
        private SINCRODEDBContext _context;

        public DataBaseService()
        {
            BuildContext();
            this._context = new SINCRODEDBContext(this._dbContextOptions.Options);
        }

        public IQueryable<TblProcesoslog> QueryTblProcesoslog()
        {
            return this._context.TblProcesoslog.Where(m => m.IdPro > 0);
        }

        public TblEmpleados GetTblEmpleadosByEmpleadoId(int IdEmpleado)
        {
            return this._context.TblEmpleados.Where(m => m.IdEmp == IdEmpleado).FirstOrDefault();
        }

        public IQueryable<TblProcesos> QueryTblProcesos()
        {
            return this._context.TblProcesos.OrderBy(m => m.FechaIniPro);
        }

        private void BuildContext()
        {
            this._dbContextOptions = new DbContextOptionsBuilder<SINCRODEDBContext>();
            this._dbContextOptions.UseSqlServer(getDataConnectionString());
        }

        private string getDataConnectionString()
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            return config["DataBaseLogs"].ToString();
        }
    }
}
