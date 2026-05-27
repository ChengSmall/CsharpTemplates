using System;
using System.Runtime;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace Cheng.Json.Convers
{

    internal sealed class c_ConverTo_IFAndCtorV<T> : ConverToJsonAndObj<T> where T : IConverToJson
    {

        public c_ConverTo_IFAndCtorV(ConstructorInfo ctor)
        {
            p_ctor = ctor;
            p_par = new object[1];
            p_canToObj = (object)ctor != null;
        }

        private readonly ConstructorInfo p_ctor;
        private object[] p_par;
        private readonly bool p_canToObj;

        public override bool CanToObj => p_canToObj;

        public override bool CanToJson => true;

        public override bool CanConverAll => p_canToObj;

        public override JsonVariable ToJsonVariable(T obj)
        {
            return obj.ToJsonVariable();
        }

        public override T ToObj(JsonVariable json)
        {
            if (json is null) throw new ArgumentNullException();
            if (!p_canToObj) throw new NotImplementedException();
            try
            {
                p_par[0] = json;
                var obj = p_ctor.Invoke(p_par);
                p_par[0] = null;
                return (T)obj;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
           
        }
    }

    internal sealed class c_ConverTo_IFAndCtor<T> : ConverToJsonAndObj<T> where T : IConverToJson
    {

        public c_ConverTo_IFAndCtor(ConstructorInfo ctor)
        {
            p_ctor = ctor;
            p_par = new object[1];
            p_canToObj = (object)ctor != null;
        }

        private readonly ConstructorInfo p_ctor;
        private object[] p_par;
        private readonly bool p_canToObj;

        public override bool CanToObj => p_canToObj;

        public override bool CanToJson => true;

        public override bool CanConverAll => p_canToObj;

        public override JsonVariable ToJsonVariable(T obj)
        {
            if (obj == null) return JsonNull.Nullable;
            return obj.ToJsonVariable();
        }

        public override T ToObj(JsonVariable json)
        {
            if (json is null) throw new ArgumentNullException();
            if (!p_canToObj) throw new NotImplementedException();
            try
            {
                p_par[0] = json;
                var obj = p_ctor.Invoke(p_par);
                p_par[0] = null;
                return (T)obj;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

        }
    }

    internal sealed class c_ConverTo_Ctor<T> : ConverToJsonAndObj<T>
    {
        public c_ConverTo_Ctor(ConstructorInfo ctor)
        {
            p_ctor = ctor;
            p_par = new object[1];
            p_canToObj = (object)ctor != null;
        }

        private readonly ConstructorInfo p_ctor;
        private object[] p_par;
        private readonly bool p_canToObj;

        public override bool CanToObj => p_canToObj;

        public override bool CanToJson => false;

        public override bool CanConverAll => false;

        public override JsonVariable ToJsonVariable(T obj)
        {
            throw new NotImplementedException();
        }

        public override T ToObj(JsonVariable json)
        {
            if (json is null) throw new ArgumentNullException();
            if (!p_canToObj) throw new NotImplementedException();
            try
            {
                p_par[0] = json;
                var obj = p_ctor.Invoke(p_par);
                p_par[0] = null;
                return (T)obj;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

        }
    }

}