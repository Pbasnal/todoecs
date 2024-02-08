using System.Net;

namespace StartUp;

public struct DocTable
{
    public string[] DocumentName;
    public string[] DocumentContent;

    public uint NumberOfEntries;

    public DocTable(int initialReservedSize)
    {
        DocumentName = new string[initialReservedSize];
        DocumentContent = new string[initialReservedSize];

        NumberOfEntries = 0;
    }
}

// Not creating a static class because we might need to have multiple 
// DocTables. But in general, could the table be static?
public class DocTableOps
{
    private DocTable docTable;

    public DocTableOps(int initialReservedSize = 100)
    {
        docTable = new DocTable(initialReservedSize);
    }

    // what if we create a generic insert? It'll require a long chain of generics though
    /*
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
    */

    // for now, I'm using the custom function for Insert. We can benchmark this as well to see if there is a performance loss
    // because of generic implementation and InsertAll 
    public uint InsertDocument(string docName, string docContent)
    {
        if (docTable.NumberOfEntries < docTable.DocumentName.Length)
        {
            docTable.DocumentName[docTable.NumberOfEntries] = docName;
            docTable.DocumentContent[docTable.NumberOfEntries] = docContent;

            docTable.NumberOfEntries++;
            return docTable.NumberOfEntries - 1;
        }
        return 0;
    }

    public void PrintAllDocs()
    {
        for(int i = 0; i < docTable.NumberOfEntries; i++)
        {
            Console.WriteLine($"{docTable.DocumentName[i]}: {docTable.DocumentContent[i]}");
        }
    }
}
