namespace StartUp;

public class DocumentTableOperations
{
    public static uint AddDocument(string documentName, string content, ref DocumentTable documentTable)
    {
        return documentTable.AddDocument(documentName, content);
        // publish into the document added queue or updated queue
    }

    public static (bool, string) ReadDocumentContent(string documentName, ref DocumentTable documentTable)
    {
        (bool documentFound, int documentIndex) = TableUtils
            .SearchInArray(documentTable.DocumentName,
                ref documentName,
                CompareStrings);

        if (!documentFound) return (false, string.Empty);

        return (false, documentTable.Content[documentIndex]);
    }

    public static void UpdateDocument(string nameOfDocumentToUpdate,
        string newDocumentName,
        string newContent,
        ref DocumentTable documentTable)
    {
        (bool documentFound, int documentIndex) = TableUtils
            .SearchInArray(documentTable.DocumentName,
                ref nameOfDocumentToUpdate,
                CompareStrings);

        if (!documentFound) return;
        // Console.WriteLine("Found to update " + documentIndex);
        // Console.WriteLine("will update " + newContent);
        // Get content to publish 
        // string documentContent = documentTable.Content[documentIndex];
        
        documentTable.UpdateDocument(documentIndex, newDocumentName, newContent);


        // publish into the document added queue or updated queue
    }

    public static void DeleteDocument(string nameOfDocumentToDelete, ref DocumentTable documentTable)
    {
        (bool documentFound, int documentIndex) = TableUtils
            .SearchInArray(documentTable.DocumentName,
                ref nameOfDocumentToDelete,
                CompareStrings);

        if (!documentFound) return;

        // get content to publish
        // string documentContent = documentTable.Content[documentIndex];

        // right now, delete is just setting them as empty. Later add the code to 
        // shift array elements too
        documentTable.DeleteDocument(documentIndex);

        // publish into the document added queue or updated queue
    }

    public static int CompareStrings(string x, string y)
    {
        if (string.IsNullOrWhiteSpace(x)) return -1;
        return x.CompareTo(y);
    }
}