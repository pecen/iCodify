namespace iCodify.Dal
{
	public interface IDalManager : IDisposable
	{
		T GetProvider<T>() where T : class;
	}
}
