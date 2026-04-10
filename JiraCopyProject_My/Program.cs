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
                Console.WriteLine("3) Выход");
                Console.Write("Выберите действие - ");
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
                        return;
                    default:
                        Console.WriteLine("Неверно, нажмите любую клавишу!");
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
    }
}