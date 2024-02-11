using StartUp;

namespace TfIdfDod.UnitTests;

public class TokensTableTests
{
    [Fact]
    public void InsertTokensAddsUniqueTokens()
    {
        TokensTableOps tokenOps = new();

        Dictionary<string, string> documents = new Dictionary<string, string>{
            {"doc1", "The Dirtmouth town in Hollow Knight has a charm eerily similar to Ashina's outskirts in Sekiro."},
            {"doc2", "Cross-references are noted between anime 'Attack on Titan' and Elden Ring's cathedral of the deep."},
            {"doc3", "Elden Ring's universe pulls a 'Fullmetal Alchemist' with its transmutation-based magic system."},
            {"doc4", "Sekiro's Headless apes share a uncanny resemblance with 'Bleach's' Menos Grande Hollows."},
            {"doc5", "Dialogue in Elden Ring 'May you find your worth in the waking world', shares commonality with the anime 'Gurren Lagann'."},
            {"doc6", "Focused nail arts in Hollow Knight share striking similarities with 'Samurai Champloo's' sword artistry."},
            {"doc7", "Hopes and dreams in 'Sword Art Online' echoes Elden Ring's Elden Lord's warning 'May you find your worth in the waking world'."},
            {"doc8", "The Bell Demon in Sekiro, rings in resonance with the cursed bells in anime 'Noragami'."},
            {"doc9", "The White Palace in Hollow Knight holds mysterious echoes of 'The Castle in the Sky' from anime studio Ghibli."},
            {"doc10", "Approach to combat in Elden Ring speaks volumes of 'Berserk's' brutal and visceral battlescapes."}
        };

        HashSet<string> uniqueTokens = documents.Values
            .Select(content => content.ToLower().Split(' '))
            .SelectMany(c => c)
            .ToHashSet();

        foreach (string documentName in documents.Keys)
        {
            tokenOps.InsertTokens(documents[documentName]);
        }

        Assert.Equal(uniqueTokens.Count, (int)tokenOps.tokensTable.NumberOfEntries);


        HashSet<string> tokensInTable = new();
        for (int i = 0; i < tokenOps.tokensTable.NumberOfEntries; i++)
        {
            string token = tokenOps.tokensTable.Tokens[i];
            Assert.Contains(token, uniqueTokens);

            Assert.DoesNotContain(token, tokensInTable);
            tokensInTable.Add(token);
        }
    }

    [Fact]
    public void RemovingTokensFirstReducesDocumentCountAndThenDeletesTheToken()
    {
        TokensTableOps tokenOps = new();

        Dictionary<string, string> documents = new Dictionary<string, string>{
            {"doc1", "this is a sample document to test removing tokens"},
            {"doc2", "this is a document to test removing tokens"},
        };

        int numberOfUniqueTokens = documents.Values
            .Select(c => c.ToLower().Split(' '))
            .SelectMany(c => c)
            .ToHashSet().Count;

        HashSet<string> uniqueTokensInDoc2 = documents["doc2"]
            .ToLower()
            .Split(' ')
            .ToHashSet();

        tokenOps.InsertTokens(documents["doc1"]);
        tokenOps.InsertTokens(documents["doc2"]);

        tokenOps.RemoveTokens(documents["doc1"]);

        // NumberOfEntries doesn't get updated by RemoveTokens at the moment
        // so it'll stay same
        Assert.Equal(numberOfUniqueTokens, (int)tokenOps.tokensTable.NumberOfEntries);

        HashSet<string> tokensInTable = new();
        for (int i = 0; i < tokenOps.tokensTable.NumberOfEntries; i++)
        {
            string token = tokenOps.tokensTable.Tokens[i];

            if(token == string.Empty) continue;

            Assert.Contains(token, uniqueTokensInDoc2);

            Assert.DoesNotContain(token, tokensInTable);
            tokensInTable.Add(token);
        }
    }
}
























