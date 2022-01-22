using QuestionBank.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionBank.Repository
{
    public interface IQuestionBankRepository
    {
        //Get Request Functions
        List<Questions> GetQuestionByCategoryIdAndDifficulty(int categoryId, int difficulty, int start, int count);

        //Post Request FUnction
        Questions AddQuestion(Questions question, int categoryId);
        List<Questions> AddMultipleQuestions(List<Questions> questions, int categoryId);
        Questions GetNextQuestion(Questions question, int categoryId);
        Questions GetFirstQuestion(int categoryId);
        Questions SkipQuestion(Questions question, int categoryId);

        //Put Request Functions
        bool UpdateQuestion(Questions question);

        //Delete Request Functions
        bool DeleteQuestion(Questions question, int categoryId);

        //Extras
        Questions GetQuestion(Questions question);
        List<Questions> GetQuestionBank();
    }
}