using System;
using System.Collections;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Reflection;

namespace de.pta.Component.ListOfValues.Internal
{
    /// <summary>
    /// Encpsulates the database access.
    /// </summary>
    /// <remarks>
    /// <pre>
    /// <b>History</b>
    /// <b>Author:</b> M.Keller, PTA GmbH
    /// <b>Date:</b> Apr/02/2003
    ///	<b>Remarks:</b> None
    /// </pre>
    /// </remarks>
    internal class LovDataReader
    {
        #region Members

        private OracleConnection dbConnection;
        private OracleDataReader dataReader;
        //private IDbConnection dbConnection;
        //private IDataReader dataReader;
        private String sqlString;
        private DbInformation dbInfo;
        private Assembly assembly;

        #endregion //End of Members

        #region Constructors

        /// <summary>
        /// Constructs the object.
        /// </summary>
        public LovDataReader(DbInformation dbInfo)
        {
            initialize();

            this.dbInfo = dbInfo;

            // Load assembly only once.
            assembly = Assembly.LoadWithPartialName(dbInfo.AssemblyName);

            //dbConnection = (IDbConnection)createInstance(dbInfo.ConnectionClass);
            dbConnection = (OracleConnection)createInstance(dbInfo.ConnectionClass);

        }

        private Object createInstance(String classToCreate)
        {
            // Get the type of the class and create an instance.
            Type type = assembly.GetType(classToCreate);
            return Activator.CreateInstance(type);
        }

        #endregion //End of Constructors

        #region Initialization

        private void initialize()
        {
            // Initializes the Object.
            dbConnection = null;
            dataReader = null;
            dbInfo = null;
        }

        #endregion //End of Initialization

        #region Accessors 
        #endregion //End of Accessors

        #region Methods

        /// <summary>
        /// Opens a database connection.
        /// </summary>
        /// <exception cref="ListOfValueException">throws an exception if the open fails.</exception>
        public void Open()
        {
            // close old connection
            dbConnection.Close();

            try
            {
                // new Connection
                //string x = "ACT_ENTW@//acttest-1:1521/ACT_ENTW";
                //x = "ACT_ENTW/ACTtest-1@//acttest-1:1521/ACT_ENTW";
                ////x = "user id=ACT_ENTW;data source=ACT_ENTW:1521/XE;password=ACTtest-1";
                //x = "Data Source=acttest-1:1521/ACT_ENTW;PERSIST SECURITY INFO=True;USER ID=ACT_ENTW;Password=ACTtest-1";
                //  dbConnection.ConnectionString = dbInfo.ConnectString;



                // dbConnection.ConnectionString = "Data Source=ACTQA.lev.de.bayer.cnb;USER ID=KBALA1;Password=W0rkwithbayer121";
                // dbConnection.ConnectionString = "Data Source=ACTQA.lev.de.bayer.cnb;USER ID=ACT_ENTW;Password=gdt_887ZZ5656GGt";

                //kbala1
                //dbConnection.ConnectionString = @"Data source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = by0qi7.de.bayer.cnb)(PORT = 1523))(CONNECT_DATA =(SERVICE_NAME = ACTQA.lev.de.bayer.cnb)));User ID=ACT_ENTW;Password=gdt_887ZZ5656GGt;";
                //QA  dbConnection.ConnectionString = @"Data source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = defr6-bcsexa-i01-g1-scan)(PORT = 1521))(CONNECT_DATA =(SERVICE_NAME = fra16r_ACTQA.bayer.cnb)));User ID=ACT_ENTW;Password=gdt_887ZZ5656GGt;";
                dbConnection.ConnectionString = @"Data source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = BY0QI9)(PORT = 1524))(CONNECT_DATA =(SID = ACT)));User ID=ACT_ENTW;Password=B#ndu4db82Gdne2DN8(4;";
               //   dbConnection.ConnectionString = @"Data source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = by0qi7.de.bayer.cnb)(PORT = 1523))(CONNECT_DATA =(SERVICE_NAME = ACTQA.lev.de.bayer.cnb)));User ID=KBALA1;Password=W0rkwithbayer121;";

                //   const string connectionString = @"Data Source=ACTQA.lev.de.bayer.cnb;Persist Security Info=True;User ID=KBALA1;Password=W0rkwithbayer121;Unicode=True;";
                //const string connectionString = @"Data source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = by0qi7.de.bayer.cnb)(PORT = 1523))(CONNECT_DATA =(SID = ACTQA)));User ID=KBALA1;Password=W0rkwithbayer121;";
                // const string connectionString = @"SERVER=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = by0qi7.de.bayer.cnb)(PORT = 1523))(CONNECT_DATA =(SID = ACTQA)));User ID=KBALA1;Password=W0rkwithbayer121;";
                //  const string connectionString = @"Data source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = MOXBWM.PITTS.BAYER.COM)(PORT = 1521))(CONNECT_DATA =(SID = EBIZBCSL)));User ID=KBALA1;Password=W0rkwithbayer121;";

                //         OracleConnection connection = new OracleConnection(connectionString);
                //          connection.Open();

                //username/password@[//]host[:port][/service_name]
                dbConnection.Open();
            }
            catch (Exception e)
            { 
                throw new ListOfValueException("ERROR_LOV_CANNOT_OPEN_DATABASE", e);
            }
        }

        /// <summary>
        /// closes the database.
        /// </summary>
        public void Close()
        {
            dataReader.Close();
            dbConnection.Close();
        }

        /// <summary>
        /// Executes a given SQL statement.
        /// </summary>
        /// <param name="sql">String with the SQL statement.</param>
        /// <param name="restriction">String with the restriction.</param>
        public void ExecuteSql(String sql, String restriction)
        {
            //IDbCommand command = null;
            OracleCommand command = null;
            sqlString = ConsiderRestriction(sql, restriction);

            // check, if the connection is established.
            if (ConnectionState.Open != dbConnection.State)
            {
                throw new ListOfValueException("ERROR_LOV_NO_DB_CONNECTION");
            }

            // create the command and set values.
            //command = (IDbCommand)createInstance(dbInfo.CommandClass);
            command = (OracleCommand)createInstance(dbInfo.CommandClass);
            command.CommandText = sqlString;
            command.Connection = dbConnection;
            command.CommandTimeout = 60;

            // if there is alredy an existing reader, which is open
            // close it.
            if ((dataReader != null) && (!dataReader.IsClosed))
            {
                dataReader.Close();
            }

            try
            {
                // execute query
                dataReader = command.ExecuteReader();
            }
            catch (Exception e)
            {
                ListOfValueException lovException = new ListOfValueException("ERROR_DATAACCESS_EXECUTING_SQL", e);

            }
        }

        private String ConsiderRestriction(String sql, String restriction)
        {

            String newSqlStatement = String.Empty;
            const String RESTRICTION = "$RESTRICTION";

            // Search for the string to replace.
            if (sql.IndexOf(RESTRICTION) >= 0)
            {
                // When a restriction is configured in the SQL statement, but is
                // not set via the LovSingleton, throw an exception.
                if (restriction.Equals(String.Empty))
                {
                    throw new ListOfValueException("ERR_RESTRICTION_IN_SQL_STATEMENT_BUT_NOT_SET_VIA_LOVSINGLETON");
                }
                else
                {
                    newSqlStatement = sql.Replace(RESTRICTION, restriction);
                }
            }
            else
            {
                // Nothing to replace, use the original
                // SQL statement.
                newSqlStatement = sql;
            }

            // Return the statement.
            return newSqlStatement;
        }

        /// <summary>
        /// Fills a list of values
        /// </summary>
        /// <param name="lov">Reference of the a ListOfValues object.</param>
        public void FillListOfValues(ListOfValues lov)
        {
            Object obj = null;
            Attribute attrib = null;
            IDataRecord dataRecord = (IDataRecord)dataReader;

            // if this does't match, the configuration is wrong!
            if (lov.ConfigAttributes.Count != dataRecord.FieldCount)
            {
                throw new ListOfValueException("ERROR_LOV_CONFIGURATION_ERROR");
            }

            try
            {
                // read the recieved data
                while (dataReader.Read())
                {
                    ListOfValuesItem item = new ListOfValuesItem(lov.ConfigAttributes.Count);

                    // iterate the fields of a line.
                    for (int i = 0; i < dataRecord.FieldCount; ++i)
                    {
                        // get the attributes
                        attrib = (Attribute)lov.ConfigAttributes[i];

                        // create an instance of the desired object.
                        // if this fails, use an System.Object.
                        try
                        {
                            Type type = Type.GetType(attrib.Type);
                            obj = Activator.CreateInstance(type);
                        }
                        catch (Exception e)
                        {
                            String error = e.Message;
                            obj = new Object();
                        }
                        obj = dataReader[i];

                        // verify if the field is a primery key.
                        if (attrib.IsPrimeryKey)
                        {
                            item.Id += dataReader[i];
                        }

                        // Add values to the item
                        item.AddValues(attrib.Id, obj);
                    }

                    // Add item to the list of values.
                    lov.AddItem(item);
                }
            }
            catch (Exception e)
            {   //			ListOfValueException 
                ListOfValueException lovException;
                lovException = new ListOfValueException("ERROR_LOV_READING_DATA", e);
                lovException.AddParameter(sqlString);
                throw lovException;
            }
        }

        #endregion // Methods
    }
}