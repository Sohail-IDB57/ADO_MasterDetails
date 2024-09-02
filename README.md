# MasterDetailsCRUDAPP

## Overview

The `MasterDetailsCRUDAPP` is a C# Windows Forms application designed to demonstrate CRUD (Create, Read, Update, Delete) operations in a master-details context using ADO.NET. This application connects to a SQL Server database to manage and display data in a hierarchical format, making it a useful example for understanding master-detail relationships in desktop applications.

## Features

- **Master-Detail Interface:** Displays master records and their associated details in a single interface.
- **CRUD Operations:** Provides functionality to create, read, update, and delete records.
- **Database Integration:** Connects to a SQL Server database using ADO.NET.

## Prerequisites

- **.NET Framework:** This project targets .NET Framework version 4.8.
- **SQL Server:** The project uses `MSSQLLocalDB` as the database server. Ensure that you have SQL Server LocalDB installed.

## Getting Started

### Cloning the Repository

To get started with the project, clone the repository using the following command:


git clone https://github.com/Sohail-IDB57/ADO_MasterDetails.git


### Setting Up the Database

1. **Create the Database:**
   - Ensure that you have SQL Server LocalDB installed.
   - Create a database named `TarineeDB` in SQL Server LocalDB. You can do this using SQL Server Management Studio (SSMS) or any SQL client tool.

2. **Run Database Scripts:**
   - Execute any necessary scripts to create tables and insert initial data into the `TarineeDB` database. The specific schema and data are not provided in this repository, so you may need to adapt the code or schema based on your requirements.

### Configuration

The connection strings for the database are defined in the `App.config` file:


<connectionStrings>
    <add name="Master" connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TarineeDB;Integrated Security=True;"
        providerName="System.Data.SqlClient" />
    <add name="MasterDetailsCRUDAPP.Properties.Settings.TarineeDBConnectionString"
        connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TarineeDB;Integrated Security=True"
        providerName="System.Data.SqlClient" />
</connectionStrings>


Ensure that the `Initial Catalog` value matches the name of your database.

### Building and Running the Application

1. **Open the Solution:**
   - Open the `MasterDetailsCRUDAPP.sln` file in Visual Studio.

2. **Build the Project:**
   - Build the project by selecting `Build > Build Solution` from the Visual Studio menu.

3. **Run the Application:**
   - Start the application by pressing `F5` or selecting `Debug > Start Debugging` in Visual Studio.

## Usage

Upon running the application, you will see a Windows Forms interface displaying master records. Select a master record to view and manage its details. You can perform CRUD operations using the provided UI elements.

## Troubleshooting

- **Connection Issues:** Ensure that the `TarineeDB` database exists and the connection strings are correctly configured.
- **Build Errors:** Check for any missing dependencies or version mismatches in the .NET Framework.


## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

