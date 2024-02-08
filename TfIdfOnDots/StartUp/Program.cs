using static StartUp.TokenFrequencyTable;

namespace StartUp;

public class Bootstrap
{
   public static void Main(string[] args)
{
      DocTableTests docTableTests = new();
      docTableTests.RunTests();
   }

   public static void Run()
   {
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

      DocumentTable documentTable = new(10);
      TokenFrequencyTable tokenFrequencyTable = new(1000);
      foreach (string documentName in documents.Keys)
      {
         uint docId = DocumentTableOperations.AddDocument(documentName,
            documents[documentName],
            ref documentTable);

         TokenFrequencyOperations.AddTokens(docId, documents[documentName], ref tokenFrequencyTable);
      }

      // reading docs
      foreach (string documentName in documents.Keys)
      {
         (bool docFound, string content) = DocumentTableOperations
            .ReadDocumentContent(documentName, ref documentTable);

         Console.WriteLine($"{documentName}: {content}");
      }

      bool readNextToken;
      TokenId tokenId;
      int tokenFrequency;
      int tokenIdToRead = 0;
      (readNextToken, tokenId, tokenFrequency) = TokenFrequencyOperations.ReadTokenFrequency(tokenIdToRead, ref tokenFrequencyTable);
      while (readNextToken)
      {
         Console.WriteLine($"{tokenId.DocumentId}: {tokenId.Token} > {tokenFrequency}");
         tokenIdToRead++;
         (readNextToken, tokenId, tokenFrequency) = TokenFrequencyOperations.ReadTokenFrequency(tokenIdToRead, ref tokenFrequencyTable);
      }

      documents = new Dictionary<string, string>{
         {"doc1", "Analyzing combat evolution in Hollow Knight to Sekiro, leads to better understanding of player immersion."},
         {"doc2", "Anime often portray deep themes. Hollow Knight series exemplifies this, drawing parallels with Sekiro."},
         {"doc3", "Elden Ring invites comparison to Sekiro but offers distinctive gameplay, inspired by epic anime plots."},
         {"doc4", "Sekiro's intricate combat mechanics echoes similarities with Hollow Knight's nail arts combat system."},
         {"doc5", "Elden Ring is inspired by high-fantasy anime, featuring world-building similar to Sekiro's Ashina deployment."},
         {"doc6", "Comparing speedruns in Hollow Knight and Elden Ring. Patterns in Sekiro that could be utilized."},
         {"doc7", "Effects of Hollow Knight's impressive art style in shaping character attachment, seen in anime fandom."},
         {"doc8", "Comparing the exploration depth in Sekiro versus Elden Ring with respect to their anime adaptations."},
         {"doc9", "Effects of powerful world narratives as seen in anime, Sekiro and Hollow Knight on player engagement."},
         {"doc10", "Analyzing the influence of anime on the creation of immersive gaming experiences in Elden Ring."}
      };

      foreach (string documentName in documents.Keys)
      {
         DocumentTableOperations.UpdateDocument(documentName,
            "new" + documentName,
            documents[documentName],
            ref documentTable);
      }

      // reading docs
      foreach (string documentName in documents.Keys)
      {
         (bool docFound, string content) = DocumentTableOperations
            .ReadDocumentContent("new" + documentName, ref documentTable);

         Console.WriteLine($"{documentName}: {content}");
      }

      foreach (string documentName in documents.Keys)
      {
         DocumentTableOperations.DeleteDocument("new" + documentName,
            ref documentTable);
      }

      // reading docs
      foreach (string documentName in documents.Keys)
      {
         (bool docFound, string content) = DocumentTableOperations
            .ReadDocumentContent("new" + documentName, ref documentTable);

         Console.WriteLine($"{documentName}: {content}");
      }
   }
}