using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NW_EF_Console_MJH.Models;

namespace NW_EF_Console_MJH
{
    class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        const int NAME_DISPLAY= 1;
        const int ID_DISPLAY = 2;
        const int CATEGORY_DISPLAY = 3;
        static void Main(string[] args)
        {
            logger.Info("Program started");
            
            var db = new NWContext();

            try
            {
                String[] mainMenu;
                mainMenu = new string[4] { "===MAIN MENU===", "1) Products", "2) Categories", "\"q\" to quit" };
                string choice;
                // Display main menu and get user's choice. Loop until q.
                do
                {
                    DisplayMenu(mainMenu);
                    choice = Console.ReadLine();
                    Console.Clear();
                    LogMenuChoice(mainMenu, choice);
                    if (choice == "1")
                    {
                        ProductMenu(db);
                    }
                    else if (choice=="2")
                    {

                    }

                    Console.WriteLine();

                } while (choice.ToLower() != "q");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            logger.Info("Program ended");
        }

        #region Helpers
        /// <summary>
        /// Display items in menu array
        /// </summary>
        /// <param name="menu"></param>
        public static void DisplayMenu(String[] menu)
        {
            foreach (String m in menu)
            {
                Console.WriteLine(m);
            }
        }

        /// <summary>
        /// NLog either the menu verbiage or the choice if q or not valid menu item
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="choice"></param>
        public static void LogMenuChoice(String[] menu, String choice)
        {
            if (int.TryParse(choice, out int n) && (n < menu.Length - 1 && n > 0))
                logger.Info($"Option {menu[n]} selected");
            else
                logger.Info($"Option {choice} selected");

        }

        /// <summary>
        /// Turns 2-line console prompt/get answer into one line.
        /// </summary>
        /// <param name="prompt"></param>
        /// <returns></returns>
        public static string getAnswer(String prompt)
        {
            Console.WriteLine(prompt);
            return Console.ReadLine();
        }

        #endregion

        #region Product Helper Methods
        public static void consoleDisplayProducts(IEnumerable<Product> query, int displayType)
        {
            System.ConsoleColor saveConsoleColor = Console.ForegroundColor;
            foreach (var item in query)
            {
                if (item.Discontinued)
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                if (displayType==NAME_DISPLAY)
                    Console.WriteLine($"{item.ProductName}");
                else if (displayType==ID_DISPLAY)
                    Console.WriteLine($"{item.ProductID}: {item.ProductName}");
                else if (displayType==CATEGORY_DISPLAY)
                    Console.WriteLine($"{item.ProductID}: {item.Category.CategoryName}/{item.ProductName}");
                if (item.Discontinued)
                    Console.ForegroundColor = saveConsoleColor;
            }
        }

        #endregion

        #region Product methods

        /// <summary>
        /// Display product menu and get user choice. Loop until q.
        /// </summary>
        /// <param name="db">database context</param>
        public static void ProductMenu(NWContext db)
        {
            String[] productMenu;
            productMenu = new string[6] { "===PRODUCT MENU===", "1) Add a New Product", "2) Edit a Product", "3) Display Products", "4) Display a Product", "\"q\" to go back" };
            string choice;
            do
            {
                DisplayMenu(productMenu); // Display the menu
                choice = Console.ReadLine(); // Get the user's input
                LogMenuChoice(productMenu, choice); // Log the input
                Console.Clear();
                // Select menu action based on choice
                if (choice == "1")
                {
                    AddProduct(db); // Add a product
                }
                else if (choice == "2")
                {
                    EditProduct(db); // Edit a product
                }
                else if (choice == "3")
                {
                    // Display products: menu so user can decide if they want to see 1) All products 2) Discontinued products 3) Active products
                    String[] displayProductsMenu;
                    displayProductsMenu = new string[5] { "---Products Display Menu---", "1) Display All Products", "2) Display Discontinued Products", "3) Display Active Products", "\"q\" to go back" };
                    string pChoice;
                    do
                    {
                        DisplayMenu(displayProductsMenu);
                        Console.WriteLine();
                        pChoice = Console.ReadLine();
                        LogMenuChoice(displayProductsMenu, pChoice);

                        DisplayProducts(db, pChoice); // Chosen display of products
        
                        Console.WriteLine();
                    } while (pChoice.ToLower() != "q");

                }
                else if (choice == "4")
                {
                    // TODO: Display a single product (the whole record)
                }
                Console.WriteLine();
            } while (choice.ToLower() != "q");

        }

        public static void EditProduct(NWContext db)
        {
            // First, find the product to edit
            int pId = FindProduct(db);
            if (pId>-1)
            {
                // TODO Edit the product
                Console.WriteLine($"Getting product {pId}.");
            }
        }

        public static int FindProduct(NWContext db)
        {
            int pId=-1;
            // Search on Product Name, Category Name
            String[] productSearchMenu;
            productSearchMenu = new string[4] { "---Product Search Menu---", "1) Search on Product Name", "2) Search on Category Name", "\"q\" to go back" };
            string choice;
            do
            {
                DisplayMenu(productSearchMenu);
                choice = Console.ReadLine();
                Console.Clear();
                LogMenuChoice(productSearchMenu, choice);

                if (choice == "1")
                {
                    // Search based on product name
                    // Get user's guess
                    string searchItem = getAnswer("Enter all or part of a product name (not case sensitive): ");
                    // Look for it
                    if (db.Products.Any(p => p.ProductName.ToLower().Contains(searchItem.ToLower())))
                    {
                        var query = db.Products.Where(p => p.ProductName.ToLower().Contains(searchItem.ToLower())).OrderBy(n=>n.ProductName);
                        Console.WriteLine($"{query.Count()} records returned\n");
                        if (query.Count() == 1)
                            foreach (var item in query)
                                pId = item.ProductID;
                        else
                        {
                            consoleDisplayProducts(query, ID_DISPLAY);
                            string itemChosen = getAnswer("Enter the id number of the product you want: ");
                            if (int.TryParse(itemChosen, out int id))
                                pId = id;
                            else
                                Console.WriteLine("No valid ID chosen.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"No products found where product name contains {searchItem}.");
                    }
                }
                else if (choice == "2")
                {
                    // Search based on category name
                    // TODO: Category Name search
                    // Get category name guess
                    string searchItem = getAnswer("Enter all or part of a category name (not case sensitive): ");
                    // Look for it
                    if (db.Products.Any(p => p.Category.CategoryName.ToLower().Contains(searchItem.ToLower())))
                    {
                        var query = db.Products.Include("Category").Where(p => p.Category.CategoryName.ToLower().Contains(searchItem.ToLower())).OrderBy(n => n.Category.CategoryName);
                        Console.WriteLine($"{query.Count()} records returned\n");
                        if (query.Count() == 1)
                            foreach (var item in query)
                                pId = item.ProductID;
                        else
                        {
                            consoleDisplayProducts(query, CATEGORY_DISPLAY);
                            string itemChosen = getAnswer("Enter the id number of the product you want: ");
                            if (int.TryParse(itemChosen, out int id))
                                pId = id;
                            else
                                Console.WriteLine("No valid ID chosen.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"No products found where product name contains {searchItem}.");
                    }


                }

                Console.WriteLine();
            } while ((choice.ToLower()) != "q" && (pId==-1));

            return pId;
        }

        public static void AddProduct(NWContext db)
        {
            Product product = new Product();
            String answer = "";
            product.ProductName = getAnswer("Enter product name (required):");

            answer = getAnswer("Enter the supplier ID (if any):");
            if (answer == "")
                product.SupplierId = null;
            else if (int.TryParse(answer, out int id))
                product.SupplierId = id;
            else
                logger.Error("Must be an integer or null.");

            answer = getAnswer("Enter the category ID (if any):");
            if (answer == "")
                product.CategoryId = null;
            else if (int.TryParse(answer, out int id))
                product.CategoryId = id;
            else
                logger.Error("Must be an integer or null.");

            product.QuantityPerUnit = getAnswer("Enter quantity per unit:");

            answer = getAnswer("Enter the unit price:");
            if (answer == "")
                product.UnitPrice = null;
            else if (Double.TryParse(answer, out double price))
                product.UnitPrice = (decimal)price;
            else
                logger.Error("Must be a money value (0.00) or null.");

            answer = getAnswer("Enter the units in stock (0-32767):");
            if (answer == "")
                product.UnitsInStock = null;
            else if (short.TryParse(answer, out short qty))
                product.UnitsInStock = qty;
            else
                logger.Error("Must be a short integer or null.");

            answer = getAnswer("Enter the units on order (0-32767):");
            if (answer == "")
                product.UnitsOnOrder = null;
            else if (short.TryParse(answer, out short qty))
                product.UnitsOnOrder = qty;
            else
                logger.Error("Must be a short integer or null.");

            answer = getAnswer("Enter the reorder level (0-32767):");
            if (answer == "")
                product.ReorderLevel = null;
            else if (short.TryParse(answer, out short qty))
                product.ReorderLevel = qty;
            else
                logger.Error("Must be a short integer or null.");

            product.Discontinued = false;

            // Initializes a new instance of the ValidationContext class using the object, the service provider, and dictionary of service consumers.
            ValidationContext context = new ValidationContext(product, null, null);
            List<ValidationResult> results = new List<ValidationResult>(); // Store the errors in a list

            // Validator is a helper class that can be used to validate objects, properties, and methods when it is included in their associated ValidationAttribute attributes.
            // TryValidateObject(Object, ValidationContext, ICollection<ValidationResult>, Boolean) Determines whether the specified object is valid using the validation context, validation results collection, and a value that specifies whether to validate all properties.
            var isValid = Validator.TryValidateObject(product, context, results, true);
            if (isValid)
            {
                // check that product name hasn't already been used
                if (db.Products.Any(p => p.ProductName == product.ProductName))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                }
                else
                {
                    logger.Info("Validation passed");

                    try
                    {
                        db.AddProduct(product);
                        logger.Info($"Product {product.ProductName} added.");
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                    }
                }
            }
            if (!isValid)
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
            }

        }

        public static void DisplayProducts(NWContext db, String pChoice)
        {
            // User decides if they want to see 1) All products 2) Discontinued products 3) Active products
            // Discontinued products should be distinguished from active products
            if (pChoice=="1")
            {
                // Display all products
                var query = db.Products.OrderBy(p => p.ProductName);
                Console.WriteLine($"{query.Count()} records returned\n");
                consoleDisplayProducts(query, NAME_DISPLAY);
            }
            else if (pChoice=="2")
            {
                // Display discontinued products
                var query = db.Products.Where(p => p.Discontinued.Equals(true)).OrderBy(p => p.ProductName);
                Console.WriteLine($"{query.Count()} records returned\n");
                consoleDisplayProducts(query, NAME_DISPLAY);
            }
            else if (pChoice=="3")
            {
                // Display active products
                var query = db.Products.Where(p => p.Discontinued.Equals(false)).OrderBy(p => p.ProductName);
                Console.WriteLine($"{query.Count()} records returned\n");
                consoleDisplayProducts(query, NAME_DISPLAY);
            }
        }

        #endregion product methods

        // End Program; End Namespace
    }
}
