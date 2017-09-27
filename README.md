## Trackable Entities Core Sample

Sample solution using Trackable Entities with .NET Core.

## NorthwindSlim Database

- This sample uses a skinny version of the Northwind sample database, called _NorthwindSlim_, which you can download from http://bit.ly/northwindslim.
- Open SQL Server Management Studio, connect to (localdb)\MsSqlLocalDb and create a new database named _NorthwindSlim_. Then run the NorthwindSlim.sql file, which will create all the database tables and populate them with data.

## Generated Trackable Entities

- Add a .NET 4.6.1 Class Library project called _NetCoreSample.Entities.Generated_.
    + Open project prpoperties and remove ".Generated" from the default namespace.
- Add a **Data Connection** in Visual Studio pointing to the NorthwindSlim database on (localdb)\MsSqlLocalDb.
- Install NuGet packages:
    + EntityFramework
    + Install TrackableEntities.Common.Core -Pre
    + TrackableEntities.CodeTemplates.Service.Net45
    > Note: Ignore the compiling transformation error.
- Edit EntityType.cs.t4 file in the CodeTemplates/EFModelFromDatabase folder.
    + First go to Tools, Extensions and Updates, and install the Tangible T4 Editor.
    + Remove the following namespace import: `using System.Data.Entity.Spatial`
    + Update the `TrackableEntities` namespace import to: `using TrackableEntities.Common.Core`
- Add a new Item, select Data, then **ADO.NET Entity Data Model**, and name it NorthwindSlim.
    + Choose Code First from Database, then select the NorthwindSlim data connection you created earlier.
    + Select all the tables and click Finish.
    + Build the project.

## Trackable Entities NetStandard Library
- Add a NetStandard class library called _NetCoreSample.Entities_
    + Install NuGet package: TrackableEntities.Common.Core -Pre
- Add a reference to **System.ComponentModel.DataAnnotations**.
- **Link** generated entities from the NetCoreSample.Entities.Generated project.
    + Right-click _NetCoreSample.Entities_ project, select Add New Item
    + Nviate to the Generated entities project and select all .cs files except NorthwindSlim.cs.
    + From the Add button dropdown select **Add As Link**.
    + Build the project.

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

## Web API Controllers

- Update `Startup.ConfigureServices` to handle cyclical references when serializing JSON.

    ```csharp
    services.AddMvc()
        .AddJsonOptions(options =>
            options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.All);
    ```

- Use the DotNet CLI to scaffold a Customer controller
    + Install NuGet package: Microsoft.VisualStudio.Web.CodeGeneration.Design
    + Open a command prompt at the Web project location and run the following:

    ```
    dotnet aspnet-codegenerator --project . controller -name CustomerController -api -outDir Controllers -m Customer -dc NorthwindSlimContext
    ```

    + Remove actions in CustomerController except `GetCustomers` and `GetCustomer`.
    + Refactor `GetCustomers` as an async action.

    ```csharp
    [HttpGet]
    public async Task<IActionResult> GetCustomers()
    {
        var customers = await _context.Customers
            .ToListAsync();
        return Ok(customers);
    }
    ```

    + Run the Web project by pressing Ctrl+F5 and navigate to api/customer and api/customer/ALFKI.

- Add an Order controller.

    ```
    dotnet aspnet-codegenerator --project . controller -name OrderController -api -outDir Controllers -m Order -dc NorthwindSlimContext
    ```

    + Refactor `GetOrders` as an async action with includes.

    ```csharp
    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _context.Orders
            .Include(m => m.Customer)
            .Include(m => m.OrderDetails)
            .ThenInclude(m => m.Product)
            .ToListAsync();
        return Ok(orders);
    }
    ```

    + Also add includes to `GetOrder`.

    ```csharp
    var order = await _context.Orders
        .Include(m => m.Customer)
        .Include(m => m.OrderDetails)
        .ThenInclude(m => m.Product)
        .SingleOrDefaultAsync(m => m.OrderId == id);
    ```

    + Navigate to api/order and api/order/1.

    + Add a `GetOrders` overload that filters by `customerId`.

    ```csharp
    // GET: api/Order/ALFKI
    [HttpGet("{customerId:alpha}")]
    public async Task<IActionResult> GetOrders([FromRoute] string customerId)
    {
        var orders = await _context.Orders
            .Include(m => m.Customer)
            .Include(m => m.OrderDetails)
            .ThenInclude(m => m.Product)
            .Where(m => m.CustomerId == customerId)
            .ToListAsync();
        return Ok(orders);
    }
    ```

    + Navigate to api/order/ALFKI.

- Refactor Put, Post and Delete actions to call `_context.ApplyChanges`.

    + Refactor `PutOrder` to replace setting state to Modified with applying changes nad return the order with an OK response.

    ```csharp
    //_context.Entry(order).State = EntityState.Modified;
    _context.ApplyChanges(order);

    // Populate reference properties
    await _context.LoadRelatedEntitiesAsync(order);

    // Reset tracking state to unchanged
    _context.AcceptChanges(order);

    //return NoContent();
    return Ok(order);
    ```

    + Refactor `PostOrder` to set TrackingState to Added and apply changes.
    + Remove the `id` parameter from `PostOrder`.

    ```csharp
    // Set state to added
    order.TrackingState = TrackingState.Added;

    // Apply changes to context
    _context.ApplyChanges(order);

    // Persist changes
    await _context.SaveChangesAsync();

    // Populate reference properties
    await _context.LoadRelatedEntitiesAsync(order);

    // Reset tracking state to unchanged
    _context.AcceptChanges(order);
    ```

    + Refactor `DeleteOrder` to set TrackingState to Deleted and apply changes,
      and return Ok without an entity.

    ```csharp
    // Retrieve order with details
    var order = await _context.Orders
        .Include(m => m.OrderDetails)
        .SingleOrDefaultAsync(m => m.OrderId == id);
    if (order == null)
    {
        return NotFound();
    }

    // Set tracking state to deleted
    order.TrackingState = TrackingState.Deleted;

    // Detach object graph
    _context.DetachEntities(order);

    // Apply changes to context
    _context.ApplyChanges(order);

    // Persist changes
    await _context.SaveChangesAsync();

    return Ok();
    ```

## Console Client

- Add a .NET 4.6.1 class library project called _NetCoreSample.Entities.Client_.
- Install NuGet packages:
    + EntityFramework
    + TrackableEntities.Client
    + TrackableEntities.CodeTemplates.Client.Net45
- Add new item, Data, ADO.NET Entity Data Model.
    + Select the NorthwindSlim data connection
- Add a .NET 4.6.1 console app called _NetCoreSample.ConsoleClient_.
- Install NuGet packages:
    + TrackableEntities.Client
    + Microsoft.AspNet.WebApi.Client
    + AspNetWebApi2Helpers.Serialization
- Reference the Entities.Client project.
- Add private helper methods.
- Add code to retrieve and update entities.

