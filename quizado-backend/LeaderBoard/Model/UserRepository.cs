using Microsoft.Extensions.Configuration;
using Neo4j.Driver.V1;
using Neo4jClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaderBoard.Model
{
    public class UserRepository: IUserRepository
    {
        private readonly IDriver driver;
        public UserRepository(IConfiguration configuration) {
            String db = configuration.GetSection("GraphDB:server").Value;
            String user = configuration.GetSection("GraphDB:user").Value;
            String password = configuration.GetSection("GraphDB:password").Value;
            this.driver = GraphDatabase.Driver(db, AuthTokens.Basic(user, password));
            EnsureCreated();
        }

        public UserRepository(){}

        /// <summary>
        /// Ensure that database has been created with START and END node and existing relationships
        /// </summary>
        private void EnsureCreated()
        {
            bool flag = false;
            string cypherQuery = "MATCH (u)-[rel]-() return count(u)+count(rel) as DB";
            
                var status = RunCypherQuery(cypherQuery).ToList();
                var jStatus = JsonConvert.SerializeObject(status[0].Values);
                Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jStatus);
                if ((long)dict["DB"] == 0)
                {
                    flag = true;
                }
            if (flag)
            {
                string cypher1 = "MERGE(u: User{ Email: \"start\"}) SET u += { Email: \"start\", Name: \"START\", PhoneNumber: \"\", ProfilePic: \"\", _SCORE_1: 100000, _SCORE_2: 100000, _SCORE_3: 100000,_SCORE_4: 100000,Coins : 0};";
                string cypher2 = "MERGE(u: User{ Email: \"end\"}) SET u += { Email: \"end\", Name: \"END\", PhoneNumber: \"\", ProfilePic: \"\", _SCORE_1: -100000, _SCORE_2: -100000, _SCORE_3: -100000,_SCORE_4: -100000, Coins : 0};";
                string cypher3 = "MATCH (u: User{ Email: \"start\"}), (y: User{ Email: \"end\"}) MERGE (u)-[:_CAT_1]->(y) with u,y MERGE (u)-[:_CAT_2]->(y) with u,y MERGE (u)-[:_CAT_3]->(y) with u,y MERGE (u)-[:_CAT_4]->(y)";
                string cypher4 = "CREATE CONSTRAINT ON (user:User) ASSERT user.Email IS UNIQUE;";
                using (var session = driver.Session())
                {
                    int transaction = session.WriteTransaction(tx =>
                    {
                        var result1 = tx.Run(cypher1, new { });
                        var result2 = tx.Run(cypher2, new { });
                        var result3 = tx.Run(cypher3, new { });
                        return result1.ToArray().Length + result2.ToArray().Length + result3.ToArray().Length;
                    });
                   _ = transaction == 0 ? true : false;
                }
                _ = RunCypherQuery(cypher4).ToArray().Length;
            }
        }
        /// <summary>
        /// Add score and organize the database in order to rank
        /// </summary>
        /// <param name="Email">Email Address</param>
        /// <param name="categoryName">Category Name</param>
        /// <param name="categoryScore">Category Score</param>
        /// <param name="QA">Question Attempted</param>
        /// <returns></returns>
        public bool AddScore(string Email, long categoryId, long categoryScore)
        {
            string cypher1 = $"MATCH (c:User) where c.Email = \"{Email}\" with c OPTIONAL MATCH(p: User)-[rel1: _CAT_{categoryId}]->(c) with p, c, rel1 OPTIONAL MATCH(c)-[rel2: _CAT_{categoryId}]->(n: User) with p, c, n, rel1, rel2 CREATE(p) -[:_CAT_{categoryId}]->(n) DELETE rel1 DELETE rel2;";
            string cypher2 = $"MATCH (c:User) where c.Email = \"{Email}\" with c MATCH (p:User)-[rel:_CAT_{categoryId}]->(n:User) where p._SCORE_{categoryId} >= {categoryScore} >= n._SCORE_{categoryId} WITH p,n,c,rel LIMIT 1 DELETE rel MERGE (p)-[:_CAT_{categoryId}]->(c) MERGE (c)-[:_CAT_{categoryId}]->(n)";
            string cypher3 = $"MATCH (c:User) where c.Email = \"{Email}\" SET c += {{_SCORE_{categoryId} : {categoryScore}}}";
            using (var session = driver.Session())
            {
                int transaction = session.WriteTransaction(tx =>
                {
                    var result1 = tx.Run(cypher1, new { });
                    var result2 = tx.Run(cypher2, new { });
                    var result3 = tx.Run(cypher3, new { });
                    return result1.ToArray().Length + result2.ToArray().Length + result3.ToArray().Length;
                });
                return transaction == 0 ? true : false;
            }
        }
        /// <summary>
        /// Method for fetching Leaderboard w.r.t category
        /// </summary>
        /// <param name="category">Category Name</param>
        /// <returns></returns>
        public List<Leaderboard> GetLeaderBoard(long categoryId)
        {
            string cypher = $"MATCH (a:User)-[:_CAT_{categoryId}*0..10]->(b:User) where a.Email = \"start\" return collect(b.Email), collect(b._SCORE_{categoryId})";
            
            
                var leaderboard = RunCypherQuery(cypher).Single();
                string  jEmails= JsonConvert.SerializeObject(leaderboard[0]);
                List<string> lEmails = (List<string>)JsonConvert.DeserializeObject<IList<string>>(jEmails);
                lEmails.RemoveAt(0);

                string jScore = JsonConvert.SerializeObject(leaderboard[1]);
                List<int> lScores = (List<int>)JsonConvert.DeserializeObject<IList<int>>(jScore);
                lScores.RemoveAt(0);

                List<Leaderboard> topPerformers = new List<Leaderboard>();
                for (int i = 0; i < lEmails.Count; i++)
                {
                    Leaderboard scoreCard = new Leaderboard();
                    scoreCard.Email = lEmails[i];
                    CategoryScore categoryScore = new CategoryScore();
                    categoryScore.CategoryId = categoryId;
                    categoryScore.Score = lScores[i];
                    scoreCard.CategoryScore = categoryScore;
                    scoreCard.Rank = i + 1;
                    topPerformers.Add(scoreCard);
                }
                return topPerformers;
            
        }

        /// <summary>
        /// Method for fetching the user details and populating the user model from Neo4j
        /// </summary>
        /// <param name="email">Email Address</param>
        /// <returns>JObject Serialized form of Neo4j request Result</returns>
        public Dictionary<string, object> GetUserDetails(String email) {
            // Cypher query for Neo4j
            string cypher = $"MATCH (u:User) where u.Email = \"{email}\" return u";
            var userDetails = RunCypherQuery(cypher).Single()[0];
            // serializing the object to json
            string jsonUserObject = JsonConvert.SerializeObject(userDetails);
            JObject jUserDetails = JObject.Parse(jsonUserObject);
            Dictionary<string, object> userProperties = GetUserFromJson(jUserDetails);
            return userProperties;
        }

        private Dictionary<string, object> GetUserFromJson(JObject userDetails)
        {
            JToken token = userDetails["Properties"];
            var stoken = JsonConvert.SerializeObject(token.ToObject<Object>());
            Dictionary<string, object> properties = JsonConvert.DeserializeObject<Dictionary<string, object>>(stoken);
            return properties;
        }
        /// <summary>
        /// Method that creates categoryScore, questionAttempted in START node and END node to initiate relationship based on new Category
        /// </summary>
        /// <param name="categoryName">Category Name</param>
        /// <returns></returns>
        public bool InsertNewCategory(long categoryId)
        {
            String emailStart = "start";
            String emailEnd = "end";
            string cypher1 = $"MERGE (u {{Email: \"{emailStart}\"}}) SET u += {{_SCORE_{categoryId} : 100000}};";
            string cypher2 = $"MERGE (u {{Email: \"{emailEnd}\"}}) SET u += {{_SCORE_{categoryId} : -100000}};";
            string cypher3 = $"MATCH (s{{Email :\"{emailStart}\"}}),(e{{Email: \"{emailEnd}\"}}) MERGE (s)-[:_CAT_{categoryId}]->(e);";
            string cypher4 = $"CREATE INDEX ON :User(_SCORE_{categoryId});";
            using (var session = driver.Session())
            {
                int transaction = session.WriteTransaction(tx =>
                {
                    var result1 = tx.Run(cypher1, new { });
                    var result2 = tx.Run(cypher2, new { });
                    var result3 = tx.Run(cypher3, new { });
                    return result1.ToArray().Length + result2.ToArray().Length + result3.ToArray().Length;
                });
                int constraintResult = RunCypherQuery(cypher4).ToArray().Length;
                return transaction+ constraintResult == 0 ? true : false;
            }
        }

        /// <summary>
        /// Method for creating a new user without category score or its details
        /// </summary>
        /// <param name="user">User Model Object to register him</param>
        /// <returns></returns>        
        public Boolean RegisterUser(User user)
        {
            string cypher = new StringBuilder()
                .AppendLine($"CREATE (u:User {{Email: \"{user.Email}\", Name: \"{user.Name}\", PhoneNumber: \"{user.PhoneNumber}\", ProfilePic: \"{user.ProfilePic}\", Coins: {user.Coins}}});")
                .ToString();
            int userDetails = RunCypherQuery(cypher).ToArray().Length;
            return userDetails == 0 ? true : false;
        }

        /// <summary>
        /// Method for updating user details with email
        /// </summary>
        /// <param name="user">User Model Object to register him.</param>
        /// <returns></returns>
        public Boolean UpdateUserDetails(User user)
        {
            string cypher = $"MERGE (u {{Email: \"{user.Email}\"}}) SET u += {{ProfilePic:\"{user.ProfilePic}\" ,Name: \"{user.Name}\", PhoneNumber: \"{user.PhoneNumber}\"}}";
            int userDetails = RunCypherQuery(cypher).ToArray().Length;
            return userDetails == 0 ? true : false;
            
        }

        /// <summary>
        /// Gets the Rank of the user based upon his category
        /// </summary>
        /// <param name="Email">Email Address</param>
        /// <param name="category">Cateogry Name</param>
        /// <returns>His rank if </returns>
        public long? GetRank(string Email, long categoryId)
        {
            string cypher = $"MATCH (u:User{{Email: \"start\"}}) with u MATCH path = (u)-[rel: _CAT_{categoryId} *]->(y) where y.Email = \"{Email}\" return length(path) as RANK";
            var status = RunCypherQuery(cypher).ToList();
            if (status.Count == 0) 
            {
                return null;
            }
            var jStatus = JsonConvert.SerializeObject(status[0].Values);
            Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jStatus);
            return (long)dict["RANK"];
        }
        /// <summary>
        /// Method to check if a user exists with the specific email address
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>        
        public bool GetUserByEmail(string Email)
        {
            string cypher = $"MATCH (u:User) where u.Email = \"{Email}\" return u";
            return RunCypherQuery(cypher).Count()==0?false:true;
        }
        /// <summary>
        /// Method to Check if category Exists
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public bool GetCategory(long categoryId) {
            string cypher = $"MATCH ()-[rel: _CAT_{categoryId}]-() return DISTINCT type(rel) AS Category";
            return RunCypherQuery(cypher).Count()==0?false:true;
        }
        /// <summary>
        /// Get the List of categories that user have given test for. 
        /// </summary>
        /// <returns></returns>
        public List<string> GetCategoryList()
        {
            string cypher = "MATCH ()-[rel]-() return collect(DISTINCT type(rel))";
            string jCategoryList = JsonConvert.SerializeObject(RunCypherQuery(cypher).Single()[0]);
            List<string> lCategory = (List<string>)JsonConvert.DeserializeObject<IList<string>>(jCategoryList);
            return lCategory;
        }
        /// <summary>
        /// Method to Run Cypher Query
        /// </summary>
        /// <param name="cypher">Query</param>
        /// <returns></returns>        
        private IStatementResult RunCypherQuery(string cypher)
        {
            using (var session = driver.Session())
            {
                IStatementResult transaction = session.WriteTransaction(tx =>
                {
                    var result = tx.Run(cypher,
                        new { });
                    return result;
                });
                return transaction;
            }
        }
        /// <summary>
        /// Update the coins
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="coin"></param>
        /// <returns></returns>
        public long UpdateCoins(string Email,long coin)
        {
            string cypher = $"MATCH (u:User) where u.Email = \"{Email}\" SET u += {{Coins : {coin} }} return u.Coins as COIN;";
            var status = RunCypherQuery(cypher).ToList();
            var jStatus = JsonConvert.SerializeObject(status[0].Values);
            Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jStatus);
            return (long)dict["COIN"];
        }
        public bool ResetDB() {
            string cypher = "MATCH (u) DETACH DELETE u;";
            bool result = RunCypherQuery(cypher).ToList().Count == 0?true:false;
            EnsureCreated();
            return result;
        }
    }
}
