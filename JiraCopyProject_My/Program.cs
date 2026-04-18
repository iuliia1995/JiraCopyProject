using JiraCopyProject_My.Services;
using JiraCopyProject_My.Models;

namespace JiraCopyProject_My
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new DatabaseService();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("--- Аналог Jira ---");
                Console.WriteLine("1) Показать все задачи");
                Console.WriteLine("2) Добавить новую задачу");
                Console.WriteLine("3) Показать задачи пользователя");
                Console.WriteLine("4) Информация об аккаунте");
                Console.WriteLine("5) Создать подзадачу");
                Console.WriteLine("6) Добавить комментарий к задаче");
                Console.WriteLine("7) Создать тег");
                Console.WriteLine("8) Назначить тег задаче");
                Console.WriteLine("9) Показать теги задачи");
                Console.WriteLine("10) Показать задачи по статусу");
                Console.WriteLine("0) Выход");
                Console.Write("Выберите действие: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": ShowTasks(db); break;
                    case "2": AddNewTask(db); break;
                    case "3": ShowUserTasks(db); break;
                    case "4": ShowAccountInfo(db); break;
                    case "5": CreateSubTask(db); break;
                    case "6": AddCommentToTask(db); break;
                    case "7": CreateNewTag(db); break;
                    case "8": AssignTagToTask(db); break;
                    case "9": ShowTaskTags(db); break;
                    case "10": ShowTasksByStatus(db); break;
                    case "0": return;
                    default: Console.WriteLine("Неверный ввод, нажмите любую клавишу"); Console.ReadKey(); break;
                }
            }
        }

        static void ShowTasks(DatabaseService db)
        {
            Console.Clear();
            Console.WriteLine("--- Список задач ---");
            var tasks = db.GetTasks();
            if (tasks.Count == 0)
                Console.WriteLine("Задач пока нет.");
            else
                foreach (var task in tasks)
                    Console.WriteLine($"#{task.Id}: {task.Title} (статус: {task.StatusId}, срок: {task.DueDate.ToShortDateString()})");

            Console.WriteLine("\nНажмите любую клавишу для возврата в меню!");
            Console.ReadKey();
        }

        static void AddNewTask(DatabaseService db)
        {
            Console.Clear();
            Console.WriteLine("Добавление новой задачи:");

            var accounts = db.GetAccounts();
            Console.WriteLine("\nСписок аккаунтов (id, логин, ФИО)");
            foreach (var acc in accounts)
                Console.WriteLine($" {acc.Id} - {acc.Login} ({acc.FullName})");

            Console.Write("Название - ");
            string title = Console.ReadLine();

            Console.Write("Описание - ");
            string description = Console.ReadLine();

            Console.Write("ID статуса 5 - Новая, 6 - В работе: ");
            if (!int.TryParse(Console.ReadLine(), out int statusId))
            {
                Console.WriteLine("Ошибка! Статут должен быть числом, нажмите любую клавишу.");
                Console.ReadKey();
                return;
            }

            Console.Write("ID исполнителя из списка выше ");
            if (!int.TryParse(Console.ReadLine(), out int assigneeId))
            {
                Console.WriteLine("Ошибка, id исполнителя должен быть числом. нажмите любую клавишу");
                Console.ReadLine();
                return;
            }

            Console.Write("ID создателя из списка выше ");
            if (!int.TryParse(Console.ReadLine(), out int creatorId))
            {
                Console.WriteLine("Ошибка, id создателя должен быть числом. нажмите любую клавишу");
                Console.ReadLine();
                return;
            }

            Console.Write("Срок сдачи ГГГГ-ММ-ДД: ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime dueDate))
            {
                Console.WriteLine("Ошибка, неверный формат даты, нажмите любую клавишу");
                Console.ReadLine();
                return;
            }

            db.AddTask(title, description, statusId, assigneeId, creatorId, dueDate, null);
            Console.WriteLine("Задача успешно создана! Нажмите любую клавишу.");
            Console.ReadKey();
        }
        static void ShowUserTasks(DatabaseService db)
        {
            Console.Clear();
            Console.WriteLine("--- Показать задачи пользователя ---");

            var accounts = db.GetAccounts();
            Console.WriteLine("\nСписок аккаунтов id, логин, ФИО");
            foreach (var acc in accounts)
                Console.WriteLine($"  {acc.Id} - {acc.Login} ({acc.FullName})");

            Console.Write("\nВведите ID пользователя ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Неверный ID. Нажмите любую клавишу.");
                Console.ReadKey();
                return;
            }

            var tasks = db.GetTasksByUser(userId);
            Console.WriteLine($"\nЗадачи пользователя (исполнитель)");
            if (tasks.Count == 0)
                Console.WriteLine("  Нет назначенных задач.");
            else
                foreach (var task in tasks)
                    Console.WriteLine($"  #{task.Id}: {task.Title} (статус: {task.StatusId}, срок: {task.DueDate.ToShortDateString()})");

            Console.WriteLine("\nНажмите любую клавишу для возврата в меню.");
            Console.ReadKey();
        }

        static void CreateSubTask(DatabaseService db)
        {
            Console.Clear();
            Console.WriteLine("Создание подзадачи");
            Console.Write("ID родительской задачи ");
            if (!int.TryParse(Console.ReadLine(), out int parentId))
            { Console.WriteLine("Ошибка ввода."); Console.ReadKey(); return; }

            Console.Write("Название ");
            string title = Console.ReadLine();
            Console.Write("Описание ");
            string desc = Console.ReadLine();

            Console.Write("ID исполнителя ");
            if (!int.TryParse(Console.ReadLine(), out int assigneeId)) return;
            Console.Write("ID создателя ");
            if (!int.TryParse(Console.ReadLine(), out int creatorId)) return;
            Console.Write("Срок сдачи (ГГГГ-ММ-ДД) ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime dueDate)) return;

            db.CreateSubTask(parentId, title, desc, assigneeId, creatorId, dueDate);
            Console.WriteLine("Подзадача создана!");
            Console.ReadKey();
        }

        static void AddCommentToTask(DatabaseService db)
        {
            Console.Clear();
            Console.WriteLine("Добавление комментария");
            Console.Write("ID задачи ");
            if (!int.TryParse(Console.ReadLine(), out int taskId)) return;
            Console.Write("ID автора ");
            if (!int.TryParse(Console.ReadLine(), out int authorId)) return;
            Console.Write("Текст комментария ");
            string comment = Console.ReadLine();

            db.AddTaskComment(taskId, authorId, comment);
            Console.WriteLine("Комментарий добавлен!");
            Console.ReadKey();
        }

        static void CreateNewTag(DatabaseService db)
        {
            Console.Clear();
            Console.WriteLine("Создание тега");
            Console.Write("Название тега ");
            string name = Console.ReadLine();
            Console.Write("Цвет (например #FF0000) ");
            string color = Console.ReadLine();
            db.CreateTag(name, color);
            Console.WriteLine("Тег создан!");
            Console.ReadKey();
        }

        static void AssignTagToTask(DatabaseService db)
        {
            Console.Clear();
            Console.WriteLine("Назначение тега задаче");
            Console.Write("ID задачи ");
            if (!int.TryParse(Console.ReadLine(), out int taskId)) return;
            Console.Write("ID тега ");
            if (!int.TryParse(Console.ReadLine(), out int tagId)) return;
            db.AddTagToTask(taskId, tagId);
            Console.WriteLine("Тег назначен!");
            Console.ReadKey();
        }

        static void ShowTaskTags(DatabaseService db)
        {
            Console.Clear();
            Console.WriteLine("Теги задачи");
            Console.Write("ID задачи ");
            if (!int.TryParse(Console.ReadLine(), out int taskId)) return;
            var tags = db.GetTaskTags(taskId);
            if (tags.Count == 0)
                Console.WriteLine("У задачи нет тегов.");
            else
                foreach (var t in tags)
                    Console.WriteLine($"- {t.TagName} (цвет {t.TagColor})");
            Console.ReadKey();
        }

        static void ShowTasksByStatus(DatabaseService db)
        {
            Console.Clear();
            Console.WriteLine("Задачи по статусу");
            Console.Write("ID статуса (5 - Новая, 6 - В работе): ");
            if (!int.TryParse(Console.ReadLine(), out int statusId)) return;
            var tasks = db.GetTasksByStatus(statusId);
            if (tasks.Count == 0)
                Console.WriteLine("Задач с таким статусом нет.");
            else
                foreach (var t in tasks)
                    Console.WriteLine($"#{t.Id}: {t.Title} (исполнитель - {t.AssigneeName ?? "не назначен"}, срок {t.DueDate.ToShortDateString()})");
            Console.ReadKey();
        }

        static void ShowAccountInfo(DatabaseService db)
        {
            Console.Clear();
            Console.WriteLine("--- Инфа об аккаунте ---");

            var accounts = db.GetAccounts();
            Console.WriteLine("\nСписок аккаунтов (id, логин, ФИО)");
            foreach (var acc in accounts)
                Console.WriteLine($"  {acc.Id} - {acc.Login} ({acc.FullName})");

            Console.Write("\nВведите ID аккаунта - ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Неверный ID, нажмите любую клавишу...");
                Console.ReadKey();
                return;
            }

            var account = db.GetAccountById(userId);
            if (account == null)
            {
                Console.WriteLine("Аккаунт не найден.");
            }
            else
            {
                Console.WriteLine($"\nФИО; {account.FullName}");
                Console.WriteLine($"Должность: {account.Position ?? "не указана"}");
                Console.WriteLine($"Email: {account.Email}");
                Console.WriteLine($"Роль: {account.Role ?? "не указана"}");
            }

            Console.WriteLine("\nНажмите любую клавишу для возврата в меню.");
            Console.ReadKey();
        }
    }
}