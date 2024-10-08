using DocumentStorage.Documents;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace DocumentStorage.DB
{
    class DBRequests
    {
        #region Класс подключения/переменные

        SQLiteConnection dbConnection;
        string dbFileName = "MyDocsStorage.sqlite";

        #endregion

        #region Конструктор

        public DBRequests()
        {
            string conn = string.Format("DataSource={0}", dbFileName);
            dbConnection = new SQLiteConnection(conn);
        }

        #endregion

        #region Создание БД, если не отсутствует

        public string DbChecking()
        {
            try
            {
                if (!System.IO.File.Exists(dbFileName))
                {
                    SQLiteConnection.CreateFile(dbFileName);
                }
                CreateTableDocs();

                CreateTableDocsAuditLog();

                CreateInsertTrigger();
                CreateUpdateTrigger();
                CreateDeleteTrigger();

                
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private void CreateTableDocs()
        {
            try
            {
                dbConnection.Open();
                SQLiteCommand cmd = dbConnection.CreateCommand();
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Docs(" +
                    "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "Title TEXT NOT NULL, " +
                    "FileName TEXT, " +
                    "Comment TEXT, " +
                    "DocData BLOB, " +
                    "DocSampleData BLOB, " +
                    "CreatedOn TEXT, " +
                    "UpdatedOn TEXT)";
                cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                throw new Exception("Возникла ошибка при создании таблицы \"Docs\":\r\n" + ex.Message);
            }
            finally
            {
                dbConnection.Close();
            }
        }

        #region Логирование изменения данных

        private void CreateTableDocsAuditLog()
        {
            try
            {
                dbConnection.Open();
                SQLiteCommand cmd = dbConnection.CreateCommand();
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS DocsAuditLog(" +
                    "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "DocId INTEGER, " +
                    "Title TEXT, " +
                    "FileName TEXT, " +
                    "Comment TEXT, " +
                    "DocData BLOB, " +
                    "DocSampleData BLOB, " +
                    "Operation TEXT, " +
                    "UpdatedOn TEXT)";
                cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                throw new Exception("Возникла ошибка при создании таблицы \"DocsAuditLog\":\r\n" + ex.Message);
            }
            finally
            {
                dbConnection.Close();
            }
        }

        private void CreateInsertTrigger()
        {
            try
            {
                dbConnection.Open();
                SQLiteCommand cmd = dbConnection.CreateCommand();
                cmd.CommandText = "CREATE TRIGGER IF NOT EXISTS trgDocInsert " +
                    "AFTER INSERT ON Docs " +
                    "BEGIN " +
                    "INSERT INTO DocsAuditLog(DocId, Title, FileName, Comment, DocData, DocSampleData, Operation, UpdatedOn) " +
                    "VALUES (NEW.Id, NEW.Title, NEW.FileName, NEW.Comment, NEW.DocData, NEW.DocSampleData, 'INSERT', CURRENT_TIMESTAMP); " +
                    "END;";
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Возникла ошибка при создании триггера на добавление записи:\r\n" + ex.Message);
            }
            finally
            {
                dbConnection.Close();
            }
        }

        private void CreateUpdateTrigger()
        {
            try
            {
                dbConnection.Open();
                SQLiteCommand cmd = dbConnection.CreateCommand();
                cmd.CommandText = "CREATE TRIGGER IF NOT EXISTS trgDocUpdate " +
                    "AFTER UPDATE ON Docs " +
                    "BEGIN " +
                    "INSERT INTO DocsAuditLog(DocId, Title, FileName, Comment, DocData, DocSampleData, Operation, UpdatedOn) " +
                    "VALUES (NEW.Id, NEW.Title, NEW.FileName, NEW.Comment, NEW.DocData, NEW.DocSampleData, 'UPDATED', CURRENT_TIMESTAMP); " +
                    "END;";
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Возникла ошибка при создании триггера на обновление записи:\r\n" + ex.Message);
            }
            finally
            {
                dbConnection.Close();
            }
        }

        private void CreateDeleteTrigger()
        {
            try
            {
                dbConnection.Open();
                SQLiteCommand cmd = dbConnection.CreateCommand();
                cmd.CommandText = "CREATE TRIGGER IF NOT EXISTS trgDocDelete " +
                    "AFTER DELETE ON Docs " +
                    "BEGIN " +
                    "INSERT INTO DocsAuditLog(DocId, Title, FileName, Comment, DocData, DocSampleData, Operation, UpdatedOn) " +
                    "VALUES (OLD.Id, OLD.Title, OLD.FileName, OLD.Comment, OLD.DocData, OLD.DocSampleData, 'DELETED', CURRENT_TIMESTAMP); " +
                    //"FROM OLD; " +
                    "END;";
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Возникла ошибка при создании триггера на удаление записи:\r\n" + ex.Message);
            }
            finally
            {
                dbConnection.Close();
            }
        }
        
        #endregion
        
        #endregion

        #region Получение списка документов

        /// <summary>
        /// Получение списка документов.
        /// </summary>
        /// <param name="err">возвращает пусто, если ошибок нет. В противном случае возвращает сообщение ошибки.</param>
        /// <returns>возвращает все документы из БД</returns>
        public List<Doc> GetAllDocs(out string err)
        {
            List<Doc> Docs;
            try
            {
                err = string.Empty;
                Docs = new List<Doc>();
                dbConnection.Open();
                SQLiteCommand cmd = dbConnection.CreateCommand();
                cmd.CommandText = "SELECT Id, Title, FileName, " +
                    "Comment, DocData, DocSampleData, CreatedOn, UpdatedOn FROM Docs";
                SQLiteDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Doc doc = new Doc();
                    doc.Id = rdr.GetInt32(0);
                    doc.Title = rdr.GetString(1);
                    doc.FileName = rdr.IsDBNull(2) ? string.Empty : rdr.GetString(2);
                    doc.Comment = rdr.IsDBNull(3) ? string.Empty : rdr.GetString(3);
                    doc.DocData = rdr.IsDBNull(4) ? null : (byte[])rdr.GetValue(4);
                    doc.DocSampleData = rdr.IsDBNull(5) ? null : (byte[])rdr.GetValue(5);
                    doc.CreatedOn = rdr.GetDateTime(6);
                    doc.UpdatedOn = rdr.GetDateTime(7);
                    Docs.Add(doc);
                }
                rdr.Close();
                return Docs;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return new List<Doc>();
            }
            finally
            {
                dbConnection.Close();
            }
        }

        #endregion

        #region редактирование списка документов

        /// <summary>
        /// Добавление документа.
        /// </summary>
        /// <param name="doc">документ для добавления в БД.</param>
        /// <param name="err">возвращает пусто, если ошибок нет. В противном случае возвращает сообщение ошибки.</param>
        /// <returns>возвращает Id добавленного документа (в случае ошибки возвращает -1).</returns>
        public long InsertDocToDb(Doc doc, out string err)
        {
            try
            {
                err = string.Empty;
                dbConnection.Open();
                SQLiteCommand cmd = dbConnection.CreateCommand();
                cmd.CommandText = "INSERT INTO Docs (Title, FileName, Comment, DocData, DocSampleData, CreatedOn, UpdatedOn) " +
                    "VALUES (@Title, @FileName, @Comment, @DocData, @DocSampleData, @CreatedOn, @UpdatedOn); select last_insert_rowid();";
                cmd.Parameters.Add("@Title", System.Data.DbType.String).Value = doc.Title;
                cmd.Parameters.Add("@FileName", System.Data.DbType.String).Value = doc.FileName;
                cmd.Parameters.Add("@Comment", System.Data.DbType.String).Value = doc.Comment;
                cmd.Parameters.Add("@DocData", System.Data.DbType.Binary).Value = doc.DocData;
                cmd.Parameters.Add("@DocSampleData", System.Data.DbType.Binary).Value = doc.DocSampleData;
                cmd.Parameters.Add("@CreatedOn", System.Data.DbType.String).Value = doc.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss.sss");
                cmd.Parameters.Add("@UpdatedOn", System.Data.DbType.String).Value = doc.UpdatedOn.ToString("yyyy-MM-dd HH:mm:ss.sss");
                return (long)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return -1;
            }
            finally
            {
                dbConnection.Close();
            }
        }

        /// <summary>
        /// Обновление документа.
        /// </summary>
        /// <param name="doc">обновленные данные документа для замены в БД.</param>
        /// <param name="err">возвращает пусто, если ошибок нет. В противном случае возвращает сообщение ошибки.</param>
        /// <returns>возвращает булевое значение попытки обновления документа.</returns>
        public bool UpdateDoc(Doc doc, out string err)
        {
            try
            {
                err = string.Empty;
                dbConnection.Open();
                SQLiteCommand cmd = dbConnection.CreateCommand();
                cmd.CommandText = "UPDATE Docs SET Title = @Title, FileName = @FileName, Comment = @Comment, DocData = @DocData, " +
                    "DocSampleData = @DocSampleData, UpdatedOn = '" + doc.UpdatedOn.ToString("yyyy-MM-dd HH:mm:ss.sss") + "' WHERE Id='" + doc.Id + "';";
                cmd.Parameters.Add("@Title", System.Data.DbType.String).Value = doc.Title;
                cmd.Parameters.Add("@FileName", System.Data.DbType.String).Value = doc.FileName;
                cmd.Parameters.Add("@Comment", System.Data.DbType.String).Value = doc.Comment;
                cmd.Parameters.Add("@DocData", System.Data.DbType.Binary).Value = doc.DocData;
                cmd.Parameters.Add("@DocSampleData", System.Data.DbType.Binary).Value = doc.DocSampleData;
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return false;
            }
            finally
            {
                dbConnection.Close();
            }
        }

        /// <summary>
        /// Удаление документа.
        /// </summary>
        /// <param name="id">id удаляемого документа.</param>
        /// <param name="err">возвращает пусто, если ошибок нет. В противном случае возвращает сообщение ошибки.</param>
        public bool DeleteDoc(long id, out string err)
        {
            try
            {
                err = string.Empty;
                dbConnection.Open();
                SQLiteCommand cmd = dbConnection.CreateCommand();
                cmd.CommandText = "DELETE FROM Docs WHERE Id='" + id + "';";
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return false;
            }
            finally
            {
                dbConnection.Close();
            }
        }

        #endregion
    }
}
