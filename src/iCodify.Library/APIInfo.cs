using Csla;
using iCodify.Dal.Enums;

namespace iCodify.Dal
{
	[Serializable]
	public class APIInfo : ReadOnlyBase<APIInfo>
	{
		#region Properties

		public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
		public int Id
		{
			get => GetProperty(IdProperty);
			set => LoadProperty(IdProperty, value);
		}

		public static readonly PropertyInfo<APIModels> ModelProperty = RegisterProperty<APIModels>(c => c.Model);
		public APIModels Model
		{
			get => GetProperty(ModelProperty);
			set => LoadProperty(ModelProperty, value);
		}

		public static readonly PropertyInfo<string> APIKeyProperty = RegisterProperty<string>(c => c.APIKey);
		public string APIKey
		{
			get => GetProperty(APIKeyProperty);
			set => LoadProperty(APIKeyProperty, value);
		}

		public static readonly PropertyInfo<string> EndpointProperty = RegisterProperty<string>(c => c.Endpoint);
		public string Endpoint
		{
			get => GetProperty(EndpointProperty);
			set => LoadProperty(EndpointProperty, value);
		}

		public static readonly PropertyInfo<string> MediaTypeProperty = RegisterProperty<string>(c => c.MediaType);
		public string MediaType
		{
			get => GetProperty(MediaTypeProperty);
			set => LoadProperty(MediaTypeProperty, value);
		}

		#endregion

		#region Factory Methods

		public static APIInfo GetAPIData(APIModels model)
		{
			return DataPortal.Fetch<APIInfo>(model);
		}

		#endregion

		#region Data Access

		[Fetch]
		private void DataPortal_Fetch(APIModels model)
		{
			using var dalManager = DalFactory.GetManager();
			var dal = dalManager.GetProvider<IAPIDataDal>();
			var data = dal.Fetch(model);

			Id = data.Id;
			Model = data.Model;
			APIKey = data.APIKey;
			Endpoint = data.Endpoint;
			MediaType = data.MediaType;
		}

		#endregion
	}
}
