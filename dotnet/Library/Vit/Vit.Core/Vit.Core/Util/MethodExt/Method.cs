using System;

namespace Vit.Core.Util.MethodExt
{
    public static class Method
    {

        #region Override
        public static Func<Arg, Ret> Override<Arg, Ret>(Func<Arg, Ret> baseFunc, Func<Func<Arg, Ret>, Arg, Ret> thisFunc)
        {
            return (arg) =>
            {
                return thisFunc(baseFunc, arg);
            };
        }

        public static Func<Ret> Override<Ret>(Func<Ret> baseFunc, Func<Func<Ret>, Ret> thisFunc)
        {
            return () =>
            {
                return thisFunc(baseFunc);
            };
        }
        #endregion



        #region  Wrap

        public static void Wrap<Ret>(ref Func<Ret> baseFunc, Func<Func<Ret>, Ret> thisFunc)
        {
            var baseFunc_value = baseFunc;
            baseFunc = () =>
            {
                return thisFunc(baseFunc_value);
            };
        }

        public static void Wrap(ref Action baseFunc, Action<Action> thisFunc)
        {
            var baseFunc_value = baseFunc;
            baseFunc = () =>
            {
                thisFunc(baseFunc_value);
            };
        }
        #endregion


    }
}
