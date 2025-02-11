﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Database.Query;

namespace BudgetStream
{
    internal class FirebaseHelper
    {
        FirebaseClient firebase = new FirebaseClient("https://mobile-app-development-60d7a-default-rtdb.asia-southeast1.firebasedatabase.app/");

        // Functions to interact with DeadlineRecord

        public async Task<string> AddDeadlineRecord(DeadlineRecord deadlineRecord)
        {
            try
            {
                // Add the deadline record to the Firebase database
                var firebaseResponse = await firebase
                    .Child("DeadlineRecords")
                    .PostAsync(deadlineRecord);

                // Retrieve the unique key generated by Firebase
                string recordKey = firebaseResponse.Key;

                // Update the DeadlineRecord object with the key
                deadlineRecord.Key = recordKey;

                // Return the key for further processing if needed
                return recordKey;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding deadline record to Firebase: {ex.Message}");
                throw;
            }
        }

        public async Task<List<DeadlineRecord>> GetDeadlineRecords()
        {
            try
            {
                var deadlineRecords = await firebase
                    .Child("DeadlineRecords")
                    .OnceAsync<DeadlineRecord>();

                var records = new List<DeadlineRecord>();

                foreach (var record in deadlineRecords)
                {
                    records.Add(record.Object);
                }

                return records;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting deadline records from Firebase: {ex.Message}");
                throw;
            }
        }

        // Modify the method signature to accept the key value
        public async Task DeleteRecordFromFirebase(string recordKeyToDelete)
        {
            try
            {
                // Reference the node in your Firebase Realtime Database where the records are stored
                var recordsRef = firebase.Child("DeadlineRecords");

                // Delete the record using the provided key
                await recordsRef.Child(recordKeyToDelete).DeleteAsync();

                Console.WriteLine($"Record with Key '{recordKeyToDelete}' deleted successfully.");
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the deletion process
                Console.WriteLine($"Error deleting record: {ex.Message}");
            }
        }




        // Functions to interact with TransactionRecords
        public async Task AddTransactionRecord(TransactionRecord record)
        {
            try
            {
                await firebase.Child("TransactionRecords").PostAsync(record);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding transaction record to Firebase: {ex.Message}");
                // Handle the exception as needed, such as logging or displaying an error message
                throw; // Rethrow the exception to propagate it further if necessary
            }
        }

        public async Task<List<TransactionRecord>> GetTransactionRecords()
        {
            try
            {
                // Retrieve transaction records from Firebase Database in real-time
                var transactionRecords = await firebase.Child("TransactionRecords").OnceAsync<TransactionRecord>();

                // Convert the retrieved data to a list of TransactionRecord objects
                var records = transactionRecords.Select(x => new TransactionRecord
                {
                    Amount = x.Object.Amount,
                    Purpose = x.Object.Purpose,
                    DateTime = x.Object.DateTime,
                    TransactionType = x.Object.TransactionType,
                    Comments = x.Object.Comments
                }).ToList();

                return records;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting transaction records from Firebase: {ex.Message}");
                throw;
            }
        }
    }
}
