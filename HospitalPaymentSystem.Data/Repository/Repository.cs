using HospitalPaymentSystem.Core.Abstractions;
using HospitalPaymentSystem.Domain.Entities;
using Microsoft.Extensions.Configuration;

using System.Data.SqlClient;


namespace HospitalPaymentSystem.Data.Repository
{
    public class Repository : IRepository
    {
        private readonly IConfiguration _config;

        public Repository(IConfiguration config)
        {
            _config = config;
        }

        public async Task CreatePatientAsync(Patient patient)
        {
            if (await PatientExistsAsync(patient.FirstName, patient.LastName) || await EmailExistsAsync(patient.Email))
            {
                throw new Exception("A patient with the same full name or email address already exists.");
            }

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sql = @"INSERT INTO Patients (FirstName, LastName, ContactNumber, Email) 
                               OUTPUT INSERTED.PatientID 
                               VALUES (@firstName, @lastName, @contactNumber, @email)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@firstName", patient.FirstName);
                cmd.Parameters.AddWithValue("@lastName", patient.LastName);
                cmd.Parameters.AddWithValue("@contactNumber", patient.ContactNumber);
                cmd.Parameters.AddWithValue("@email", patient.Email);

                await conn.OpenAsync();
                int newPatientID = (int)await cmd.ExecuteScalarAsync();
            }
        }

        public async Task<bool> PostPaymentAsync(Payment payment)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    string sql = "INSERT INTO Payments (PatientID, Amount, PaymentDate) VALUES (@patientId, @amount, @paymentDate)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@patientId", payment.PatientID);
                    cmd.Parameters.AddWithValue("@amount", payment.Amount);
                    cmd.Parameters.AddWithValue("@paymentDate", payment.PaymentDate);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    return true;
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<List<Patient>> SearchPatientsAsync(string searchTerm)
        {
            List<Patient> patients = new List<Patient>();

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sql = @"SELECT p.PatientID, p.FirstName, p.LastName, p.ContactNumber, p.Email,
                              py.PaymentID, py.Amount, py.PaymentDate 
                              FROM Patients p
                              LEFT JOIN Payments py ON p.PatientID = py.PatientID
                              WHERE p.PatientID = @searchTerm 
                              OR p.FirstName LIKE @searchTermPattern 
                              OR p.LastName LIKE @searchTermPattern
                              OR CONCAT(p.FirstName, ' ', p.LastName) LIKE @fullNamePattern";
                SqlCommand cmd = new SqlCommand(sql, conn);

                if (int.TryParse(searchTerm, out int searchTermInt))
                {
                    cmd.Parameters.AddWithValue("@searchTerm", searchTermInt);
                    cmd.Parameters.AddWithValue("@searchTermPattern", DBNull.Value);
                    cmd.Parameters.AddWithValue("@fullNamePattern", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@searchTerm", DBNull.Value);
                    cmd.Parameters.AddWithValue("@searchTermPattern", $"%{searchTerm}%");
                    cmd.Parameters.AddWithValue("@fullNamePattern", $"%{searchTerm}%");
                }

                await conn.OpenAsync();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int patientId = reader.GetInt32(reader.GetOrdinal("PatientID"));

                        Patient patient = patients.Find(p => p.PatientID == patientId);
                        if (patient == null)
                        {
                            patient = new Patient
                            {
                                PatientID = patientId,
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                ContactNumber = reader.GetString(reader.GetOrdinal("ContactNumber")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Payments = new List<Payment>()
                            };
                            patients.Add(patient);
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("PaymentID")))
                        {
                            patient.Payments.Add(new Payment
                            {
                                PaymentID = reader.GetInt32(reader.GetOrdinal("PaymentID")),
                                Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                                PaymentDate = reader.GetDateTime(reader.GetOrdinal("PaymentDate"))
                            });
                        }
                    }
                }
            }

            return patients;
        }

        public async Task<List<Patient>> GetAllPatientsAsync()
        {
            List<Patient> patients = new List<Patient>();

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sql = @"SELECT p.PatientID, p.FirstName, p.LastName, p.ContactNumber, p.Email, 
                                      py.PaymentID, py.Amount, py.PaymentDate 
                               FROM Patients p
                               LEFT JOIN Payments py ON p.PatientID = py.PatientID";
                SqlCommand cmd = new SqlCommand(sql, conn);

                await conn.OpenAsync();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int patientId = reader.GetInt32(reader.GetOrdinal("PatientID"));

                        Patient patient = patients.Find(p => p.PatientID == patientId);
                        if (patient == null)
                        {
                            patient = new Patient
                            {
                                PatientID = patientId,
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                ContactNumber = reader.GetString(reader.GetOrdinal("ContactNumber")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Payments = new List<Payment>()
                            };
                            patients.Add(patient);
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("PaymentID")))
                        {
                            patient.Payments.Add(new Payment
                            {
                                PaymentID = reader.GetInt32(reader.GetOrdinal("PaymentID")),
                                Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                                PaymentDate = reader.GetDateTime(reader.GetOrdinal("PaymentDate"))
                            });
                        }
                    }
                }
            }

            return patients;
        }

        public async Task<Patient> GetPatientByIdAsync(int patientId)
        {
            Patient patient = null;

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sql = @"SELECT p.PatientID, p.FirstName, p.LastName, p.ContactNumber, p.Email, 
                                      py.PaymentID, py.Amount, py.PaymentDate 
                               FROM Patients p
                               LEFT JOIN Payments py ON p.PatientID = py.PatientID
                               WHERE p.PatientID = @patientId";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@patientId", patientId);

                await conn.OpenAsync();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        if (patient == null)
                        {
                            patient = new Patient
                            {
                                PatientID = reader.GetInt32(reader.GetOrdinal("PatientID")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                ContactNumber = reader.GetString(reader.GetOrdinal("ContactNumber")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Payments = new List<Payment>()
                            };
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("PaymentID")))
                        {
                            patient.Payments.Add(new Payment
                            {
                                PaymentID = reader.GetInt32(reader.GetOrdinal("PaymentID")),
                                Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                                PaymentDate = reader.GetDateTime(reader.GetOrdinal("PaymentDate"))
                            });
                        }
                    }
                }
            }

            return patient;
        }

        private async Task<bool> PatientExistsAsync(string firstName, string lastName)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sql = @"SELECT COUNT(*) FROM Patients WHERE FirstName = @firstName AND LastName = @lastName";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@firstName", firstName);
                cmd.Parameters.AddWithValue("@lastName", lastName);

                await conn.OpenAsync();
                int count = (int)await cmd.ExecuteScalarAsync();

                return count > 0;
            }
        }

        private async Task<bool> EmailExistsAsync(string email)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sql = @"SELECT COUNT(*) FROM Patients WHERE Email = @email";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@email", email);

                await conn.OpenAsync();
                int count = (int)await cmd.ExecuteScalarAsync();

                return count > 0;
            }
        }

        private string GetConnectionString()
        {
            return _config.GetConnectionString("HospitalPaymentsDB");
        }
    }
}

