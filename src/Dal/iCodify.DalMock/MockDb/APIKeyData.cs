using iCodify.Dal.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCodify.DalMock.MockDb
{
	public class APIKeyData
	{
		public int Id { get; set; }
		public APIModels Model { get; set; } 
		public string APIKey { get; set; } = string.Empty;
		public string Endpoint { get; set; } = string.Empty;
		public string MediaType { get; set; } = "application/json";
	}
}
