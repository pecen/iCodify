using System.Configuration;

namespace iCodify.Dal
{
	public static class DalFactory
	{
		private static Type? _dalType;

		public static IDalManager GetManager()
		{
			if (_dalType == null)
			{
				var dalTypeName = ConfigurationManager.AppSettings["DalManagerType"];
				
				if (!string.IsNullOrEmpty(dalTypeName))
				{
					_dalType = Type.GetType(dalTypeName) 
							?? throw new ArgumentException(string.Format("Type {0} could not be found", dalTypeName));
				}
				else
				{
					throw new NullReferenceException("DalManagerType");
				}
			}
			
			var instance = Activator.CreateInstance(_dalType);
			
			if (instance is not IDalManager dalManager)
			{
				throw new InvalidCastException($"Type {_dalType.FullName} does not implement IDalManager.");
			}
			
			return dalManager;
		}
	}
}
