using Npgsql;
using JiraCopyProject_My.Models;

namespace JiraCopyProject_My.Services
{
    public class DatabaseService
    {
        private string connectionString = "Host=46.191.235.28;Port=5432;Database=JiraCopy_Bakieva;Username=postgres;Password=Asdf=1234Asdf=1234";

        public List<Account> GetAccounts()
        {
            var accounts = new List<Account>();

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT id, login, email, fullname, position, role, created_at, is_active FROM \"Accounts\"";
                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        accounts.Add(new Account
                        {
                            Id = reader.GetInt32(0),
                            Login = reader.GetString(1),
                            Email = reader.GetString(2),
                            FullName = reader.GetString(3),
                            Position = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Role = reader.IsDBNull(5) ? null : reader.GetString(5),
                            CreatedAt = reader.GetDateTime(6),
                            IsActive = reader.GetBoolean(7)
                        });
                    }
                }
            }
            return accounts;
        }

        public List<Models.Task> GetTasks()
        {
            var tasks = new List<Models.Task>();

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT id, title, description, status_id, create_date, due_date, team_id, assignee_id, creator_id, parent_task_id, created_at, updated_at FROM \"Tasks\"";
                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tasks.Add(new Models.Task
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                            StatusId = reader.GetInt32(3),
                            CreateDate = reader.GetDateTime(4),
                            DueDate = reader.GetDateTime(5),
                            TeamId = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                            AssigneeId = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                            CreatorId = reader.GetInt32(8),
                            ParentTaskId = reader.IsDBNull(9) ? null : reader.GetInt32(9),
                            CreatedAt = reader.GetDateTime(10),
                            UpdatedAt = reader.GetDateTime(11)
                        });
                    }
                }
            }
            return tasks;
        }
        public List<Team> GetTeams()
        {
            var teams = new List<Team>();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT id, name, description, team_lead_id, created_at FROM \"Teams\"";
                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        teams.Add(new Team
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                            TeamLeadId = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                            CreatedAt = reader.GetDateTime(4)
                        });
                    }
                }
            }
            return teams;
        }

        public List<AccountStat> GetAccountStatistics()
        {
            Console.WriteLine();
            var stats = new List<AccountStat>();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"
            SELECT 
                a.id, 
                a.fullname, 
                a.position, 
                a.role,
                COALESCE(created.cnt, 0) as tasks_created,
                COALESCE(assigned.cnt, 0) as tasks_assigned
            FROM ""Accounts"" a
            LEFT JOIN (SELECT creator_id, COUNT(*) as cnt FROM ""Tasks"" GROUP BY creator_id) created ON created.creator_id = a.id
            LEFT JOIN (SELECT assignee_id, COUNT(*) as cnt FROM ""Tasks"" GROUP BY assignee_id) assigned ON assigned.assignee_id = a.id
            ORDER BY a.id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stats.Add(new AccountStat
                        {
                            Id = reader.GetInt32(0),
                            FullName = reader.GetString(1),
                            Position = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Role = reader.IsDBNull(3) ? null : reader.GetString(3),
                            TasksCreatedCount = reader.GetInt32(4),
                            TasksAssignedCount = reader.GetInt32(5)
                        });
                    }
                }
            }
            return stats;
        }
        public void AddTask(string title, string description, int statusId, int assigneeId, int creatorId, DateTime dueDate, int? parentTaskId = null)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CALL \"InsertTask\"(@p_title, @p_description, @p_status_id, @p_assignee_id, @p_creator_id, @p_date, @p_parent_task_id)", conn))
                {
                    cmd.Parameters.AddWithValue("@p_title", title);
                    cmd.Parameters.AddWithValue("@p_description", description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@p_status_id", statusId);
                    cmd.Parameters.AddWithValue("@p_assignee_id", assigneeId);
                    cmd.Parameters.AddWithValue("@p_creator_id", creatorId);
                    cmd.Parameters.Add(new NpgsqlParameter("@p_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = dueDate.Date });
                    cmd.Parameters.AddWithValue("@p_parent_task_id", parentTaskId ?? (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        //public void CreateSubTask(int parentTaskId, string title, string description, int assigneeId, int creatorId, DateTime dueDate, int statusId = 5)
        //{
        //    using (var conn = new NpgsqlConnection(connectionString))
        //    {
        //        conn.Open();
        //        using (var cmd = new NpgsqlCommand("CALL \"CreateSubTask\"(@p_parent_task_id, @p_title, @p_description, @p_assignee_id, @p_creator_id, @p_due_date, @p_status)", conn))
        //        {
        //            cmd.Parameters.AddWithValue("@p_parent_task_id", parentTaskId);
        //            cmd.Parameters.AddWithValue("@p_title", title);
        //            cmd.Parameters.AddWithValue("@p_description", description ?? (object)DBNull.Value);
        //            cmd.Parameters.AddWithValue("@p_assignee_id", assigneeId);
        //            cmd.Parameters.AddWithValue("@p_creator_id", creatorId);
        //            cmd.Parameters.Add(new NpgsqlParameter("@p_due_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = dueDate.Date });
        //            cmd.Parameters.AddWithValue("@p_status", statusId);
        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //}
        // Создание подзадачи
        public void CreateSubTask(int parentTaskId, string title, string description, int assigneeId, int creatorId, DateTime dueDate, int statusId = 5)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CALL \"CreateSubTask\"(@p_parent_task_id, @p_title, @p_description, @p_assignee_id, @p_creator_id, @p_due_date, @p_status)", conn))
                {
                    cmd.Parameters.AddWithValue("@p_parent_task_id", parentTaskId);
                    cmd.Parameters.AddWithValue("@p_title", title);
                    cmd.Parameters.AddWithValue("@p_description", description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@p_assignee_id", assigneeId);
                    cmd.Parameters.AddWithValue("@p_creator_id", creatorId);
                    cmd.Parameters.Add(new NpgsqlParameter("@p_due_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = dueDate.Date });
                    cmd.Parameters.AddWithValue("@p_status", statusId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Добавление комментария
        public void AddTaskComment(int taskId, int authorId, string comment)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CALL \"AddTaskComment\"(@p_task_id, @p_author_id, @p_comment)", conn))
                {
                    cmd.Parameters.AddWithValue("@p_task_id", taskId);
                    cmd.Parameters.AddWithValue("@p_author_id", authorId);
                    cmd.Parameters.AddWithValue("@p_comment", comment);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Получение комментариев
        public List<(int Id, string AuthorName, string Comment, DateTime CreatedAt)> GetTaskComments(int taskId)
        {
            var result = new List<(int, string, string, DateTime)>();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM \"GetTaskComments\"(@p_task_id)", conn))
                {
                    cmd.Parameters.AddWithValue("@p_task_id", taskId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add((
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2),
                                reader.GetDateTime(3)
                            ));
                        }
                    }
                }
            }
            return result;
        }

        // Создание тега
        public void CreateTag(string name, string color)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CALL \"CreateTag\"(@p_name, @p_color)", conn))
                {
                    cmd.Parameters.AddWithValue("@p_name", name);
                    cmd.Parameters.AddWithValue("@p_color", color);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Назначить тег задаче
        public void AddTagToTask(int taskId, int tagId)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CALL \"AddTagToTask\"(@p_task_id, @p_tag_id)", conn))
                {
                    cmd.Parameters.AddWithValue("@p_task_id", taskId);
                    cmd.Parameters.AddWithValue("@p_tag_id", tagId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Удалить тег с задачи
        public void RemoveTagFromTask(int taskId, int tagId)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CALL \"RemoveTagFromTask\"(@p_task_id, @p_tag_id)", conn))
                {
                    cmd.Parameters.AddWithValue("@p_task_id", taskId);
                    cmd.Parameters.AddWithValue("@p_tag_id", tagId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Получить теги задачи
        public List<(int TagId, string TagName, string TagColor)> GetTaskTags(int taskId)
        {
            var result = new List<(int, string, string)>();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM \"GetTaskTags\"(@p_task_id)", conn))
                {
                    cmd.Parameters.AddWithValue("@p_task_id", taskId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add((
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2)
                            ));
                        }
                    }
                }
            }
            return result;
        }

        // Получить задачи по статусу
        public List<(int Id, string Title, string AssigneeName, DateTime DueDate)> GetTasksByStatus(int statusId)
        {
            var result = new List<(int, string, string, DateTime)>();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM \"GetTasksByStatus\"(@p_status_id)", conn))
                {
                    cmd.Parameters.AddWithValue("@p_status_id", statusId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add((
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.IsDBNull(2) ? null : reader.GetString(2),
                                reader.GetDateTime(3)
                            ));
                        }
                    }
                }
            }
            return result;
        }
        public List<Models.Task> GetTasksByUser(int accountId)
        {
            var tasks = new List<Models.Task>();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT id, title, description, status_id, create_date, due_date, team_id, assignee_id, creator_id, parent_task_id, created_at, updated_at FROM \"Tasks\" WHERE assignee_id = @accountId";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@accountId", accountId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tasks.Add(new Models.Task
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                                StatusId = reader.GetInt32(3),
                                CreateDate = reader.GetDateTime(4),
                                DueDate = reader.GetDateTime(5),
                                TeamId = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                                AssigneeId = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                                CreatorId = reader.GetInt32(8),
                                ParentTaskId = reader.IsDBNull(9) ? null : reader.GetInt32(9),
                                CreatedAt = reader.GetDateTime(10),
                                UpdatedAt = reader.GetDateTime(11)
                            });
                        }
                    }
                }
            }
            return tasks;
        }
        public Account GetAccountById(int accountId)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT id, login, password_hash, email, fullname, position, role, created_at, is_active FROM \"Accounts\" WHERE id = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", accountId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Account
                            {
                                Id = reader.GetInt32(0),
                                Login = reader.GetString(1),
                                PasswordHash = reader.GetString(2),
                                Email = reader.GetString(3),
                                FullName = reader.GetString(4),
                                Position = reader.IsDBNull(5) ? null : reader.GetString(5),
                                Role = reader.IsDBNull(6) ? null : reader.GetString(6),
                                CreatedAt = reader.GetDateTime(7),
                                IsActive = reader.GetBoolean(8)
                            };
                        }
                        return null;
                    }
                }
            }
        }
    }
}