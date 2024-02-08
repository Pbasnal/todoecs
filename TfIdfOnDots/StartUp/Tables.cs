
using System.ComponentModel;

public struct DocTable
{
    public string[] DocumentName;
    public string[] DocumentContent;

    public int NumberOfEntries;

    public DocTable(int initialReservedSize)
    {
        DocumentName = new string[initialReservedSize];
        DocumentContent = new string[initialReservedSize];

        NumberOfEntries = 0;
    }
}

public static class Algos
{
    public static bool UpdateComponent<T>(int insertAt, T componentValue, ref T[] column)
    {
        if(insertAt < column.Length) {
            column[insertAt] = componentValue;
            return true;
        }
        return false;
    }
}


public struct DocumentTable
{
    public string[] DocumentName { get; set; }
    public string[] Content { get; set; }
    public int NumberOfEntries { get; set; }

    public DocumentTable(int initialReservedSize)
    {
        DocumentName = new string[initialReservedSize];
        Content = new string[initialReservedSize];
        NumberOfEntries = 0;
    }

    public void AddDocument(string documentName, string documentContent)
    {
        // skipping the size check for now
        DocumentName[NumberOfEntries] = documentName;
        Content[NumberOfEntries++] = documentContent;
    }

    internal void UpdateDocument(int documentIndex, string newDocumentName, string newContent)
    {
        DocumentName[documentIndex] = newDocumentName;
        Content[documentIndex] = newContent;
    }

    internal void DeleteDocument(int documentIndex)
    {
        DocumentName[documentIndex] = string.Empty;
        Content[documentIndex] = string.Empty;
    }
}

public struct TokenFrequencyTable
{
    public uint[] DocumentId { get; }
    public string[] Token { get; }
    public uint[] TokenFrequency { get; }
}