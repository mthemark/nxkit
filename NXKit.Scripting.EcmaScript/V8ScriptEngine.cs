﻿using System;
using System.ComponentModel.Composition;
using System.Diagnostics.Contracts;
using System.Dynamic;
using NXKit.Util;

namespace NXKit.Scripting.EcmaScript
{

    /// <summary>
    /// Provides a ECMAScript implementation using the Google V8 engine.
    /// </summary>
    [ScriptEngine]
    public class V8ScriptEngine :
        IScriptEngine,
        IDisposable
    {

        static readonly MediaRangeList ACCEPT = new MediaRange[]
        {
            "application/ecmascript",
            "application/javascript",
            "text/javascript",
        };


        NXDocumentHost host;
        Lazy<Microsoft.ClearScript.V8.V8ScriptEngine> engine;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="host"></param>
        [ImportingConstructor]
        public V8ScriptEngine(NXDocumentHost host)
        {
            Contract.Requires<ArgumentNullException>(host != null);

            this.host = host;
            this.engine = new Lazy<Microsoft.ClearScript.V8.V8ScriptEngine>(() =>
                new Microsoft.ClearScript.V8.V8ScriptEngine());
        }

        public bool CanExecute(string type, string code)
        {
            return ACCEPT.Matches(type);
        }

        public void Execute(string type, string code)
        {
            if (!CanExecute(type, code))
                throw new InvalidOperationException();

            engine.Value.Execute(code);
        }

        public object Evaluate(string type, string code)
        {
            if (!CanExecute(type, code))
                throw new InvalidOperationException();

            return engine.Value.Evaluate(code);
        }

        public void Load()
        {

        }

        public void Save()
        {
            var n = ((DynamicObject)engine.Value.Script).GetDynamicMemberNames();
            foreach (var i in n)
                Console.WriteLine(i);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            if (engine.IsValueCreated)
                engine.Value.Dispose();
        }

        ~V8ScriptEngine()
        {
            Dispose();
        }



    }

}
