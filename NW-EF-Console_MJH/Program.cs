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
                // Display main menu; get and record user's choice. Loop until q.
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

#region Helper methods
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
                logger.Info($"Choice {choice} entered");

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

        //TODO: Is there a way to combine all these validations into one routine?
        //TODO: Would it be better to call the data annotation validation after each field?

        /// <summary>
        /// Show user current information and prompt for a new value. Validate as an int. 
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="currval"></param>
        /// <returns>Return the validated int or the original value if no input.</returns>
        public static int? validateInt(string prompt, int? currval)
        {
            bool done = false;

            int? retval = currval;
            if (currval == null)
                prompt = prompt + "null";
            while (!done)
            {
                string answer = getAnswer(prompt);
                if (answer == "")
                    done = true;
                else
                {
                    if (int.TryParse(answer, out int newInt))
                    {
                        retval = newInt;
                        done = true;
                    }
                    else
                        Console.WriteLine("Please enter an integer or press return to leave this field.");
                }
            }

            return retval;
        }

        /// <summary>
        /// Show user current field information and prompt for change. Validate as a decimal. 
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="currval"></param>
        /// <returns>Return new value or the original value if no input.</returns>
        public static decimal? validateDecimal(string prompt, decimal? currval)
        {
            bool done = false;

            decimal? retval = currval;
            if (currval == null)
                prompt = prompt + "null";
            while (!done)
            {
                string answer = getAnswer(prompt);
                if (answer == "")
                    done = true;
                else
                {
                    if (Decimal.TryParse(answer, out decimal newDecimal))
                    {
                        retval = newDecimal;
                        done = true;
                    }
                    else
                        Console.WriteLine("Please enter a money value or press return to leave this field.");
                }
            }

            return retval;
        }

        /// <summary>
        /// Show user current field value and prompt for change. Validate short. 
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="currval"></param>
        /// <returns>Return new value or current if none entered.</returns>
        public static short? validateShort(string prompt, short? currval)
        {
            bool done = false;

            short? retval = currval;
            if (currval == null)
                prompt = prompt + "null";
            while (!done)
            {
                string answer = getAnswer(prompt);
                if (answer == "")
                    done = true;
                else
                {
                    if (short.TryParse(answer, out short newShort))
                    {
                        retval = newShort;
                        done = true;
                    }
                    else
                        Console.WriteLine("Please enter a number or press return to leave this field.");
                }
            }

            return retval;
        }

        /// <summary>
        /// Show user current field value. Ask for update. Validate as boolean. Return new value or original if no new value entered.
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="currval"></param>
        /// <param name="retval"></param>
        /// <returns></returns>
        public static bool validateBoolean(string prompt, bool currval)
        {
            bool done = false;

            bool retval = currval;
            if (currval == null)
                prompt = prompt + "null";
            while (!done)
            {
                string answer = getAnswer(prompt);
                if (answer == "")
                    done = true;
                else
                {
                    if (Boolean.TryParse(answer, out bool newBool))
                    {
                        retval = newBool;
                        done = true;
                    }
                    else
                        Console.WriteLine("Please enter true or false, or press return to leave this field.");
                }
            }

            return retval;
        }


    #endregion

#region Product Helper Methods

    /// <summary>
    /// Display products in the given style
    /// </summary>
    /// <param name="query">Products to display</param>
    /// <param name="displayType">Style to display</param>
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
                    Console.WriteLine($"{item.ProductName}: id {item.ProductID}");
                else if (displayType==CATEGORY_DISPLAY)
                    Console.WriteLine($"{item.ProductName}: id/category {item.ProductID}/{item.Category.CategoryName}");
                if (item.Discontinued)
                    Console.ForegroundColor = saveConsoleColor;
            }
        }

        #endregion

#region Product methods

        /// <summary>
        /// Display product menu; get and record user choice. Loop until q.
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
                    // Display All Products Menu: 
                    // User can decide if they want to see 1) All products 2) Discontinued products 3) Active products
                    Console.Clear();
                    String[] displayProductsMenu;
                    displayProductsMenu = new string[5] { "---Display All Products Menu---", "1) Display All Products", "2) Display Discontinued Products", "3) Display Active Products", "\"q\" to go back" };
                    string pChoice;
                    do
                    {
                        DisplayMenu(displayProductsMenu);
                        Console.WriteLine();
                        pChoice = Console.ReadLine();
                        LogMenuChoice(displayProductsMenu, pChoice);

                        DisplayAllProducts(db, pChoice); // Chosen display of products
        
                        Console.WriteLine();
                    } while (pChoice.ToLower() != "q");
                }
                else if (choice == "4")
                {
                    var product = FindAndDisplayOneProduct(db); // Display a single product (the whole record)
                }
                Console.WriteLine();
            } while (choice.ToLower() != "q");

        }

        /// <summary>
        /// Find one product; display it.
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static Product FindAndDisplayOneProduct(NWContext db)
        {
            // Display a single product (the whole record)
            var product = FindOneProduct(db);
            if (product != null)
            {
                Console.WriteLine(db.DisplayAProduct(product));
            }
            return product;
        }


        /// <summary>
        /// Find one product
        /// </summary>
        /// <param name="db"></param>
        /// <returns>Product or null</returns>
        public static Product FindOneProduct(NWContext db)
        {
            int pId=-1; // Chosen product ID; start with invalid id -1
            // Display Single Product Search Menu:
            // user can search on Product Name, Category Name, Product ID
            String[] productSearchMenu;
            productSearchMenu = new string[5] { "---Product Search Menu---", "1) Search on Product Name", "2) Search on Category Name", "3) Search on Product ID", "\"q\" to go back" };
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
                        var query = db.Products.Where(p => p.ProductName.ToLower().Contains(searchItem.ToLower())).OrderBy(n => n.ProductName);
                        Console.WriteLine($"{query.Count()} record(s) returned\n");
                        consoleDisplayProducts(query, ID_DISPLAY);
                        string itemChosen = getAnswer("Enter the id number of the product you want: ");
                        if (int.TryParse(itemChosen, out int id))
                            pId = id;
                        else
                            Console.WriteLine("Invalid number entered.");
                    }
                    else
                    {
                        Console.WriteLine($"No products found where product name contains {searchItem}.");
                    }
                }
                else if (choice == "2")
                {
                    // Search based on category name
                    // Get category name guess
                    string searchItem = getAnswer("Enter all or part of a category name (not case sensitive): ");
                    // Look for it
                    if (db.Products.Any(p => p.Category.CategoryName.ToLower().Contains(searchItem.ToLower())))
                    {
                        var query = db.Products.Include("Category").Where(p => p.Category.CategoryName.ToLower().Contains(searchItem.ToLower())).OrderBy(n => n.Category.CategoryName);
                        Console.WriteLine($"{query.Count()} record(s) returned\n");
                        consoleDisplayProducts(query, CATEGORY_DISPLAY);
                        string itemChosen = getAnswer("Enter the id number of the product you want: ");
                        if (int.TryParse(itemChosen, out int id))
                            pId = id;
                        else
                            logger.Info("Invalid number entered.");
                    }
                    else
                    {
                        logger.Info($"No products found where category name contains {searchItem}.");
                    }
                }
                else if (choice == "3")
                {
                    string searchItem = getAnswer("Enter Product ID: ");
                    if (int.TryParse(searchItem, out int id))
                    {
                        if (db.Products.Any(p => p.ProductID.Equals(pId)))
                        {
                            Product product = db.Products.Include("Category").FirstOrDefault(p => p.ProductID == pId);
                            Console.WriteLine($"1 record returned\n");
                            pId = product.ProductID;
                        }
                    }
                    else
                        logger.Info($"No products found with product id {searchItem}.");
                }

                Console.WriteLine();
            } while ((choice.ToLower()) != "q" && (pId==-1));

            if (pId > -1)
            {
                // Double check if the user hasn't mistyped the ID
                if (db.Products.Any(p => p.ProductID.Equals(pId)))
                {
                    logger.Info($"Returning product id {pId}.");
                    Product product = db.Products.Include("Category").FirstOrDefault(p => p.ProductID == pId);
                    return product;
                }
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Add a new product. Make sure the product name is unique.
        /// </summary>
        /// <param name="db"></param>
        public static void AddProduct(NWContext db)
        {
            Product product = new Product();
            product.ProductName = getAnswer("Enter product name (required):");
            product.SupplierId = validateInt("Enter the supplier ID (if any). Press return for ", null);
            product.CategoryId = validateInt("Enter the category ID (if any). Press return for ", null);
            product.QuantityPerUnit = getAnswer("Enter quantity per unit. Press return for ");
            product.UnitPrice = validateDecimal("Enter the unit price. Press return for ", null);
            product.UnitsInStock = validateShort("Enter the units in stock (0-32767). Press return for ", null);
            product.UnitsOnOrder = validateShort("Enter the units on order (0-32767). Press return for ", null);
            product.ReorderLevel = validateShort("Enter the reorder level (0-32767). Press return for ", null);
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

        /// <summary>
        /// Display all products: All-all, all-discontinued, all-active
        /// </summary>
        /// <param name="db"></param>
        /// <param name="pChoice"></param>
        public static void DisplayAllProducts(NWContext db, String pChoice)
        {
            // User decides if they want to see 1) All-all products 2) All Discontinued products 3) All Active products
            // Discontinued products should are distinguished from active products by printing in dark red
            if (pChoice=="1")
            {
                // Display all products
                var query = db.Products.OrderBy(p => p.ProductName);
                Console.WriteLine($"{query.Count()} record(s) returned\n");
                consoleDisplayProducts(query, NAME_DISPLAY);
            }
            else if (pChoice=="2")
            {
                // Display discontinued products
                var query = db.Products.Where(p => p.Discontinued.Equals(true)).OrderBy(p => p.ProductName);
                Console.WriteLine($"{query.Count()} record(s) returned\n");
                consoleDisplayProducts(query, NAME_DISPLAY);
            }
            else if (pChoice=="3")
            {
                // Display active products
                var query = db.Products.Where(p => p.Discontinued.Equals(false)).OrderBy(p => p.ProductName);
                Console.WriteLine($"{query.Count()} record(s) returned\n");
                consoleDisplayProducts(query, NAME_DISPLAY);
            }
        }

        /// <summary>
        /// Find one product, allow user to change the fields, validate, and save.
        /// </summary>
        /// <param name="db"></param>
        public static void EditProduct(NWContext db)
        {
            // Find and display the product to edit
            Product p = FindAndDisplayOneProduct(db);            
            if (p!=null)
            {
                // Found. Get updated information
                Product updatedProduct = new Product();
                EditProductFields(p, updatedProduct);

                // Validate updated product
                ValidationContext context = new ValidationContext(updatedProduct, null, null);
                List<ValidationResult> results = new List<ValidationResult>(); // Store the errors in a list

                var isValid = Validator.TryValidateObject(updatedProduct, context, results, true);
                if (isValid)
                {
                    // If the product name has changed, check that it's unique
                    if (updatedProduct.ProductName != p.ProductName)
                    {
                        if (db.Products.Any(p1 => p1.ProductName == updatedProduct.ProductName))
                        {
                            // generate validation error
                            isValid = false;
                            results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                        }
                    }

                    if (isValid)
                    {
                        logger.Info("Validation passed");

                        try
                        {
                            updatedProduct.ProductID = p.ProductID;
                            db.UpdateProduct(updatedProduct);
                            logger.Info($"Product {p.ProductID}: {updatedProduct.ProductName} updated.");
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
        }

        /// <summary>
        /// Display each current product field and allow user to enter a change
        /// </summary>
        /// <param name="p"></param>
        /// <param name="updatedProduct"></param>
        public static void EditProductFields(Product p, Product updatedProduct)
        {

            // Update product name
            updatedProduct.ProductName = p.ProductName;
            String answer = getAnswer($"Enter new product name or press return to keep { p.ProductName})");
            if (answer != "")
                updatedProduct.ProductName = answer;

            updatedProduct.SupplierId = validateInt($"Enter new supplier ID or press return to keep {p.SupplierId}", p.SupplierId);
            updatedProduct.CategoryId = validateInt($"Enter new category ID or press return to keep {p.CategoryId}", p.CategoryId);

            // Update quantity per unit
            updatedProduct.QuantityPerUnit = p.QuantityPerUnit;
            answer = getAnswer($"Enter new quantity per unit or press return to keep {p.QuantityPerUnit}");
            if (answer != "")
                updatedProduct.QuantityPerUnit = answer;

            updatedProduct.UnitPrice = validateDecimal($"Enter new unit price or press return to keep {p.UnitPrice,10:c2}", p.UnitPrice);
            updatedProduct.UnitsInStock = validateShort($"Enter new units in stock (0-32767) or press return to keep {p.UnitsInStock}", p.UnitsInStock);
            updatedProduct.UnitsOnOrder = validateShort($"Enter new units on order (0-32767) or press return to keep {p.UnitsOnOrder}", p.UnitsOnOrder);
            updatedProduct.ReorderLevel = validateShort($"Enter new reorder level (0-32767) or press return to keep {p.ReorderLevel}", p.ReorderLevel);
            updatedProduct.Discontinued = validateBoolean($"Enter new discontinued flag or press return to keep {p.Discontinued}", p.Discontinued);

        }
        
        #endregion product methods

        // End Program; End Namespace
    }
}
