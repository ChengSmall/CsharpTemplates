using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.IO;


namespace Cheng.DataStructure.DynamicVariables
{

    internal sealed class c_ConverTo_Array<T> : ConverToDynVariableAndObj<T[]>
    {
        public c_ConverTo_Array()
        {
        }

        public override DynVariable ToDynVariable(T[] obj)
        {
            if (obj is null) return DynVariable.EmptyValue;
            DynList dlist = new DynList(obj.Length);
            var cov = ConverToDynVariableAndObj<T>.Default;
            for (int i = 0; i < obj.Length; i++)
            {
                dlist.Add(cov.ToDynVariable(obj[i]));
            }
            return dlist;
        }

        public override T[] ToObj(DynVariable dynVariable)
        {
            if (dynVariable is null || dynVariable.IsEmpty) return null;

            try
            {
                var dlist = dynVariable.DynamicList;
                if (dlist.Count == 0) return Array.Empty<T>();
                T[] arr = new T[dlist.Count];
                var cov = ConverToDynVariableAndObj<T>.Default;
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = cov.ToObj(dlist[i]);
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
        }

    }

    internal sealed class c_ConverTo_This : ConverToDynVariableAndObj<DynVariable>
    {
        public c_ConverTo_This()
        { 
        }

        public override DynVariable ToDynVariable(DynVariable obj)
        {
            if (obj is null) return DynVariable.EmptyValue;
            return obj.Clone();
        }

        public override DynVariable ToObj(DynVariable dynVariable)
        {
            if (dynVariable is null) return DynVariable.EmptyValue;
            return dynVariable.Clone();
        }

    }

}