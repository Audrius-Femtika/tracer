﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace Tracer.Fody.Weavers
{
    public class TraceLoggingConfiguration
    {
        private readonly ITraceLoggingFilter _filter;
        private readonly AssemblyName _adapterAssemblyName;
        private readonly string _loggerAdapterTypeFullName;
        private readonly string _logManagerAdapterTypeFullName;
        private readonly string _staticLoggerTypeFullName;

        private TraceLoggingConfiguration(ITraceLoggingFilter filter, string adapterAssemblyDisplayName, 
            string loggerAdapterTypeName, string logManagerAdapterTypeName, string staticLoggerTypeFullName)
        {
            _filter = filter ?? NullFilter.Instance;
            _adapterAssemblyName = new AssemblyName(adapterAssemblyDisplayName ?? "Tracer.LogAdapter, Version=1.0.0.0");
            _loggerAdapterTypeFullName = loggerAdapterTypeName ?? "Tracer.LogAdapter.ILog";
            _logManagerAdapterTypeFullName = logManagerAdapterTypeName ?? "Tracer.LogAdapter.LogManager";
            _staticLoggerTypeFullName = staticLoggerTypeFullName ?? "Tracer.LogAdapter.Log";
        }

        public static TraceLoggingConfigurationBuilder New
        {
            get {  return new TraceLoggingConfigurationBuilder(); }
        }

        public ITraceLoggingFilter Filter
        {
            get { return _filter; }
        }

        public AssemblyNameReference AssemblyNameReference
        {
            get
            {
                return new AssemblyNameReference(_adapterAssemblyName.Name, _adapterAssemblyName.Version);
            }
        }

        public TypeName LogMannager
        {
            get
            {
                return new TypeName(_logManagerAdapterTypeFullName);
            }
        }

        public TypeName Logger
        {
            get
            {
                return new TypeName(_loggerAdapterTypeFullName);
            }
        }

        public TypeName StaticLogger
        {
            get
            {
                return new TypeName(_staticLoggerTypeFullName);
            }
        }

        public class TypeName
        {
            private readonly string _namespace;
            private readonly string _name;

            public TypeName(string fullName)
            {
                //TODO checks, validation
                var nameSplit = fullName.Split('.');
                _name = nameSplit[nameSplit.Length - 1];
                _namespace = String.Join(".", nameSplit.Take(nameSplit.Length - 1));
            }

            public TypeName(string ns, string name)
            {
                _namespace = ns;
                _name = name;
            }

            public string Namespace
            {
                get { return _namespace; }
            }

            public string Name
            {
                get { return _name; }
            }
        }

        private class NullFilter : ITraceLoggingFilter
        {
            public static readonly NullFilter Instance = new NullFilter();

            private NullFilter()
            {}

            public bool ShouldAddTrace(MethodDefinition definition)
            {
                return true;
            }
        }

        public class TraceLoggingConfigurationBuilder
        {
            private ITraceLoggingFilter _filter;
            private string _loggerAdapterAssemblyDisplayName;
            private string _loggerAdapterTypeName;
            private string _logManagerAdapterTypeName;
            private string _staticLoggerTypeName;

            public static implicit operator TraceLoggingConfiguration(TraceLoggingConfigurationBuilder builder)
            {
                return new TraceLoggingConfiguration(
                    builder._filter,
                    builder._loggerAdapterAssemblyDisplayName,
                    builder._loggerAdapterTypeName,
                    builder._logManagerAdapterTypeName,
                    builder._staticLoggerTypeName);
            }

            public TraceLoggingConfigurationBuilder WithFilter(ITraceLoggingFilter filter)
            {
                _filter = filter;
                return this;
            }

            public TraceLoggingConfigurationBuilder WithAdapterAssembly(string displayName)
            {
                _loggerAdapterAssemblyDisplayName = displayName;
                return this;
            }

            public TraceLoggingConfigurationBuilder WithLogManager(string name)
            {
                _logManagerAdapterTypeName = name;
                return this;
            }

            public TraceLoggingConfigurationBuilder WithLogger(string name)
            {
                _loggerAdapterTypeName = name;
                return this;
            }

            public TraceLoggingConfigurationBuilder WithStaticLogger(string name)
            {
                _staticLoggerTypeName = name;
                return this;
            }
        }

    }


}
