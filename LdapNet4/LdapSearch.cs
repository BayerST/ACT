using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.DirectoryServices.Protocols;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.AccountManagement;


namespace LdapNet4
{
    public class LdapSearch
    {

     	#region Constants

        private const int LDAP_PORT_DEFAULT = 389;
        private const int LDAP_PORT_SSL = 636;

		#endregion Constants
		
		#region Members

//		private LdapClient  mLdapClient;

		private string   mLdapServerName;
		private bool     mLdapUseSSL     ;
		private string   mLdapBindUserPrefix;
		private string   mLdapBindUserBaseDn;

		private string   mInfoMessage;

		#endregion Members

        #region Accessors

        /// <summary>
        /// Gets or sets current mLdapServerName
        /// </summary>
        public string LdapServerName
        {
            get { return mLdapServerName; }
            set { mLdapServerName = value; }
        }

        /// <summary>
        /// Gets or sets mLdapUseSSL
        /// </summary>
        public bool LdapUseSSL
        {
            get { return mLdapUseSSL; }
            set { mLdapUseSSL = value; }
        }

        /// <summary>
        /// Gets or sets mLdapBindUserPrefix
        /// </summary>
        public string LdapBindUserPrefix
        {
            get { return mLdapBindUserPrefix; }
            set { mLdapBindUserPrefix = value; }
        }

        /// <summary>
        /// Gets or sets mLdapBindUserBaseDn
        /// </summary>
        public string LdapBindUserBaseDn
        {
            get { return mLdapBindUserBaseDn; }
            set { mLdapBindUserBaseDn = value; }
        }
        #endregion

		#region Constructors

		/// <summary>
		/// Standard Constructor for LDAP search.
		/// </summary>
		public LdapSearch()
		{
			this.Initialize();
		} 

		



		#endregion Constructors

		#region Inititialization

		/// <summary>
		/// Initialize the members, LDAP configuration from app.config
		/// </summary>
		private void Initialize()
		{
            //mLdapServerName     = (Convert.ToString(ConfigurationSettings.AppSettings["LdapSearch.ServerName"]));
            //mLdapUseSSL         = (Convert.ToBoolean(ConfigurationSettings.AppSettings["LdapSearch.SSL"]));
            //mLdapBindUserPrefix = (Convert.ToString(ConfigurationSettings.AppSettings["LdapSearch.BindUserPrefix"]));
            //mLdapBindUserBaseDn = (Convert.ToString(ConfigurationSettings.AppSettings["LdapSearch.BindUserBaseDn"]));


            //mLdapServerName = (Convert.ToString( .AppSettings["LdapSearch.ServerName"]));
            //mLdapUseSSL = (Convert.ToBoolean(ConfigurationManager.AppSettings["LdapSearch.SSL"]));
            //mLdapBindUserPrefix = (Convert.ToString(ConfigurationManager.AppSettings["LdapSearch.BindUserPrefix"]));
            //mLdapBindUserBaseDn = (Convert.ToString(ConfigurationManager.AppSettings["LdapSearch.BindUserBaseDn"]));
            

		}


		#endregion Inititialization


		#region Methods


 		/// <summary>
		/// Bind to LDAP-Server with C#
		/// </summary>
		public bool LdapBind(string pUserName, string pUserPwd)
		{
			bool bindOk = false;
            
			string bindUserDn = String.Empty;
			string connectString = String.Empty;
			int ldapServerPort = LDAP_PORT_SSL;



			try
			{
				bindUserDn = mLdapBindUserPrefix + "=" + pUserName + "," + mLdapBindUserBaseDn;
				ldapServerPort = ( mLdapUseSSL ) ? LDAP_PORT_SSL : LDAP_PORT_DEFAULT;
				connectString = mLdapServerName + ":" + ldapServerPort.ToString();

				mInfoMessage = "Start LdapNet4_BindToServer" + Environment.NewLine;
				// Connect
				mInfoMessage += "Connect Start to " + connectString + Environment.NewLine;
				mInfoMessage += ( mLdapUseSSL ) ? "Use SSL" : "";
				mInfoMessage += " bindUserDn: " + bindUserDn + Environment.NewLine;

					// connect
//					mLdapClient = null;
//						mLdapClient = new LdapClient(mLdapServerName,ldapServerPort,true,mLdapUseSSL);
					// bind
					//					clnt.ldap_simple_bind_s("bind_dn", "your_pass");					
//					mLdapClient.ldap_simple_bind_s(bindUserDn, pUserPwd);					

                    // NetworkCredential
                    LdapDirectoryIdentifier ldapIdentifier  = new LdapDirectoryIdentifier(mLdapServerName,ldapServerPort);

                    using (LdapConnection ldapConn = new LdapConnection(ldapIdentifier))
                    {
                        //System.DirectoryServices.Protocols.LdapConnection ldapConn = new LdapConnection(ldapIdentifier);

                        NetworkCredential credential = new NetworkCredential();
                        credential.UserName = bindUserDn;
                        credential.Password = pUserPwd;

                        ldapConn.Credential = credential;

                        //ldapConn.SessionOptions.
                        ldapConn.Bind();


                        //
                        mInfoMessage += " ConnectionURL: " + connectString + Environment.NewLine;
                        bindOk = true;
                    }





			}
			catch (System.DirectoryServices.Protocols.LdapException ldap_netEx)
			{
				mInfoMessage += "LdapException: " + ldap_netEx.Message + Environment.NewLine;
				mInfoMessage += "LdapException: " + ldap_netEx.StackTrace + Environment.NewLine;
		
				LdapException ldapEx = new LdapException("Error during LDAP authentication: " + ldap_netEx.Message,ldap_netEx);
				throw ldapEx;
			}
			catch (Exception ex)
			{
				bindOk = false;
				mInfoMessage += "Error in LdapConnectToServer:" + Environment.NewLine;
				mInfoMessage += "Connect to "+ connectString + "failed." + Environment.NewLine;
				mInfoMessage += ex.Message + Environment.NewLine;
				mInfoMessage += ex.StackTrace + Environment.NewLine;

				LdapException ldapEx = new LdapException("LDAP Error: " + ex.Message, ex);
				throw ldapEx;
			}
			finally
			{
				mInfoMessage += "Connect bind " + bindOk.ToString() + Environment.NewLine;
				//Globals.GetInstance().Log.Info(mInfoMessage );

			}
			//this.LdapDisconnect();
          
			return bindOk;	
		}


		/// <summary>
		/// Disconnect from LDAP-server.
		/// </summary>
        //public void LdapDisconnect()
        //{
        //    try
        //    {
        //        mLdapClient.Dispose();
        //    }
        //    catch (Exception ex)
        //    {
        //        mInfoMessage += "Error in LdapDisconnect: " + Environment.NewLine;
        //        mInfoMessage += ex.Message + Environment.NewLine;			
        //    }

		//}

		#endregion Methods


		#region Accessors

//		/// <summary>
//		/// Boolean indication wether the LDAP login was successfull
//		/// </summary>
//		public bool LdapLoginOk
//		{
//			get { return mLdapSearchParamsVO.LoginSuccessful; }
//		}
//

		public string InfoMessage
		{
			get { return mInfoMessage; }
		}


		#endregion Accessors
    }
}
