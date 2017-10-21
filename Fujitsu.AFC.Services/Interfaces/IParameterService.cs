using Fujitsu.AFC.Model;

namespace Fujitsu.AFC.Services.Interfaces
{
    public interface IParameterService : IService<Parameter>
    {
        Parameter Find(string parameterName);
        T GetParameterByName<T>(string parameterName);
        T GetParameterByNameAndCache<T>(string parameterName);
        T GetParameterByNameOrCreate<T>(string parameterName, T defaultValue);
        void SaveParameter<T>(string parameterName, T value);
    }
}
