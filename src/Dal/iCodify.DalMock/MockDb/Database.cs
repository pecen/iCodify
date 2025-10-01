using iCodify.Dal.Enums;

namespace iCodify.DalMock.MockDb
{
	public class Database
	{
		public static List<APIKeyData> APIKeys { get; private set; }

		static Database()
		{
			APIKeys = new List<APIKeyData>
			{
				//new APIKeyData{ Id = 0, Model = APIModels.Opper, APIKey = "op-ESZCBUT1VRVO2F3TGTI8", Endpoint = "https://api.opper.ai/v2/call" },
				new APIKeyData{ Id = 0, Model = APIModels.Opper, APIKey = "op-A2EGIMSYY4RJILF8IM0X", Endpoint = "https://api.opper.ai/v2/call" },
				new APIKeyData{ Id = 1, Model = APIModels.OpenAI, APIKey = "sadlofjewpfedhkgaldfv", Endpoint = "https://api.chatgpt.com/v2/request" },
				new APIKeyData{ Id = 2, Model = APIModels.Gemini, APIKey = "ewroiyjbkldbqrrrö-4rkf", Endpoint = "https://api.gemini.ai/v2/getanswer" }
			};
		}
	}
}
