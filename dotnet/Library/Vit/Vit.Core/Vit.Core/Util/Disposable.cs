using System;

namespace Vit.Core.Util
{
    public class Disposable : IDisposable
    {
        Action onDispose;
        public Disposable(Action onDispose)
        {
            this.onDispose = onDispose;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            onDispose?.Invoke();
        }
    }
}
