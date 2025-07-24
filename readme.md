Commands used wrt EF migrations and the database

(#1) is always required to create a migration class, whenever a change in the model happens or first time creation.
It does not apply anything to the DB, just creates the class (with Up and Down methods inside it)
An apply/execute migration (#2) to the DB is still required later. We can automate this later after created.

1. dotnet ef migrations add InitialCreate --output-dir Data\Migrations
2. dotnet ef database update
