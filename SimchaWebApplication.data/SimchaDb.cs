using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SimchaWebApplication.data
{
    public class SimchaDb
    {
        private string _connectionString;

        public SimchaDb(string connectionStriong)
        {
            _connectionString = connectionStriong;
        }
        public int AddContributor(Contributor contributor)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Contributors VALUES(@firstName, @lastName, @cell, @alwaysInclude) SELECT @@IDENTITY ";
            object value = contributor.Cell;
            if (value == null)
            {
                value = DBNull.Value;
            }
            command.Parameters.AddWithValue("@firstName", contributor.FirstName);
            command.Parameters.AddWithValue("@lastName", contributor.LastName);
            command.Parameters.AddWithValue("@cell", value);
            command.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
            connection.Open();
            return int.Parse(command.ExecuteScalar().ToString());
        }
        public void AddDeposit(Deposit deposit)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Deposits VALUES(@amount, @contributorId, @date)";
            command.Parameters.AddWithValue("@amount", deposit.Amount);
            command.Parameters.AddWithValue("@contributorId", deposit.ContributorId);
            command.Parameters.AddWithValue("@date", deposit.Date);
            connection.Open();
            command.ExecuteNonQuery();
        }
        public IEnumerable<Contributor> GetContributors(int? SimchaId)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT c.*, SUM(d.Amount) AS TotalDeposits, SUM(co.Amount) As TotalContributions
                                    FROM Contributors c
                                    LEFT JOIN Deposits d
                                    ON c.Id = d.ContributorId
                                    LEFT JOIN Contributions co
                                    ON c.Id = co.ContributorId
                                    GROUP BY c.AlwaysInclude, c.Cell, c.FirstName, c.Id, c.LastName";
            connection.Open();
            if(SimchaId != null)
            {
                command.CommandText = command.CommandText.Insert(386, " WHERE co.SimchaId = @id");
                command.Parameters.AddWithValue("@id", SimchaId);
            }
            SqlDataReader reader = command.ExecuteReader();
            List<Contributor> contributors = new List<Contributor>();
            while (reader.Read())
            {
                decimal totalDeposits = (reader["TotalDeposits"] == DBNull.Value) ? 0 : (decimal)reader["TotalDeposits"];
                decimal totalContributions = (reader["TotalContributions"] == DBNull.Value) ? 0 : (decimal)reader["TotalContributions"];
                Contributor c = new Contributor
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    Cell = (reader["Cell"] == DBNull.Value) ? null : (string)reader["Cell"],
                    balance = totalDeposits - totalContributions,
                    AlwaysInclude = (bool)reader["AlwaysInclude"]

                };
                contributors.Add(c);
            }
            return contributors;
        }
        public IEnumerable<Contributor> GetContributors()
        {
            return GetContributors(null);
        }
        public Contributor GetContributor(int contributorId)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT c.*, SUM(d.Amount) AS TotalDeposits, SUM(co.Amount) As TotalContributions
                                    FROM Contributors c
                                    LEFT JOIN Deposits d
                                    ON c.Id = d.ContributorId
                                    LEFT JOIN Contributions co
                                    ON c.Id = co.ContributorId
                                    WHERE c.id = @id
                                    GROUP BY c.AlwaysInclude, c.Cell, c.FirstName, c.Id, c.LastName";
            command.Parameters.AddWithValue("@id", contributorId);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            reader.Read();
            decimal totalDeposits = (reader["TotalDeposits"] == DBNull.Value) ? 0 : (decimal)reader["TotalDeposits"];
            decimal totalContributions = (reader["TotalContributions"] == DBNull.Value) ? 0 : (decimal)reader["TotalContributions"];
            Contributor c = new Contributor
            {
                Id = (int)reader["Id"],
                FirstName = (string)reader["FirstName"],
                LastName = (string)reader["LastName"],
                Cell = (reader["Cell"] == DBNull.Value) ? null : (string)reader["Cell"],
                balance = totalDeposits - totalContributions,
                AlwaysInclude = (bool)reader["AlwaysInclude"]

            };
            return c;
        }
        public IEnumerable<Contribution> GetTransactions(int ContributorId)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT AMOUNT, Date, NULL AS SimchaName FROM Deposits 
                                    WHERE ContributorId = @id
                                    UNION ALL
                                    SELECT C.Amount, C.Date, s.Name AS SimchaName
                                    FROM Contributions c
                                    JOIN Simchas s
                                    ON ContributorId = s.Id
                                    WHERE ContributorId = @id
                                    ORDER BY Date";
            connection.Open();
            command.Parameters.AddWithValue("@id", ContributorId);
            SqlDataReader reader = command.ExecuteReader();
            List<Contribution> deposits = new List<Contribution>();
            while (reader.Read())
            {
                Contribution d = new Contribution
                {
                    Amount = (decimal)reader["Amount"],
                    Date = (DateTime)reader["Date"],
                    SimchaName = (reader["SimchaName"] == DBNull.Value) ? "Deposit" : (string)reader["SimchaName"]

                };
                deposits.Add(d);
            }
            return deposits;
        }

        public decimal GetTotalBalance()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT SUM(Amount) AS TotalS FROM Deposits 
                                   UNION
                                   SELECT SUM(Amount) FROM Contributions";
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            reader.Read();
            decimal contributions = (decimal)reader["TotalS"];
            reader.Read();
            decimal deposits = (decimal)reader["TotalS"];
            decimal balance = deposits - contributions;


            return balance;
        }
        public void AddSimcha(Simcha Simcha)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Simchas VALUES(@name, @date)";
            command.Parameters.AddWithValue("@name", Simcha.Name);
            command.Parameters.AddWithValue("@date", Simcha.Date);
            connection.Open();
            command.ExecuteNonQuery();
        }
        public IEnumerable<Simcha> GetSimchas()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT s.*, SUM(c.Amount) As TotalContributions, COUNT(c.Id) As ContributorCount
                                    FROM Simchas s
                                    LEFT JOIN Contributions c
                                    ON s.Id = c.SimchaId
                                    GROUP BY s.Id, s.Date, s.Name";
            connection.Open();
            List<Simcha> simchas = new List<Simcha>();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Simcha simcha = new Simcha
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Date = (DateTime)reader["Date"],
                    TotalContributions = (reader["TotalContributions"] != DBNull.Value) ? (decimal)reader["TotalContributions"] : 0,
                    ContributorCount = (reader["ContributorCount"] != DBNull.Value) ? (int)reader["ContributorCount"] : 0
                };
                simchas.Add(simcha);
            }
            return simchas;
        }
        public int GetContributorCount()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(Id) FROM Contributors";
            connection.Open();
            return (int)command.ExecuteScalar();
        }
        public Simcha GetSimcha(int Id)
        {
            
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM Simchas 
                                    WHERE Id = @id";
            command.Parameters.AddWithValue("@id", Id);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            reader.Read();
            Simcha s = new Simcha
            {
                Id = (int)reader["Id"],
                Name = (string)reader["Name"],
                Date = (DateTime)reader["Date"]

            };
            return s;
        }
        public IEnumerable<Contribution> GetContributions(int? Id)
        {

            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM Contributions ";
            if(Id != null)
            {
                command.CommandText += "WHERE SimchaId = @id";
                command.Parameters.AddWithValue("@id", Id);
            }
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            List<Contribution> contributions = new List<Contribution>();
            while (reader.Read())
            {
                Contribution c = new Contribution
                {
                    ContributorId = (int)reader["ContributorId"],
                    Amount = (decimal)reader["Amount"],
                    Date = (DateTime)reader["Date"]
                };
                contributions.Add(c);
            }
            return contributions;

                                    
        }
        public IEnumerable<Contributor> GetContributorsWithContribution(int SimchaId)
        {
            List<Contributor> contributors = (List<Contributor>)GetContributors();
            IEnumerable<Contribution> contributions = GetContributions(SimchaId);
            foreach(Contributor c in contributors)
            {
                foreach(Contribution x in contributions)
                {
                    if (x.ContributorId == c.Id)
                    {
                        c.Amount = x.Amount;
                        break;
                    }
                    c.Amount = null;                     
                }                
            }
            return contributors;
        }
       public IEnumerable<Contribution> GetContributions()
        {
            return GetContributions(null);
        } //just experimenting method overloading
        public void DeleteContributions(int Id)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "DELETE Contributions WHERE SimchaId = @id";
            command.Parameters.AddWithValue("@id",Id);
            connection.Open();
            command.ExecuteNonQuery();
        }
        public void AddContribution(Contribution contribution)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Contributions VALUES(@amount, @contributorId, @simchaId, @date)";
            command.Parameters.AddWithValue("@amount", contribution.Amount);
            command.Parameters.AddWithValue("@contributorId", contribution.ContributorId);
            command.Parameters.AddWithValue("@simchaId", contribution.SimchaId);
            command.Parameters.AddWithValue("@date", DateTime.Now);
            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
