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
        static void Main(string[] args)
        {
            logger.Info("Program started");
            
            var db = new NWContext();

            try
            {
                String[] mainMenu;
                mainMenu = new string[4] { "===MAIN MENU===", "1) Products", "2) Categories", "\"q\" to quit" };
                string choice;

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

        #region Menu Helpers
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

        #endregion

        #region Product menthods

        public static void ProductMenu(NWContext db)
        {
            String[] productMenu;
            productMenu = new string[6] { "===PRODUCT MENU===", "1) Add a New Product", "2) Edit a Product", "3) Display Products", "4) Display a Product", "\"q\" to quit" };
            string choice;
            do
            {
                DisplayMenu(productMenu);
                choice = Console.ReadLine();
                Console.Clear();
                LogMenuChoice(productMenu, choice);

                if (choice == "1")
                {
                    AddProduct(db);
                }
                else if (choice == "2")
                {
                    // TODO: Edit a product
                }
                else if (choice == "3")
                {
                    // User decides if they want to see 1) All products 2) Discontinued products 3) Active products
                    String[] displayProductsMenu;
                    displayProductsMenu = new string[5] { "---Products Display Menu---", "1) Display All Products", "2) Display Discontinued Products", "3) Display Active Products", "\"q\" to quit" };
                    string pChoice;
                    do
                    {
                        DisplayMenu(displayProductsMenu);
                        Console.WriteLine();
                        pChoice = Console.ReadLine();
                        LogMenuChoice(displayProductsMenu, pChoice);

                        DisplayProducts(db, pChoice);
        
                        Console.WriteLine();
                    } while (pChoice.ToLower() != "q");

                }
                else if (choice == "4")
                {
                    // TODO: Display a product
                }
                Console.WriteLine();
            } while (choice.ToLower() != "q");

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

            ValidationContext context = new ValidationContext(product, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(product, context, results, true);
            if (isValid)
            {
                // check for unique name
                if (db.Products.Any(p => p.ProductName == product.ProductName))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                }
                else
                {
                    logger.Info("Validation passed");

                    // TODO: save product to db
                    try
                    {
                        db.Products.Add(product);
                        db.SaveChanges();
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
            System.ConsoleColor saveConsoleColor = Console.ForegroundColor;
            if (pChoice=="1")
            {
                // Display all products
                var query = db.Products.OrderBy(p => p.ProductName);
                Console.WriteLine($"{query.Count()} records returned\n");
                foreach (var item in query)
                {
                    if (item.Discontinued)
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"{item.ProductName}");
                    if (item.Discontinued)
                        Console.ForegroundColor = saveConsoleColor;

                }

            }
            else if (pChoice=="2")
            {
                // Display discontinued products
                var query = db.Products.Where(p => p.Discontinued.Equals(true)).OrderBy(p => p.ProductName);
                Console.WriteLine($"{query.Count()} records returned\n");
                Console.ForegroundColor = ConsoleColor.DarkRed;
                foreach (var item in query)
                {
                    Console.WriteLine($"{item.ProductName}");
                }
                Console.ForegroundColor = saveConsoleColor;
            }
            else if (pChoice=="3")
            {
                // Display active products
                var query = db.Products.Where(p => p.Discontinued.Equals(false)).OrderBy(p => p.ProductName);
                Console.WriteLine($"{query.Count()} records returned\n");
                foreach (var item in query)
                {
                    Console.WriteLine($"{item.ProductName}");
                }
            }
        }

        #endregion product methods

        // End Program; End Namespace
    }
}
