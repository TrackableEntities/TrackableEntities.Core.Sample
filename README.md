## Trackable Entities Core Sample

Sample solution using Trackable Entities with .NET Core.

## NorthwindSlim Database

- This sample uses a skinny version of the Northwind sample database, called _NorthwindSlim_, which you can download from http://bit.ly/northwindslim.
- Open SQL Server Management Studio, connect to (localdb)\MsSqlLocalDb and create a new database named _NorthwindSlim_. Then run the NorthwindSlim.sql file, which will create all the database tables and populate them with data.

## Generated Trackable Entities

- Add a .NET 4.6.1 Class Library project called _NetCoreSample.Entities.Generated_.
- Add a **Data Connection** in Visual Studio pointing to the NorthwindSlim database on (localdb)\MsSqlLocalDb.
- Install NuGet packages:
    + EntityFramework
    + Install TrackableEntities.EF.6
    + TrackableEntities.CodeTemplates.Service.Net45
- Add a new Item, select Data, then **ADO.NET Entity Data Model**, and name it NorthwindSlim.
    + Choose Code First from Database, then select the NorthwindSlim data connection you created earlier.
    + Select all the tables and click Finish.

## Trackable Entities NetStandard Library
- Add a NetStandard class library called _NetCoreSample.Entities_
    + Install NuGet package: TrackableEntities.Common.Core -Pre
- Add a reference to **System.ComponentModel.DataAnnotations**.
- **Add entities** (do not link) from the NetCoreSample.Entities.Generated project.
- Edit each class add a using for **TrackableEntities.Common.Core**.

## Data Migrations

- Add a new **ASP.NET Core** web application
    + Select Web API template targeting .NET Core with ASP.NET Core 2.0.
- Install NuGet packages:
    + Microsoft.EntityFrameworkCore.SqlServer
    + TrackableEntities.EF.Core -Pre
    + Microsoft.EntityFrameworkCore.Tools
- Add a reference from the Web project to the Entities project.
    + Then build the solution.
- Edit the Web csproj file to add the following to the last ItemGroup

    ```xml
    <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="2.0.0" />
    ```

- Add an App_Data directory to the Web project.
- Update appsettings.json to add a connection string to a local database file

    ```json
    "ConnectionStrings": {
    "NetCoreSample": "Data Source=(localdb)\\MSSQLLocalDB;initial catalog=NetCoreSample;Integrated Security=True; MultipleActiveResultSets=True"
    }
    ```

- Add a NorthwindSlimContext class to the Web project.

    ```csharp
    public class NorthwindSlimContext : DbContext
    {
        public NorthwindSlimContext(DbContextOptions<NorthwindSlimContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
    }
    ```

- Add a NorthwindSlimContextFactory class to the Web project.


    ```csharp
    public class NorthwindSlimContextFactory : IDesignTimeDbContextFactory<NorthwindSlimContext>
    {
        public NorthwindSlimContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<NorthwindSlimContext>();
            optionsBuilder.UseSqlServer(
                "Data Source=(localdb)\\MSSQLLocalDB;initial catalog=NetCoreSample;Integrated Security=True; MultipleActiveResultSets=True");
            return new NorthwindSlimContext(optionsBuilder.Options);
        }
    }
    ```

- Add an initial migration.
    + Open command prompt at Web project to add an initial migration.

    ```
    dotnet ef migrations add Initial
    ```

- Use the ef tools to create the database.

    ```
    dotnet ef database update
    ```

- Configure services for DI
    + Add code in Startup.ConfigureServices to add the DbContext class

    ```csharp
    var connectionString = Configuration.GetConnectionString("NetCoreSample");
    services.AddDbContext<NorthwindSlimContext>(options => options.UseSqlServer(connectionString));
    ```

- Next