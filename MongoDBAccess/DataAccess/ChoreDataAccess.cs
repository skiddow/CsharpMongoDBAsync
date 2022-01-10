using MongoDBAccess.Models;
using MongoDB.Driver;

namespace MongoDBAccess.DataAccess;

public  class ChoreDataAccess : IDisposable
{

    public void Dispose()
    {
       
    }

    // Doc at https://youtu.be/exXavNOqaVo?t=1573

    private const string ConnectionString = "mongodb://localhost:27017";
    private const string DatabaseName = "choredb";
    private const string ChoreCollection = "chore_chart";
    private const string UserCollection = "users";
    private const string ChoreHistoryCollection = "chore_history";

    public IMongoCollection<T> ConnectToMongo<T>(in string collection)
    {
        // Doc at https://youtu.be/exXavNOqaVo?t=1799
        var client = new MongoClient(ConnectionString);
        var db = client.GetDatabase(DatabaseName);
        return db.GetCollection<T>(collection);
    }

    public async Task<List<UserModel>> GetAllUsers()
    {
        var usersCollection = ConnectToMongo<UserModel>(UserCollection);
        var result = await usersCollection.FindAsync(_ => true);
        return result.ToList();
    }

    public async Task<List<ChoreModel>> GetAllChores()
    {
        var choresCollection = ConnectToMongo<ChoreModel>(ChoreCollection);
        var result = await choresCollection.FindAsync(_ => true);
        return result.ToList();
    }

    public async Task<List<ChoreModel>> GetAllChoresForUser(UserModel user)
    {
        var choresCollection = ConnectToMongo<ChoreModel>(ChoreCollection);
        var result = await choresCollection.FindAsync(c => c.AssignedTo.Id == user.Id);
        return result.ToList();
    }

    public Task CreateUser(UserModel user)
    {
        var usersCollection = ConnectToMongo<UserModel>(UserCollection);
        return usersCollection.InsertOneAsync(user);
    }

    public Task CreateChore(ChoreModel chore)
    {
        var choresCollection = ConnectToMongo<ChoreModel>(ChoreCollection);
        return choresCollection.InsertOneAsync(chore);
    }

    public Task UpdateChore(ChoreModel chore)
    {
        var choresCollection = ConnectToMongo<ChoreModel>(ChoreCollection);
        var filter = Builders<ChoreModel>.Filter.Eq("Id", chore.Id);
        return choresCollection.ReplaceOneAsync(filter, chore, new ReplaceOptions { IsUpsert = true });
    }

    public Task DeleteChore(ChoreModel chore)
    {
        var choresCollection = ConnectToMongo<ChoreModel>(ChoreCollection);
        return choresCollection.DeleteOneAsync(c => c.Id == chore.Id);
    }

    public async Task CompleteChore(ChoreModel chore)
    {
        //var choresCollection = ConnectToMongo<ChoreModel>(ChoreCollection);
        //var filter = Builders<ChoreModel>.Filter.Eq("Id", chore.Id);
        //await choresCollection.ReplaceOneAsync(filter, chore);

        //var choreHistoryCollection = ConnectToMongo<ChoreHistoryModel>(ChoreHistoryCollection);
        //await choreHistoryCollection.InsertOneAsync(new ChoreHistoryModel(chore));

        var client  = new MongoClient(ConnectionString);
        using var session = await client.StartSessionAsync();

        session.StartTransaction();

        try
        {
            var db = client.GetDatabase(DatabaseName);
            var choresCollection = db.GetCollection<ChoreModel>(ChoreCollection);
            var filter = Builders<ChoreModel>.Filter.Eq("Id", chore.Id);
            await choresCollection.ReplaceOneAsync(filter, chore);

            var choreHistoryColection = db.GetCollection<ChoreHistoryModel>(ChoreHistoryCollection);
            await choreHistoryColection.InsertOneAsync(new ChoreHistoryModel(chore));

            await session.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync();
            Console.WriteLine(ex.Message);
        }
    }
}
