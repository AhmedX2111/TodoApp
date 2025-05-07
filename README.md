The backend of the Todo App is built with ASP.NET Core and provides a REST API for managing Todo items. The API exposes endpoints for performing CRUD (Create, Read, Update, Delete) operations on Todo items and interacts with a SQL Server database via Entity Framework Core.

Technologies Used
ASP.NET Core for building the API.

Entity Framework Core for database interactions.

SQL Server for storing Todo data.

CORS support to enable communication between the backend and frontend.

Setup Instructions
Follow the steps below to set up the backend of the Todo App.

1. Clone the Repository
First, clone the repository to your local machine.

git clone https://github.com/yourusername/TodoApp.git
cd TodoApp/backend

2. Install .NET SDK

3. Restore Dependencies
   Restore the required dependencies using the .NET CLI.

dotnet restore

4. Set Up the Database
The backend uses Entity Framework Core for database interactions. To create the database schema, apply the migrations as follows:

dotnet ef migrations add InitialCreate
dotnet ef database update

5. Run the Project
Once the setup is complete, run the backend API using the following command:

dotnet run


API Endpoints
The API provides the following endpoints for managing Todos:

GET /api/todo - Retrieve a list of all Todos.

POST /api/todo - Create a new Todo.

GET /api/todo/{id} - Retrieve a specific Todo by ID.

PUT /api/todo/{id} - Update an existing Todo.

DELETE /api/todo/{id} - Delete a Todo by ID.

PATCH /api/todo/{id}/complete - Mark a Todo as completed.

What Was Completed
Developed a RESTful API with the following functionalities:

Create a new Todo.

Retrieve all Todos or a specific Todo by ID.

Update Todo details (title, description, status, priority).

Mark a Todo as completed.

Delete a Todo.

Configured Entity Framework Core with SQL Server to persist data.

Enabled CORS to allow communication between the backend API and the frontend.

What Was Challenging
Handling database migrations and keeping track of changes to the database schema during development.

Ensuring smooth communication between the backend and frontend, particularly dealing with CORS issues.

Implementing robust error handling and input validation to avoid unexpected issues when interacting with the API.
