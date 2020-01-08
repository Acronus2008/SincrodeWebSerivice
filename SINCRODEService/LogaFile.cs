using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using ExcelDataReader;
using Microsoft.Extensions.Configuration;
using Renci.SshNet;

namespace SINCRODEService
{
    class LogaFile
    {
        private const string _logFileLocation = @"C:\temp\servicelog.txt";

        private static void Log(string logMessage)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_logFileLocation));
            File.AppendAllText(_logFileLocation, DateTime.Now.ToString() + " : " + logMessage + Environment.NewLine);
        }

        public static void DownloadFile(string servidorftp, string usuario, string password, string carpetaDestino)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            string remoteFile = config["FileLoga"];

            if (config["SFTP"].ToUpper() == "FALSE")
            {
                try
                {
                    FtpWebRequest reqFTP;
                    //reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(servidor + archivoOrigen));
                    //reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://gentalia.ddns.net/SincronizadorDE/DATOSLOGA_V2.xls"));
                    Uri uri = new Uri("ftp://" + servidorftp +"/"+ remoteFile);
                    Log("Ruta del fichero LOGA en el FTP: " + "ftp://" + servidorftp +"/"+ remoteFile);
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(uri);
                    reqFTP.Credentials = new NetworkCredential(usuario, password);
                    reqFTP.KeepAlive = false;
                    reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                    reqFTP.UseBinary = true;
                    reqFTP.Proxy = null;
                    reqFTP.UsePassive = true;
                    //Log("Camino del fpt " + reqFTP);
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();

                    Stream responseStream = response.GetResponseStream();

                    SaveStreamAsFile(carpetaDestino, responseStream, "DATOSLOGA_V2.xls");
                    Log("Fichero LOGA salvado en: " + carpetaDestino);
                    response.Close();
                }
                catch (WebException ex)
                {
                    //throw wEx;
                    Log("No se pudo acceder al FTP: " + "ftp://" + servidorftp + "/" + remoteFile);
                    Log(ex.Message);
                }
                catch (Exception ex)
                {
                    throw ex;

                }
            }
            else//Acceso a un SFTP
            {
                string fileName = remoteFile.Substring(remoteFile.LastIndexOf("/") + 1);
                int Port = Convert.ToInt32(config["FTPPort"]);
                string RemoteFileName = remoteFile;
                string LocalDestinationFilename = carpetaDestino + "/" + fileName;

                try
                { 
                    Log("Ruta de acceso al SFTP: " + servidorftp+ remoteFile);
                    //Log("Nombre del fichero " + fileName);
                    using (var sftp = new SftpClient(servidorftp, Port, usuario, password))
                    {
                        sftp.Connect();

                        using (var file = File.OpenWrite(LocalDestinationFilename))
                        {
                            sftp.DownloadFile(remoteFile, file);
                            Log("Fichero LOGA Salvado en " + file.Name);
                        }

                        sftp.Disconnect();
                    }
                }
                catch (Exception ex)
                {
                    Log("No se puso acceder al SFTP: "+ servidorftp + remoteFile);
                    Log(ex.Message);
                }
            }
        }

        public static void SaveStreamAsFile(string filePath, Stream inputStream, string fileName)
        {
            DirectoryInfo info = new DirectoryInfo(filePath);
            if (!info.Exists)
            {
                info.Create();
            }

            string path = Path.Combine(filePath, fileName);
            using (FileStream outputFileStream = new FileStream(path, FileMode.Create))
            {
                inputStream.CopyTo(outputFileStream);
            }
        }

        public class CamposLOGA
        {
            public string Nombre
            {
                set; get;
            }
            public string Apellidos
            {
                set; get;
            }
            public string NifDni
            {
                set; get;
            }
            public string NoPersonal
            {
                set; get;
            }
            public string IdOracle
            {
                set; get;
            }
            public string CodigoCentro
            {
                set; get;
            }
            public string UbicacionCentroTrabajo
            {
                set; get;
            }
            public string CodigoDepartamento
            {
                set; get;
            }
            public string DescripcionDepartamento
            {
                set; get;
            }
            public string PNRSupEmp
            {
                set; get;
            }
            public string NombreResponsable
            {
                set; get;
            }
            public string ApellidosResponsable
            {
                set; get;
            }
            public string CodigoNegocio
            {
                set; get;
            }
            public string CodigoSociedad
            {
                set; get;
            }
            public string CodigoSubNegocio
            {
                set; get;
            }
            public string DescripcionCentroTrabajo
            {
                set; get;
            }
            public string DescripcionNegocio
            {
                set; get;
            }
            public string DescripcionSociedad
            {
                set; get;
            }
            public string DescripcionSubNegocio
            {
                set; get;
            }
            public double? PorcentajeReduccionJornada
            {
                set; get;
            }
            public int? JornadaLaboralFestiva
            {
                set; get;
            }
            public int? JornadaLaboralLunes
            {
                set; get;
            }
            public int? JornadaLaboralMartes
            {
                set; get;
            }
            public int? JornadaLaboralMiercoles
            {
                set; get;
            }
            public int? JornadaLaboralJueves
            {
                set; get;
            }
            public int? JornadaLaboralViernes
            {
                set; get;
            }
            public int? JornadaLaboralSabado
            {
                set; get;
            }
            public int? JornadaLaboralDomingo
            {
                set; get;
            }
            public int? TipoContrato
            {
                set; get;
            }
            public string Ad
            {
                set; get;
            }
            public int? CodContratoEmp
            {
                set; get;
            }
            public int? CoJornadaEmp
            {
                set; get;
            }
        }

        public static IExcelDataReader getExcelReader(string filePath)
        {

            // ExcelDataReader works with the binary Excel file, so it needs a FileStream
            // to get started. This is how we avoid dependencies on ACE or Interop:
            FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            // We return the interface, so that
            IExcelDataReader reader = null;
            try
            {
                if (filePath.EndsWith(".xls"))
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    var enc1252 = Encoding.GetEncoding(1252);
                    var stream  = new StreamReader(fileStream, enc1252).BaseStream;
                    
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                if (filePath.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
                }
                return reader;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<CamposLOGA> ProcessWorkbook(string filePath)
        {
            List<CamposLOGA> listaLOGA = new List<CamposLOGA>();

            var reader = getExcelReader(filePath);
            var workSheet = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true
                }
            }).Tables[0];

            try
            {
                for (var i = 0; i < workSheet.Rows.Count; i++)
                {
                    var campos = new CamposLOGA();
                    campos.Nombre = workSheet.Rows[i][0].ToString();
                    campos.Apellidos = workSheet.Rows[i][1].ToString();
                    campos.NifDni = workSheet.Rows[i][2].ToString();
                    campos.NoPersonal = workSheet.Rows[i][3].ToString();
                    campos.IdOracle = workSheet.Rows[i][4].ToString();
                    campos.PNRSupEmp = workSheet.Rows[i][6].ToString();
                    campos.NombreResponsable = workSheet.Rows[i][7].ToString();
                    campos.ApellidosResponsable = workSheet.Rows[i][8].ToString();
                    campos.CodigoCentro = workSheet.Rows[i][9].ToString();
                    campos.UbicacionCentroTrabajo = workSheet.Rows[i][10].ToString();
                    campos.DescripcionCentroTrabajo = workSheet.Rows[i][11].ToString();
                    campos.CodigoDepartamento = workSheet.Rows[i][12].ToString();
                    campos.DescripcionDepartamento = workSheet.Rows[i][13].ToString();
                    campos.CodigoSociedad = workSheet.Rows[i][14].ToString();
                    campos.DescripcionSociedad = workSheet.Rows[i][15].ToString();
                    campos.CodigoNegocio = workSheet.Rows[i][16].ToString();
                    campos.DescripcionNegocio = workSheet.Rows[i][17].ToString();
                    campos.CodigoSubNegocio = workSheet.Rows[i][18].ToString();
                    campos.DescripcionSubNegocio = workSheet.Rows[i][19].ToString();
                    campos.PorcentajeReduccionJornada = workSheet.Rows[i][20] == null || Convert.IsDBNull(workSheet.Rows[i][20]) ? (double?)null : Convert.ToDouble(workSheet.Rows[i][20]);
                    campos.JornadaLaboralFestiva = workSheet.Rows[i][21]==null || Convert.IsDBNull(workSheet.Rows[i][21]) ? (int?)null:Convert.ToInt32(workSheet.Rows[i][21]) ;
                    campos.JornadaLaboralLunes =  workSheet.Rows[i][22]==null || Convert.IsDBNull(workSheet.Rows[i][22]) ? (int?)null : Convert.ToInt32(workSheet.Rows[i][22]);
                    campos.JornadaLaboralMartes =  workSheet.Rows[i][23]==null || Convert.IsDBNull(workSheet.Rows[i][23]) ? (int?)null : Convert.ToInt32(workSheet.Rows[i][23]);
                    campos.JornadaLaboralMiercoles =  workSheet.Rows[i][24]==null || Convert.IsDBNull(workSheet.Rows[i][24]) ? (int?)null : Convert.ToInt32(workSheet.Rows[i][24]);
                    campos.JornadaLaboralJueves =  workSheet.Rows[i][25]==null || Convert.IsDBNull(workSheet.Rows[i][25]) ? (int?)null : Convert.ToInt32(workSheet.Rows[i][25]);
                    campos.JornadaLaboralViernes =  workSheet.Rows[i][26]==null || Convert.IsDBNull(workSheet.Rows[i][26]) ? (int?)null : Convert.ToInt32(workSheet.Rows[i][26]);
                    campos.JornadaLaboralSabado = workSheet.Rows[i][27]==null || Convert.IsDBNull(workSheet.Rows[i][27]) ? (int?)null : Convert.ToInt32(workSheet.Rows[i][27]);
                    campos.JornadaLaboralDomingo = workSheet.Rows[i][28]==null || Convert.IsDBNull(workSheet.Rows[i][28]) ? (int?)null : Convert.ToInt32(workSheet.Rows[i][28]);
                    campos.Ad = workSheet.Rows[i][29].ToString();
                    campos.TipoContrato = workSheet.Rows[i][30] == null || Convert.IsDBNull(workSheet.Rows[i][30]) ? (int?)null : Convert.ToInt32(workSheet.Rows[i][30]);
                    //No se llenan estos dos campo porque en el LOGA tiene cadena y Lazaro me dijo q los ignoraramos de momento. 
                    //campos.CodContratoEmp = workSheet.Rows[i][31] == null || Convert.IsDBNull(workSheet.Rows[i][31]) ? (int?)null : Convert.ToInt32(workSheet.Rows[i][31]);
                    //campos.CoJornadaEmp = workSheet.Rows[i][32] == null || Convert.IsDBNull(workSheet.Rows[i][32]) ? (int?)null : Convert.ToInt32(workSheet.Rows[i][32]);
                    listaLOGA.Add(campos);
                }
            }
            catch (Exception ex)
            {
                Log("Excepción en el ProcessWorkbook " + ex.ToString());
            }
            return listaLOGA;
        }
    }
}
