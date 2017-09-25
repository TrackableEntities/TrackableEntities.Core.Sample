## Trackable Entities Core Sample

Sample solution using Trackable Entities with .NET Core.

## NorthwindSlim Database

- This sample uses a skinny version of the Northwind sample database, called _NorthwindSlim_, which you can download from http://bit.ly/northwindslim.
- Open SQL Server Management Studio, connect to (localdb)\MsSqlLocalDb and create a new database named _NorthwindSlim_. Then run the NorthwindSlim.sql file, which will create all the database tables and populate them with data.

## Generate Trackable Entities

- Add a .NET 4.6.1 Class Library project called _NetCoreSample.Entities.Generated_.
- Add a **Data Connection** in Visual Studio pointing to the NorthwindSlim database on (localdb)\MsSqlLocalDb.
- Install NuGet packages:
    + EntityFramework
    + Install TrackableEntities.EF.6
    + TrackableEntities.CodeTemplates.Service.Net45
- Add a new Item, select Data, then **ADO.NET Entity Data Model**, and name it NorthwindSlim.
    + Choose Code First from Database, then select the NorthwindSlim data connection you created earlier.
    + Select all the tables and click Finish.

## Copy Generated Entities to NetStandard Library
- Add a NetStandard class library called _NetCoreSample.Entities_
    + Install NuGet package: TrackableEntities.Common.Core -Pre
- Add a reference to **System.ComponentModel.DataAnnotations**.
- **Add entities** (do not link) from the NetCoreSample.Entities.Generated project.
- Edit each class add a using for **TrackableEntities.Common.Core**.


