
using System.Windows;
using System.IO;
namespace ExportTasksLibrary
{
    public class ExportTasks
    {

        public bool ExportToTxt(List<string> lines, string filePath)
        {
            try
            {
                File.WriteAllLines(filePath, lines);
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public bool ExportToJson(string jsonString, string filePath)
        {
            try
            {
                File.WriteAllText(filePath, jsonString);
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

    }
}