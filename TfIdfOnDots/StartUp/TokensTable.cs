namespace StartUp;

public struct TokensTable
{
    public string[] Tokens { get; set; }

    // Count of documents that Token[i] occurs in
    public uint[] DocumentCount { get; set; }
    public float[] IdfScores { get; set; }

    public uint NumberOfEntries { get; set; }

    public TokensTable(int initialReservedSize)
    {
        Tokens = new string[initialReservedSize];
        DocumentCount = new uint[initialReservedSize];
        IdfScores = new float[initialReservedSize];
    }
}

public class TokensTableOps
{
    public TokensTable tokensTable ;

    public TokensTableOps(int initialReservedSize = 1000)
    {
        tokensTable = new(initialReservedSize);
    }

    public uint InsertTokens(string docContent)
    {
        string[] tokens = docContent.ToLower().Split(' ')
                                .ToHashSet()
                                .ToArray();

        for (int i = 0; i < tokens.Length; i++)
        {
            bool tokenExists = false;
            for (int j = 0; j < tokensTable.NumberOfEntries; j++)
            {
                if (tokens[i] == tokensTable.Tokens[j])
                {
                    tokensTable.DocumentCount[j]++;
                    tokenExists = true;
                }
            }

            if (!tokenExists)
            {
                tokensTable.Tokens[tokensTable.NumberOfEntries] = tokens[i];
                tokensTable.DocumentCount[tokensTable.NumberOfEntries] = 1;
                tokensTable.NumberOfEntries++;
            }
        }
        return 0;
    }

    public uint RemoveTokens(string docContent)
    {
        HashSet<string> tokens = docContent.ToLower().Split(' ')
                                .ToHashSet();

        for (int j = 0; j < tokensTable.NumberOfEntries; j++)
        {
            if (tokens.Contains(tokensTable.Tokens[j]))
            {
                tokensTable.DocumentCount[j]--;
                if(tokensTable.DocumentCount[j] == 0)
                {
                    tokensTable.Tokens[j] = string.Empty;
                }
            }
        }

        return 0;
    }

    public void PrintAllTokens()
    {
        for (int i = 0; i < tokensTable.NumberOfEntries; i++)
        {
            Console.WriteLine($"'{tokensTable.Tokens[i]}' is in {tokensTable.DocumentCount[i]} documents");
        }
    }
}

