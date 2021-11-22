namespace BookShop
{
    using Data;
    using Initializer;
    using System;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            //var command = Console.ReadLine();

            //var year = int.Parse(Console.ReadLine());

            //var result = GetBooksByAgeRestriction(db, command);

            var goldenBook = GetGoldenBooks(db);

            //var getBookByPrice = GetBooksByPrice(db);

            //var getBooksNotReleasedIn = GetBooksNotReleasedIn(db, year);

            //var bookTitleByCategories = GetBooksByCategory(db, command);

            //var releaseBeforeDate = GetBooksReleasedBefore(db, command);

            //var authorSearch = GetAuthorNamesEndingIn(db, command);

            //var bookSearch = GetBookTitlesContaining(db, command);

            //var bookSearchByAuthor = GetBooksByAuthor(db, command);

            //var countBook = CountCopiesByAuthor(db);

            //var profitByCategories = GetTotalProfitByCategory(db);

            //var mostResentBook = GetMostRecentBooks(db);

            Console.WriteLine(goldenBook);


        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var sb = new StringBuilder();
            var titles = context
                .Books
                .Where(x => x.AgeRestriction.ToString().ToLower() == command.ToLower())
                .Select(x => new
                {
                    x.Title
                })
                .OrderBy(x => x.Title)
                .ToList();

            foreach (var title in titles)
            {
                sb.AppendLine(title.Title);
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var sb = new StringBuilder();
            var books = context
                .Books
                .Where(x => x.Copies < 5000)
                .Select(x => new
                {
                    x.BookId,
                    x.Title
                })
                .OrderBy(x => x.BookId)
                .ToList();
            foreach (var b in books)
            {
                sb.AppendLine(b.Title);
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var sb = new StringBuilder();
            var books = context
                .Books
                .Where(x => x.Price > 40)
                .Select(x => new
                {
                    x.Title,
                    x.Price
                })
                .OrderByDescending(x => x.Price)
                .ToList();
            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:f2}");
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var sb = new StringBuilder();

            var books = context
                .Books
                .Where(x => x.ReleaseDate.Value.Year != year)
                .Select(x => new 
                { 
                    x.BookId,
                    x.Title 
                })
                .OrderBy(x => x.BookId)
                .ToList();

            foreach (var b in books)
            {
                sb.AppendLine(b.Title);
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var categories = input
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLower());

            var books = context
                .Books
                .Where(x => x.BookCategories.Any(bc => categories.Contains(bc.Category.Name.ToLower())))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToList();

            return String.Join(Environment.NewLine, books);
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var sb = new StringBuilder();
            var books = context
                .Books
                .Where(x => Convert.ToDateTime(date) > x.ReleaseDate.Value.Date)
                .Select(x => new
                {
                    x.Title,
                    EditionType = x.EditionType.ToString(),
                    x.Price,
                    x.ReleaseDate
                })
                .OrderByDescending(x => x.ReleaseDate)
                .ToList();
            foreach (var b in books)
            {
                sb.AppendLine($"{b.Title} - {b.EditionType} - ${b.Price}");
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var sb = new StringBuilder();
            var authors = context
                .Authors
                .Where(x => x.FirstName.EndsWith(input))
                .Select(x => new
                {
                    FullName = x.FirstName + ' ' + x.LastName
                })
                .OrderBy(x => x.FullName)
                .ToList();
            foreach (var a in authors)
            {
                sb.AppendLine(a.FullName);
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context
                .Books
                .Where(x => x.Title.ToLower().Contains(input.ToLower()))
                .Select(x => x.Title)
                .OrderBy(x => x)
                .ToList();
            return String.Join(Environment.NewLine, books);
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var sb = new StringBuilder();

            var bookAuthors = context
                .Books
                .Where(x => x.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .Select(x => new
                {
                    x.BookId,
                    x.Title,
                    Author = x.Author.FirstName + ' ' + x.Author.LastName
                })
                .OrderBy(x => x.BookId)
                .ToList();
            foreach (var ba in bookAuthors)
            {
                sb.AppendLine($"{ba.Title} ({ba.Author})");
            }
            return sb.ToString().TrimEnd();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var booksCount = context
                .Books
                .Where(x => x.Title.Length > lengthCheck)
                .Count();

            return booksCount;
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var sb = new StringBuilder();
            var authorCopies = context
                .Authors
                .Select(x => new
                {
                    AuthorName = x.FirstName + ' ' + x.LastName,
                    BookCopies = x.Books.Sum(b => b.Copies)                   
                })
                .OrderByDescending(x => x.BookCopies)
                .ToList();
            foreach (var ac in authorCopies)
            {
                sb.AppendLine($"{ac.AuthorName} - {ac.BookCopies}");
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var sb = new StringBuilder();
            var categories = context
                .Categories
                .Select(x => new
                {
                    x.Name,
                    TotalProfit = x.CategoryBooks.Select(bc => new
                    {
                        BookProfit = bc.Book.Price * bc.Book.Copies
                    })
                    .Sum(x => x.BookProfit)
                })
                .OrderByDescending(x => x.TotalProfit)
                .ToList();
            foreach (var c in categories)
            {
                sb.AppendLine($"{c.Name} ${c.TotalProfit:f2}");
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var sb = new StringBuilder();
            var categoriesBooks = context
                .Categories
                .Select(c => new
                {
                    Category = c.Name,
                    TopBooks = c.CategoryBooks
                    .OrderByDescending(bc => bc.Book.ReleaseDate.Value.Year)
                    .Take(3)
                    .Select(bc => new
                    {
                        BookTitle = bc.Book.Title,
                        BookYear = bc.Book.ReleaseDate.Value.Year
                    })
                    .ToList()
                })
                .OrderBy(c => c.Category)
                .ToList();
            foreach (var category in categoriesBooks)
            {
                sb.AppendLine($"--{category.Category}");
                foreach (var book in category.TopBooks)
                {
                    sb.AppendLine($"{book.BookTitle} ({book.BookYear})");
                }
            }
            return sb.ToString().TrimEnd();
        }
    }
}
