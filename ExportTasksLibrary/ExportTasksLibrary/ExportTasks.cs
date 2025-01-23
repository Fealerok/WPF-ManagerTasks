
using System.Windows;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
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

        public bool ExportToJson(List<string> lines, string filePath)
        {
            List<TaskForJSON> tasksList = new List<TaskForJSON>();
            try
            {
                foreach (string line in lines) 
                {
                    List<string> lineData = line.Split(", ").ToList();
                    MessageBox.Show(lineData[4]);
                    tasksList.Add(new TaskForJSON()
                    {
                         Id = int.Parse(lineData[0].Split(": ")[1]),
                        Title = lineData[1].Split(": ")[1],
                        Description = lineData[2].Split(": ")[1],
                        DueDate = lineData[3].Split(": ")[1],
                        AssignedUser = lineData[5].Split(": ")[1],
                        Project = lineData[7].Split(": ")[1],
                        Team = lineData[6].Split(": ")[1],
                        Status = lineData[4].Split(": ")[1],
                    });

                }




                // Настройка сериализатора
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true, // Красивый формат с отступами
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Отключаем экранирование
                };
                //Сериализация в JSON
                string jsonString = JsonSerializer.Serialize(tasksList, options);

                File.WriteAllText(filePath, jsonString);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
                throw;
            }
        }

    }

    public class TaskForJSON
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string DueDate { get; set; }
        public string Status { get; set; }
        public string AssignedUser { get; set; }
        public string Project { get; set; }
        public string Team { get; set; }
    }
}