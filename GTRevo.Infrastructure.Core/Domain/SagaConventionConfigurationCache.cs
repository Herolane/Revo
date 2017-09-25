﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTRevo.Core.Core;
using GTRevo.Core.Core.Lifecycle;

namespace GTRevo.Infrastructure.Core.Domain
{
    public class SagaConventionConfigurationCache : ISagaConventionConfigurationCache, IApplicationStartListener
    {
        private static Dictionary<Type, SagaConfigurationInfo> configurationInfos;

        private readonly ITypeExplorer typeExplorer;

        public SagaConventionConfigurationCache(ITypeExplorer typeExplorer)
        {
            this.typeExplorer = typeExplorer;
            CreateConfigurationInfos();
            ConfigurationInfos = new ReadOnlyDictionary<Type, SagaConfigurationInfo>(configurationInfos);
        }
        
        public ReadOnlyDictionary<Type, SagaConfigurationInfo> ConfigurationInfos { get; }
        
        public void OnApplicationStarted()
        {
            //construction of the object itself is enough
        }

        public static SagaConfigurationInfo GetSagaConfigurationInfo(Type sagaType)
        {
            if (!typeof(Saga).IsAssignableFrom(sagaType))
            {
                throw new ArgumentException($"Only Saga-derived sagas are configured using conventions");
            }

            if (configurationInfos == null)
            {
                return SagaConventionConfigurationScanner.GetSagaConfiguration(sagaType);
            }

            SagaConfigurationInfo configurationInfo;
            if (!configurationInfos.TryGetValue(sagaType, out configurationInfo))
            {
                throw new ArgumentException($"Unknown saga type to get convention configuration info for: " + sagaType.FullName);
            }

            return configurationInfo;
        }
        
        private void CreateConfigurationInfos()
        {
            configurationInfos = new Dictionary<Type, SagaConfigurationInfo>();
            var sagaTypes = typeExplorer.GetAllTypes()
                .Where(x => typeof(Saga).IsAssignableFrom(x)
                    && !x.IsAbstract && !x.IsGenericTypeDefinition);

            foreach (Type sagaType in sagaTypes)
            {
                configurationInfos.Add(sagaType, SagaConventionConfigurationScanner.GetSagaConfiguration(sagaType));
            }
        }
    }
}
