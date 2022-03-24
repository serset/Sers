using System.Threading.Tasks;

namespace Vit.Core.Module.Log.LogCollector
{
    public interface ILogCollector
    {
        void Write(LogMessage msg); 
    }
}
