## How to build and test

1. Checkout source code
2. Open YukiBlogBackend.sln (VS 2022 was used while under development)
3. Build solution
4. Create the DB by executing this powershell file: "YukiBlogBackend\Blog.Api\DatabaseSetup\CreateFreshDatabase.ps1"
5. Within VS 2022 select "Container(Dockerfile)" as the debug/test profile - this is will prompt Docker Desktop to open
6. Run the application and take note of the port it's running on
7. In the http files (YukiBlogBackend\Blog.Api\HttpTestFiles\), update line #1 in each file to use the correct port
8. Test the application via the http files
9. Once complete you can get a test coverage report by executing this file: "YukiBlogBackend\Blog.Tests\test-coverage.ps1"


## Commands used wrt EF migrations and the database

(#1) is always required to create a migration class, whenever a change in the model happens or first time creation.
It does not apply anything to the DB, just creates the class (with Up and Down methods inside it)
An apply/execute migration (#2) to the DB is still required later. We can automate this later after created.

1. dotnet ef migrations add InitialCreate --output-dir Data\Migrations
2. dotnet ef database update
