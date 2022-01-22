using MongoDB.Driver;

namespace QuestionBank.Model
{
    public interface ICategoryContext
    {
        IMongoCollection<Category> Category { get; }
    }
}