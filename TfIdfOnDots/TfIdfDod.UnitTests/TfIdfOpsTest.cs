using StartUp;

namespace TfIdfDod.UnitTests;

public class TfIdfOpsTest
{
    [Fact]
    public void TestTfIdfCalculations()
    {

        TfIdfScoreOps tfIdfScoreOps = new();
        DocTableOps docTableOps = new();
        DocumentTokensOps docTokenOps = new();
        TokensTableOps tokensTableOps = new();

        Dictionary<string, string> documents = new Dictionary<string, string>{
            // {"doc1", "The Dirtmouth town in Hollow"},
            // {"doc2", "The Dirtmouth town in Hollow Knight"},

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

        foreach (var docName in documents.Keys)
        {
            string content = documents[docName];

            uint docId = docTableOps.InsertDocument(docName, content);
            docTokenOps.UpdateTermsOfDocument(docId, content);
            tokensTableOps.InsertTokens(content);
        }


        tfIdfScoreOps.CalculateTheTfIdfScores(ref docTableOps.docTable,
            ref docTokenOps.documentTokenTable,
            ref tokensTableOps.tokensTable);

        for (int i = 0; i < tokensTableOps.tokensTable.NumberOfEntries; i++)
        {
            Assert.True(tokensTableOps.tokensTable.IdfScores[i] > 0);
        }


        for (int i = 0; i < docTokenOps.documentTokenTable.NumberOfEntries; i++)
        {
            Assert.True(docTokenOps.documentTokenTable.TfScores[i] > 0);
            Assert.True(docTokenOps.documentTokenTable.TfIdfScores[i] > 0);
        }
    }
}
