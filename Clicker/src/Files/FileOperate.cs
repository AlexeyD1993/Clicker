using Clicker.src.Params;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Clicker.src.Files
{
    class FileOperate
    {
        private static string fileNameZadan = @"params.xml";
        public static void WriteAllParams(List<SeleniumParams> paramList, string fileName)
        {
            DeleteZadanFile();

            XmlSerializer ser = new XmlSerializer(typeof(List<SeleniumParams>));

            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                ser.Serialize(fs, paramList);
            }
        }

        public static bool FileZadanExist(string fileName)
        {
            return File.Exists(fileName);
        }

        public static List<SeleniumParams> ReadAllParams(string fileName)
        {
            List<SeleniumParams> seleniumParams = null;
            try
            {
                if (FileZadanExist(fileName))
                {

                    var serializer = new XmlSerializer(typeof(List<SeleniumParams>));

                    using (FileStream fs = new FileStream(fileName, FileMode.Open))
                    {
                        seleniumParams = (List<SeleniumParams>)serializer.Deserialize(fs);
                    }

                    return seleniumParams;
                }
                else
                    return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void DeleteZadanFile()
        {
            try
            {
                File.Delete(fileNameZadan);
            }
            catch
            {
            }
        }
    }
}
