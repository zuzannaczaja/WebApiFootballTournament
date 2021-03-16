namespace WebApiFootballTournament.Helpers
{
    public interface IPropertyCheckerService
    {
        bool TypeHasProperties<T>(string fields);
    }
}