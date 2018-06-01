using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SimchaOrganizer.Data
{
    public class Simcha
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
        public int ContributorAmount { get; set; }
    }
    public class Contributor
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Cell { get; set; }
        public bool Include { get; set; }
        public DateTime DateJoined { get; set; }
        public decimal TotalLeft { get; set; }
    }
    public class History
    {
        public string Action { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
    public class UpdatedInfo
    {
        public int ContributorID { get; set; }
        public int SimchaID { get; set; }
        public decimal Amount { get; set; }
        public bool DidContribute { get; set; }
    }
    public class DBManager
    {
        private string _connection;
        public DBManager(string conn)
        {
            _connection = conn;
        }

        public IEnumerable<Simcha> SimchaList()
        {
            using (SqlConnection connection = new SqlConnection(_connection))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "select s.*, count(c.Amount) as pplamount, sum(c.Amount) as total from Simchas s " +
                                       "left join SimchasAndCont c on c.SimchaID = s.ID group by s.Date, s.SimchaName, s.ID";

                connection.Open();
                List<Simcha> list = new List<Simcha>();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    decimal total = 0;
                    object totalValue = reader["total"];
                    if (totalValue != DBNull.Value)
                    {
                        total = (decimal)totalValue;
                        total = (total * -1);
                    }
                    Simcha simcha = new Simcha
                    {
                        ID = (int)reader["ID"],
                        Name = (string)reader["SimchaName"],
                        Date = (DateTime)reader["Date"],
                        ContributorAmount = (int)reader["pplamount"],
                        Total = total
                    };
                    list.Add(simcha);
                }
                return list;
            }

        }
        public int TotalContributors()
        {
            using (SqlConnection connection = new SqlConnection(_connection))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "select count(*) From Contributors";

                connection.Open();
                return (int)command.ExecuteScalar();
            }

        }
        public IEnumerable<Contributor> ContributorList()
        {
            using (SqlConnection connection = new SqlConnection(_connection))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "select * from Contributors";
                connection.Open();
                List<Contributor> list = new List<Contributor>();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    bool always = false;
                    object value = reader["AlwaysInclude"];
                    if (value != DBNull.Value)
                    {
                        always = (bool)value;
                    }

                    int id = (int)reader["ID"];
                    decimal sum = (GetContribution(id)) + (GetDeposits(id));

                    Contributor c = new Contributor
                    {
                        ID = id,
                        Name = (string)reader["Name"],
                        Cell = (string)reader["Cell"],
                        DateJoined = (DateTime)reader["DateJoined"],
                        Include = always,
                        TotalLeft = sum
                    };
                    list.Add(c);

                }
                return list;
            }

        }

        private decimal GetContribution(int cid)
        {
            using (SqlConnection connection = new SqlConnection(_connection))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "select sum(sc.Amount) from SimchasAndCont sc " +
                    "where sc.ContributorID = @id";
                command.Parameters.AddWithValue("@id", cid);
                connection.Open();
                var result = command.ExecuteScalar();
                decimal ttl = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                return ttl;
            }
        }
        private decimal GetDeposits(int cid)
        {
            using (SqlConnection connection = new SqlConnection(_connection))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "select sum(d.Amount) from Deposits d " +
                    "where d.ContributorID = @id";

                command.Parameters.AddWithValue("@id", cid);
                connection.Open();
                var result = command.ExecuteScalar();
                decimal ttl = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                return ttl;
            }
        }

        public void EditPerson(Contributor c)
        {
            using (SqlConnection connection = new SqlConnection(_connection))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "update contributors set name = @name, cell = @cell, AlwaysInclude = @include where ID = @id";
                command.Parameters.AddWithValue("@id", c.ID);
                command.Parameters.AddWithValue("@name", c.Name);
                command.Parameters.AddWithValue("@cell", c.Cell);
                command.Parameters.AddWithValue("@include", c.Include);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public List<Contributor> ContributorList(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connection))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "select * from Contributors c join SimchasAndCont sc on c.ID = sc.ContributorID where sc.SimchaID = @id";
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                List<Contributor> list = new List<Contributor>();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Contributor c = new Contributor
                    {
                        ID = (int)reader["ID"],
                        Name = (string)reader["Name"],
                        Cell = (string)reader["Cell"],
                        DateJoined = (DateTime)reader["DateJoined"],
                        TotalLeft = (decimal)reader["Amount"]

                    };
                    list.Add(c);
                }

                return list;
            }

        }
        public void AddSimcha(string simcha, DateTime date)
        {
            using (var connection = new SqlConnection(_connection))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "insert into Simchas values (@simcha, @date)";
                command.Parameters.AddWithValue("@simcha", simcha);
                command.Parameters.AddWithValue("@date", date);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<History> History(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connection))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "select s.SimchaName, sc.Amount, sc.Date from Simchas s " +
                    "left join SimchasAndCont sc on s.ID = sc.SimchaID " +
                    "where sc.ContributorID = @id";
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                var list = new List<History>();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {

                    History h = new History
                    {
                        Action = (string)reader["SimchaName"],
                        Amount = (decimal)reader["Amount"],
                        Date = (DateTime)reader["Date"],

                    };
                    list.Add(h);
                }
                list.AddRange(DepositsForHistory(id));
                return list;
            }

        }
        private List<History> DepositsForHistory(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connection))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "select * from Deposits where ContributorID = @id";
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                var list = new List<History>();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    History h = new History
                    {
                        Action = "Deposit",
                        Amount = (decimal)reader["Amount"],
                        Date = (DateTime)reader["Date"],

                    };
                    list.Add(h);
                }

                return list;
            }

        }

        public void AddDeposit(int id, decimal amount, DateTime date)
        {
            using (var connection = new SqlConnection(_connection))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "insert into Deposits Values (@id, @amount, @date)";
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@amount", amount);
                command.Parameters.AddWithValue("@date", date);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public string GetSimchaForID(int id)
        {
            using (var connection = new SqlConnection(_connection))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "Select * from Simchas where ID = @id";
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                string simchaName = null;
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {

                    simchaName = (string)reader["SimchaName"];

                }
                return simchaName;
            }
        }

        public void AddContributor(List<UpdatedInfo> list)
        {
            using (var connection = new SqlConnection(_connection))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "insert into SimchasAndCont values (@simchaid, @conID, @amount, GetDate())";
                connection.Open();
                foreach (UpdatedInfo p in list)
                {
                    command.Parameters.AddWithValue("@simchaid", p.SimchaID);
                    command.Parameters.AddWithValue("@conID", p.ContributorID);
                    command.Parameters.AddWithValue("@amount", -p.Amount);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();

                }


            }
        }
        public void UpdateContribution(List<UpdatedInfo> list)
        {
            using (var connection = new SqlConnection(_connection))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "Update SimchasAndCont set amount = @amount where SimchaID = @sid and ContributorID = @cid";
                connection.Open();
                foreach (UpdatedInfo p in list)
                {
                    command.Parameters.AddWithValue("@sid", p.SimchaID);
                    command.Parameters.AddWithValue("@cid", p.ContributorID);
                    command.Parameters.AddWithValue("@amount", -p.Amount);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();

                }


            }
        }
        public void AddPerson(Contributor c)
        {
            using (var connection = new SqlConnection(_connection))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "insert into Contributors values (@name, @cell, @date, @include) select Scope_Identity()";
                
                command.Parameters.AddWithValue("@name", c.Name);
                command.Parameters.AddWithValue("@cell", c.Cell);
                command.Parameters.AddWithValue("@date", c.DateJoined);
                command.Parameters.AddWithValue("@include", c.Include);
                connection.Open();
                c.ID = (int)(decimal)command.ExecuteScalar();


            }

        }
        public void AddInitialDeposit(int amount, int id, DateTime date)
        {
            using (var connection = new SqlConnection(_connection))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "insert into Deposits values (@id, @amount, @date)";
                connection.Open();
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@amount", amount);
                command.Parameters.AddWithValue("@date", date);
                
                command.ExecuteNonQuery();

               
            }
        }
    }
}
