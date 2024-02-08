This repo will contian my experiments with Data Oriented Design and Entity Component System written in c#.

## TfIdfOnDots

### Catalog of thoughts while coding
If we represent data in tables how do we represent the operations? In OOP, operations are placed together with the data in a class. We could have used inheritance with interfaces such that the public functions for CRUD operations are same across different tables or data.

But in DOD, we need to reduce the virtual table calls. Also, the data is split between multiple arrays inside a table. Each array can be thought of as a column. This is good with respect to running the same operation of every element of the column, but to insert data into all the columns at once will require a separate function for each table. 

what if we create a generic insert? It'll require a long chain of generics though

For example, for the table DocTable, to have a generic Insert function, we need to create another type which can hold the row to be inserted
public struct DocTableRow
{
    public string DocumentName;
    public string DocumentContent; 
}

we might have to do something like this to ensure that the insert function can not be used
with a wrong combination of row and table
public struct DocTable : ITable<DocTableRow> 
{
    public string[] DocumentName;
    public string[] DocumentContent; 
}

public int Insert<T,R>(R row, T table) where T: ITable<R>

****************
But another method would be to think about merge instead of insert.
public int InsertAll<T>(T table1, T table2)

This function will merge both the tables into table1. This way we can use the same DocTable and insert multiple rows at the same time.
