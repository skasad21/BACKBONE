using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BACKBONE.DB
{
    public class MssqlDbHelper : IDBHelper
    {
        private readonly string _connectionString;

        public MssqlDbHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // 1. Query<T>
        // Example: IEnumerable<MyModel> results = dapperHelper.Query<MyModel>("SELECT * FROM MyTable");
        public IEnumerable<T> Query<T>(string sql, object? parameters = null, CommandType? commandType = null)
        {
            using (var connection = CreateConnection())
            {
                return connection.Query<T>(sql, parameters, commandType: commandType);
            }
        }

        // 2. QueryAsync<T>
        // Example: IEnumerable<MyModel> results = await dapperHelper.QueryAsync<MyModel>("SELECT * FROM MyTable");
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null, CommandType? commandType = null)
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<T>(sql, parameters, commandType: commandType);
            }
        }

        // 3. QueryFirstOrDefault<T>
        // Example: MyModel result = dapperHelper.QueryFirstOrDefault<MyModel>("SELECT * FROM MyTable WHERE Id = @Id", new { Id = 1 });
        public T QueryFirstOrDefault<T>(string sql, object? parameters = null, CommandType? commandType = null)
        {
            using (var connection = CreateConnection())
            {
                return connection.QueryFirstOrDefault<T>(sql, parameters, commandType: commandType);
            }
        }


        // New Added
        public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null, CommandType? commandType = null)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    // Open the connection if it's not already open
                    if (connection.State != ConnectionState.Open)
                         connection.Open();

                    return await connection.QuerySingleOrDefaultAsync<T>(sql, parameters, commandType: commandType);
                }
            }
            catch (SqlException ex)
            {
                // Log the exception (you could use a logging framework here)
                Console.WriteLine($"SQL Error: {ex.Message}");
                throw; // Re-throw or return a default value (e.g., null)
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"Error: {ex.Message}");
                throw; // Re-throw or return a default value (e.g., null)
            }
        }

        public T? QuerySingleOrDefault<T>(string sql, object? parameters = null, CommandType? commandType = null)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    // Open the connection if it's not already open
                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    // Execute the query synchronously
                    return connection.QuerySingleOrDefault<T>(sql, parameters, commandType: commandType);
                }
            }
            catch (SqlException ex)
            {
                // Log the exception (you could use a logging framework here)
                Console.WriteLine($"SQL Error: {ex.Message}");
                throw; // Re-throw or return a default value (e.g., null)
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"Error: {ex.Message}");
                throw; // Re-throw or return a default value (e.g., null)
            }
        }




        // 4. Execute
        // Example: int rowsAffected = dapperHelper.Execute("UPDATE MyTable SET Column1 = @Value WHERE Id = @Id", new { Value = "NewValue", Id = 1 });
        public int Execute(string sql, object? parameters = null, CommandType? commandType = null)
        {
            using (var connection = CreateConnection())
            {
                return connection.Execute(sql, parameters, commandType: commandType);
            }
        }

        // 5. ExecuteAsync
        // Example: int rowsAffected = await dapperHelper.ExecuteAsync("UPDATE MyTable SET Column1 = @Value WHERE Id = @Id", new { Value = "NewValue", Id = 1 });
        public async Task<int> ExecuteAsync(string sql, object? parameters = null, CommandType? commandType = null)
        {
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync(sql, parameters, commandType: commandType);
            }
        }

        // 6. Insert<T>
        // Example: MyModel insertedObject = dapperHelper.Insert<MyModel>("INSERT INTO MyTable (Column1, Column2) VALUES (@Value1, @Value2)", new { Value1 = "Value1", Value2 = "Value2" });
        public T Insert<T>(string sql, object? parameters = null, CommandType? commandType = null)
        {
            using (var connection = CreateConnection())
            {
                return connection.QuerySingleOrDefault<T>(sql, parameters, commandType: commandType);
            }
        }

        // 7. ExecuteTransaction
        // Example:
        // var commands = new List<SqlCommandInfo>
        // {
        //     new SqlCommandInfo { Sql = "INSERT INTO MyTable (Column1) VALUES (@Value1)", Parameters = new { Value1 = "Value1" } },
        //     new SqlCommandInfo { Sql = "UPDATE MyTable SET Column1 = @Value1 WHERE Id = @Id", Parameters = new { Value1 = "NewValue", Id = 1 } }
        // };
        // int rowsAffected = dapperHelper.ExecuteTransaction(commands);
        public int ExecuteTransaction(IEnumerable<SqlCommandInfo> commands)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int rowsAffected = 0;
                        foreach (var command in commands)
                        {
                            rowsAffected += connection.Execute(command.Sql, command.Parameters, transaction: transaction, commandType: command.CommandType);
                        }
                        transaction.Commit();
                        return rowsAffected;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        // 8. ExecuteTransactionAsync
        // Example: await dapperHelper.ExecuteTransactionAsync(commands);
        public async Task<int> ExecuteTransactionAsync(List<SqlCommandInfo> commands)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var command in commands)
                        {
                            await connection.ExecuteAsync(command.Sql, command.Parameters, transaction: transaction);
                        }
                        transaction.Commit();
                        return commands.Count;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // 9. ExecuteStoredProcedure
        // Example: var results = dapperHelper.ExecuteStoredProcedure("StoredProcedureName", new { Param1 = "Value1" });
        public IEnumerable<IDictionary<string, object>> ExecuteStoredProcedure(string storedProcedureName, object? parameters = null)
        {
            using (var connection = CreateConnection())
            {
                return connection.Query(storedProcedureName, parameters, commandType: CommandType.StoredProcedure).Select(x => (IDictionary<string, object>)x);
            }
        }

        // 10. ExecuteScalar
        // Example: int count = dapperHelper.ExecuteScalar("SELECT COUNT(*) FROM MyTable");
        public int ExecuteScalar(string sql, object? parameters = null, CommandType? commandType = null)
        {
            using (var connection = CreateConnection())
            {
                return connection.ExecuteScalar<int>(sql, parameters, commandType: commandType);
            }
        }

        // 11. QueryMultiple<T>
        // Example:
        // var sql = "SELECT * FROM Customers; SELECT * FROM Orders; SELECT * FROM Products;";
        // var resultSets = dapperHelper.QueryMultiple<dynamic>(sql);
        // var customers = resultSets.ElementAt(0);
        // var orders = resultSets.ElementAt(1);
        // var products = resultSets.ElementAt(2);
        public IEnumerable<IEnumerable<T>> QueryMultiple<T>(string sql, object? parameters = null, CommandType? commandType = null)
        {
            using (var connection = CreateConnection())
            {
                using (var multi = connection.QueryMultiple(sql, parameters, commandType: commandType))
                {
                    var resultSets = new List<IEnumerable<T>>();
                    while (!multi.IsConsumed)
                    {
                        resultSets.Add(multi.Read<T>());
                    }
                    return resultSets;
                }
            }
        }

        // 12. ConvertCollectionToDataTable
        // Example:
        // var dataTable = dapperHelper.ConvertCollectionToDataTable(results);
        public DataTable ConvertCollectionToDataTable<T>(IEnumerable<T> collection)
        {
            var dataTable = new DataTable();
            var properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            foreach (var item in collection)
            {
                var row = dataTable.NewRow();
                foreach (var prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }
    }
}



/*
 
 1. Handling Output Parameters in Stored Procedures

SP:
CREATE PROCEDURE GetEmployeeInfo
    @EmployeeID INT,
    @EmployeeName NVARCHAR(50) OUTPUT,
    @Department NVARCHAR(50) OUTPUT
AS
BEGIN
    SELECT @EmployeeName = Name, @Department = Department FROM Employees WHERE Id = @EmployeeID;
END

C# Code to Handle Output Parameters:
public IDictionary<string, object> ExecuteStoredProcedureWithOutput(string storedProcedureName, DynamicParameters parameters)
{
    using (var connection = CreateConnection())
    {
        connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
        return parameters.ParameterNames.ToDictionary(paramName => paramName, paramName => parameters.Get<object>(paramName));
    }
}

// Usage Example
var parameters = new DynamicParameters();
parameters.Add("@EmployeeID", 1, DbType.Int32);
parameters.Add("@EmployeeName", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
parameters.Add("@Department", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

var result = dapperHelper.ExecuteStoredProcedureWithOutput("GetEmployeeInfo", parameters);

// Now you can access the output parameters like this:
string employeeName = result["@EmployeeName"].ToString();
string department = result["@Department"].ToString();


2. Returning Multiple Result Sets from Stored Procedures

CREATE PROCEDURE GetEmployeeAndDepartments
AS
BEGIN
    SELECT * FROM Employees;
    SELECT * FROM Departments;
END


C# Code to Handle Multiple Result Sets:

public (IEnumerable<Employee>, IEnumerable<Department>) ExecuteStoredProcedureWithMultipleResults(string storedProcedureName)
{
    using (var connection = CreateConnection())
    {
        using (var multi = connection.QueryMultiple(storedProcedureName, commandType: CommandType.StoredProcedure))
        {
            var employees = multi.Read<Employee>();
            var departments = multi.Read<Department>();
            return (employees, departments);
        }
    }
}

// Usage Example
var (employees, departments) = dapperHelper.ExecuteStoredProcedureWithMultipleResults("GetEmployeeAndDepartments");


3. Advanced Dynamic Query Handling

public IEnumerable<dynamic> ExecuteDynamicQuery(string sql, object parameters = null)
{
    using (var connection = CreateConnection())
    {
        return connection.Query(sql, parameters);
    }
}

// Usage Example
var result = dapperHelper.ExecuteDynamicQuery("SELECT * FROM Employees WHERE DepartmentId = @DeptId", new { DeptId = 5 });

foreach (var row in result)
{
    Console.WriteLine($"Employee ID: {row.EmployeeId}, Name: {row.Name}");
}



4. Advanced Usage of ExecuteScalar with Different Return Types

public T ExecuteScalar<T>(string sql, object parameters = null, CommandType? commandType = null)
{
    using (var connection = CreateConnection())
    {
        return connection.ExecuteScalar<T>(sql, parameters, commandType: commandType);
    }
}

// Usage Example (returning an integer)
int employeeCount = dapperHelper.ExecuteScalar<int>("SELECT COUNT(*) FROM Employees WHERE DepartmentId = @DeptId", new { DeptId = 5 });

// Usage Example (returning a DateTime)
DateTime lastLogin = dapperHelper.ExecuteScalar<DateTime>("SELECT MAX(LastLoginDate) FROM Employees WHERE DepartmentId = @DeptId", new { DeptId = 5 });


5. Complex Transactions with Multiple Queries

public int ExecuteComplexTransaction(List<SqlCommandInfo> commands)
{
    using (var connection = CreateConnection())
    {
        connection.Open();
        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                int totalAffectedRows = 0;
                foreach (var command in commands)
                {
                    var rowsAffected = connection.Execute(command.Sql, command.Parameters, transaction: transaction, commandType: command.CommandType);
                    totalAffectedRows += rowsAffected;
                }
                transaction.Commit();
                return totalAffectedRows;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Transaction failed", ex);
            }
        }
    }
}

// Usage Example
var commands = new List<SqlCommandInfo>
{
    new SqlCommandInfo { Sql = "UPDATE Employees SET Name = @Name WHERE EmployeeId = @Id", Parameters = new { Name = "John", Id = 1 }},
    new SqlCommandInfo { Sql = "DELETE FROM Employees WHERE EmployeeId = @Id", Parameters = new { Id = 2 }}
};

int affectedRows = dapperHelper.ExecuteComplexTransaction(commands);



6. Mapping Stored Procedure Results to Strongly Typed Models


CREATE PROCEDURE GetEmployeeDetails
    @EmployeeID INT
AS
BEGIN
    SELECT EmployeeId, Name, DepartmentId, HireDate FROM Employees WHERE EmployeeId = @EmployeeID;
END


C# Code to Map Stored Procedure Results:

public Employee ExecuteStoredProcedureMapping(string storedProcedureName, object parameters)
{
    using (var connection = CreateConnection())
    {
        return connection.QueryFirstOrDefault<Employee>(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
    }
}

// Usage Example
var employee = dapperHelper.ExecuteStoredProcedureMapping("GetEmployeeDetails", new { EmployeeID = 1 });


7. Using ConvertCollectionToDataTable in Bulk Operations

public void BulkInsertEmployees(IEnumerable<Employee> employees)
{
    var dataTable = dapperHelper.ConvertCollectionToDataTable(employees);

    using (var connection = CreateConnection())
    {
        using (var bulkCopy = new SqlBulkCopy((SqlConnection)connection))
        {
            bulkCopy.DestinationTableName = "Employees";
            bulkCopy.WriteToServer(dataTable);
        }
    }
}

// Usage Example
var employees = new List<Employee>
{
    new Employee { EmployeeId = 1, Name = "John", DepartmentId = 1 },
    new Employee { EmployeeId = 2, Name = "Jane", DepartmentId = 2 }
};

dapperHelper.BulkInsertEmployees(employees);




 
 
 
 
 */