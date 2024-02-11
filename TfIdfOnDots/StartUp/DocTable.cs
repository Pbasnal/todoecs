namespace StartUp;

public struct DocTable
{
    public string[] DocumentName;
    public string[] DocumentContent;
    public uint[] TokenCounts;

    public uint NumberOfEntries;

    public DocTable(int initialReservedSize)
    {
        DocumentName = new string[initialReservedSize];
        DocumentContent = new string[initialReservedSize];
        TokenCounts = new uint[initialReservedSize];
        
        NumberOfEntries = 0;
    }
}

// Not creating a static class because we might need to have multiple 
// DocTables. But in general, could the table be static?
public class DocTableOps
{
    public DocTable docTable;

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
            docTable.TokenCounts[docTable.NumberOfEntries] = (uint)docContent.Split(' ').Length;

            docTable.NumberOfEntries++;
            return docTable.NumberOfEntries - 1;
        }
        return 0;
    }

    public bool UpdateDocument(int index, string newDocName, string documentContent)
    {
        if (index < docTable.DocumentName.Length)
        {
            docTable.DocumentName[index] = newDocName;
            docTable.DocumentContent[index] = documentContent;
            docTable.TokenCounts[index] = (uint)documentContent.Split(' ').Length;

            return true;
        }
        return false;
    }

    public bool RemoveDocument(int index)
    {
        if (index >= docTable.NumberOfEntries || index < 0) return false;

        for (int i = index; i < docTable.NumberOfEntries; i++)
        {
            docTable.DocumentName[i] = docTable.DocumentName[i + 1];
        }
        for (int i = index; i < docTable.NumberOfEntries; i++)
        {
            docTable.DocumentContent[index] = docTable.DocumentContent[i + 1];
        }

        docTable.NumberOfEntries--;
        return true;
    }

    public bool IsEmpty() => docTable.NumberOfEntries == 0;

    public void PrintDoc(int index)
    {
        Console.WriteLine($"{docTable.DocumentName[index]}: {docTable.DocumentContent[index]}");
    }

    public void PrintAllDocs()
    {
        for (int i = 0; i < docTable.NumberOfEntries; i++)
        {
            Console.WriteLine($"{docTable.DocumentName[i]}: {docTable.DocumentContent[i]}");
        }
    }
}
