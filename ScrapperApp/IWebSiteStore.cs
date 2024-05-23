public interface IWebSiteStore
{
    Task Save(string filename, byte[] bytes);
}