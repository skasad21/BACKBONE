using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACKBONE.Core.ResponseClasses
{
    public class EQResponse<T>
    {
        public string? Message { get; set; }
        public bool? Success { get; set; }
        public EQResponseData<T>? Data { get; set; }
    }
}



/*
0. Handling a single value:

EQResponse<int> response = new EQResponse<int>();
response.Message = "Success";
response.Success = true;
response.Data = new EQResponseData<int> { SingleValue = 42 };

1. Handling a list of values:

EQResponse<string> response = new EQResponse<string>();
response.Message = "Success";
response.Success = true;
response.Data = new EQResponseData<string> { ListValue = new List<string> { "Hello", "World" } };

2. Handling a DataTable:

EQResponse<DataTable> response = new EQResponse<DataTable>();
response.Message = "Success";
response.Success = true;

DataTable dataTable = new DataTable();
dataTable.Columns.Add("ID", typeof(int));
dataTable.Columns.Add("Name", typeof(string));
dataTable.Rows.Add(1, "John");
dataTable.Rows.Add(2, "Jane");

response.Data = new EQResponseData<DataTable> { DataTableValue = dataTable };

3. A list of objects:

public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}

EQResponse<List<Person>> response = new EQResponse<List<Person>>();
response.Message = "Success";
response.Success = true;

List<Person> people = new List<Person>()
{
    new Person { Name = "John", Age = 25 },
    new Person { Name = "Jane", Age = 30 },
    new Person { Name = "Bob", Age = 40 }
};

response.Data = new EQResponseData<List<Person>> { ListValue = people };


*/
