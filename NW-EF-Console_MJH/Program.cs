using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using NW_EF_Console_MJH.Models;

namespace NW_EF_Console_MJH
{
    /// <summary>
    /// Program to maintain Products and Categories.
    /// Author: Mary Hughes
    /// </summary>
    class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        const int NAME_DISPLAY= 1;
        const int ID_DISPLAY = 2;
        const int WITH_CATEGORY_DISPLAY = 3;
        const int WITH_PRODUCT_DISPLAY = 3;
        const int NOT_FOUND = -1;
        static void Main(string[] args)
        {
            logger.Info("Program started");
            
            var db = new NWContext();
            
            try
            {
                // Use data annotations and handle ALL user errors gracefully & log all errors using NLog
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
                        CategoryMenu(db);
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
        /// <param name="menu">String array containing menu items</param>
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
        /// <returns>String</returns>
        public static string getAnswer(String prompt)
        {
            Console.WriteLine(prompt);
            return Console.ReadLine();
        }
        /// <summary>
        /// Overload of getAnswer to include field data
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="currval"></param>
        /// <returns>String</returns>
        public static string getAnswer(String prompt, String currval)
        {
            if (currval == null)
                prompt = prompt + "null";
            else if (currval == "")
                prompt += "blank";
            Console.WriteLine(prompt);
            return Console.ReadLine();
        }
        /// <summary>
        /// Overload of getAnswer for required strings
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="currval"></param>
        /// <param name="req"></param>
        /// <returns>String</returns>
        /// NOTE: Supposedly c# can overload based on number or type of parameters. But when I originally had this as String prompt, bool req, and I added a category but first entered no name, I got an error on add.
        public static string getAnswer(String prompt, String currval, bool req)
        {
            String retval = "";
            retval=getAnswer(prompt);
            if (req && (retval==""))
            {
                bool done = false;
                while (!done)
                {
                    logger.Info("Value is required.");
                    Console.WriteLine(prompt);
                    retval = Console.ReadLine();
                    if (retval != "")
                        done = true;
                }
            }
            return retval;

        }


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
                        if (newInt>=0)
                        {
                            retval = newInt;
                            done = true;
                        }
                    }
                }
                if (!done)
                    logger.Info("Please enter an integer 0 or greater or press return to leave this field.");
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
                        if (newDecimal>=0)
                        {
                            retval = newDecimal;
                            done = true;
                        }
                    }
                }
                if (!done)
                    logger.Info("Please enter a money value 0 or greater or press return to leave this field.");
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
                        if (newShort>=0)
                        {
                            retval = newShort;
                            done = true;
                        }
                    }
                }
                if (!done)
                    logger.Info("Please enter a number between 0 and 32757 or press return to leave this field.");
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
            prompt += currval;
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
                        logger.Info("Please enter true or false, or press return to leave this field.");
                }
            }

            return retval;
        }

        #endregion

        #region Category Helper Methods

        /// <summary>
        /// validateCategory - validates the category record using data annotations. Designed to be used after each field.
        /// </summary>
        /// <param name="db">database context</param>
        /// <param name="category">category record</param>
        /// <returns></returns>
        public static Boolean validateCategory(NWContext db, Category category)
        {
            ValidationContext context = new ValidationContext(category, null, null); // New instance of ValidationContext using category.
            List<ValidationResult> results = new List<ValidationResult>(); // Store the errors in a list

            var isValid = Validator.TryValidateObject(category, context, results, true);
            if (!isValid)
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
            }
            return isValid;
        }


        /// <summary>
        /// Displays categories either name/description or with id.
        /// </summary>
        /// <param name="query">query holding catetories</param>
        /// <param name="displayType">NAME_DISPLAY, ID_DISPLAY, or WITH_PRODUCT_DISPLAY</param>
        public static void consoleDisplayCategories(IEnumerable<Category> query, int displayType)
        {
            foreach (var q in query)
            {
                if (displayType == NAME_DISPLAY)
                    Console.WriteLine($"{q.CategoryName}: {q.Description}");
                else if (displayType == ID_DISPLAY)
                    Console.WriteLine($"{q.CategoryName}: id {q.CategoryId}");
                else if (displayType == WITH_PRODUCT_DISPLAY)
                {
                    Console.WriteLine(q.CategoryName);
                    if (q.Products.Count() == 0)
                        Console.WriteLine("\t<no products>");
                    else
                    {
                        IEnumerable<Product> products = q.Products.OrderBy(p => p.ProductName);
                        foreach (Product p in products)
                        {
                            if (!p.Discontinued)
                                Console.WriteLine($"\t{p.ProductName}");
                        }
                    }
                }
            }
        }

        #endregion

#region Product Helper Methods


        /// <summary>
        /// validateProduct - validates the product record using data annotations. Designed to be used after each field.
        /// </summary>
        /// <param name="db">database context</param>
        /// <param name="product">product record</param>
        /// <returns></returns>
        public static Boolean validateProduct(NWContext db, Product product)
        {
            ValidationContext context = new ValidationContext(product, null, null); // New instance of ValidationContext using product.
            List<ValidationResult> results = new List<ValidationResult>(); // Store the errors in a list

            var isValid = Validator.TryValidateObject(product, context, results, true);
            if (!isValid)
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
            }
            return isValid;
        }


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
                // Discontinued products display in DarkRed console color 
                if (item.Discontinued)
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                if (displayType==NAME_DISPLAY)
                    Console.WriteLine($"{item.ProductName}");
                else if (displayType==ID_DISPLAY)
                    Console.WriteLine($"{item.ProductName}: id {item.ProductID}");
                else if (displayType==WITH_CATEGORY_DISPLAY)
                    Console.WriteLine($"{item.ProductName}: id/category {item.ProductID}/{item.Category.CategoryName}");
                if (item.Discontinued)
                    Console.ForegroundColor = saveConsoleColor;
            }
        }

        /// <summary>
        /// Validate the product id typed by the user is both an int and found
        /// </summary>
        /// <param name="db">database context</param>
        /// <param name="searchItem">user-typed value</param>
        /// <returns>product id if found, else NOT_FOUND</returns>
        public static int validateProductId(NWContext db, string searchItem)
        {
            int retval = NOT_FOUND;
            // Make sure the user typed a valid int
            if (int.TryParse(searchItem, out int id))
            {
                // Make sure the user typed a valid Id
                if (db.Products.Any(p => p.ProductID.Equals(id)))
                {
                    Console.WriteLine($"1 record returned\n");
                    retval = id;
                }
                else
                    logger.Info($"No products found with product ID {id}.");
            }
            else
                logger.Info($"Product id {searchItem} is not a valid ID.");

            return retval;
        }

        #endregion

#region Category methods

        public static void CategoryMenu(NWContext db)
        {
            String[] categoryMenu;
            categoryMenu = new string[8] { "===CATEGORY MENU===", "1) Add a New Category", "2) Edit a Category", "3) Display All Categories", "4) Display All Categories and Their Active Products", "5) Display a Category and Its Products", "6) Delete a category", "\"q\" to go back" };
            string choice;
            do
            {
                DisplayMenu(categoryMenu); // Display the menu
                choice = Console.ReadLine(); // Get the user's input
                LogMenuChoice(categoryMenu, choice); // Log the input
                Console.Clear();
                // Select menu action based on choice
                if (choice == "1")
                {
                    // Add a category
                    AddCategory(db);
                }
                else if (choice == "2")
                {
                    // Edit a category
                    EditCategory(db);
                }
                else if (choice == "3")
                {
                    // Display All Categories (Category Name and Description)
                    DisplayAllCategories(db);
                }
                else if (choice == "4")
                {
                    // Display all categories and their active products (Category Name, Product Name)
                    DisplayAllCategoriesWithProducts(db);
                }
                else if (choice == "5")
                {
                    // Display a single category and its active products (Category Name, Product Name)
                    DisplayACategoryWithProducts(db);
                }
                else if (choice == "6")
                {
                    // Delete a specified existing record from the Categories table (account for Orphans in related tables)
                    DeleteCategory(db);
                }
                Console.WriteLine();
            } while (choice.ToLower() != "q");

        }

        /// <summary>
        /// DeleteCategory finds the desired category if it exists and deletes (accounting for orphans in related tables)
        /// </summary>
        /// <param name="db">database context</param>
        public static void DeleteCategory(NWContext db)
        {
            int cId = findACategory(db);
            if (cId > NOT_FOUND)
            {
                var category = db.Categories.FirstOrDefault(c => c.CategoryId.Equals(cId));
                if (category != null)
                {
                    // Choice of delete category plus all products or remove neither
                    var query = db.Products.Where(c => c.CategoryId == cId);
                    Console.WriteLine($"WARNING: Deleting this category will also delete {query.Count()} products.");
                    if (getAnswer($"**Are you sure you want to delete BOTH the category {category.CategoryName} and {query.Count()} products (Y/N)?").ToUpper() == "Y")
                    {
                        // Delete products and their category
                        foreach (Product p in query)
                            db.DeleteMultipleProducts(p);
                        db.AfterDeleteMultipleProducts();
                        db.DeleteCategory(category);
                    }
                }
            }
        }


        /// <summary>
        /// searchForCategoryname - create a lambda expression to search for the category name
        /// I was trying to use this in the TicketSystem search where the user had a choice of searching by Status, Priority, or Submitter
        /// </summary>
        /// <param name="constantString">item to search for</param>
        /// <returns>Func<Category, bool> lambda function</returns>
        public static Func<Category, bool> searchForCategoryName(string constantString)
        {
            //  replaces c => c.CategoryName.ToUpper() == category.CategoryName.ToUpper()
            var parameter = Expression.Parameter(typeof(Category), "c"); // the c to the left of the =>
            // This joins the left and right sides of the == together: the second line is c.CategoryName.ToUpper(); the third line is category.CategoryName.ToUpper(); the Equal is the ==
            var comparison = Expression.Equal(
                        Expression.Call(Expression.Property(parameter, "CategoryName"), "ToUpper", Type.EmptyTypes),
                                Expression.Constant(constantString));
            // This joins the left and right sides of the => together
            Func<Category, bool> filterExpression = Expression.Lambda<Func<Category, bool>>(comparison, parameter).Compile();

            return filterExpression;
        }


        /// <summary>
        /// Add a category
        /// </summary>
        /// <param name="db">database context</param>
        public static void AddCategory(NWContext db)
        {
            // Validate the category record after each field but also at the end
            Category category = new Category();
            Boolean isValid = false;
            do
            {
                category.CategoryName = getAnswer("Enter category name (required): ", "", true);
                isValid = validateCategory(db, category);
                if (isValid)
                {
                    // check that category name hasn't already been used
                    // both of the following lambda expressions replace this: 
                    //      if (db.Categories.Any(c => c.CategoryName.ToUpper() == category.CategoryName.ToUpper()))

                    // A dynamic lambda expression
                    var parameterExpression = Expression.Parameter(typeof(Category), "c"); // this is the c to the left of the =>
                    var member = Expression.Property(parameterExpression, "CategoryName"); // this is the c.CategoryName just to the right of the =>
                    var left = Expression.Call(member, "ToUpper", Type.EmptyTypes); // This adds the ToUpper() to the c.CategoryName
                    var constant = Expression.Constant(category.CategoryName.ToUpper()); // This is the category.Categoryname.ToUpper to the right of the == (search constant)
                    var body = Expression.Equal(left, constant); // This builds the right side of the => (left is CategoryName.ToUpper(), constant is category.CategoryName.ToUpper(), Equal is ==
                    var finalExpression = Expression.Lambda<Func<Category, bool>>(body, parameterExpression);    // Builds the lambda expression
                    // would be called with: if (db.Categories.Any(finalExpression))

                    // In a callable function
                    Func<Category, bool> newFE = searchForCategoryName(category.CategoryName.ToUpper());

                    if (db.Categories.Any(newFE))
                    {
                        // generate validation error
                        isValid = false;
                        logger.Error("Name exists", new string[] { "CategoryName" });
                    }
                }
            } while (!isValid);
            isValid = false;
            do
            {
                category.Description = getAnswer("Enter category description: ");
                isValid = validateCategory(db, category);

            } while (!isValid);

            ValidationContext context = new ValidationContext(category, null, null); // New instance of ValidationContext using category.
            List<ValidationResult> results = new List<ValidationResult>(); // Store the errors in a list

            isValid = Validator.TryValidateObject(category, context, results, true);
            if (isValid)
            {
                logger.Info("Validation passed");

                try
                {
                    db.AddCategory(category);
                    logger.Info($"Category {category.CategoryName} added.");
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
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
        /// Prompt user for changes to the category fields
        /// </summary>
        /// <param name="c">curent category information</param>
        /// <param name="updatedCategory">changed category information</param>
        public static void EditCategoryFields(Category c, Category updatedCategory, NWContext db)
        {
            // Validate the category record after each field but also at the end
            // Update category name
            Boolean isValid = false;
            do
            {
                updatedCategory.CategoryName = c.CategoryName;
                String answer = getAnswer($"Enter new category name or press return to keep {c.CategoryName}", c.CategoryName);
                // A return will keep the current value
                if (answer != "")
                {
                    updatedCategory.CategoryName = answer;
                    isValid = validateCategory(db, updatedCategory);
                    if (isValid)
                    {
                        // check that category name hasn't already been used
                        if (db.Categories.Any(uc => uc.CategoryName == updatedCategory.CategoryName))
                        {
                            // generate validation error
                            isValid = false;
                            logger.Error("Name exists", new string[] { "CategoryName" });
                        }
                    }
                }
                else
                    isValid = true;
            } while (!isValid);

            // Update description
            isValid = false;
            do
            {
                updatedCategory.Description = c.Description;
                String answer = getAnswer($"Enter new category description or press return to keep {c.Description}", c.Description);
                if (answer != "")
                    updatedCategory.Description = answer;
                isValid = validateCategory(db, updatedCategory);
            } while (!isValid);
        }

        /// <summary>
        /// Edit a category
        /// </summary>
        /// <param name="db">database context</param>
        public static void EditCategory(NWContext db)
        {
            int cId = findACategory(db);
            if (cId > NOT_FOUND)
            {
//                var category = db.Categories.Find(cId);
                var category = db.Categories.FirstOrDefault(c => c.CategoryId.Equals(cId));
                if (category!=null)
                {
                    // Found. Get updated information
                    Category updatedCategory = new Category();
                    EditCategoryFields(category, updatedCategory, db);

                    // Validate updated category
                    ValidationContext context = new ValidationContext(updatedCategory, null, null);
                    List<ValidationResult> results = new List<ValidationResult>(); // Store the errors in a list

                    var isValid = Validator.TryValidateObject(updatedCategory, context, results, true);
                    if (isValid)
                    {
                        // If the category name has changed, check that it's unique
                        if (updatedCategory.CategoryName != category.CategoryName)
                        {
                            if (db.Categories.Any(c => c.CategoryName==updatedCategory.CategoryName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                            }
                        }

                        if (isValid)
                        {
                            logger.Info("Validation passed");

                            try
                            {
                                updatedCategory.CategoryId = category.CategoryId;
                                db.UpdateCategory(updatedCategory);
                                logger.Info($"Category {category.CategoryId}: {updatedCategory.CategoryName} updated.");
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
        }

        public static int findACategory(NWContext db)
        {
            int cId = NOT_FOUND;
            // Ask user to pick Category ID based on Category Name search.
            string searchItem = getAnswer("Enter all or part of a category name (not case sensitive): ");

            if (db.Categories.Any(c => c.CategoryName.ToLower().Contains(searchItem.ToLower())))
            {
                var query = db.Categories.Where(c => c.CategoryName.ToLower().Contains(searchItem.ToLower())).OrderBy(c => c.CategoryName);
                Console.WriteLine($"{query.Count()} record(s) returned\n");
                consoleDisplayCategories(query, ID_DISPLAY);
                string itemChosen = getAnswer("Enter the id number of the category you want: ");
                if (int.TryParse(itemChosen, out int id))
                    cId = id;
                else
                    logger.Info($"Category id {itemChosen} is not a valid ID.");
            }
            else
            {
                logger.Info($"No categories found where name contains {searchItem}.");
            }

            if (cId > NOT_FOUND)
            {
                // Double check if the user hasn't mistyped the ID
                if (!db.Categories.Any(c => c.CategoryId.Equals(cId)))
                {
                    Console.WriteLine($"Error finding category id {cId}");
                    cId = NOT_FOUND;
                }
            }
            return cId;
        }

        /// <summary>
        /// Display all categories
        /// </summary>
        /// <param name="db">database context</param>
        public static void DisplayAllCategories(NWContext db)
        {
            // Display all categories
            var query = db.Categories.OrderBy(c => c.CategoryName);
            Console.WriteLine($"{query.Count()} record(s) returned\n");
            consoleDisplayCategories(query, NAME_DISPLAY);
        }

        /// <summary>
        /// Display all categories and their related active products
        /// </summary>
        /// <param name="db">database context</param>
        public static void DisplayAllCategoriesWithProducts(NWContext db)
        {
            var category = db.Categories.Include("Products").OrderBy(c=> c.CategoryName);
            Console.WriteLine($"{category.Count()} categories returned.");
            consoleDisplayCategories(category, WITH_PRODUCT_DISPLAY);
        }

        /// <summary>
        /// Find and display a category with its active products
        /// </summary>
        /// <param name="db">database context</param>
        public static void DisplayACategoryWithProducts(NWContext db)
        {
            int cId = findACategory(db);
            if (cId > NOT_FOUND)
            {
                var cat = db.Categories.Include("Products").Where(c => c.CategoryId.Equals(cId));
                consoleDisplayCategories(cat, WITH_PRODUCT_DISPLAY);
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
            productMenu = new string[7] { "===PRODUCT MENU===", "1) Add a New Product", "2) Edit a Product", "3) Display Products", "4) Display a Product", "5) Delete a Product","\"q\" to go back" };
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
                    var product = FindAndDisplayOneProduct(db, "Display"); // Display a single product (the whole record)
                }
                else if (choice=="5")
                {
                    // Delete a specified existing record from the Products table (account for Orphans in related tables)
                    Product p = FindAndDisplayOneProduct(db, "Delete");
                    if (p!=null)
                    {
                        if (getAnswer("Okay to delete this product (Y/N)?").ToUpper() == "Y")
                        {
                            // Is this the last product for the category?
                            var query = db.Products.Where(c => c.CategoryId == (int)p.CategoryId);
                            if (query.Count() > 1)
                            {
                                // There is another...
                                logger.Info($"Deleting {p.ProductName}");
                                db.DeleteProduct(p);
                            }
                            else
                            {
                                // This is the last one
                                // Choice of delete product plus remove category or remove neither
                                Console.WriteLine("WARNING: This is the last product for its category.");
                                if (getAnswer("Do you want to delete BOTH the product and its category (Y/N)?").ToUpper() == "Y")
                                    if (getAnswer("**Are you sure you want to delete BOTH (Y/N)?").ToUpper() == "Y")
                                    {
                                        // Delete product and category
                                        var category = db.Categories.FirstOrDefault(c => c.CategoryId == (int)p.CategoryId);
                                        db.DeleteCategory(category);
                                        db.DeleteProduct(p);
                                    }
                            }
                        }
                    }
                }
                Console.WriteLine();
            } while (choice.ToLower() != "q");

        }

        /// <summary>
        /// Find one product; display it.
        /// </summary>
        /// <param name="db">database context</param>
        /// <returns></returns>
        public static Product FindAndDisplayOneProduct(NWContext db, string callingMenu)
        {
            // Display a single product (the whole record)
            var product = FindOneProduct(db, callingMenu);
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
        public static Product FindOneProduct(NWContext db, string callingMenu)
        {
            int pId= NOT_FOUND; // Chosen product ID; start with invalid id NOT_FOUND
            // Display Single Product Search Menu:
            // user can search on Product Name, Category Name, Product ID
            String[] productSearchMenu;
            productSearchMenu = new string[6] {"==="+callingMenu+"===", "---Product Search Menu---", "1) Search on Product Name", "2) Search on Category Name", "3) Search on Product ID", "\"q\" to go back" };
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
                        pId = validateProductId(db, itemChosen);
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
                        consoleDisplayProducts(query, WITH_CATEGORY_DISPLAY);
                        string itemChosen = getAnswer("Enter the id number of the product you want: ");
                        pId = validateProductId(db, itemChosen);
                    }
                    else
                    {
                        logger.Info($"No products found where category name contains {searchItem}.");
                    }
                }
                else if (choice == "3")
                {
                    string searchItem = getAnswer("Enter Product ID: ");
                    pId = validateProductId(db, searchItem);
                }

                Console.WriteLine();
            } while ((choice.ToLower()) != "q" && (pId == NOT_FOUND));

            if (pId > NOT_FOUND)
            {
                // Get product record
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
        /// <param name="db">database context</param>
        public static void AddProduct(NWContext db)
        {
            // Validate the product record after each field but also at the end
            Product product = new Product();
            Boolean isValid = false;
            do
            {
                product.ProductName = getAnswer("Enter product name (required, maximum 40 characters):", "", true);
                isValid = validateProduct(db, product);
                if (isValid)
                {
                    // check that product name hasn't already been used
                    if (db.Products.Any(p => p.ProductName == product.ProductName))
                    {
                        isValid = false;
                        Console.WriteLine("Name exists", new string[] { "ProductName" });
                    }
                }
            } while (!isValid);

            isValid = false;
            do
            {
                product.SupplierId = validateInt("Enter the supplier ID (if any). Press return for ", null);
                isValid = validateProduct(db, product);
                // If the user entered a Supplier Id, make sure it exists
                if (isValid)
                {
                    if ((product.SupplierId != null) && !(db.Suppliers.Any(s => s.SupplierId == product.SupplierId)))
                    {
                        isValid = false;
                        Console.WriteLine($"Supplier {product.SupplierId} does not exist");
                    }
                }
            } while (!isValid);
            isValid = false;
            do
            {
                product.CategoryId = validateInt("Enter the category ID (if any). Press return for ", null);
                isValid = validateProduct(db, product);
                // If the user entered a Category Id, make sure it exists
                if (isValid)
                {
                    if ((product.CategoryId != null) && !(db.Categories.Any(c => c.CategoryId == product.CategoryId)))
                    {
                        isValid = false;
                        Console.WriteLine($"Category { product.CategoryId } does not exist");
                    }
                }
            } while (!isValid);
            isValid = false;
            do
            {
                product.QuantityPerUnit = getAnswer("Enter quantity per unit (maximum 20 characters). Press return for ", "");
                isValid = validateProduct(db, product);
            } while (!isValid);
            isValid = false;
            do
            {
                product.UnitPrice = validateDecimal("Enter the unit price (any fractions of pennies will be rounded). Press return for ", null);
                isValid = validateProduct(db, product);
            } while (!isValid);
            isValid = false;
            do
            {
                product.UnitsInStock = validateShort("Enter the units in stock (0-32767). Press return for ", null);
                isValid = validateProduct(db, product);
            } while (!isValid);
            isValid = false;
            do
            {
                product.UnitsOnOrder = validateShort("Enter the units on order (0-32767). Press return for ", null);
                isValid = validateProduct(db, product);
            } while (!isValid);
            isValid = false;
            do
            {
                product.ReorderLevel = validateShort("Enter the reorder level (0-32767). Press return for ", null);
                isValid = validateProduct(db, product);
            } while (!isValid);
            product.Discontinued = false;

            // Initializes a new instance of the ValidationContext class using the object, the service provider, and dictionary of service consumers.
            ValidationContext context = new ValidationContext(product, null, null);
            List<ValidationResult> results = new List<ValidationResult>(); // Store the errors in a list

            // Validator is a helper class that can be used to validate objects, properties, and methods when it is included in their associated ValidationAttribute attributes.
            // TryValidateObject(Object, ValidationContext, ICollection<ValidationResult>, Boolean) Determines whether the specified object is valid using the validation context, validation results collection, and a value that specifies whether to validate all properties.
            isValid = Validator.TryValidateObject(product, context, results, true);
            if (isValid)
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
        /// <param name="db">database context</param>
        /// <param name="pChoice">product display choice (1-all; 2-discontinued; 3-active)</param>
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
        /// <param name="db">database context</param>
        public static void EditProduct(NWContext db)
        {
            // Find and display the product to edit
            Product p = FindAndDisplayOneProduct(db, "Edit");            
            if (p!=null)
            {
                // Found. Get updated information
                Product updatedProduct = new Product();
                EditProductFields(p, updatedProduct, db);

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
                    // If the user entered an updated Category Id, make sure it exists
                    if ((updatedProduct.CategoryId != p.CategoryId) && (!db.Categories.Any(c => c.CategoryId == updatedProduct.CategoryId)))
                    {
                        isValid = false;
                        results.Add(new ValidationResult("Category does not exist", new string[] { "CategoryId" }));
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
        public static void EditProductFields(Product p, Product updatedProduct, NWContext db)
        {
            // Validate the product record after each field but also at the end
            // Update product name
            Boolean isValid = false;
            do
            {
                updatedProduct.ProductName = p.ProductName;
                String answer = getAnswer($"Enter new product name (required, maximum 40 characters) or press return to keep { p.ProductName}", p.ProductName);
                // A return will keep the current name
                if (answer != "")
                {
                    updatedProduct.ProductName = answer;
                    isValid = validateProduct(db, updatedProduct);
                    if (isValid)
                    {
                        // check that product name hasn't already been used
                        if (db.Products.Any(up => up.ProductName == updatedProduct.ProductName))
                        {
                            isValid = false;
                            Console.WriteLine("Name exists", new string[] { "ProductName" });
                        }
                    }
                }
                else
                    isValid = true;
            } while (!isValid);

            isValid = false;
            do
            {
                updatedProduct.SupplierId = validateInt($"Enter new supplier ID or press return to keep {p.SupplierId}", p.SupplierId);
                isValid = validateProduct(db, updatedProduct);
                if (isValid)
                {
                    // If the user entered a Supplier Id, make sure it exists
                    if ((updatedProduct.SupplierId != null) && !(db.Suppliers.Any(s => s.SupplierId == updatedProduct.SupplierId)))
                    {
                        isValid = false;
                        Console.WriteLine($"Supplier {updatedProduct.SupplierId} does not exist");
                    }
                }
            } while (!isValid);
            isValid = false;
            do
            {
                updatedProduct.CategoryId = validateInt($"Enter new category ID or press return to keep {p.CategoryId}", p.CategoryId);
                isValid = validateProduct(db, updatedProduct);
                // If the user entered a Category Id, make sure it exists
                if (isValid)
                {
                    if ((updatedProduct.CategoryId != null) && !(db.Categories.Any(c => c.CategoryId == updatedProduct.CategoryId)))
                    {
                        isValid = false;
                        Console.WriteLine($"Category { "updatedProduct.CategoryId" } does not exist");
                    }
                }
            } while (!isValid);

            // Update quantity per unit
            isValid = false;
            do
            {
                updatedProduct.QuantityPerUnit = p.QuantityPerUnit;
                String answer = getAnswer($"Enter new quantity per unit (maximum 20 characters) or press return to keep {p.QuantityPerUnit} ", p.QuantityPerUnit);
                if (answer != "")
                    updatedProduct.QuantityPerUnit = answer;
                isValid = validateProduct(db, updatedProduct);
            } while (!isValid);
            isValid = false;
            do
            {
                updatedProduct.UnitPrice = validateDecimal($"Enter new unit price (any fractions of cents will be rounded) or press return to keep {p.UnitPrice,0:c2} ", p.UnitPrice);
                isValid = validateProduct(db, updatedProduct);
            } while (!isValid);
            isValid = false;
            do
            {
                updatedProduct.UnitsInStock = validateShort($"Enter new units in stock (0-32767) or press return to keep {p.UnitsInStock}", p.UnitsInStock);
                isValid = validateProduct(db, updatedProduct);
            } while (!isValid);
            isValid = false;
            do
            {
                updatedProduct.UnitsOnOrder = validateShort($"Enter new units on order (0-32767) or press return to keep {p.UnitsOnOrder}", p.UnitsOnOrder);
                isValid = validateProduct(db, updatedProduct);
            } while (!isValid);
            isValid = false;
            do
            {
                updatedProduct.ReorderLevel = validateShort($"Enter new reorder level (0-32767) or press return to keep {p.ReorderLevel}", p.ReorderLevel);
                isValid = validateProduct(db, updatedProduct);
            } while (!isValid);
            isValid = false;
            do
            {
                updatedProduct.Discontinued = validateBoolean($"Enter new discontinued flag (true/false) or press return to keep ", p.Discontinued);
                isValid = validateProduct(db, updatedProduct);
            } while (!isValid);

        }

        #endregion product methods

            // End Program; End Namespace
        }
}
