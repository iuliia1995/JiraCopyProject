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
                Console.WriteLine("5) Выход");
                Console.Write("Выберите действие. ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowTasks(db);
                        break;
                    case "2":
                        AddNewTask(db);
                        break;
                    case "3":
                        ShowUserTasks(db);
                        break;
                    case "4":
                        ShowAccountInfo(db);
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Неверный ввод, нажмите любую клавишу");
                        Console.ReadKey();
                        break;
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

            Console.Write("Название - ");
            string title = Console.ReadLine();

            Console.Write("Описание - ");
            string description = Console.ReadLine();

            Console.Write("ID статуса 5 - Новая, 6 - В работе: ");
            int statusId = int.Parse(Console.ReadLine());

            Console.Write("ID исполнителяЖ ");
            int assigneeId = int.Parse(Console.ReadLine());

            Console.Write("ID создателяЖ ");
            int creatorId = int.Parse(Console.ReadLine());

            Console.Write("Срок сдачи ГГГГ-ММ-ДД: ");
            DateTime dueDate = DateTime.Parse(Console.ReadLine());

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