using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using OpenDentBusiness;

namespace PluginExample {
	public class ConvertPluginDatabase {
		private static Version FromVersion;

		public static void Begin() {//unlike in the main program, this conversion script runs on every startup.
			Version MainAppVersion=new Version(System.Windows.Forms.Application.ProductVersion);//eg. 6.8.0.0
			Version MainAppMajMin=new Version(MainAppVersion.Major,MainAppVersion.Minor);//eg. 6.8
			Version ThisAssemblyVersion=System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;//eg. 6.7.0.0
			Version ThisAssMajMin=new Version(ThisAssemblyVersion.Major,ThisAssemblyVersion.Minor);//eg. 6.7
			if(ThisAssMajMin > MainAppMajMin) {
				//Prevent the use of a new plugin with an old version of OD.  User should upgrade OD.
				throw new ApplicationException("Version of plug-in may not be newer than version of Open Dental.");
			}
			//In case the developer of a plug-in gets hit by a bus etc, there should be no restriction on using an old plugin with a new version of OD.
			//If a plug-in is distributed, there should be a separate plugin available for each minor version.
			string command="SELECT ValueString FROM preference WHERE PrefName='Plugin_JSS_DataBaseVersion'";//notice the very unique name.
			DataTable table=DataCore.GetTable(command);
			if(table.Rows.Count==0) {//This plugin has never run before.
				command="INSERT INTO preference (PrefName,ValueString) VALUES('Plugin_JSS_DataBaseVersion','1.0.0.0')";
				DataCore.NonQ(command);
				FromVersion=new Version(1,0);
			}
			else {
				FromVersion=new Version(table.Rows[0][0].ToString());
			}			
			if(FromVersion < new Version("6.8.0.0")) {//6.8.1.0
				//remember to NEVER use "0" versions (head), which are still in development.
				command="DROP TABLE IF EXISTS jss_dev_myveryuniquetable";//best practice to drop first in case file is present from an old backup.
				DataCore.NonQ(command);
				command=@"CREATE TABLE jss_dev_myveryuniquetable (
					PriKeyNum INT AUTO_INCREMENT,
					ForeignKeyNum INT,
					Descript VARCHAR(255),
					PRIMARY KEY(PriKeyNum)
					)";
				DataCore.NonQ(command);
				command="UPDATE preference SET ValueString = '6.8.0.0' WHERE PrefName = 'Plugin_JSS_DataBaseVersion'";
				DataCore.NonQ(command);
			}
			To7_1_0();
		}

		private static void To7_1_0(){//don't forget to change the version of this dll in AssemblyInfo.cs in order to trigger this method.
			if(FromVersion < new Version("7.1.0.0")) {
				string command;
				command="ALTER TABLE jss_dev_myveryuniquetable CHANGE PriKeyNum PriKeyNum bigint NOT NULL auto_increment";
				DataCore.NonQ(command);
				command="ALTER TABLE jss_dev_myveryuniquetable CHANGE ForeignKeyNum ForeignKeyNum bigint NOT NULL";
				DataCore.NonQ(command);
				command="UPDATE preference SET ValueString = '7.1.0.0' WHERE PrefName = 'Plugin_JSS_DataBaseVersion'";
				DataCore.NonQ(command);
			}
			//To7_2_0(){//etc
		}

		//private static void To7_2_0() {
			//if(FromVersion < new Version("7.2.0.0")) {//etc
				//etc
			//}
			//To7_3_0(){//etc
		//}

		


	}
}