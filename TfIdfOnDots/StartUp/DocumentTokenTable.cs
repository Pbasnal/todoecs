namespace StartUp;

public struct DocumentToken
{
    public uint DocumentId { get; set; }
    public string Token { get; set; }

    public DocumentToken(uint docId, string token)
    {
        DocumentId = docId;
        Token = token;
    }

    public bool Equals(DocumentToken docToken)
    {
        return DocumentId == docToken.DocumentId && Token == docToken.Token;
    }
}

public struct DocumentTokenTable
{
    public DocumentToken[] DocumentTokens { get; set; }
    public uint[] TokenFrequency { get; set; }
    public float[] TfScores { get; set; }
    public float[] TfIdfScores { get; set; }

    public int NumberOfEntries { get; set; }

    public DocumentTokenTable(int initialReservedSize)
    {
        DocumentTokens = new DocumentToken[initialReservedSize];
        TokenFrequency = new uint[initialReservedSize];
        TfScores = new float[initialReservedSize];
        TfIdfScores = new float[initialReservedSize];
        NumberOfEntries = 0;
    }
}

public class DocumentTokensOps
{
    public DocumentTokenTable documentTokenTable;

    public DocumentTokensOps(int initialReservedSize = 1000)
    {
        documentTokenTable = new DocumentTokenTable(initialReservedSize);
    }

    public int UpdateTermsOfDocument(uint docId, string docContent)
    {
        // This method needs to clear all tokens of the docId
        // and then update the tokens with the new content

        string[] tokens = docContent.Split(' ');
        HashSet<int> visitedTokens = new();
        // using the worst possible algorithm to implement it at the moment

        Dictionary<string, uint> newTokens = new();

        DocumentToken[] documentTokens = documentTokenTable.DocumentTokens;
        DocumentToken docToken = new();
        docToken.DocumentId = docId;
        for (int i = 0; i < tokens.Length; i++)
        {
            docToken.Token = tokens[i].ToLower();
            bool tokenFound = false;
            for (int j = 0; j < documentTokenTable.NumberOfEntries; j++)
            {
                if (!documentTokens[j].Equals(docToken)) continue;

                if (visitedTokens.Contains(j))
                    documentTokenTable.TokenFrequency[j] += 1;
                else
                {
                    documentTokenTable.TokenFrequency[j] = 1;
                    visitedTokens.Add(j);
                }
                tokenFound = true;
            }

            if (!tokenFound)
            {
                documentTokens[documentTokenTable.NumberOfEntries].DocumentId = docId;
                documentTokens[documentTokenTable.NumberOfEntries].Token = docToken.Token;

                documentTokenTable.TokenFrequency[documentTokenTable.NumberOfEntries] = 1;
                visitedTokens.Add(documentTokenTable.NumberOfEntries);
                documentTokenTable.NumberOfEntries++;
            }
        }

        foreach (var newToken in newTokens.Keys)
        {
            documentTokens[documentTokenTable.NumberOfEntries].DocumentId = docId;
            documentTokens[documentTokenTable.NumberOfEntries].Token = newToken;

            documentTokenTable.TokenFrequency[documentTokenTable.NumberOfEntries] = newTokens[newToken];
            documentTokenTable.NumberOfEntries++;
        }
        return 0;
    }

    public bool RemoveTokensOfDocument(uint docId)
    {
        // this function should remove all the tokens of the given document
        DocumentToken[] documentTokens = documentTokenTable.DocumentTokens;
        for (int j = 0; j < documentTokenTable.NumberOfEntries; j++)
        {
            if (documentTokens[j].DocumentId != docId) continue;

            documentTokens[j].Token = "";
            documentTokenTable.TokenFrequency[j] = 0;
        }
        return false;
    }

    public void PrintAllTokenFrequencies()
    {
        for (int i = 0; i < documentTokenTable.NumberOfEntries; i++)
        {
            uint docId = documentTokenTable.DocumentTokens[i].DocumentId;
            string token = documentTokenTable.DocumentTokens[i].Token;
            uint frequency = documentTokenTable.TokenFrequency[i];

            Console.WriteLine($"[{docId} | {token}]: {frequency}");
        }
    }
}


























