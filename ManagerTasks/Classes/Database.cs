using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ManagerTasks.Classes
{
    public class Database
    {
        private string _connectionString = "Data Source=db.db";

        public Database()
        {
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                @"
                CREATE TABLE IF NOT EXISTS Teams (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS TeamUsers (
                    TeamId INTEGER,
                    UserId INTEGER,
                    PRIMARY KEY (TeamId, UserId),
                    FOREIGN KEY (TeamId) REFERENCES Teams(Id) ON DELETE CASCADE,
                    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
                );

                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    FullName TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    Username TEXT NOT NULL,
                    Password TEXT NOT NULL,
                    RoleId INTEGER,
                    TeamId INTEGER,
                    FOREIGN KEY(RoleId) REFERENCES Roles(Id),
                    FOREIGN KEY(TeamId) REFERENCES Teams(Id)
                );

                CREATE TABLE IF NOT EXISTS Roles (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Projects (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Statuses (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL
                );

                
                INSERT OR IGNORE INTO Statuses (Id, Name) VALUES (1, 'В работе');
                INSERT OR IGNORE INTO Statuses (Id, Name) VALUES (2, 'Выполнено');
                INSERT OR IGNORE INTO Statuses (Id, Name) VALUES (3, 'Отложено');

                CREATE TABLE IF NOT EXISTS Tasks (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Description TEXT,
                    DueDate TEXT,
                    StatusId INTEGER, -- Ссылка на таблицу Statuses
                    AssignedUserId INTEGER,
                    ProjectId INTEGER,
                    FOREIGN KEY(StatusId) REFERENCES Statuses(Id),
                    FOREIGN KEY(AssignedUserId) REFERENCES Users(Id),
                    FOREIGN KEY(ProjectId) REFERENCES Projects(Id)
                );

                INSERT OR IGNORE INTO Roles (Id, Name) VALUES (1, 'Администратор');
                INSERT OR IGNORE INTO Roles (Id, Name) VALUES (2, 'Пользователь');
            ";
                command.ExecuteNonQuery();
            }
        }


        public List<User> GetUsers()
        {
            var users = new List<User>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Users";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Id = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            Email = reader.GetString(2),
                            TeamId = reader.GetInt32(3)
                        });
                    }
                }
            }
            return users;
        }

        public void AddTeam(Team team)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Teams (Name) VALUES (@Name)";
                command.Parameters.AddWithValue("@Name", team.Name);
                command.ExecuteNonQuery();
            }
        }

        public void UpdateTeam(Team team)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
            UPDATE Teams 
            SET Name = @Name
            WHERE Id = @Id";
                command.Parameters.AddWithValue("@Name", team.Name);
                command.Parameters.AddWithValue("@Id", team.Id);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteTeam(int teamId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Teams WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", teamId);
                command.ExecuteNonQuery();
            }
        }

        public List<Team> GetTeams()
        {
            var teams = new List<Team>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Teams";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var team = new Team
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        };

                        // Загрузка пользователей команды
                        team.Users = GetTeamUsers(team.Id);
                        teams.Add(team);
                    }
                }
            }
            return teams;
        }

        // Добавление пользователя в команду
        public void AddUserToTeam(int teamId, int userId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // Проверяем, существует ли команда
                var checkTeamCommand = connection.CreateCommand();
                checkTeamCommand.CommandText = "SELECT COUNT(*) FROM Teams WHERE Id = @TeamId";
                checkTeamCommand.Parameters.AddWithValue("@TeamId", teamId);
                var teamExists = (long)checkTeamCommand.ExecuteScalar() > 0;

                // Проверяем, существует ли пользователь
                var checkUserCommand = connection.CreateCommand();
                checkUserCommand.CommandText = "SELECT COUNT(*) FROM Users WHERE Id = @UserId";
                checkUserCommand.Parameters.AddWithValue("@UserId", userId);
                var userExists = (long)checkUserCommand.ExecuteScalar() > 0;

                if (!teamExists || !userExists)
                {
                    throw new InvalidOperationException("Team or User does not exist.");
                }

                // Добавляем пользователя в команду
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO TeamUsers (TeamId, UserId) VALUES (@TeamId, @UserId)";
                command.Parameters.AddWithValue("@TeamId", teamId);
                command.Parameters.AddWithValue("@UserId", userId);
                command.ExecuteNonQuery();
            }
        }

        // Удаление пользователя из команды
        public void RemoveUserFromTeam(int teamId, int userId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM TeamUsers WHERE TeamId = @TeamId AND UserId = @UserId";
                command.Parameters.AddWithValue("@TeamId", teamId);
                command.Parameters.AddWithValue("@UserId", userId);
                command.ExecuteNonQuery();
            }
        }

        // Получение пользователей команды
        public List<User> GetTeamUsers(int teamId)
        {
            var users = new List<User>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
            SELECT Users.* 
            FROM Users 
            JOIN TeamUsers ON Users.Id = TeamUsers.UserId 
            WHERE TeamUsers.TeamId = @TeamId";
                command.Parameters.AddWithValue("@TeamId", teamId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Id = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            Email = reader.GetString(2),
                            TeamId = reader.GetInt32(3)
                        });
                    }
                }
            }
            return users;
        }

        public void AddUser(User user)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Users (Username, Email, TeamId) VALUES (@Username, @Email, @TeamId)";
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@TeamId", user.TeamId);
                command.ExecuteNonQuery();
            }
        }

        // Остальные методы для Projects и Tasks остаются без изменений

        public List<Project> GetProjects()
        {
            var projects = new List<Project>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Projects";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        projects.Add(new Project
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }
            return projects;
        }


        public List<Status> GetStatuses()
        {
            var statuses = new List<Status>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Statuses";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        statuses.Add(new Status
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }
            return statuses;
        }

        public void AddTask(Task task)
        {
            MessageBox.Show(task.ProjectId.ToString());
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Tasks (Title, Description, DueDate, StatusId, AssignedUserId, ProjectId) VALUES (@Title, @Description, @DueDate, @StatusId, @AssignedUserId, @ProjectId)";
                command.Parameters.AddWithValue("@Title", task.Title);
                command.Parameters.AddWithValue("@Description", task.Description);
                command.Parameters.AddWithValue("@DueDate", task.DueDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@StatusId", task.StatusId);
                command.Parameters.AddWithValue("@AssignedUserId", task.AssignedUserId);
                command.Parameters.AddWithValue("@ProjectId", task.ProjectId == 0 ? (object)DBNull.Value : task.ProjectId);
                command.ExecuteNonQuery();
            }
        }

        public List<Task> GetTasks()
        {
            var tasks = new List<Task>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Tasks.*, Statuses.Name as StatusName, Users.Username, Projects.Name as ProjectName FROM Tasks LEFT JOIN Statuses ON Tasks.StatusId = Statuses.Id LEFT JOIN Users ON Tasks.AssignedUserId = Users.Id LEFT JOIN Projects ON Tasks.ProjectId = Projects.Id";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tasks.Add(new Task
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Description = reader.GetString(2),
                            DueDate = DateTime.Parse(reader.GetString(3)),
                            StatusId = reader.GetInt32(4),
                            Status = new Status { Id = reader.GetInt32(4), Name = reader.GetString(7) },
                            AssignedUserId = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                            AssignedUser = reader.IsDBNull(5) ? null : new User { Username = reader.GetString(8) },
                            ProjectId = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                            Project = reader.IsDBNull(6) ? null : new Project { Name = reader.GetString(9) }
                        });
                    }
                }
            }
            return tasks;
        }

        public void UpdateTask(Task task)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
            UPDATE Tasks 
            SET Title = @Title, 
                Description = @Description, 
                DueDate = @DueDate, 
                StatusId = @StatusId, 
                AssignedUserId = @AssignedUserId, 
                ProjectId = @ProjectId 
            WHERE Id = @Id";
                command.Parameters.AddWithValue("@Title", task.Title);
                command.Parameters.AddWithValue("@Description", task.Description);
                command.Parameters.AddWithValue("@DueDate", task.DueDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@StatusId", task.StatusId);
                command.Parameters.AddWithValue("@AssignedUserId", task.AssignedUserId == 0 ? (object)DBNull.Value : task.AssignedUserId);
                command.Parameters.AddWithValue("@ProjectId", task.ProjectId == 0 ? (object)DBNull.Value : task.ProjectId);
                command.Parameters.AddWithValue("@Id", task.Id);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteTask(int taskId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Tasks WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", taskId);
                command.ExecuteNonQuery();
            }
        }

        public void AddProject(Project project)
        {
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project), "Project cannot be null.");
            }

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Projects (Name) VALUES (@Name)";
                command.Parameters.AddWithValue("@Name", project.Name);
                command.ExecuteNonQuery();
            }
        }

        public void UpdateProject(Project project)
        {
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project), "Project cannot be null.");
            }

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
            UPDATE Projects 
            SET Name = @Name
            WHERE Id = @Id";

                // Логирование параметров
                Console.WriteLine($"Executing SQL: {command.CommandText}");
                Console.WriteLine($"Parameters: Name={project.Name}, Id={project.Id}");

                command.Parameters.AddWithValue("@Name", project.Name);
                command.Parameters.AddWithValue("@Id", project.Id);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqliteException ex)
                {
                    Console.WriteLine($"SQLite Error: {ex.Message}");
                    throw; // Повторно выбрасываем исключение для дальнейшей диагностики
                }
            }
        }

        public void DeleteProject(int projectId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Projects WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", projectId);
                command.ExecuteNonQuery();
            }
        }




        public bool RegisterUser(User user)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Users (FullName, Email, Username, Password, RoleId, TeamId) VALUES (@FullName, @Email, @Username, @Password, @RoleId, @TeamId)";
                command.Parameters.AddWithValue("@FullName", user.FullName);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Password", user.Password); // В реальном приложении пароль должен быть хэширован
                command.Parameters.AddWithValue("@RoleId", 2); // По умолчанию присваиваем роль "Пользователь"
                command.Parameters.AddWithValue("@TeamId", user.TeamId == 0 ? (object)DBNull.Value : user.TeamId);
                command.ExecuteNonQuery();
                return true;
            }
        }

        public User LoginUser(string username, string password)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Users.*, Roles.Name as RoleName, Teams.Name as TeamName FROM Users JOIN Roles ON Users.RoleId = Roles.Id LEFT JOIN Teams ON Users.TeamId = Teams.Id WHERE Username = @Username AND Password = @Password";
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password); // В реальном приложении пароль должен быть хэширован
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = reader.GetInt32(0),
                            FullName = reader.GetString(1),
                            Email = reader.GetString(2),
                            Username = reader.GetString(3),
                            Password = reader.GetString(4),
                            RoleId = reader.GetInt32(5),
                            Role = new Role { Id = reader.GetInt32(5), Name = reader.GetString(7) },
                            TeamId = reader.IsDBNull(6) ? 0 : reader.GetInt32(6), // TeamId
                            Team = reader.IsDBNull(6) ? null : new Team { Id = reader.GetInt32(6), Name = reader.GetString(8) }
                        };
                    }
                }
            }
            return null;
        }
    }
}
