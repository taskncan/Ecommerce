# Shopping_App

Developed with ASP.NET CORE MVC framework 3.1 version

* You can install missing packages with dotnet restore  command. Or via nuget package manager visual studio. 
* UI Components built with bootstrap css library.
* MYSql used for database operations and table detais here. Database url connection string can be updated from appsettings.json Default Connections.
* Database can be create from “ecommerce.sql” file.

User Controller
•	Login (Stored details in session)
•	Register
•	Logout
•	Delete cart item
•	List all the items with their details
•	Can see his/her orders and can cancel their status
•	Add item to basket
•	Get user basket items and details
•	Can checkout basket items
•	Take payment details and order

Item Controller
•	Details by id
•	Create items (by admin)
•	Edit  (by admin)
•	Delete (by admin)

Admin Controller
•	Can see all of the orders and update their status

Extra Details
•	User role obtained with role column in user table.
    1 -> admin
    0 -> user
•	 Logged in user details stored in session with “USERKEY” key

•	Folder structure

•	AppContext -> Includes Db operations class
•	Controllers -> Web application operations
•	Models -> Includes models for database operations and view models for web app
•	Views -> Includes view templates written by Razor syntax




