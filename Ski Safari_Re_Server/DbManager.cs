using MySql.Data.MySqlClient;
using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

public class DbManager
{
	public static MySqlConnection mysql;

	private static JavaScriptSerializer Js = new JavaScriptSerializer();

	public static bool Connect(string db, string ip, int port, string user, string pw, string Name)
	{
		try
		{
			DbManager.mysql.Close();
		}
		catch
		{
		}
		DbManager.mysql = new MySqlConnection();
		string connectionString = string.Format("Database={0};Data Source={1}; port={2};User Id={3}; Password={4}", new object[]
		{
			db,
			ip,
			port,
			user,
			pw
		});
		DbManager.mysql.ConnectionString = connectionString;
		bool result;
		try
		{
			DbManager.mysql.Open();
			ReadLog._ReadLog("[数据库]connect succ ", null, true);
			result = true;
		}
		catch (Exception ex)
		{
			ReadLog._ReadLog("[数据库]connect fail, " + ex.Message, null, true);
			result = false;
		}
		return result;
	}

	private static bool IsSafeString(string str)
	{
		return !Regex.IsMatch(str, "[-|;|,|\\/|\\(|\\)|\\[|\\]|\\}|\\{|%|@|\\*|!|\\']");
	}

	public static bool IsAccountExistname(string id)
	{
		bool result;
		try
		{
			bool flag = !DbManager.IsSafeString(id);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				string text = string.Format("select * from account where name='{0}';", id);
				try
				{
					MySqlCommand mySqlCommand = new MySqlCommand(text, DbManager.mysql);
					MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
					bool hasRows = mySqlDataReader.HasRows;
					mySqlDataReader.Close();
					flag2 = !hasRows;
				}
				catch (Exception ex)
				{
					ReadLog._ReadLog("[数据库] IsSafeString err, " + ex.Message, null, true);
					flag2 = false;
				}
			}
			result = flag2;
		}
		catch
		{
			result = false;
		}
		return result;
	}

	public static bool IsAccountExist(string id)
	{
		bool result;
		try
		{
			bool flag = !DbManager.IsSafeString(id);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				string text = string.Format("select * from account where id='{0}';", id);
				try
				{
					MySqlCommand mySqlCommand = new MySqlCommand(text, DbManager.mysql);
					MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
					bool hasRows = mySqlDataReader.HasRows;
					mySqlDataReader.Close();
					flag2 = !hasRows;
				}
				catch (Exception ex)
				{
					ReadLog._ReadLog("[数据库] IsSafeString err, " + ex.Message, null, true);
					flag2 = false;
				}
			}
			result = flag2;
		}
		catch
		{
			result = false;
		}
		return result;
	}

	public static bool versionsvalue(float versionsvalue)
	{
		bool result;
		try
		{
			MySqlConnection mySqlConnection = new MySqlConnection();
			string connectionString = string.Format("Database={0};Data Source={1}; port={2};User Id={3}; Password={4}", new object[]
			{
				"server_data_fps+rpg",
				"127.0.0.1",
				3306,
				"root",
				"",
				"name"
			});
			mySqlConnection.ConnectionString = connectionString;
			try
			{
				mySqlConnection.Open();
			}
			catch (Exception ex)
			{
				ReadLog._ReadLog("[数据库]connect fail, " + ex.Message, null, true);
			}
			string text = string.Format("insert into versionsvalue set versionsvalue ='{0}';", versionsvalue);
			bool flag;
			try
			{
				MySqlCommand mySqlCommand = new MySqlCommand(text, mySqlConnection);
				mySqlCommand.ExecuteNonQuery();
				flag = true;
			}
			catch (Exception ex2)
			{
				ReadLog._ReadLog("[数据库] Register fail " + ex2.Message, null, true);
				flag = false;
			}
			mySqlConnection.Close();
			result = flag;
		}
		catch
		{
			result = false;
		}
		return result;
	}

	public static bool Register(string id, string pw, string name)
	{
		bool flag = !DbManager.IsSafeString(id);
		bool result;
		if (flag)
		{
			ReadLog._ReadLog("[数据库] Register fail, id not safe", null, true);
			result = false;
		}
		else
		{
			bool flag2 = !DbManager.IsSafeString(pw);
			if (flag2)
			{
				ReadLog._ReadLog("[数据库] Register fail, pw not safe", null, true);
				result = false;
			}
			else
			{
				bool flag3 = !DbManager.IsAccountExist(id) || !DbManager.IsAccountExistname(name);
				if (flag3)
				{
					ReadLog._ReadLog("[数据库] Register fail, id exist", null, true);
					result = false;
				}
				else
				{
					string text = string.Format("insert into account set id ='{0}' ,pw ='{1}',name='{2}';", id, pw, name);
					try
					{
						MySqlCommand mySqlCommand = new MySqlCommand(text, DbManager.mysql);
						mySqlCommand.ExecuteNonQuery();
						result = true;
					}
					catch (Exception ex)
					{
						ReadLog._ReadLog("[数据库] Register fail " + ex.Message, null, true);
						result = false;
					}
				}
			}
		}
		return result;
	}

	public static bool AddCreateObject(string id, string name, int nub)
	{
		bool result;
		try
		{
			bool flag = !DbManager.IsSafeString(id);
			bool flag2;
			if (flag)
			{
				ReadLog._ReadLog("[数据库] CreatePlayer fail, id not safe", null, true);
				flag2 = false;
			}
			else
			{
				Playercreat playercreat = new Playercreat();
				playercreat.creatobjects.Add(name, nub);
				string arg = DbManager.Js.Serialize(playercreat);
				string text = string.Format("insert into player set id ='{0}' ,creatdata='{1}';", id, arg);
				try
				{
					MySqlCommand mySqlCommand = new MySqlCommand(text, DbManager.mysql);
					mySqlCommand.ExecuteNonQuery();
					flag2 = true;
				}
				catch (Exception ex)
				{
					ReadLog._ReadLog("[数据库] CreatePlayer err, " + ex.Message, null, true);
					flag2 = false;
				}
			}
			result = flag2;
		}
		catch
		{
			result = false;
		}
		return result;
	}

	public static bool CreatePlayer(string id, string name)
	{
		bool flag = !DbManager.IsSafeString(id);
		bool result;
		if (flag)
		{
			ReadLog._ReadLog("[数据库] CreatePlayer fail, id not safe", null, true);
			result = false;
		}
		else
		{
			PlayerData playerData = new PlayerData();
			playerData.P_name = name;
			string arg = DbManager.Js.Serialize(playerData);
			Playercreat playercreat = new Playercreat();
			playercreat.creatobjects.Add("Box", 10);
			string text = DbManager.Js.Serialize(playercreat);
			text = text.Remove(0, 15);
			text = text.Remove(text.Length - 1);
			string text2 = string.Format("insert into player set id='{0}',data ='{1}';", id, arg);
			string text3 = string.Format("update player set creatdata='{0}' where id ='{1}';", text, id);
			try
			{
				MySqlCommand mySqlCommand = new MySqlCommand(text2, DbManager.mysql);
				mySqlCommand.ExecuteNonQuery();
				MySqlCommand mySqlCommand2 = new MySqlCommand(text3, DbManager.mysql);
				mySqlCommand2.ExecuteNonQuery();
				mySqlCommand.Clone();
				result = true;
			}
			catch (Exception ex)
			{
				ReadLog._ReadLog("[数据库] CreatePlayer err, " + ex.Message, null, true);
				result = false;
			}
		}
		return result;
	}

	public static float Getvalue()
	{
		float result;
		try
		{
			MySqlConnection mySqlConnection = new MySqlConnection();
			string connectionString = string.Format("Database={0};Data Source={1}; port={2};User Id={3}; Password={4}", new object[]
			{
                "server_data",
				"127.0.0.1",
				3306,
				"root",
				"",
				"name"
			});
			mySqlConnection.ConnectionString = connectionString;
			try
			{
				mySqlConnection.Open();
			}
			catch (Exception ex)
			{
				ReadLog._ReadLog("[数据库]connect fail, " + ex.Message, null, true);
			}
			string text = string.Format("select * from versionsvalue limit 1;", new object[0]);
			float num2;
			try
			{
				MySqlCommand mySqlCommand = new MySqlCommand(text, mySqlConnection);
				MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
				bool flag = !mySqlDataReader.HasRows;
				if (flag)
				{
					mySqlDataReader.Close();
				}
				mySqlDataReader.Read();
				float num = float.Parse(mySqlDataReader.GetString("versionsvalue"));
				mySqlDataReader.Close();
				num2 = num;
			}
			catch (Exception ex2)
			{
				ReadLog._ReadLog("[数据库] GetPlayerData fail, " + ex2.Message, null, true);
				num2 = 0f;
			}
			mySqlConnection.Close();
			result = num2;
		}
		catch
		{
			result = 0f;
		}
		return result;
	}

	public static PlayerData Get(string id)
	{
		PlayerData result;
		try
		{
			bool flag = !DbManager.IsSafeString(id);
			PlayerData playerData;
			if (flag)
			{
				ReadLog._ReadLog("[数据库] CheckPassword fail, id not safe", null, true);
				playerData = null;
			}
			else
			{
				string text = string.Format("select * from player where id='{0}' ;", id);
				try
				{
					MySqlCommand mySqlCommand = new MySqlCommand(text, DbManager.mysql);
					MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
					bool flag2 = !mySqlDataReader.HasRows;
					if (flag2)
					{
						mySqlDataReader.Close();
						playerData = null;
					}
					else
					{
						mySqlDataReader.Read();
						string @string = mySqlDataReader.GetString("data");
						PlayerData playerData2 = DbManager.Js.Deserialize<PlayerData>(@string);
						mySqlDataReader.Close();
						playerData = playerData2;
					}
				}
				catch (Exception ex)
				{
					ReadLog._ReadLog("[数据库] GetPlayerData fail, " + ex.Message, null, true);
					playerData = null;
				}
			}
			result = playerData;
		}
		catch
		{
			result = null;
		}
		return result;
	}

	public static bool CheckPassword(string id, string pw, string name)
	{
		bool result;
		try
		{
			bool flag = !DbManager.IsSafeString(id);
			bool flag2;
			if (flag)
			{
				ReadLog._ReadLog("[数据库] CheckPassword fail, id not safe", null, true);
				flag2 = false;
			}
			else
			{
				bool flag3 = !DbManager.IsSafeString(pw);
				if (flag3)
				{
					ReadLog._ReadLog("[数据库] CheckPassword fail, pw not safe", null, true);
					flag2 = false;
				}
				else
				{
					string text = string.Format("select * from account where id='{0}' and pw='{1}';", id, pw);
					string.Format("select * from account where id='{0}' and pw='{1}'and name= '{2}';", id, pw, name);
					try
					{
						MySqlCommand mySqlCommand = new MySqlCommand(text, DbManager.mysql);
						MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
						bool hasRows = mySqlDataReader.HasRows;
						mySqlDataReader.Close();
						flag2 = hasRows;
					}
					catch (Exception ex)
					{
						ReadLog._ReadLog("[数据库] CheckPassword err, " + ex.Message+"\n"+ex, null, true);
						flag2 = false;
					}
				}
			}
			result = flag2;
		}
		catch
		{
			result = false;
		}
		return result;
	}

	public static string GetID(string id)
	{
		string result;
		try
		{
			bool flag = !DbManager.IsSafeString(id);
			string text;
			if (flag)
			{
				ReadLog._ReadLog("[数据库] CheckPassword fail, id not safe", null, true);
				text = null;
			}
			else
			{
				string text2 = string.Format("select * from account where id='{0}' ;", id);
				try
				{
					MySqlCommand mySqlCommand = new MySqlCommand(text2, DbManager.mysql);
					MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
					bool flag2 = !mySqlDataReader.HasRows;
					if (flag2)
					{
						mySqlDataReader.Close();
						text = null;
					}
					else
					{
						mySqlDataReader.Read();
						string @string = mySqlDataReader.GetString("name");
						mySqlDataReader.Close();
						text = @string;
					}
				}
				catch (Exception ex)
				{
					ReadLog._ReadLog("[数据库] GetPlayerData fail, " + ex.Message, null, true);
					text = null;
				}
			}
			result = text;
		}
		catch
		{
			result = null;
		}
		return result;
	}

	public static PlayerData GetPlayerData(string id)
	{
		PlayerData result;
		try
		{
			bool flag = !DbManager.IsSafeString(id);
			PlayerData playerData;
			if (flag)
			{
				ReadLog._ReadLog("[数据库] GetPlayerData fail, id not safe", null, true);
				playerData = null;
			}
			else
			{
				string text = string.Format("select * from player where id ='{0}';", id);
				try
				{
                    MySqlCommand mySqlCommand = new MySqlCommand(text, DbManager.mysql);
					MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
					bool flag2 = !mySqlDataReader.HasRows;
					if (flag2)
					{
						mySqlDataReader.Close();
						playerData = null;
					}
					else
					{
						mySqlDataReader.Read();
						string @string = mySqlDataReader.GetString("data");
						PlayerData playerData2 = DbManager.Js.Deserialize<PlayerData>(@string);
						mySqlDataReader.Close();
						playerData = playerData2;
					}
				}
				catch (Exception ex)
				{
					ReadLog._ReadLog("[数据库] GetPlayerData fail, " + ex.Message, null, true);
					playerData = null;
				}
			}
			result = playerData;
		}
		catch
		{
			result = null;
		}
		return result;
	}

	public static string GetPlayerCreatData(string id)
	{
		string result;
		try
		{
			bool flag = !DbManager.IsSafeString(id);
			string text;
			if (flag)
			{
				ReadLog._ReadLog("[数据库] GetPlayerData fail, id not safe", null, true);
				text = null;
			}
			else
			{
				string text2 = string.Format("select * from player where id ='{0}';", id);
				try
				{
					MySqlCommand mySqlCommand = new MySqlCommand(text2, DbManager.mysql);
					MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
					bool flag2 = !mySqlDataReader.HasRows;
					if (flag2)
					{
						mySqlDataReader.Close();
						text = null;
					}
					else
					{
						mySqlDataReader.Read();
						string @string = mySqlDataReader.GetString("creatdata");
                        PlayerData playerData2 = DbManager.Js.Deserialize<PlayerData>(@string);
                        ReadLog._ReadLog(@string, null, true);
						mySqlDataReader.Close();
						text = @string;
					}
				}
				catch (Exception ex)
				{
					ReadLog._ReadLog("[数据库] GetPlayerData fail, " + ex.Message, null, true);
					text = null;
				}
			}
			result = text;
		}
		catch
		{
			result = null;
		}
		return result;
	}
    public static ItemData GetItem(string id)
    {
        ItemData result;
        ItemData Data= new ItemData { };
        try
        {
            bool flag = !DbManager.IsSafeString(id);
            string text;
            if (flag)
            {
                ReadLog._ReadLog("[数据库] GetPlayerData fail, id not safe", null, true);
                text = null;
            }
            else
            {
                string text2 = string.Format("select * from player where id ='{0}';", id);
                try
                {
                    MySqlCommand mySqlCommand = new MySqlCommand(text2, DbManager.mysql);
                    MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
                    bool flag2 = !mySqlDataReader.HasRows;
                    if (flag2)
                    {
                        mySqlDataReader.Close();
                        text = null;
                    }
                    else
                    {
                        mySqlDataReader.Read();
                        string @string = mySqlDataReader.GetString("item");
                        Data = DbManager.Js.Deserialize<ItemData>(@string);
                        ReadLog._ReadLog(@string, null, true);
                        mySqlDataReader.Close();
                        result = Data;
                    }
                }
                catch (Exception ex)
                {
                    ReadLog._ReadLog("[数据库] GetPlayerData fail, " + ex.Message, null, true);
                    text = null;
                }
            }
            result = Data;
        }
        catch
        {
            result = null;
        }
        return result;
    }
    public static bool UpdatePlayerData(string id, PlayerData playerData)
	{
		bool result;
		try
		{
			string arg = DbManager.Js.Serialize(playerData);
			string text = string.Format("update player set data='{0}' where id ='{1}';", arg, id);
			bool flag;
			try
			{
				MySqlCommand mySqlCommand = new MySqlCommand(text, DbManager.mysql);
				mySqlCommand.ExecuteNonQuery();
				flag = true;
			}
			catch (Exception ex)
			{
				ReadLog._ReadLog("[数据库] UpdatePlayerData err, " + ex.Message, null, true);
				flag = false;
			}
			result = flag;
		}
		catch
		{
			result = false;
		}
		return result;
	}

    public static bool UpdatePlayerItem(string id,Player player, ItemData itemData)
    {
        //结果
        bool result;
       
            //玩家coin
            string coin ="";
            //目标coin
            string coin_;
            //查询目标coin
            string text_ = string.Format("select * from Shop where itemid ='{0}';", itemData.itemId);
            try
            {
                MySqlCommand mySqlCommand = new MySqlCommand(text_, DbManager.mysql);
                MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
                bool flag2 = !mySqlDataReader.HasRows;
                if (flag2)
                {
                    mySqlDataReader.Close();
                    coin_ = null;
                }
                else
                {
                    mySqlDataReader.Read();
                    string @string = mySqlDataReader.GetString("coin");
                    ReadLog._ReadLog("Coin" + @string);
                    mySqlDataReader.Close();
                    coin_ = @string;
                }
            }
            catch (Exception ex)
            {
                ReadLog._ReadLog("[数据库] GetShopData fail, " + ex.Message, null, true);
                coin_ = null;
            }
            //判断是否为空
            if (coin_ != null && coin_ != "")
            {
            PlayerData pd = GetPlayerData(id);
            coin = pd.coin.ToString() ;
                //判断金币是否足够
                if (int.Parse(coin) - (int.Parse(coin_) * itemData.itemNub) >= 0)
                {
                    //反序列
                    string arg = DbManager.Js.Serialize(itemData);
                    //查询是否能够录入
                    string text2 = string.Format("update player set item='{0}' where id ='{1}';", arg, id);
                    bool flag;
                    try
                    {
                        MySqlCommand mySqlCommand_ = new MySqlCommand(text2, DbManager.mysql);
                        mySqlCommand_.ExecuteNonQuery();
                        pd.coin -= (int.Parse(coin_) * itemData.itemNub);
                        UpdatePlayerData(id, pd);
                        flag = true;
                    }
                    catch (Exception ex)
                    {
                        ReadLog._ReadLog("1");

                        ReadLog._ReadLog("[数据库] UpdatePlayerItem err, " + ex.Message, null, true);
                        flag = false;
                    }
                    result = flag;
                }else
                {
                    ReadLog._ReadLog("2");
                    result = false;
                }
            }
            else//如果为空
            {
                ReadLog._ReadLog("3");

                result = false;
            }
        

        
    
        return result;
    }
}
