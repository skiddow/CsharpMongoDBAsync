using MongoDBAccess.Models;
using MongoDB.Driver;

namespace MongoDBAccess.DataAccess;

public  class ChoreDataAccess : IDisposable
{

    public void Dispose()
    {
       
    }

    // Doc at https://youtu.be/exXavNOqaVo?t=1573

    private const string ConnectionString = "mongodb://127.0.0.1:27017";
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

    public async Task<List<ChoreModel>> GetAllChore()
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

    
}
