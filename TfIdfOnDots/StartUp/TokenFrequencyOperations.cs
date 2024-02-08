using static StartUp.TokenFrequencyTable;

namespace StartUp;

public class TokenFrequencyOperations
{
    public static void AddTokens(uint docId, string documentContent, ref TokenFrequencyTable tokenFrequencyTable)
    {
        string[] tokens = documentContent
        .Replace(",", "")
        .Replace(".", "")
        .Replace("\'", "")
        .Split(' ');


        for (int i = 0; i < tokens.Length; i++)
        {
            TokenId tokenId = new(docId, tokens[i]);
            (bool tokenFound, int tokenIndex) = TableUtils
           .SearchInArray(tokenFrequencyTable.TokenIds,
               ref tokenId,
               CompareTokens);
            if (tokenFound)
            {
                tokenFrequencyTable.UpdateToken(tokenIndex, docId, tokens[i], 1);
            }
            else
            {
                tokenFrequencyTable.AddToken(docId, tokens[i], 1);
            }
        }
    }

    public static (bool, TokenId, int) ReadTokenFrequency(int id, ref TokenFrequencyTable tokenFrequencyTable)
    {
        if (id < tokenFrequencyTable.NumberOfEntries)
            return (true, tokenFrequencyTable.TokenIds[id], tokenFrequencyTable.TokenFrequency[id]);
        return (false, default, 0);
    }


    public static int CompareTokens(TokenId x, TokenId y)
    {
        if (string.IsNullOrWhiteSpace(x.Token)) return -1;
        int docIdComparison = x.DocumentId.CompareTo(y.DocumentId);
        if (docIdComparison != 0) return docIdComparison;

        return x.Token.CompareTo(y.Token);
    }
}
