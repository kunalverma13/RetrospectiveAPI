using Contracts;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace MongoDBFunctions
{
    public class CRUD : ICRUD
    {
        private IMongoDatabase db;

        public CRUD(string databaseName)
        {

            var client = new MongoClient("mongodb+srv://kunalverma13:bhagatSingh1308@cluster0.pbyg3.mongodb.net/myFirstDatabase?retryWrites=true&w=majority");
            //var database = client.GetDatabase("test");


            //var client = new MongoClient();
            db = client.GetDatabase(databaseName);
        }

        public T InsertRecord<T>(string tableName, T record)
        {
            var collection = db.GetCollection<T>(tableName);
            collection.InsertOne(record);
            return record;
        }

        public IEnumerable<T> LoadRecords<T>(string tableName)
        {
            var collection = db.GetCollection<T>(tableName);
            return collection.Find(new BsonDocument()).ToList();
        }

        public T LoadRecordById<T>(string tableName, Guid Id)
        {
            var collection = db.GetCollection<T>(tableName);
            var filter = Builders<T>.Filter.Eq("id", Id);
            return collection.Find(filter).First();
        }

        public void DeleteRecordById<T>(string tableName, Guid Id)
        {
            var collection = db.GetCollection<T>(tableName);
            var filter = Builders<T>.Filter.Eq("id", Id);
            collection.DeleteOne(filter);
        }

        public T Upsert<T>(string tableName, T record, Guid Id)
        {
            var collection = db.GetCollection<T>(tableName);
            var filter = Builders<T>.Filter.Eq("id", Id);
            collection.ReplaceOne(filter, record, new ReplaceOptions { IsUpsert = true });
            return record;
        }
    }
}
