namespace StartUp;

public interface Tests
{
    void RunTests();
}

public class DocTableTests : Tests
{
    public List<Action> testsToRun;

    public DocTableTests()
    {
        testsToRun = new List<Action>{
            InitialiseDocTable,
            UpdateDocuments,
            RemoveDocuments
        };
    }

    public void RunTests()
    {
        foreach (var test in testsToRun)
        {
            test?.Invoke();
        }
    }

    private void InitialiseDocTable()
    {
        DocTableOps docTableOps = new(100);

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

        foreach (string documentName in documents.Keys)
        {
            docTableOps.InsertDocument(documentName, documents[documentName]);
        }

        docTableOps.PrintAllDocs();
    }

    private void UpdateDocuments()
    {
        DocTableOps docTableOps = new(100);

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

        int index = 0;

        Console.WriteLine("\nDocument updates");
        foreach (string documentName in documents.Keys)
        {
            docTableOps.InsertDocument($"{index}doc", $"{index}content");
            docTableOps.PrintDoc(index);

            docTableOps.UpdateDocument(index, documentName, documents[documentName]);
            docTableOps.PrintDoc(index);
            index++;
        }
    }

    private void RemoveDocuments()
    {
        DocTableOps docTableOps = new(100);

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

        Console.WriteLine("\nDocument insertion for removing");
        foreach (string documentName in documents.Keys)
        {
            docTableOps.InsertDocument(documentName, documents[documentName]);
        }
        
        docTableOps.PrintAllDocs();
        
        while (!docTableOps.IsEmpty())
        {
            docTableOps.RemoveDocument(0);
        }

        Console.WriteLine("After removing all docs");
        docTableOps.PrintAllDocs();
    }
}
