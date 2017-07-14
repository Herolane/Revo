﻿using System;
using System.Collections.Generic;
using System.Linq;
using GTRevo.Core;
using GTRevo.Core.Core;
using GTRevo.DataAccess.Entities;

namespace GTRevo.DataAccess.EF6.Model
{
    public class EntityTypeDiscovery
    {
        private readonly ITypeExplorer typeExplorer;

        public EntityTypeDiscovery(ITypeExplorer typeExplorer)
        {
            this.typeExplorer = typeExplorer;
        }

        public static string DetectEntitySchemaSpace(Type entityType)
        {
            var attrs = (DatabaseEntityAttribute)entityType
                .GetCustomAttributes(typeof(DatabaseEntityAttribute), false)
                .FirstOrDefault();

            if (attrs?.SchemaSpace != null)
            {
                return attrs.SchemaSpace;
            }
            else if (entityType.BaseType != null)
            {
                return DetectEntitySchemaSpace(entityType.BaseType);
            }

            return "Default";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Schema spaces to entity types.</returns>
        public MultiValueDictionary<string, Type> DiscoverEntities()
        {
            MultiValueDictionary<string, Type> schemaSpaceEntityTypes = new MultiValueDictionary<string, Type>();
            IEnumerable<Type> entityTypes = FindEntityTypes();

            foreach (Type entityType in entityTypes)
            {
                string schemaSpace = DetectEntitySchemaSpace(entityType);
                schemaSpaceEntityTypes.Add(schemaSpace, entityType);
            }

            return schemaSpaceEntityTypes;
        }

        private IEnumerable<Type> FindEntityTypes()
        {
            return typeExplorer.GetAllTypes()
                .Where(x => x.IsClass && !x.IsAbstract && !x.IsGenericTypeDefinition
                    && x.GetCustomAttributes(typeof(DatabaseEntityAttribute), true).Length > 0);
        }
    }
}