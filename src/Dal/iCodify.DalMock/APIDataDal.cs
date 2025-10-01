using iCodify.Dal;
using iCodify.Dal.Dto;
using iCodify.Dal.Enums;
using iCodify.DalMock.MockDb;
using System.Xml.Linq;

namespace iCodify.DalMock
{
	public class APIDataDal : IAPIDataDal
	{
		public APIDataDto Fetch(int id)
		{
			var data = (from r in Database.APIKeys
						where r.Id == id
						select new APIDataDto
						{
							Id = r.Id,
							Model = r.Model,
							APIKey = r.APIKey,
							Endpoint = r.Endpoint,
							MediaType = r.MediaType
						}).FirstOrDefault();

			return data 
				?? throw new InvalidOperationException($"APIKey with id {id} not found.");
		}

		public APIDataDto Fetch(string name)
		{
			var data = (from r in Database.APIKeys
						where r.Model.Equals(name)
						select new APIDataDto
						{
							Id = r.Id,
							Model = r.Model,
							APIKey = r.APIKey,
							Endpoint = r.Endpoint,
							MediaType = r.MediaType
						}).FirstOrDefault();

			return data 
				?? throw new InvalidOperationException($"APIKey with name '{name}' not found.");
		}

		public APIDataDto Fetch(APIModels model)
		{
			var data = (from r in Database.APIKeys
						where r.Model.Equals(model)
						select new APIDataDto
						{
							Id = r.Id,
							Model = r.Model,
							APIKey = r.APIKey,
							Endpoint = r.Endpoint,
							MediaType = r.MediaType
						}).FirstOrDefault();

			return data
				?? throw new InvalidOperationException($"APIKey with model '{model}' not found.");
		}
	}
}
