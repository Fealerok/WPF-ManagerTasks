using ManagerTasks.Windows;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ManagerTasks.Classes
{
    public class Database
    {
        private static Database _instance;
        public string Username;
        private string _connectionString = "Data Source=db.db";

        private Database()
        {
            InitializeDatabase();
        }

        public static Database GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Database();
            }

            return _instance;
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
                    TeamId INTEGER,
                    FOREIGN KEY(StatusId) REFERENCES Statuses(Id),
                    FOREIGN KEY(AssignedUserId) REFERENCES Users(Id),
                    FOREIGN KEY(ProjectId) REFERENCES Projects(Id),
                    FOREIGN KEY(TeamId) REFERENCES Teams(Id)
                );

                INSERT OR IGNORE INTO Roles (Id, Name) VALUES (1, 'Администратор');
                INSERT OR IGNORE INTO Roles (Id, Name) VALUES (2, 'Пользователь');
            ";
                command.ExecuteNonQuery();
            }
        }

        public string GetUserRole()
        {
            string role = "";
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT name FROM Roles JOIN Users ON Users.RoleId = Roles.id WHERE Users.username=@Username";
                command.Parameters.AddWithValue("@Username", Username);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        role = reader.GetString(0);
                    }
                }
            }

            return role;
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

        public void SetUsernameForDB(string username)
        {
            Username = username;
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


                        team.Users = GetTeamUsers(team.Id);
                        teams.Add(team);
                    }
                }
            }
            return teams;
        }


        public void AddUserToTeam(int teamId, int userId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();


                var checkTeamCommand = connection.CreateCommand();
                checkTeamCommand.CommandText = "SELECT COUNT(*) FROM Teams WHERE Id = @TeamId";
                checkTeamCommand.Parameters.AddWithValue("@TeamId", teamId);
                var teamExists = (long)checkTeamCommand.ExecuteScalar() > 0;


                var checkUserCommand = connection.CreateCommand();
                checkUserCommand.CommandText = "SELECT COUNT(*) FROM Users WHERE Id = @UserId";
                checkUserCommand.Parameters.AddWithValue("@UserId", userId);
                var userExists = (long)checkUserCommand.ExecuteScalar() > 0;

                if (!teamExists || !userExists)
                {
                    throw new InvalidOperationException("Team or User does not exist.");
                }


                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO TeamUsers (TeamId, UserId) VALUES (@TeamId, @UserId)";
                command.Parameters.AddWithValue("@TeamId", teamId);
                command.Parameters.AddWithValue("@UserId", userId);
                command.ExecuteNonQuery();
            }
        }

 
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

        public int GetIdLoggedUser(string username)
        {
            int id = 0;
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT id FROM Users WHERE username=@Username";
                command.Parameters.AddWithValue("@Username", username);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        id = reader.GetInt32(0);
                    }
                }
            }

            return id;
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
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
            INSERT INTO Tasks (Title, Description, DueDate, StatusId, AssignedUserId, ProjectId, TeamId) 
            VALUES (@Title, @Description, @DueDate, @StatusId, @AssignedUserId, @ProjectId, @TeamId)";
                command.Parameters.AddWithValue("@Title", task.Title);
                command.Parameters.AddWithValue("@Description", task.Description);
                command.Parameters.AddWithValue("@DueDate", task.DueDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@StatusId", task.StatusId);
                command.Parameters.AddWithValue("@AssignedUserId", task.AssignedUserId == 0 ? (object)DBNull.Value : task.AssignedUserId);
                command.Parameters.AddWithValue("@ProjectId", task.ProjectId == 0 ? (object)DBNull.Value : task.ProjectId);
                command.Parameters.AddWithValue("@TeamId", task.TeamId == 0 ? (object)DBNull.Value : task.TeamId);
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
                command.CommandText = @"
            SELECT Tasks.*, Statuses.Name as StatusName, Users.Username, Projects.Name as ProjectName, Teams.Name as TeamName 
            FROM Tasks 
            LEFT JOIN Statuses ON Tasks.StatusId = Statuses.Id 
            LEFT JOIN Users ON Tasks.AssignedUserId = Users.Id 
            LEFT JOIN Projects ON Tasks.ProjectId = Projects.Id
            LEFT JOIN Teams ON Tasks.TeamId = Teams.Id";
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
                            Status = new Status { Id = reader.GetInt32(4), Name = reader.GetString(8) },
                            AssignedUserId = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                            AssignedUser = reader.IsDBNull(5) ? null : new User { Username = reader.GetString(9) },
                            ProjectId = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                            Project = reader.IsDBNull(6) ? null : new Project { Name = reader.GetString(10) },
                            TeamId = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                            Team = reader.IsDBNull(11) ? null : new Team { Id = reader.GetInt32(7), Name = reader.GetString(11) }
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
                ProjectId = @ProjectId, 
                TeamId = @TeamId 
            WHERE Id = @Id";
                command.Parameters.AddWithValue("@Title", task.Title);
                command.Parameters.AddWithValue("@Description", task.Description);
                command.Parameters.AddWithValue("@DueDate", task.DueDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@StatusId", task.StatusId);
                command.Parameters.AddWithValue("@AssignedUserId", task.AssignedUserId == 0 ? (object)DBNull.Value : task.AssignedUserId);
                command.Parameters.AddWithValue("@ProjectId", task.ProjectId == 0 ? (object)DBNull.Value : task.ProjectId);
                command.Parameters.AddWithValue("@TeamId", task.TeamId == 0 ? (object)DBNull.Value : task.TeamId);
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
                    throw; 
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
                command.Parameters.AddWithValue("@Password", user.Password); 
                command.Parameters.AddWithValue("@RoleId", 2); 
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
                command.Parameters.AddWithValue("@Password", password); 
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

        public bool CheckUserData(string login, string email)
        {

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT username FROM Users WHERE username=@Username AND email=@Email";
                command.Parameters.AddWithValue("@Username", login);
                command.Parameters.AddWithValue("@Email", email); 
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void SetNewPassword(string password, string login)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"UPDATE Users SET password=@Password WHERE username=@Username";
                command.Parameters.AddWithValue("@Password", password);
                command.Parameters.AddWithValue("@Username", login);
                command.ExecuteNonQuery();
            }
        }

        public List<User> GetUsersWithDetails()
        {
            var users = new List<User>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
            SELECT Users.*, Roles.Name as RoleName, Teams.Name as TeamName 
            FROM Users 
            LEFT JOIN Roles ON Users.RoleId = Roles.Id 
            LEFT JOIN Teams ON Users.TeamId = Teams.Id";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Id = reader.GetInt32(0),
                            FullName = reader.GetString(1),
                            Email = reader.GetString(2),
                            Username = reader.GetString(3),
                            RoleId = reader.GetInt32(5),
                            Role = new Role { Id = reader.GetInt32(5), Name = reader.GetString(7) },
                            TeamId = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                            Team = reader.IsDBNull(6) ? null : new Team { Id = reader.GetInt32(6), Name = reader.GetString(8) }
                        });
                    }
                }
            }
            return users;
        }

        public List<Role> GetRoles()
        {
            var roles = new List<Role>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Roles";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        roles.Add(new Role
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }
            return roles;
        }

        public List<Team> GetTeamsForUserWindow()
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
                        teams.Add(new Team
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }
            return teams;
        }

        public void DeleteUser(int userId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // Удаляем связанные записи из TeamUsers (если есть)
                var deleteTeamUsersCommand = connection.CreateCommand();
                deleteTeamUsersCommand.CommandText = "DELETE FROM TeamUsers WHERE UserId = @UserId";
                deleteTeamUsersCommand.Parameters.AddWithValue("@UserId", userId);
                deleteTeamUsersCommand.ExecuteNonQuery();

                // Удаляем пользователя
                var deleteUserCommand = connection.CreateCommand();
                deleteUserCommand.CommandText = "DELETE FROM Users WHERE Id = @UserId";
                deleteUserCommand.Parameters.AddWithValue("@UserId", userId);
                deleteUserCommand.ExecuteNonQuery();
            }
        }

        public void UpdateUser(User user)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
            UPDATE Users 
            SET FullName = @FullName, 
                Email = @Email, 
                Username = @Username, 
                RoleId = @RoleId, 
                TeamId = @TeamId 
            WHERE Id = @Id";
                command.Parameters.AddWithValue("@FullName", user.FullName);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@RoleId", user.RoleId);
                command.Parameters.AddWithValue("@TeamId", user.TeamId == 0 ? (object)DBNull.Value : user.TeamId);
                command.Parameters.AddWithValue("@Id", user.Id);
                command.ExecuteNonQuery();
            }
        }

        public User GetCurrentUser()
        {
            User user = null;
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT * from Users WHERE Username=@Username";
                command.Parameters.AddWithValue("@Username", Username);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            
                            user = new User
                            {
                                Id = reader.GetInt32(0),
                                FullName = reader.GetString(1),
                                Email = reader.GetString(2),
                                Username = reader.GetString(3),
                                Password = reader.GetString(4),
                                RoleId = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                                TeamId = reader.IsDBNull(6) ? 0 : reader.GetInt32(6)
                            };
                        }
                        
                    }
                }
            }
            return user;
        }
    }
}
