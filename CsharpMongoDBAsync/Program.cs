using MongoDBAccess.DataAccess;
using MongoDB.Driver;
using CsharpMongoDBAsync;
using MongoDBAccess.Models;

//string connectionStrin = "mongodb://127.0.0.1:27017";
//string databaseName = "simple_db";
//string collectionName = "people";

//var client = new MongoClient(connectionStrin);
//var db = client.GetDatabase(databaseName);
//var collection = db.GetCollection<PersonModel>(collectionName);

//var person = new PersonModel { 
//    FirstName = "SKIDDOW",
//    LastName = "KIDDOW"
//};

//await collection.InsertOneAsync(person);

//var results = await collection.FindAsync(_ => true); // Doc https://youtu.be/exXavNOqaVo?t=835

//foreach (var result in results.ToList())
//{
//    Console.WriteLine($"{result.Id}: {result.FirstName}: {result.LastName}");
//}



using (ChoreDataAccess db = new ChoreDataAccess())
{
    await db.CreateUser(new UserModel()
    {
        FirstName = "SKIDDOW",
        LastName = "KIDDO"
    });

    var users = await db.GetAllUsers();

    var chore = new ChoreModel()
    {
        AssignedTo = users.First(),
        ChoreText = "Mow the Lawn",
        FrequencyInDays = 7
    };

    await db.CreateChore(chore);
}
    