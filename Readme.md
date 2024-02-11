This repo will contian my experiments with Data Oriented Design and Entity Component System written in c#.

## TfIdfOnDots

### Catalog of thoughts while coding

#### Writing functions for data  
If we represent data in tables how do we represent the operations? In OOP, operations are placed together with the data in a class. We could have used inheritance with interfaces such that the public functions for CRUD operations are same across different tables or data.

But in DOD, we want to reduce the virtual table calls. Also, the data is split between multiple arrays inside a table. Each array can be thought of as a column. This is good with respect to running the same operation of every element of the column, but to insert data into all the columns at once will require a separate function for each table. 

what if we create a generic insert? It'll require a long chain of generics though

For example, for the table DocTable, to have a generic Insert function, we need to create another type which can hold the row to be inserted
public struct DocTableRow
{
    public string DocumentName;
    public string DocumentContent; 
}

we might have to do something like this to ensure that the insert function can not be used
with a wrong combination of row and table
public struct DocTable : ITable<DocTableRow> 
{
    public string[] DocumentName;
    public string[] DocumentContent; 
}

public int Insert<T,R>(R row, T table) where T: ITable<R>

****************
But another method would be to think about merge instead of insert.
public int InsertAll<T>(T table1, T table2)

This function will merge both the tables into table1. This way we can use the same DocTable and insert multiple rows at the same time.


#### Structure of Data
To store frequency of tokens, we can use the below the structure
```c#
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
}
```
This structure keeps token and id of the document that it belongs to together. This structure assumes that the code which wants to access this data is going to need DocumentId and Token simultaneously.

But what if that's not true?
And a function wants to go over the array of DocumentIds only? That function will have to load the associated token as well in memory which will be waste for it's functionality. The table structure would look something like below in that case

```c#
public struct TokenFrequencyTable
{
    public uint[] DocumentIds { get; set; }
    public string[] Tokens { get; set; }
    public int[] TokenFrequency { get; }
    public int NumberOfEntries { get; set; }
}
```

Conclusion: The structure of data is linked to how we need to **use** it. For now, I'm using the first structure because that seems to be right strucutre for my requirements.


### While writing IdfTable and Ops
why so many tables and classes? What if I do it in just 1 table and class to keep it simple.

```c#
public struct TfIdfTable
{
    public string[] DocName;
    public string[] DocContent;
    public int[] TokenCount;

    // the below arrays size will be different and much higher than the above arrays
    // The DocTokens array will contain unique token for each document.
    // TokenFrequency will contain frequency of the token in the document.
    public string[] DocTokens;
    public int[] DocTokenFrequency;
    
    // Size of below arrays will be the total number of unique tokens in the entire
    // data set.
    public string[] TokensAcrossAllDocs;
    public int[] CountOfDocsContainingToken; 
}
```

This was interesting. Even though I knew there are 3 different sizes of tables, I didn't realise it fully till I wrote everything in one place.

Another point: the whole system is built around documents. So if we write separate classes for these 3 different type of data, all three will have almost similar interface accepting documentId


#### Learnings for TfIdf excercise
1. Data and operations are not completely decoupled. The operations that we want to perform, determine the data structure that we need to use.
2. I'm still finding my balance of pre-mature optimisation and proper data split.
3. I'm still overcomplicating the code by making it tricky. Tf-Idf calculation should not be this hard. Granted that I was trying to build a system in which documents can be added or removed on runtime.
4. Everything doesn't have to be a flat array. It makes sense to use multi-dimensional arrays as well at some places, hash sets, queues etc.
5. I didn't run any benchmarks to see how the code affected the performance. I felt very confused while coding. I had no idea if I was going in the right direction or not. I just wanted to get some clarity and didn't invest in perf runs.
6. DOD is helpful in processing large of data quickly. But the code is taking 1 document at a time. Systems should probably take an entire array as input and the Insert function should really be a merge function or InsertAll.
7. Another important factor because of which I didn't run benchmarks is that I have used highly in-efficient algorithms for now. And the code doesn't really care about data consistency, data compaction. None of the operations which actually move elements within the array are used. Searching is done by linear search and a calculation required running multiple nested loops.
8. So, it's better to write the whole functionality within 1 table first and then split it. That probably will help in refining the code much easily.




