namespace StartUp;

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

    public uint AddDocument(string documentName, string documentContent)
    {
        // skipping the size check for now
        DocumentName[NumberOfEntries] = documentName;
        Content[NumberOfEntries++] = documentContent;

        return (uint)(NumberOfEntries - 1);
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
    public struct TokenId
    {
        public uint DocumentId { get; set; }
        public string Token { get; set; }

        public TokenId(uint docId, string token)
        {
            DocumentId = docId;
            Token = token;
        }
    }

    public TokenId[] TokenIds { get; set; }
    public int[] TokenFrequency { get; }
    public int NumberOfEntries { get; set; }

    public TokenFrequencyTable(int initialReservedSize)
    {
        TokenIds = new TokenId[initialReservedSize];
        TokenFrequency = new int[initialReservedSize];
        NumberOfEntries = 0;
    }

    public int AddToken(uint docId, string token, int count)
    {
        // skipping the size check for now
        TokenIds[NumberOfEntries].DocumentId = docId;
        TokenIds[NumberOfEntries].Token = token;
        TokenFrequency[NumberOfEntries] += count;

        NumberOfEntries++;
        return NumberOfEntries - 1;
    }

    public void UpdateToken(int id, uint docId, string token, int count)
    {
        // skipping the size check for now
        TokenIds[id].DocumentId = docId;
        TokenIds[id].Token = token;
        TokenFrequency[id] += count;
    }

    public void RemoveToken(int id)
    {
        // skipping the size check for now
        TokenIds[id].DocumentId = 0;
        TokenIds[id].Token = string.Empty;
        TokenFrequency[id] = 0;
    }
}