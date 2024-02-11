namespace StartUp;

public class TfIdfScoreOps
{
    public void CalculateTheTfIdfScores(ref DocTable docTable,
            ref DocumentTokenTable docTokenTable,
            ref TokensTable tokensTable)
    {
        for (int docId = 0; docId < docTable.NumberOfEntries; docId++)
        {
            uint totalTokensInDoc = docTable.TokenCounts[docId];

            DocumentToken documentToken;
            for (int tokenId = 0; tokenId < docTokenTable.NumberOfEntries; tokenId++)
            {
                documentToken = docTokenTable.DocumentTokens[tokenId];
                if (documentToken.DocumentId == docId)
                {
                    docTokenTable.TfScores[tokenId] = (float)docTokenTable.TokenFrequency[tokenId] / totalTokensInDoc;
                }
            }

            for (int tokenId = 0; tokenId < tokensTable.NumberOfEntries; tokenId++)
            {
                tokensTable.IdfScores[tokenId] = MathF.Log2((float)docTable.NumberOfEntries / (tokensTable.DocumentCount[tokenId] + 1));
            }
        }

        for (int tknId = 0; tknId < tokensTable.NumberOfEntries; tknId++)
        {
            string token = tokensTable.Tokens[tknId];
            bool tokenFound = false;
            for (int docTknId = 0; docTknId < docTokenTable.NumberOfEntries; docTknId++)
            {
                if (token == docTokenTable.DocumentTokens[docTknId].Token)
                {
                    docTokenTable.TfIdfScores[docTknId] = docTokenTable.TfScores[docTknId] * tokensTable.IdfScores[tknId];

                    tokenFound = true;
                }
            }
            if (!tokenFound)
            {
                Console.WriteLine("0 score!");
            }
        }
    }
}



















