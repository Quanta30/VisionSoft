# Documentation: Model Class

## Overview

The `Model` class is a generic data access layer designed to simplify database interactions for individual tables within your Blazor application. It acts as a bridge between your UI components and the database, abstracting the underlying SQL queries for common CRUD (Create, Read, Update, Delete) operations. It uses a `Dictionary<string, string>` to represent a single record, where the dictionary keys are the column names and the values are the record's data. This approach allows for dynamic handling of different table structures without needing to create a separate class for each database table.

---

## Key Features

- **Dynamic Record Representation**: Uses a `Dictionary` to hold record data, making it adaptable to any table structure.
- **Automated CRUD Operations**: Provides simple methods like `Save()`, `Update()`, and `Delete()` to manage records.
- **Transactional Support**: Can add its generated queries to a `Transaction` object for operations involving multiple tables.
- **Automatic Primary Key Generation**: Can automatically generate the next primary key for both simple numeric keys and keys with a string prefix (e.g., "INV-001").
- **State Management**: Includes methods to populate the model from a `DataRow`, clear its state for a new entry, and re-initialize it.

---

## Properties

### `public Dictionary<string, string> dict`
This is the primary dictionary that holds the data for the current record.
- **Key**: `string` - The name of a column in the database table.
- **Value**: `string` - The corresponding value for that column.
This dictionary is intended to be the main data source for binding to your UI components (e.g., `T_Input`, `T_Select`).

### `public Dictionary<string, string> dict2`
This is a secondary dictionary that serves as a snapshot of the record's original state, captured when the `Populate()` method is called.
- **Purpose**: Its primary function is to hold the original primary key value of a record being edited. This is crucial for the `WHERE` clause in an `UPDATE` statement, ensuring that you update the correct record even if the primary key itself is editable in the UI.

---

## Constructors

### `public Model(string tableName, string primaryKeyColumn)`
Use this constructor for tables that have a simple, numeric primary key.
- **`tableName`**: The name of the database table this model will manage.
- **`primaryKeyColumn`**: The name of the column that serves as the primary key.

### `public Model(string tableName, string primaryKeyColumn, string primaryKeyPrefix)`
Use this constructor for tables where the primary key is a string composed of a fixed prefix and a number (e.g., "INV-101", "CUST-056").
- **`tableName`**: The name of the database table.
- **`primaryKeyColumn`**: The name of the primary key column.
- **`primaryKeyPrefix`**: The fixed string prefix for the primary key.

---

## Public Methods

### CRUD Operations

- **`bool Save()`**: Inserts the current data in `dict` as a new record into the table.
- **`void Save(Transaction transaction)`**: Generates an `INSERT` SQL statement and adds it to the provided `Transaction` object.
- **`bool Update()`**: Updates an existing record. It uses the primary key stored in `dict2` to identify which record to update.
- **`void Update(Transaction transaction)`**: Generates an `UPDATE` SQL statement and adds it to the provided `Transaction` object.
- **`bool Delete(int key)`**: Deletes a record from the table based on its numeric primary key.

### Data Population

- **`void Populate(DataRow row)`**: Fills the `dict` with values from a given `DataRow`. It also copies this state to `dict2` to preserve the original record for updates.
- **`void PopulateViaKey(object key)`**: Fetches a record from the database using its primary key and then calls `Populate()` to load the data.

### State Management

- **`void InitiateKeys()`**: Fetches all column names for the specified table and initializes the `dict` with these keys and empty string values. This is called automatically by the constructor.
- **`void Clear()`**: Resets the model to a clean state by clearing all values in `dict` and generating a new primary key.

### Primary Key Management

- **`void SetPrimaryKey()`**: Generates the next available numeric primary key and sets it in `dict`.
- **`void SetPrimaryKey(string prefix)`**: Generates the next available prefixed primary key (e.g., if the last key was "INV-101", the next will be "INV-102") and sets it in `dict`.
- **`void SetPrefix(string prefix)`**: A public method to change the primary key prefix and regenerate the key accordingly.

---

## Important Security Note

The current implementation of this class builds SQL queries by concatenating strings. **This makes the application highly vulnerable to SQL Injection attacks.** While basic single-quote escaping is used, it is not a sufficient security measure. It is **strongly recommended** to refactor the underlying database helper class (`ClsDatabase`) and the `Transaction` class to use **parameterized queries**. This is the industry-standard way to prevent SQL injection and ensure your application's data is secure.

---

## Example Usage

Here is a typical example of how to use the `Model` class in a Blazor component to save a new customer record.

```csharp
@page "/customer-form"
@using System.Data

// Assume you have your UI components here (T_Input, T_Button, etc.)

@code {
    private Model customerModel;
    private T_Msg Notifier; // Reference to your T_Msg component

    protected override void OnInitialized()
    {
        // Initialize the model for the 'CustomerMaster' table
        // with 'CustomerCode' as the prefixed primary key.
        customerModel = new Model("CustomerMaster", "CustomerCode", "CUST-");
    }

    private async Task HandleSave()
    {
        try
        {
            // The 'dict' is already bound to the UI components.
            // For example: <T_Input @bind-Value="customerModel.dict["CustomerName"]" />

            bool isSuccess = customerModel.Save();

            if (isSuccess)
            {
                await Notifier.Success("Customer saved successfully!");
                customerModel.Clear(); // Clear the form for a new entry.
            }
            else
            {
                await Notifier.Error("Failed to save customer.");
            }
        }
        catch (Exception ex)
        {
            await Notifier.Error($"An error occurred: {ex.Message}");
        }
    }
}
