using System;

namespace Vit.Core.Util
{
    public class Disposable: IDisposable
    {
        Action onDispose;
        public Disposable(Action onDispose) 
        {
            this.onDispose = onDispose;
        }

        public void Dispose()
        {
            onDispose?.Invoke();
        }
    }
}
